using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Infraestructure.Common.Browser
{
    public interface IBrowserProvider
    {
        Task<IBrowser> GetBrowserAsync();
    }
}
