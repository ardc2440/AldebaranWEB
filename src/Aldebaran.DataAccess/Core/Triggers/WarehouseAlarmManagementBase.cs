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
                .Where(r => referenceIds.Contains(r.ReferenceId))
                .ToListAsync(cancellationToken);

            if (referenceList.Count != referenceIds.Count)
            {
                var missingIds = referenceIds.Except(referenceList.Select(r => r.ReferenceId)).ToList();
                throw new ArgumentException($"No se encontraron las siguientes referencias: {string.Join(", ", missingIds)}");
            }

            var customerOrderList = await _context.CustomerOrders
                .AsNoTracking()
                .Where(w => w.CustomerOrderDetails.Any(a => referenceIds.Contains(a.ReferenceId) &&
                                                            (a.RequestedQuantity - a.ProcessedQuantity - a.DeliveredQuantity) > 0))
                .Select(order => new {
                    order.CustomerOrderId,
                    RelevantDetails = order.CustomerOrderDetails
                                        .Where(a => referenceIds.Contains(a.ReferenceId) &&
                                                    (a.RequestedQuantity - a.ProcessedQuantity - a.DeliveredQuantity) > 0)
                                        .Select(a => new
                                        {
                                            a.ReferenceId,
                                            PendingQuantity = a.RequestedQuantity - a.ProcessedQuantity - a.DeliveredQuantity
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
