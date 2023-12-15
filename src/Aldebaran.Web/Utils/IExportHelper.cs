using Microsoft.AspNetCore.Mvc;

namespace Aldebaran.Web.Utils
{
    public interface IExportHelper
    {
        FileStreamResult ToCSV<T>(List<T> data, string fileName = null);
        FileStreamResult ToExcel<T>(List<T> data, string fileName = null);
    }
}
