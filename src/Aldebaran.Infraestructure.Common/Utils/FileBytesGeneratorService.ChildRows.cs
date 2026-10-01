using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Globalization;

namespace Aldebaran.Infraestructure.Common.Utils
{
    /// <summary>
    /// Excel con filas padre/hijo agrupadas (outline de Excel con "+").
    /// Aditivo: no modifica GetExcelBytes ni sus usos; reutiliza los helpers privados de la clase.
    /// </summary>
    public partial class FileBytesGeneratorService
    {
        private const byte ChildOutlineLevel = 1;
        private const double ImageRowHeight = 100;

        public Task<byte[]> GetExcelBytesWithChildRows<T>(List<T> parents, Func<T, IEnumerable<T>> childrenSelector)
        {
            ArgumentNullException.ThrowIfNull(parents);
            ArgumentNullException.ThrowIfNull(childrenSelector);

            var columns = GetProperties(typeof(T)).ToList();
            var groups = BuildRowGroups(parents, childrenSelector);

            using var stream = new MemoryStream();
            using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
            {
                var worksheetPart = CreateSingleSheetWorkbook(document);
                var worksheet = worksheetPart.Worksheet;

                // Orden exigido por el esquema: sheetPr, sheetFormatPr, cols, sheetData, drawing.
                // Sin SheetProtection: Excel no permite expandir/contraer grupos en hojas protegidas.
                worksheet.Append(new SheetProperties(new OutlineProperties { SummaryBelow = false }));
                worksheet.Append(new SheetFormatProperties { DefaultRowHeight = 15D, OutlineLevelRow = ChildOutlineLevel });
                worksheet.Append(BuildWorksheetColumns(columns, groups.SelectMany(g => g.Children.Prepend(g.Parent)).ToList()));

                var sheetData = worksheet.AppendChild(new SheetData());
                sheetData.Append(BuildHeaderRow(columns));

                uint rowIndex = 2;
                foreach (var (parent, children) in groups)
                {
                    sheetData.Append(BuildParentRow(worksheetPart, parent, columns, rowIndex++, hasChildren: children.Count > 0));

                    foreach (var child in children)
                        sheetData.Append(BuildChildRow(child, columns, rowIndex++));
                }

                document.WorkbookPart!.Workbook.Save();
            }

            return Task.FromResult(stream.ToArray());
        }

        private static List<(T Parent, List<T> Children)> BuildRowGroups<T>(List<T> parents, Func<T, IEnumerable<T>> childrenSelector) =>
            parents
                .Where(parent => parent != null)
                .Select(parent => (parent, (childrenSelector(parent) ?? Enumerable.Empty<T>()).Where(child => child != null).ToList()))
                .ToList();

        private static WorksheetPart CreateSingleSheetWorkbook(SpreadsheetDocument document)
        {
            var workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet();

            GenerateWorkbookStylesPartContent(workbookPart.AddNewPart<WorkbookStylesPart>());

            workbookPart.Workbook.AppendChild(new Sheets()).Append(new Sheet
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Sheet1"
            });

            workbookPart.Workbook.Save();
            return worksheetPart;
        }

        private static Columns BuildWorksheetColumns<T>(List<PropertyDetail> columns, List<T> rows)
        {
            var worksheetColumns = new Columns();
            uint columnIndex = 1;

            foreach (var column in columns)
            {
                var maxLength = rows
                    .Select(row => GetValue(row, column.Name)?.ToString()?.Length ?? 0)
                    .Append((column.DisplayName ?? column.Name).Length)
                    .Max();

                var width = column.IsImage ? 20 : Math.Min(Math.Max(maxLength + 4, 10), 60);

                worksheetColumns.Append(new Column { Min = columnIndex, Max = columnIndex, Width = width, CustomWidth = true });
                columnIndex++;
            }

            return worksheetColumns;
        }

        private static Row BuildHeaderRow(List<PropertyDetail> columns)
        {
            var headerRow = new Row { RowIndex = 1U };

            foreach (var column in columns)
                headerRow.Append(new Cell
                {
                    CellValue = new CellValue(column.DisplayName ?? column.Name),
                    DataType = new EnumValue<CellValues>(CellValues.String)
                });

            return headerRow;
        }

        /// <summary>Fila padre (nivel 0): datos + imagen; si tiene hijos queda marcada como contraída.</summary>
        private static Row BuildParentRow<T>(WorksheetPart worksheetPart, T item, List<PropertyDetail> columns, uint rowIndex, bool hasChildren)
        {
            var row = new Row { RowIndex = rowIndex };
            var containsImage = false;
            uint columnIndex = 1;

            foreach (var column in columns)
            {
                var value = GetValue(item, column.Name);

                if (column.IsImage)
                {
                    row.Append(new Cell());

                    if (value is string imagePath && !string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
                    {
                        InsertImage(worksheetPart, imagePath, (int)rowIndex, columnIndex);
                        containsImage = true;
                    }
                }
                else
                {
                    row.Append(BuildValueCell(value, column));
                }

                columnIndex++;
            }

            if (containsImage)
            {
                row.Height = ImageRowHeight;
                row.CustomHeight = true;
            }

            if (hasChildren)
                row.Collapsed = true;

            return row;
        }

        /// <summary>Fila hija (nivel 1, oculta): solo datos, la columna de imagen queda vacía.</summary>
        private static Row BuildChildRow<T>(T item, List<PropertyDetail> columns, uint rowIndex)
        {
            var row = new Row { RowIndex = rowIndex, OutlineLevel = ChildOutlineLevel, Hidden = true };

            foreach (var column in columns)
                row.Append(column.IsImage ? new Cell() : BuildValueCell(GetValue(item, column.Name), column));

            return row;
        }

        /// <summary>Celda según el tipo de la propiedad; un valor nulo produce una celda vacía.</summary>
        private static Cell BuildValueCell(object? value, PropertyDetail column)
        {
            if (value == null)
                return new Cell();

            var underlyingType = Nullable.GetUnderlyingType(column.Type) ?? column.Type;
            var typeCode = Type.GetTypeCode(underlyingType);

            if (typeCode == TypeCode.DateTime)
                return new Cell
                {
                    CellValue = new CellValue(((DateTime)value).ToOADate().ToString(CultureInfo.InvariantCulture)),
                    DataType = new EnumValue<CellValues>(CellValues.Number),
                    StyleIndex = 1U
                };

            if (typeCode == TypeCode.Boolean)
                return new Cell
                {
                    CellValue = new CellValue(value.ToString()!.ToLowerInvariant()),
                    DataType = new EnumValue<CellValues>(CellValues.Boolean)
                };

            if (IsNumeric(typeCode))
                return new Cell
                {
                    CellValue = new CellValue(Convert.ToString(value, CultureInfo.InvariantCulture)!),
                    DataType = new EnumValue<CellValues>(CellValues.Number)
                };

            return new Cell
            {
                CellValue = new CellValue($"{value}".Trim()),
                DataType = new EnumValue<CellValues>(CellValues.String)
            };
        }
    }
}
