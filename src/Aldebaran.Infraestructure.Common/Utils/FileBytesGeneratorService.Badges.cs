using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using A = DocumentFormat.OpenXml.Drawing;
using Xdr = DocumentFormat.OpenXml.Drawing.Spreadsheet;

namespace Aldebaran.Infraestructure.Common.Utils
{
    /// <summary>
    /// Marca de texto (rectángulo rojo con letras blancas) dibujada sobre la imagen de una celda.
    /// Aditivo: solo la usan las columnas con ExcelImageAttribute.BadgeProperty.
    /// </summary>
    public partial class FileBytesGeneratorService
    {
        private const long BadgeOffsetX = 60000;
        private const long BadgeOffsetY = 60000;
        private const long BadgeWidth = 720000;
        private const long BadgeHeight = 230000;
        private const string BadgeFillColor = "D32F2F";
        private const string BadgeTextColor = "FFFFFF";

        private static void InsertBadge(WorksheetPart worksheetPart, string text, uint rowIndex, uint columnIndex)
        {
            var worksheetDrawing = GetOrCreateWorksheetDrawing(worksheetPart);
            var shapeId = (uint)(worksheetDrawing.Descendants<Xdr.NonVisualDrawingProperties>().Count() + 1);

            var anchor = new Xdr.OneCellAnchor(
                new Xdr.FromMarker(
                    new Xdr.ColumnId((columnIndex - 1).ToString()),
                    new Xdr.ColumnOffset(BadgeOffsetX.ToString()),
                    new Xdr.RowId((rowIndex - 1).ToString()),
                    new Xdr.RowOffset(BadgeOffsetY.ToString())),
                new Xdr.Extent { Cx = BadgeWidth, Cy = BadgeHeight },
                BuildBadgeShape(shapeId, text),
                new Xdr.ClientData());

            worksheetDrawing.Append(anchor);
            worksheetDrawing.Save();
        }

        private static Xdr.Shape BuildBadgeShape(uint shapeId, string text) =>
            new(
                new Xdr.NonVisualShapeProperties(
                    new Xdr.NonVisualDrawingProperties { Id = shapeId, Name = $"Marca {shapeId}" },
                    new Xdr.NonVisualShapeDrawingProperties()),
                new Xdr.ShapeProperties(
                    new A.Transform2D(
                        new A.Offset { X = 0, Y = 0 },
                        new A.Extents { Cx = BadgeWidth, Cy = BadgeHeight }),
                    new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.RoundRectangle },
                    new A.SolidFill(new A.RgbColorModelHex { Val = BadgeFillColor }),
                    new A.Outline(new A.NoFill())),
                new Xdr.TextBody(
                    new A.BodyProperties
                    {
                        Anchor = A.TextAnchoringTypeValues.Center,
                        Wrap = A.TextWrappingValues.None,
                        LeftInset = 0, RightInset = 0, TopInset = 0, BottomInset = 0
                    },
                    new A.ListStyle(),
                    new A.Paragraph(
                        new A.ParagraphProperties { Alignment = A.TextAlignmentTypeValues.Center },
                        new A.Run(
                            new A.RunProperties(new A.SolidFill(new A.RgbColorModelHex { Val = BadgeTextColor }))
                            {
                                Language = "es-CO",
                                FontSize = 900,
                                Bold = true
                            },
                            new A.Text(text)))));

        private static Xdr.WorksheetDrawing GetOrCreateWorksheetDrawing(WorksheetPart worksheetPart)
        {
            var drawingsPart = worksheetPart.DrawingsPart;
            if (drawingsPart == null)
            {
                drawingsPart = worksheetPart.AddNewPart<DrawingsPart>();
                worksheetPart.Worksheet.Append(new Drawing { Id = worksheetPart.GetIdOfPart(drawingsPart) });
                drawingsPart.WorksheetDrawing = new Xdr.WorksheetDrawing();
            }

            return drawingsPart.WorksheetDrawing;
        }
    }
}
