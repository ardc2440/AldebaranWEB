using Aldebaran.Infraestructure.Common.Browser;

namespace Aldebaran.Application.FileWritingService.Services.Browser
{
    /// <summary>
    /// Proveedor de Chromium que, además de entregar la instancia, permite descartarla
    /// para que la siguiente solicitud lance una nueva.
    /// </summary>
    public interface IResettableBrowserProvider : IBrowserProvider
    {
        Task ResetAsync();
    }
}
