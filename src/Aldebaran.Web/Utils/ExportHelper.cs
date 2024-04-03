using Aldebaran.Infraestructure.Common.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Aldebaran.Web.Utils
{
    public class ExportHelper : IExportHelper
    {
        private readonly IFileBytesGeneratorService fileBytesGeneratorService;
        public ExportHelper(IFileBytesGeneratorService fileBytesGeneratorService)
        {
            this.fileBytesGeneratorService = fileBytesGeneratorService ?? throw new ArgumentNullException(nameof(fileBytesGeneratorService));
        }
        public FileStreamResult ToCSV<T>(List<T> data, string fileName = null)
        {
            var byteArray = fileBytesGeneratorService.GetCsvBytes(data).Result;
            return new FileStreamResult(new MemoryStream(byteArray), "text/csv")
            {
                FileDownloadName = (!string.IsNullOrEmpty(fileName) ? fileName : "Export") + ".csv"
            };
        }
        public FileStreamResult ToExcel<T>(List<T> data, string fileName = null)
        {
            var byteArray = fileBytesGeneratorService.GetExcelBytes(data).Result;
            return new FileStreamResult(new MemoryStream(byteArray), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = (!string.IsNullOrEmpty(fileName) ? fileName : "Export") + ".xlsx"
            };
        }
    }
}
