using Aldebaran.DataAccess.Entities;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Aldebaran.DataAccess.Core.Triggers
{
    public class WarehouseAlarmManagementBase : InventoryManagementBase
    {
        private readonly AldebaranDbContext _context;

        public WarehouseAlarmManagementBase(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddWarehouseAlarmAsync<T>(string documentCode, int documentNumber, IEnumerable<T> detail, Func<T, int> getReferenceId, CancellationToken cancellationToken) where T : class
        {
            var documentType = await _context.DocumentTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.DocumentTypeCode.Equals(documentCode), cancellationToken)
                ?? throw new ArgumentNullException($"Tipo de documento con código {documentCode} no encontrado");

            var referenceIds = detail.Select(getReferenceId).Where(id => id > 0).ToList();
            if (!referenceIds.Any())
            {
                throw new ArgumentException("No se encontraron referencias válidas en los detalles.");
            }

            var referenceList = await _context.ItemReferences
                .AsNoTracking()
                .Where(w => referenceIds.Contains(w.ReferenceId))
                .ToListAsync(cancellationToken);

            if (referenceList.Count != referenceIds.Count)
            {
                var missingIds = referenceIds.Except(referenceList.Select(r => r.ReferenceId)).ToList();
                throw new ArgumentException($"No se encontraron las siguientes referencias: {string.Join(", ", missingIds)}");
            }

            var customerOrderList = await _context.CustomerOrders
                .AsNoTracking()
                .Include(i=>i.StatusDocumentType)
                .Where(w => (new[] {1,2,3 }).Contains(w.StatusDocumentType.StatusOrder) &&
                            w.CustomerOrderDetails.Any(a => referenceIds.Contains(a.ReferenceId) &&
                                                            (a.RequestedQuantity - a.ProcessedQuantity - a.DeliveredQuantity) > 0))
                .Select(s => new {
                    AlarmOrderId = s.CustomerOrderId,
                    Details = s.CustomerOrderDetails
                                    .Where(w => referenceIds.Contains(w.ReferenceId) &&
                                                (w.RequestedQuantity - w.ProcessedQuantity - w.DeliveredQuantity) > 0)
                                    .Select(s => new
                                    {
                                        s.ReferenceId,
                                        PendingQuantity = s.RequestedQuantity - s.ProcessedQuantity - s.DeliveredQuantity
                                    })
                }).ToListAsync(cancellationToken);

            var jReferenceList = JsonConvert.SerializeObject(referenceList.Select(s => new { s.ReferenceId }));
            var jCustomerOrderList = JsonConvert.SerializeObject(customerOrderList);

            var entity = new LocalWarehouseAlarm
            {
                AlarmDate = DateTime.Now,
                DocumentNumber = documentNumber,
                DocumentTypeId = documentType.DocumentTypeId,
                ReferenceList = jReferenceList,
                CustomerOrderList = jCustomerOrderList
            };

            _context.LocalWarehouseAlarms.Add(entity);
        }

    }
}
