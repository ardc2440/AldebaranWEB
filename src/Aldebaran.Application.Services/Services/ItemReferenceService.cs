using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class ItemReferenceService : IItemReferenceService
    {
        private readonly IItemReferenceRepository _repository;
        private readonly IPurchaseOrderDetailRepository _repositoryPurchaseOrder;
        private readonly IMapper _mapper;
        public ItemReferenceService(IItemReferenceRepository repository, IMapper mapper, IPurchaseOrderDetailRepository repositoryPurchaseOrder)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IItemReferenceRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
            _repositoryPurchaseOrder = repositoryPurchaseOrder;
        }

        public async Task AddAsync(ItemReference itemReference, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.ItemReference>(itemReference) ?? throw new ArgumentNullException("Referencia no puede ser nula.");
            await _repository.AddAsync(entity, ct);
        }

        public async Task DeleteAsync(int itemReferenceId, CancellationToken ct = default)
        {
            await _repository.DeleteAsync(itemReferenceId, ct);
        }

        public async Task<ItemReference?> FindAsync(int itemReferenceId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(itemReferenceId, ct);
            var dataMapped = _mapper.Map<ItemReference?>(data);
                        
            if (dataMapped!=null) dataMapped.HavePurchaseOrderDetail = await _repositoryPurchaseOrder.ExistsDetailByReferenceId(dataMapped.ReferenceId, ct);

            return dataMapped;
        }

        public async Task<bool> ExistsByReferenceCode(string referenceCode, int itemId, CancellationToken ct = default)
        {
            return await _repository.ExistsByReferenceCode(referenceCode, itemId, ct);
        }
        public async Task<bool> ExistsByReferenceName(string referenceName, int itemId, CancellationToken ct = default)
        {
            return await _repository.ExistsByReferenceName(referenceName, itemId, ct);
        }

        public async Task<IEnumerable<ItemReference>> GetByItemIdAsync(int itemId, CancellationToken ct = default)
        {
            var data = await _repository.GetByItemIdAsync(itemId, ct);
            var dataMapped = _mapper.Map<List<ItemReference>>(data);

            foreach (var item in dataMapped)
                item.HavePurchaseOrderDetail = await _repositoryPurchaseOrder.ExistsDetailByReferenceId(item.ReferenceId, ct);  
            
            return dataMapped;
        }

        public async Task UpdateAsync(int itemReferenceId, ItemReference itemReference, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.ItemReference>(itemReference) ?? throw new ArgumentNullException("Referencia no puede ser nula.");
            await _repository.UpdateAsync(itemReferenceId, entity, ct);
        }

        public async Task<IEnumerable<ItemReference>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            var dataMapped = _mapper.Map<List<ItemReference>>(data);
                        
            return dataMapped;
        }

        public async Task<IEnumerable<ItemReference>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(searchKey, ct);
            var dataMapped = _mapper.Map<List<ItemReference>>(data);
                        
            return dataMapped;
        }

        public async Task<IEnumerable<ItemReference>> GetByStatusAsync(bool isActive, CancellationToken ct = default)
        {
            var data = await _repository.GetByStatusAsync(isActive, ct);
            var dataMapped = _mapper.Map<List<ItemReference>>(data);
                        
            return dataMapped;
        }
        
        public async Task<IEnumerable<ItemReference>> GetReportsReferencesAsync(bool? isReferenceActive = null, bool? isItemActive = null, bool? isExternalInventory = null, CancellationToken ct = default)
        {
            var data = await _repository.GetReportsReferencesAsync(isReferenceActive, isItemActive, isExternalInventory, ct);
            var dataMapped = _mapper.Map<List<ItemReference>>(data);
                        
            return dataMapped;
        }
    }
}
