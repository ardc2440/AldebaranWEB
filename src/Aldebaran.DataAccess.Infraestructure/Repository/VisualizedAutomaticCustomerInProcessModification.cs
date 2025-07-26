using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class VisualizedAutomaticCustomerInProcessModificationRepository : RepositoryBase<AldebaranDbContext>, IVisualizedAutomaticCustomerInProcessModificationRepository
    {
        public VisualizedAutomaticCustomerInProcessModificationRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task AddAsync(VisualizedAutomaticCustomerOrderInProcessModification item, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                try
                {
                    // Crear una copia para no modificar el objeto original
                    var itemToSave = new VisualizedAutomaticCustomerOrderInProcessModification
                    {
                        Id = item.Id,
                        ActionType = TransformActionTypeToDbFormat(item.ActionType), // Transformar en la copia
                        Employee_Id = item.Employee_Id,
                        Visualized_Date = item.Visualized_Date
                    };
                    
                    await dbContext.VisualizedAutomaticCustomerOrderInProcessModifications.AddAsync(itemToSave, ct);
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    // Ya no necesitamos resetear el estado porque no modificamos el objeto original
                    throw;
                }
            }, ct);
        }

        /// <summary>
        /// Transforma el ActionType de formato legible a formato de base de datos
        /// </summary>
        /// <param name="actionType">El tipo de acción en formato legible</param>
        /// <returns>El código de una letra para la base de datos</returns>
        private static string TransformActionTypeToDbFormat(string actionType)
        {
            return actionType switch
            {
                "Modificación" => "M",
                "Cancelación" => "C",
                _ => actionType // Si ya viene en formato correcto (M, C) o es otro valor, no transformar
            };
        }
    }
}
