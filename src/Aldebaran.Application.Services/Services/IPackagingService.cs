using Aldebaran.Application.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services.Services
{
    public interface IPackagingService
    {
        Task<Packaging?> FindByItemId(int itemId, CancellationToken ct = default);
    }
}
