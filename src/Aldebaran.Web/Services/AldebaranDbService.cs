using Aldebaran.Web.Data;
using Aldebaran.Web.Models.AldebaranDb;
using Aldebaran.Web.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Text.Encodings.Web;

namespace Aldebaran.Web
{
    public partial class AldebaranDbService
    {
        AldebaranDbContext Context
        {
            get
            {
                return this.context;
            }
        }

        private readonly AldebaranDbContext context;
        private readonly NavigationManager navigationManager;

        public AldebaranDbService(AldebaranDbContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }

        public async Task ExportAdjustmentDetailsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/adjustmentdetails/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/adjustmentdetails/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAdjustmentDetailsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/adjustmentdetails/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/adjustmentdetails/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAdjustmentDetailsRead(ref IQueryable<AdjustmentDetail> items);

        public async Task<IQueryable<AdjustmentDetail>> GetAdjustmentDetails(Query query = null)
        {
            var items = Context.AdjustmentDetails.AsQueryable();

            items = items.Include(i => i.Adjustment);
            items = items.Include(i => i.ItemReference);
            items = items.Include(i => i.Warehouse);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAdjustmentDetailsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAdjustmentDetailGet(AdjustmentDetail item);
        partial void OnGetAdjustmentDetailByAdjustmentDetailId(ref IQueryable<AdjustmentDetail> items);

        public async Task<AdjustmentDetail> GetAdjustmentDetailByAdjustmentDetailId(int adjustmentdetailid)
        {
            var items = Context.AdjustmentDetails
                              .AsNoTracking()
                              .Where(i => i.ADJUSTMENT_DETAIL_ID == adjustmentdetailid);

            items = items.Include(i => i.Adjustment);
            items = items.Include(i => i.ItemReference);
            items = items.Include(i => i.Warehouse);

            OnGetAdjustmentDetailByAdjustmentDetailId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAdjustmentDetailGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAdjustmentDetailCreated(AdjustmentDetail item);
        partial void OnAfterAdjustmentDetailCreated(AdjustmentDetail item);

        public async Task<AdjustmentDetail> CreateAdjustmentDetail(AdjustmentDetail adjustmentdetail)
        {
            OnAdjustmentDetailCreated(adjustmentdetail);

            var existingItem = Context.AdjustmentDetails
                              .Where(i => i.ADJUSTMENT_DETAIL_ID == adjustmentdetail.ADJUSTMENT_DETAIL_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.AdjustmentDetails.Add(adjustmentdetail);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(adjustmentdetail).State = EntityState.Detached;
                throw;
            }

            OnAfterAdjustmentDetailCreated(adjustmentdetail);

            return adjustmentdetail;
        }

        public async Task<AdjustmentDetail> CancelAdjustmentDetailChanges(AdjustmentDetail item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAdjustmentDetailUpdated(AdjustmentDetail item);
        partial void OnAfterAdjustmentDetailUpdated(AdjustmentDetail item);

        public async Task<AdjustmentDetail> UpdateAdjustmentDetail(int adjustmentdetailid, AdjustmentDetail adjustmentdetail)
        {
            OnAdjustmentDetailUpdated(adjustmentdetail);

            var itemToUpdate = Context.AdjustmentDetails
                              .Where(i => i.ADJUSTMENT_DETAIL_ID == adjustmentdetail.ADJUSTMENT_DETAIL_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(adjustmentdetail);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAdjustmentDetailUpdated(adjustmentdetail);

            return adjustmentdetail;
        }

        partial void OnAdjustmentDetailDeleted(AdjustmentDetail item);
        partial void OnAfterAdjustmentDetailDeleted(AdjustmentDetail item);

        public async Task<AdjustmentDetail> DeleteAdjustmentDetail(int adjustmentdetailid)
        {
            var itemToDelete = Context.AdjustmentDetails
                              .Where(i => i.ADJUSTMENT_DETAIL_ID == adjustmentdetailid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnAdjustmentDetailDeleted(itemToDelete);

            Context.AdjustmentDetails.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAdjustmentDetailDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportAdjustmentReasonsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/adjustmentreasons/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/adjustmentreasons/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAdjustmentReasonsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/adjustmentreasons/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/adjustmentreasons/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAdjustmentReasonsRead(ref IQueryable<AdjustmentReason> items);

        public async Task<IQueryable<AdjustmentReason>> GetAdjustmentReasons(Query query = null)
        {
            var items = Context.AdjustmentReasons.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAdjustmentReasonsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnActivityTypeGet(ActivityType item);
        partial void OnGetActivityTypeByActivityTypeId(ref IQueryable<ActivityType> items);

        public async Task<ActivityType> GetActivityTypeById(short activityTypeId)
        {
            var items = Context.ActivityTypes
                              .AsNoTracking()
                              .Where(i => i.ACTIVITY_TYPE_ID == activityTypeId);

            OnGetActivityTypeByActivityTypeId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnActivityTypeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAdjustmentReasonGet(AdjustmentReason item);
        partial void OnGetAdjustmentReasonByAdjustmentReasonId(ref IQueryable<AdjustmentReason> items);

        public async Task<AdjustmentReason> GetAdjustmentReasonByAdjustmentReasonId(short adjustmentreasonid)
        {
            var items = Context.AdjustmentReasons
                              .AsNoTracking()
                              .Where(i => i.ADJUSTMENT_REASON_ID == adjustmentreasonid);

            OnGetAdjustmentReasonByAdjustmentReasonId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAdjustmentReasonGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAdjustmentReasonCreated(AdjustmentReason item);
        partial void OnAfterAdjustmentReasonCreated(AdjustmentReason item);

        public async Task<AdjustmentReason> CreateAdjustmentReason(AdjustmentReason adjustmentreason)
        {
            OnAdjustmentReasonCreated(adjustmentreason);

            var existingItem = Context.AdjustmentReasons
                              .Where(i => i.ADJUSTMENT_REASON_ID == adjustmentreason.ADJUSTMENT_REASON_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.AdjustmentReasons.Add(adjustmentreason);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(adjustmentreason).State = EntityState.Detached;
                throw;
            }

            OnAfterAdjustmentReasonCreated(adjustmentreason);

            return adjustmentreason;
        }

        public async Task<AdjustmentReason> CancelAdjustmentReasonChanges(AdjustmentReason item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAdjustmentReasonUpdated(AdjustmentReason item);
        partial void OnAfterAdjustmentReasonUpdated(AdjustmentReason item);

        public async Task<AdjustmentReason> UpdateAdjustmentReason(short adjustmentreasonid, AdjustmentReason adjustmentreason)
        {
            OnAdjustmentReasonUpdated(adjustmentreason);

            var itemToUpdate = Context.AdjustmentReasons
                              .Where(i => i.ADJUSTMENT_REASON_ID == adjustmentreason.ADJUSTMENT_REASON_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(adjustmentreason);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAdjustmentReasonUpdated(adjustmentreason);

            return adjustmentreason;
        }

        partial void OnAdjustmentReasonDeleted(AdjustmentReason item);
        partial void OnAfterAdjustmentReasonDeleted(AdjustmentReason item);

        public async Task<AdjustmentReason> DeleteAdjustmentReason(short adjustmentreasonid)
        {
            var itemToDelete = Context.AdjustmentReasons
                              .Where(i => i.ADJUSTMENT_REASON_ID == adjustmentreasonid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnAdjustmentReasonDeleted(itemToDelete);

            Context.AdjustmentReasons.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAdjustmentReasonDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportAdjustmentTypesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/adjustmenttypes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/adjustmenttypes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAdjustmentTypesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/adjustmenttypes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/adjustmenttypes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAdjustmentTypesRead(ref IQueryable<AdjustmentType> items);

        public async Task<IQueryable<AdjustmentType>> GetAdjustmentTypes(Query query = null)
        {
            var items = Context.AdjustmentTypes.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAdjustmentTypesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAdjustmentTypeGet(AdjustmentType item);
        partial void OnGetAdjustmentTypeByAdjustmentTypeId(ref IQueryable<AdjustmentType> items);

        public async Task<AdjustmentType> GetAdjustmentTypeByAdjustmentTypeId(short adjustmenttypeid)
        {
            var items = Context.AdjustmentTypes
                              .AsNoTracking()
                              .Where(i => i.ADJUSTMENT_TYPE_ID == adjustmenttypeid);

            OnGetAdjustmentTypeByAdjustmentTypeId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAdjustmentTypeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAdjustmentTypeCreated(AdjustmentType item);
        partial void OnAfterAdjustmentTypeCreated(AdjustmentType item);

        public async Task<AdjustmentType> CreateAdjustmentType(AdjustmentType adjustmenttype)
        {
            OnAdjustmentTypeCreated(adjustmenttype);

            var existingItem = Context.AdjustmentTypes
                              .Where(i => i.ADJUSTMENT_TYPE_ID == adjustmenttype.ADJUSTMENT_TYPE_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.AdjustmentTypes.Add(adjustmenttype);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(adjustmenttype).State = EntityState.Detached;
                throw;
            }

            OnAfterAdjustmentTypeCreated(adjustmenttype);

            return adjustmenttype;
        }

        public async Task<AdjustmentType> CancelAdjustmentTypeChanges(AdjustmentType item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAdjustmentTypeUpdated(AdjustmentType item);
        partial void OnAfterAdjustmentTypeUpdated(AdjustmentType item);

        public async Task<AdjustmentType> UpdateAdjustmentType(short adjustmenttypeid, AdjustmentType adjustmenttype)
        {
            OnAdjustmentTypeUpdated(adjustmenttype);

            var itemToUpdate = Context.AdjustmentTypes
                              .Where(i => i.ADJUSTMENT_TYPE_ID == adjustmenttype.ADJUSTMENT_TYPE_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(adjustmenttype);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAdjustmentTypeUpdated(adjustmenttype);

            return adjustmenttype;
        }

        partial void OnAdjustmentTypeDeleted(AdjustmentType item);
        partial void OnAfterAdjustmentTypeDeleted(AdjustmentType item);

        public async Task<AdjustmentType> DeleteAdjustmentType(short adjustmenttypeid)
        {
            var itemToDelete = Context.AdjustmentTypes
                              .Where(i => i.ADJUSTMENT_TYPE_ID == adjustmenttypeid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnAdjustmentTypeDeleted(itemToDelete);

            Context.AdjustmentTypes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAdjustmentTypeDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportAdjustmentsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/adjustments/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/adjustments/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAdjustmentsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/adjustments/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/adjustments/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAdjustmentsRead(ref IQueryable<Adjustment> items);

        public async Task<IQueryable<Adjustment>> GetAdjustments(Query query = null)
        {
            var items = Context.Adjustments.AsQueryable();

            items = items.Include(i => i.AdjustmentReason);
            items = items.Include(i => i.AdjustmentType);
            items = items.Include(i => i.Employee);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAdjustmentsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAdjustmentGet(Adjustment item);
        partial void OnGetAdjustmentByAdjustmentId(ref IQueryable<Adjustment> items);

        public async Task<Adjustment> GetAdjustmentByAdjustmentId(int adjustmentid)
        {
            var items = Context.Adjustments
                              .AsNoTracking()
                              .Where(i => i.ADJUSTMENT_ID == adjustmentid);

            items = items.Include(i => i.AdjustmentReason);
            items = items.Include(i => i.AdjustmentType);
            items = items.Include(i => i.Employee);

            OnGetAdjustmentByAdjustmentId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAdjustmentGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAdjustmentCreated(Adjustment item);
        partial void OnAfterAdjustmentCreated(Adjustment item);

        public async Task<Adjustment> CreateAdjustment(Adjustment adjustment)
        {
            OnAdjustmentCreated(adjustment);

            var itemToCreate = Context.Adjustments
                              .Where(i => i.ADJUSTMENT_ID == adjustment.ADJUSTMENT_ID)
                              .FirstOrDefault();

            if (itemToCreate != null)
            {
                throw new Exception("Item already available");
            }

            itemToCreate = new Adjustment()
            {
                ADJUSTMENT_DATE = adjustment.ADJUSTMENT_DATE,
                ADJUSTMENT_REASON_ID = adjustment.ADJUSTMENT_REASON_ID,
                ADJUSTMENT_TYPE_ID = adjustment.ADJUSTMENT_TYPE_ID,
                CREATION_DATE = adjustment.CREATION_DATE,
                EMPLOYEE_ID = adjustment.EMPLOYEE_ID,
                NOTES = adjustment.NOTES,
                STATUS_DOCUMENT_TYPE_ID = adjustment.STATUS_DOCUMENT_TYPE_ID,
                AdjustmentDetails = new List<AdjustmentDetail>()
            };

            foreach (var item in adjustment.AdjustmentDetails)
            {
                itemToCreate.AdjustmentDetails.Add(new AdjustmentDetail()
                {
                    REFERENCE_ID = item.REFERENCE_ID,
                    WAREHOUSE_ID = item.WAREHOUSE_ID,
                    QUANTITY = item.QUANTITY
                });
            }

            try
            {
                Context.Adjustments.Add(itemToCreate);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToCreate).State = EntityState.Detached;
                throw;
            }

            OnAfterAdjustmentCreated(adjustment);
            adjustment.ADJUSTMENT_ID = itemToCreate.ADJUSTMENT_ID;
            return adjustment;
        }

        public async Task<Adjustment> CancelAdjustmentChanges(Adjustment item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAdjustmentUpdated(Adjustment item);
        partial void OnAfterAdjustmentUpdated(Adjustment item);

        public async Task<Adjustment> UpdateAdjustment(Adjustment adjustment)
        {
            OnAdjustmentUpdated(adjustment);

            var itemToUpdate = Context.Adjustments
                              .Where(i => i.ADJUSTMENT_ID == adjustment.ADJUSTMENT_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            itemToUpdate.EMPLOYEE_ID = adjustment.EMPLOYEE_ID;
            itemToUpdate.NOTES = adjustment.NOTES;
            itemToUpdate.ADJUSTMENT_DATE = adjustment.ADJUSTMENT_DATE;
            itemToUpdate.ADJUSTMENT_REASON_ID = adjustment.ADJUSTMENT_REASON_ID;
            itemToUpdate.ADJUSTMENT_TYPE_ID = adjustment.ADJUSTMENT_TYPE_ID;

            foreach (var item in adjustment.AdjustmentDetails)
            {
                if (item.ADJUSTMENT_DETAIL_ID > 0)
                {
                    var detailToUpdate = context.AdjustmentDetails.FirstOrDefault(i => i.ADJUSTMENT_DETAIL_ID.Equals(item.ADJUSTMENT_DETAIL_ID));

                    detailToUpdate.QUANTITY = item.QUANTITY;
                    detailToUpdate.REFERENCE_ID = item.REFERENCE_ID;
                    detailToUpdate.WAREHOUSE_ID = item.WAREHOUSE_ID;
                    context.AdjustmentDetails.Update(detailToUpdate);
                }
                else
                {
                    var detailToUpdate = new AdjustmentDetail()
                    {
                        ADJUSTMENT_ID = adjustment.ADJUSTMENT_ID,
                        QUANTITY = item.QUANTITY,
                        REFERENCE_ID = item.REFERENCE_ID,
                        WAREHOUSE_ID = item.WAREHOUSE_ID
                    };
                    context.AdjustmentDetails.Add(detailToUpdate);
                }
            }

            var itemsToDelete = from t1 in context.AdjustmentDetails.Where(i => i.ADJUSTMENT_ID.Equals(adjustment.ADJUSTMENT_ID))
                                where !(from t2 in adjustment.AdjustmentDetails
                                        select t2.ADJUSTMENT_DETAIL_ID).Contains(t1.ADJUSTMENT_DETAIL_ID)
                                select t1;
            foreach (var item in itemsToDelete)
                context.AdjustmentDetails.Remove(item);

            context.Adjustments.Update(itemToUpdate);
            context.SaveChanges();

            OnAfterAdjustmentUpdated(adjustment);

            return adjustment;
        }

        public async Task<Adjustment> CancelAdjustment(Adjustment adjustment, short newStatus)
        {
            var itemToCancel = Context.Adjustments
                              .Where(i => i.ADJUSTMENT_ID == adjustment.ADJUSTMENT_ID)
                              .FirstOrDefault();

            if (itemToCancel == null)
            {
                throw new Exception("Item no longer available");
            }

            itemToCancel.STATUS_DOCUMENT_TYPE_ID = newStatus;

            Context.Adjustments.Update(itemToCancel);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToCancel).State = EntityState.Unchanged;
                throw;
            }

            return itemToCancel;
        }

        public async Task ExportAreasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/areas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/areas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAreasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/areas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/areas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAreasRead(ref IQueryable<Area> items);

        public async Task<IQueryable<Area>> GetAreas(Query query = null)
        {
            var items = Context.Areas.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAreasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAreaGet(Area item);
        partial void OnGetAreaByAreaId(ref IQueryable<Area> items);

        public async Task<Area> GetAreaByAreaId(short areaid)
        {
            var items = Context.Areas
                              .AsNoTracking()
                              .Where(i => i.AREA_ID == areaid);

            OnGetAreaByAreaId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAreaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAreaCreated(Area item);
        partial void OnAfterAreaCreated(Area item);

        public async Task<Area> CreateArea(Area area)
        {
            OnAreaCreated(area);

            var existingItem = Context.Areas
                              .Where(i => i.AREA_ID == area.AREA_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Areas.Add(area);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(area).State = EntityState.Detached;
                throw;
            }

            OnAfterAreaCreated(area);

            return area;
        }

        public async Task<Area> CancelAreaChanges(Area item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAreaUpdated(Area item);
        partial void OnAfterAreaUpdated(Area item);

        public async Task<Area> UpdateArea(short areaid, Area area)
        {
            OnAreaUpdated(area);

            var itemToUpdate = Context.Areas
                              .Where(i => i.AREA_ID == area.AREA_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(area);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAreaUpdated(area);

            return area;
        }

        partial void OnAreaDeleted(Area item);
        partial void OnAfterAreaDeleted(Area item);

        public async Task<Area> DeleteArea(short areaid)
        {
            var itemToDelete = Context.Areas
                              .Where(i => i.AREA_ID == areaid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnAreaDeleted(itemToDelete);

            Context.Areas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAreaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportCitiesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/cities/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/cities/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCitiesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/cities/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/cities/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCitiesRead(ref IQueryable<City> items);

        public async Task<IQueryable<City>> GetCities(Query query = null)
        {
            var items = Context.Cities.AsQueryable();

            items = items.Include(i => i.Department);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCitiesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCityGet(City item);
        partial void OnGetCityByCityId(ref IQueryable<City> items);

        public async Task<City> GetCityByCityId(int cityid)
        {
            var items = Context.Cities
                              .AsNoTracking()
                              .Where(i => i.CITY_ID == cityid);

            items = items.Include(i => i.Department);

            OnGetCityByCityId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCityGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCityCreated(City item);
        partial void OnAfterCityCreated(City item);

        public async Task<City> CreateCity(City city)
        {
            OnCityCreated(city);

            var existingItem = Context.Cities
                              .Where(i => i.CITY_ID == city.CITY_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Cities.Add(city);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(city).State = EntityState.Detached;
                throw;
            }

            OnAfterCityCreated(city);

            return city;
        }

        public async Task<City> CancelCityChanges(City item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCityUpdated(City item);
        partial void OnAfterCityUpdated(City item);

        public async Task<City> UpdateCity(int cityid, City city)
        {
            OnCityUpdated(city);

            var itemToUpdate = Context.Cities
                              .Where(i => i.CITY_ID == city.CITY_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(city);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCityUpdated(city);

            return city;
        }

        partial void OnCityDeleted(City item);
        partial void OnAfterCityDeleted(City item);

        public async Task<City> DeleteCity(int cityid)
        {
            var itemToDelete = Context.Cities
                              .Where(i => i.CITY_ID == cityid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnCityDeleted(itemToDelete);

            Context.Cities.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCityDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportCountriesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/countries/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/countries/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCountriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/countries/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/countries/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCountriesRead(ref IQueryable<Country> items);

        public async Task<IQueryable<Country>> GetCountries(Query query = null)
        {
            var items = Context.Countries.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCountriesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCountryGet(Country item);
        partial void OnGetCountryByCountryId(ref IQueryable<Country> items);

        partial void OnCustomerOrderGet(CustomerOrder item);
        partial void OnGetCustomerOrderByCustomerOrderId(ref IQueryable<CustomerOrder> items);

        public async Task<CustomerOrder> GetCustomerOrdersById(int customerOrderId)
        {
            var items = Context.CustomerOrders
                              .AsNoTracking()
                              .Where(i => i.CUSTOMER_ORDER_ID == customerOrderId);

            OnGetCustomerOrderByCustomerOrderId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCustomerOrderGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        public async Task<Country> GetCountryByCountryId(int countryid)
        {
            var items = Context.Countries
                              .AsNoTracking()
                              .Where(i => i.COUNTRY_ID == countryid);

            OnGetCountryByCountryId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCountryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCountryCreated(Country item);
        partial void OnAfterCountryCreated(Country item);

        public async Task<Country> CreateCountry(Country country)
        {
            OnCountryCreated(country);

            var existingItem = Context.Countries
                              .Where(i => i.COUNTRY_ID == country.COUNTRY_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Countries.Add(country);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(country).State = EntityState.Detached;
                throw;
            }

            OnAfterCountryCreated(country);

            return country;
        }

        public async Task<Country> CancelCountryChanges(Country item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCountryUpdated(Country item);
        partial void OnAfterCountryUpdated(Country item);

        public async Task<Country> UpdateCountry(int countryid, Country country)
        {
            OnCountryUpdated(country);

            var itemToUpdate = Context.Countries
                              .Where(i => i.COUNTRY_ID == country.COUNTRY_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(country);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCountryUpdated(country);

            return country;
        }

        partial void OnCountryDeleted(Country item);
        partial void OnAfterCountryDeleted(Country item);

        public async Task<Country> DeleteCountry(int countryid)
        {
            var itemToDelete = Context.Countries
                              .Where(i => i.COUNTRY_ID == countryid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnCountryDeleted(itemToDelete);

            Context.Countries.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCountryDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportCurrenciesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/currencies/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/currencies/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCurrenciesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/currencies/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/currencies/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCurrenciesRead(ref IQueryable<Currency> items);

        public async Task<IQueryable<Currency>> GetCurrencies(Query query = null)
        {
            var items = Context.Currencies.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCurrenciesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCurrencyGet(Currency item);
        partial void OnGetCurrencyByCurrencyId(ref IQueryable<Currency> items);

        public async Task<Currency> GetCurrencyByCurrencyId(short currencyid)
        {
            var items = Context.Currencies
                              .AsNoTracking()
                              .Where(i => i.CURRENCY_ID == currencyid);

            OnGetCurrencyByCurrencyId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCurrencyGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCurrencyCreated(Currency item);
        partial void OnAfterCurrencyCreated(Currency item);

        public async Task<Currency> CreateCurrency(Currency currency)
        {
            OnCurrencyCreated(currency);

            var existingItem = Context.Currencies
                              .Where(i => i.CURRENCY_ID == currency.CURRENCY_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Currencies.Add(currency);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(currency).State = EntityState.Detached;
                throw;
            }

            OnAfterCurrencyCreated(currency);

            return currency;
        }

        public async Task<Currency> CancelCurrencyChanges(Currency item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCurrencyUpdated(Currency item);
        partial void OnAfterCurrencyUpdated(Currency item);

        public async Task<Currency> UpdateCurrency(short currencyid, Currency currency)
        {
            OnCurrencyUpdated(currency);

            var itemToUpdate = Context.Currencies
                              .Where(i => i.CURRENCY_ID == currency.CURRENCY_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(currency);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCurrencyUpdated(currency);

            return currency;
        }

        partial void OnCurrencyDeleted(Currency item);
        partial void OnAfterCurrencyDeleted(Currency item);

        public async Task<Currency> DeleteCurrency(short currencyid)
        {
            var itemToDelete = Context.Currencies
                              .Where(i => i.CURRENCY_ID == currencyid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnCurrencyDeleted(itemToDelete);

            Context.Currencies.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCurrencyDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportCustomerContactsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/customercontacts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/customercontacts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCustomerContactsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/customercontacts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/customercontacts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCustomerContactsRead(ref IQueryable<CustomerContact> items);

        public async Task<IQueryable<CustomerContact>> GetCustomerContacts(Query query = null)
        {
            var items = Context.CustomerContacts.AsQueryable();

            items = items.Include(i => i.Customer);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCustomerContactsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCustomerContactGet(CustomerContact item);
        partial void OnGetCustomerContactByCustomerContactId(ref IQueryable<CustomerContact> items);

        public async Task<CustomerContact> GetCustomerContactByCustomerContactId(int customercontactid)
        {
            var items = Context.CustomerContacts
                              .AsNoTracking()
                              .Where(i => i.CUSTOMER_CONTACT_ID == customercontactid);

            items = items.Include(i => i.Customer);

            OnGetCustomerContactByCustomerContactId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCustomerContactGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCustomerContactCreated(CustomerContact item);
        partial void OnAfterCustomerContactCreated(CustomerContact item);

        public async Task<CustomerContact> CreateCustomerContact(CustomerContact customercontact)
        {
            OnCustomerContactCreated(customercontact);

            var existingItem = Context.CustomerContacts
                              .Where(i => i.CUSTOMER_CONTACT_ID == customercontact.CUSTOMER_CONTACT_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.CustomerContacts.Add(customercontact);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(customercontact).State = EntityState.Detached;
                throw;
            }

            OnAfterCustomerContactCreated(customercontact);

            return customercontact;
        }

        public async Task<CustomerContact> CancelCustomerContactChanges(CustomerContact item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCustomerContactUpdated(CustomerContact item);
        partial void OnAfterCustomerContactUpdated(CustomerContact item);

        public async Task<CustomerContact> UpdateCustomerContact(int customercontactid, CustomerContact customercontact)
        {
            OnCustomerContactUpdated(customercontact);

            var itemToUpdate = Context.CustomerContacts
                              .Where(i => i.CUSTOMER_CONTACT_ID == customercontact.CUSTOMER_CONTACT_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(customercontact);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCustomerContactUpdated(customercontact);

            return customercontact;
        }

        partial void OnCustomerContactDeleted(CustomerContact item);
        partial void OnAfterCustomerContactDeleted(CustomerContact item);

        public async Task<CustomerContact> DeleteCustomerContact(int customercontactid)
        {
            var itemToDelete = Context.CustomerContacts
                              .Where(i => i.CUSTOMER_CONTACT_ID == customercontactid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnCustomerContactDeleted(itemToDelete);

            Context.CustomerContacts.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCustomerContactDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportCustomersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/customers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/customers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCustomersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/customers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/customers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCustomersRead(ref IQueryable<Customer> items);

        public async Task<IQueryable<Customer>> GetCustomers(Query query = null)
        {
            var items = Context.Customers.AsQueryable();

            items = items.Include(i => i.City);
            items = items.Include(i => i.IdentityType);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCustomersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCustomerGet(Customer item);
        partial void OnGetCustomerByCustomerId(ref IQueryable<Customer> items);

        public async Task<Customer> GetCustomerByCustomerId(int customerid)
        {
            var items = Context.Customers
                              .AsNoTracking()
                              .Where(i => i.CUSTOMER_ID == customerid);

            items = items.Include(i => i.City);
            items = items.Include(i => i.IdentityType);

            OnGetCustomerByCustomerId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCustomerGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCustomerCreated(Customer item);
        partial void OnAfterCustomerCreated(Customer item);

        public async Task<Customer> CreateCustomer(Customer customer)
        {
            OnCustomerCreated(customer);

            var existingItem = Context.Customers
                              .Where(i => i.CUSTOMER_ID == customer.CUSTOMER_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Customers.Add(customer);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(customer).State = EntityState.Detached;
                throw;
            }

            OnAfterCustomerCreated(customer);

            return customer;
        }

        public async Task<Customer> CancelCustomerChanges(Customer item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCustomerUpdated(Customer item);
        partial void OnAfterCustomerUpdated(Customer item);

        public async Task<Customer> UpdateCustomer(int customerid, Customer customer)
        {
            OnCustomerUpdated(customer);

            var itemToUpdate = Context.Customers
                              .Where(i => i.CUSTOMER_ID == customer.CUSTOMER_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(customer);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCustomerUpdated(customer);

            return customer;
        }

        partial void OnCustomerDeleted(Customer item);
        partial void OnAfterCustomerDeleted(Customer item);

        public async Task<Customer> DeleteCustomer(int customerid)
        {
            var itemToDelete = Context.Customers
                              .Where(i => i.CUSTOMER_ID == customerid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnCustomerDeleted(itemToDelete);

            Context.Customers.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCustomerDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportDepartmentsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/departments/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/departments/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportDepartmentsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/departments/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/departments/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnDepartmentsRead(ref IQueryable<Department> items);

        public async Task<IQueryable<Department>> GetDepartments(Query query = null)
        {
            var items = Context.Departments.AsQueryable();

            items = items.Include(i => i.Country);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnDepartmentsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnDepartmentGet(Department item);
        partial void OnGetDepartmentByDepartmentId(ref IQueryable<Department> items);

        public async Task<Department> GetDepartmentByDepartmentId(int departmentid)
        {
            var items = Context.Departments
                              .AsNoTracking()
                              .Where(i => i.DEPARTMENT_ID == departmentid);

            items = items.Include(i => i.Country);

            OnGetDepartmentByDepartmentId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnDepartmentGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnDepartmentCreated(Department item);
        partial void OnAfterDepartmentCreated(Department item);

        public async Task<Department> CreateDepartment(Department department)
        {
            OnDepartmentCreated(department);

            var existingItem = Context.Departments
                              .Where(i => i.DEPARTMENT_ID == department.DEPARTMENT_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Departments.Add(department);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(department).State = EntityState.Detached;
                throw;
            }

            OnAfterDepartmentCreated(department);

            return department;
        }

        public async Task<Department> CancelDepartmentChanges(Department item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnDepartmentUpdated(Department item);
        partial void OnAfterDepartmentUpdated(Department item);

        public async Task<Department> UpdateDepartment(int departmentid, Department department)
        {
            OnDepartmentUpdated(department);

            var itemToUpdate = Context.Departments
                              .Where(i => i.DEPARTMENT_ID == department.DEPARTMENT_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(department);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterDepartmentUpdated(department);

            return department;
        }

        partial void OnDepartmentDeleted(Department item);
        partial void OnAfterDepartmentDeleted(Department item);

        public async Task<Department> DeleteDepartment(int departmentid)
        {
            var itemToDelete = Context.Departments
                              .Where(i => i.DEPARTMENT_ID == departmentid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnDepartmentDeleted(itemToDelete);

            Context.Departments.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterDepartmentDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportEmployeesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/employees/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/employees/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportEmployeesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/employees/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/employees/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnActivityTypesAreaRead(ref IQueryable<ActivityTypeArea> items);

        public async Task<IQueryable<ActivityTypeArea>> GetActivityTypesArea(Query query = null)
        {
            var items = Context.ActivityTypesAreas.AsQueryable();

            items = items.Include(i => i.ActivityType);
            items = items.Include(i => i.Area);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnActivityTypesAreaRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnEmployeesRead(ref IQueryable<Employee> items);

        public async Task<IQueryable<Employee>> GetEmployees(Query query = null)
        {
            var items = Context.Employees.AsQueryable();

            items = items.Include(i => i.Area);
            items = items.Include(i => i.IdentityType);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnEmployeesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnEmployeeGet(Employee item);
        partial void OnGetEmployeeByEmployeeId(ref IQueryable<Employee> items);

        public async Task<Employee> GetEmployeeByEmployeeId(int employeeid)
        {
            var items = Context.Employees
                              .AsNoTracking()
                              .Where(i => i.EMPLOYEE_ID == employeeid);

            items = items.Include(i => i.Area);
            items = items.Include(i => i.IdentityType);

            OnGetEmployeeByEmployeeId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnEmployeeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnEmployeeCreated(Employee item);
        partial void OnAfterEmployeeCreated(Employee item);

        public async Task<Employee> CreateEmployee(Employee employee)
        {
            OnEmployeeCreated(employee);

            var existingItem = Context.Employees
                              .Where(i => i.EMPLOYEE_ID == employee.EMPLOYEE_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Employees.Add(employee);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(employee).State = EntityState.Detached;
                throw;
            }

            OnAfterEmployeeCreated(employee);

            return employee;
        }

        public async Task<Employee> CancelEmployeeChanges(Employee item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnEmployeeUpdated(Employee item);
        partial void OnAfterEmployeeUpdated(Employee item);

        public async Task<Employee> UpdateEmployee(int employeeid, Employee employee)
        {
            OnEmployeeUpdated(employee);

            var itemToUpdate = Context.Employees
                              .Where(i => i.EMPLOYEE_ID == employee.EMPLOYEE_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(employee);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterEmployeeUpdated(employee);

            return employee;
        }

        partial void OnEmployeeDeleted(Employee item);
        partial void OnAfterEmployeeDeleted(Employee item);

        public async Task<Employee> DeleteEmployee(int employeeid)
        {
            var itemToDelete = Context.Employees
                              .Where(i => i.EMPLOYEE_ID == employeeid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnEmployeeDeleted(itemToDelete);

            Context.Employees.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterEmployeeDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportForwarderAgentsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/forwarderagents/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/forwarderagents/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportForwarderAgentsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/forwarderagents/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/forwarderagents/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnForwarderAgentsRead(ref IQueryable<ForwarderAgent> items);

        public async Task<IQueryable<ForwarderAgent>> GetForwarderAgents(Query query = null)
        {
            var items = Context.ForwarderAgents.AsQueryable();

            items = items.Include(i => i.City);
            items = items.Include(i => i.Forwarder);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnForwarderAgentsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnForwarderAgentGet(ForwarderAgent item);
        partial void OnGetForwarderAgentByForwarderAgentId(ref IQueryable<ForwarderAgent> items);

        public async Task<ForwarderAgent> GetForwarderAgentByForwarderAgentId(int forwarderagentid)
        {
            var items = Context.ForwarderAgents
                              .AsNoTracking()
                              .Where(i => i.FORWARDER_AGENT_ID == forwarderagentid);

            items = items.Include(i => i.City);
            items = items.Include(i => i.Forwarder);

            OnGetForwarderAgentByForwarderAgentId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnForwarderAgentGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnForwarderAgentCreated(ForwarderAgent item);
        partial void OnAfterForwarderAgentCreated(ForwarderAgent item);

        public async Task<ForwarderAgent> CreateForwarderAgent(ForwarderAgent forwarderagent)
        {
            OnForwarderAgentCreated(forwarderagent);

            var existingItem = Context.ForwarderAgents
                              .Where(i => i.FORWARDER_AGENT_ID == forwarderagent.FORWARDER_AGENT_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.ForwarderAgents.Add(forwarderagent);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(forwarderagent).State = EntityState.Detached;
                throw;
            }

            OnAfterForwarderAgentCreated(forwarderagent);

            return forwarderagent;
        }

        public async Task<ForwarderAgent> CancelForwarderAgentChanges(ForwarderAgent item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnForwarderAgentUpdated(ForwarderAgent item);
        partial void OnAfterForwarderAgentUpdated(ForwarderAgent item);

        public async Task<ForwarderAgent> UpdateForwarderAgent(int forwarderagentid, ForwarderAgent forwarderagent)
        {
            OnForwarderAgentUpdated(forwarderagent);

            var itemToUpdate = Context.ForwarderAgents
                              .Where(i => i.FORWARDER_AGENT_ID == forwarderagent.FORWARDER_AGENT_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(forwarderagent);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterForwarderAgentUpdated(forwarderagent);

            return forwarderagent;
        }

        partial void OnForwarderAgentDeleted(ForwarderAgent item);
        partial void OnAfterForwarderAgentDeleted(ForwarderAgent item);

        public async Task<ForwarderAgent> DeleteForwarderAgent(int forwarderagentid)
        {
            var itemToDelete = Context.ForwarderAgents
                              .Where(i => i.FORWARDER_AGENT_ID == forwarderagentid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnForwarderAgentDeleted(itemToDelete);

            Context.ForwarderAgents.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterForwarderAgentDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportForwardersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/forwarders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/forwarders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportForwardersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/forwarders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/forwarders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnForwardersRead(ref IQueryable<Forwarder> items);

        public async Task<IQueryable<Forwarder>> GetForwarders(Query query = null)
        {
            var items = Context.Forwarders.AsQueryable();

            items = items.Include(i => i.City);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnForwardersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnForwarderGet(Forwarder item);
        partial void OnGetForwarderByForwarderId(ref IQueryable<Forwarder> items);

        public async Task<Forwarder> GetForwarderByForwarderId(int forwarderid)
        {
            var items = Context.Forwarders
                              .AsNoTracking()
                              .Where(i => i.FORWARDER_ID == forwarderid);

            items = items.Include(i => i.City);

            OnGetForwarderByForwarderId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnForwarderGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnForwarderCreated(Forwarder item);
        partial void OnAfterForwarderCreated(Forwarder item);

        public async Task<Forwarder> CreateForwarder(Forwarder forwarder)
        {
            OnForwarderCreated(forwarder);

            var existingItem = Context.Forwarders
                              .Where(i => i.FORWARDER_ID == forwarder.FORWARDER_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Forwarders.Add(forwarder);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(forwarder).State = EntityState.Detached;
                throw;
            }

            OnAfterForwarderCreated(forwarder);

            return forwarder;
        }

        public async Task<Forwarder> CancelForwarderChanges(Forwarder item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnForwarderUpdated(Forwarder item);
        partial void OnAfterForwarderUpdated(Forwarder item);

        public async Task<Forwarder> UpdateForwarder(int forwarderid, Forwarder forwarder)
        {
            OnForwarderUpdated(forwarder);

            var itemToUpdate = Context.Forwarders
                              .Where(i => i.FORWARDER_ID == forwarder.FORWARDER_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(forwarder);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterForwarderUpdated(forwarder);

            return forwarder;
        }

        partial void OnForwarderDeleted(Forwarder item);
        partial void OnAfterForwarderDeleted(Forwarder item);

        public async Task<Forwarder> DeleteForwarder(int forwarderid)
        {
            var itemToDelete = Context.Forwarders
                              .Where(i => i.FORWARDER_ID == forwarderid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnForwarderDeleted(itemToDelete);

            Context.Forwarders.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterForwarderDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportIdentityTypesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/identitytypes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/identitytypes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportIdentityTypesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/identitytypes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/identitytypes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnIdentityTypesRead(ref IQueryable<IdentityType> items);

        public async Task<IQueryable<IdentityType>> GetIdentityTypes(Query query = null)
        {
            var items = Context.IdentityTypes.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnIdentityTypesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnIdentityTypeGet(IdentityType item);
        partial void OnGetIdentityTypeByIdentityTypeId(ref IQueryable<IdentityType> items);

        public async Task<IdentityType> GetIdentityTypeByIdentityTypeId(int identitytypeid)
        {
            var items = Context.IdentityTypes
                              .AsNoTracking()
                              .Where(i => i.IDENTITY_TYPE_ID == identitytypeid);

            OnGetIdentityTypeByIdentityTypeId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnIdentityTypeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnIdentityTypeCreated(IdentityType item);
        partial void OnAfterIdentityTypeCreated(IdentityType item);

        public async Task<IdentityType> CreateIdentityType(IdentityType identitytype)
        {
            OnIdentityTypeCreated(identitytype);

            var existingItem = Context.IdentityTypes
                              .Where(i => i.IDENTITY_TYPE_ID == identitytype.IDENTITY_TYPE_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.IdentityTypes.Add(identitytype);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(identitytype).State = EntityState.Detached;
                throw;
            }

            OnAfterIdentityTypeCreated(identitytype);

            return identitytype;
        }

        public async Task<IdentityType> CancelIdentityTypeChanges(IdentityType item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnIdentityTypeUpdated(IdentityType item);
        partial void OnAfterIdentityTypeUpdated(IdentityType item);

        public async Task<IdentityType> UpdateIdentityType(int identitytypeid, IdentityType identitytype)
        {
            OnIdentityTypeUpdated(identitytype);

            var itemToUpdate = Context.IdentityTypes
                              .Where(i => i.IDENTITY_TYPE_ID == identitytype.IDENTITY_TYPE_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(identitytype);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterIdentityTypeUpdated(identitytype);

            return identitytype;
        }

        partial void OnIdentityTypeDeleted(IdentityType item);
        partial void OnAfterIdentityTypeDeleted(IdentityType item);

        public async Task<IdentityType> DeleteIdentityType(int identitytypeid)
        {
            var itemToDelete = Context.IdentityTypes
                              .Where(i => i.IDENTITY_TYPE_ID == identitytypeid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnIdentityTypeDeleted(itemToDelete);

            Context.IdentityTypes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterIdentityTypeDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportItemReferencesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/itemreferences/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/itemreferences/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportItemReferencesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/itemreferences/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/itemreferences/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnItemReferencesRead(ref IQueryable<ItemReference> items);

        public async Task<IQueryable<ItemReference>> GetItemReferences(Query query = null)
        {
            var items = Context.ItemReferences.AsQueryable();

            items = items.Include(i => i.Item);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnItemReferencesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnItemReferenceGet(ItemReference item);
        partial void OnGetItemReferenceByReferenceId(ref IQueryable<ItemReference> items);

        public async Task<ItemReference> GetItemReferenceByReferenceId(int referenceid)
        {
            var items = Context.ItemReferences
                              .AsNoTracking()
                              .Where(i => i.REFERENCE_ID == referenceid);

            items = items.Include(i => i.Item);

            OnGetItemReferenceByReferenceId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnItemReferenceGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnItemReferenceCreated(ItemReference item);
        partial void OnAfterItemReferenceCreated(ItemReference item);

        public async Task<ItemReference> CreateItemReference(ItemReference itemreference)
        {
            OnItemReferenceCreated(itemreference);

            var existingItem = Context.ItemReferences
                              .Where(i => i.REFERENCE_ID == itemreference.REFERENCE_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.ItemReferences.Add(itemreference);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemreference).State = EntityState.Detached;
                throw;
            }

            OnAfterItemReferenceCreated(itemreference);

            return itemreference;
        }

        public async Task<ItemReference> CancelItemReferenceChanges(ItemReference item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItemReferenceUpdated(ItemReference item);
        partial void OnAfterItemReferenceUpdated(ItemReference item);

        public async Task<ItemReference> UpdateItemReference(int referenceid, ItemReference itemreference)
        {
            OnItemReferenceUpdated(itemreference);

            var itemToUpdate = Context.ItemReferences
                              .Where(i => i.REFERENCE_ID == itemreference.REFERENCE_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(itemreference);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterItemReferenceUpdated(itemreference);

            return itemreference;
        }

        partial void OnItemReferenceDeleted(ItemReference item);
        partial void OnAfterItemReferenceDeleted(ItemReference item);

        public async Task<ItemReference> DeleteItemReference(int referenceid)
        {
            var itemToDelete = Context.ItemReferences
                              .Where(i => i.REFERENCE_ID == referenceid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnItemReferenceDeleted(itemToDelete);

            Context.ItemReferences.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterItemReferenceDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportItemsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/items/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/items/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportItemsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/items/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/items/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnItemsRead(ref IQueryable<Item> items);

        public async Task<IQueryable<Item>> GetItems(Query query = null)
        {
            var items = Context.Items.AsQueryable();

            items = items.Include(i => i.MeasureUnit);
            items = items.Include(i => i.Currency);
            items = items.Include(i => i.MeasureUnit1);
            items = items.Include(i => i.Line);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnItemsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnItemGet(Item item);
        partial void OnGetItemByItemId(ref IQueryable<Item> items);

        public async Task<Item> GetItemByItemId(int itemid)
        {
            var items = Context.Items
                              .AsNoTracking()
                              .Where(i => i.ITEM_ID == itemid);

            items = items.Include(i => i.MeasureUnit);
            items = items.Include(i => i.Currency);
            items = items.Include(i => i.MeasureUnit1);
            items = items.Include(i => i.Line);

            OnGetItemByItemId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnItemGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnItemCreated(Item item);
        partial void OnAfterItemCreated(Item item);

        public async Task<Item> CreateItem(Item item)
        {
            OnItemCreated(item);

            var existingItem = Context.Items
                              .Where(i => i.ITEM_ID == item.ITEM_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Items.Add(item);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(item).State = EntityState.Detached;
                throw;
            }

            OnAfterItemCreated(item);

            return item;
        }

        public async Task<Item> CancelItemChanges(Item item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItemUpdated(Item item);
        partial void OnAfterItemUpdated(Item item);

        public async Task<Item> UpdateItem(int itemid, Item item)
        {
            OnItemUpdated(item);

            var itemToUpdate = Context.Items
                              .Where(i => i.ITEM_ID == item.ITEM_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(item);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterItemUpdated(item);

            return item;
        }

        partial void OnItemDeleted(Item item);
        partial void OnAfterItemDeleted(Item item);

        public async Task<Item> DeleteItem(int itemid)
        {
            var itemToDelete = Context.Items
                              .Where(i => i.ITEM_ID == itemid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnItemDeleted(itemToDelete);

            Context.Items.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterItemDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportItemsAreasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/itemsareas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/itemsareas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportItemsAreasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/itemsareas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/itemsareas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnItemsAreasRead(ref IQueryable<ItemsArea> items);

        public async Task<IQueryable<ItemsArea>> GetItemsAreas(Query query = null)
        {
            var items = Context.ItemsAreas.AsQueryable();

            items = items.Include(i => i.Area);
            items = items.Include(i => i.Item);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnItemsAreasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnItemsAreaGet(ItemsArea item);
        partial void OnGetItemsAreaByItemIdAndAreaId(ref IQueryable<ItemsArea> items);

        public async Task<ItemsArea> GetItemsAreaByItemIdAndAreaId(int itemid, short areaid)
        {
            var items = Context.ItemsAreas
                              .AsNoTracking()
                              .Where(i => i.ITEM_ID == itemid && i.AREA_ID == areaid);

            items = items.Include(i => i.Area);
            items = items.Include(i => i.Item);

            OnGetItemsAreaByItemIdAndAreaId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnItemsAreaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnItemsAreaCreated(ItemsArea item);
        partial void OnAfterItemsAreaCreated(ItemsArea item);

        public async Task<ItemsArea> CreateItemsArea(ItemsArea itemsarea)
        {
            OnItemsAreaCreated(itemsarea);

            var existingItem = Context.ItemsAreas
                              .Where(i => i.ITEM_ID == itemsarea.ITEM_ID && i.AREA_ID == itemsarea.AREA_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.ItemsAreas.Add(itemsarea);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemsarea).State = EntityState.Detached;
                throw;
            }

            OnAfterItemsAreaCreated(itemsarea);

            return itemsarea;
        }

        public async Task<ItemsArea> CancelItemsAreaChanges(ItemsArea item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItemsAreaUpdated(ItemsArea item);
        partial void OnAfterItemsAreaUpdated(ItemsArea item);

        public async Task<ItemsArea> UpdateItemsArea(int itemid, short areaid, ItemsArea itemsarea)
        {
            OnItemsAreaUpdated(itemsarea);

            var itemToUpdate = Context.ItemsAreas
                              .Where(i => i.ITEM_ID == itemsarea.ITEM_ID && i.AREA_ID == itemsarea.AREA_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(itemsarea);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterItemsAreaUpdated(itemsarea);

            return itemsarea;
        }

        partial void OnItemsAreaDeleted(ItemsArea item);
        partial void OnAfterItemsAreaDeleted(ItemsArea item);

        public async Task<ItemsArea> DeleteItemsArea(int itemid, short areaid)
        {
            var itemToDelete = Context.ItemsAreas
                              .Where(i => i.ITEM_ID == itemid && i.AREA_ID == areaid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnItemsAreaDeleted(itemToDelete);

            Context.ItemsAreas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterItemsAreaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportLinesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/lines/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/lines/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportLinesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/lines/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/lines/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnLinesRead(ref IQueryable<Line> items);

        public async Task<IQueryable<Line>> GetLines(Query query = null)
        {
            var items = Context.Lines.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnLinesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnLineGet(Line item);
        partial void OnGetLineByLineId(ref IQueryable<Line> items);

        public async Task<Line> GetLineByLineId(short lineid)
        {
            var items = Context.Lines
                              .AsNoTracking()
                              .Where(i => i.LINE_ID == lineid);

            OnGetLineByLineId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnLineGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnLineCreated(Line item);
        partial void OnAfterLineCreated(Line item);

        public async Task<Line> CreateLine(Line line)
        {
            OnLineCreated(line);

            var existingItem = Context.Lines
                              .Where(i => i.LINE_ID == line.LINE_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Lines.Add(line);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(line).State = EntityState.Detached;
                throw;
            }

            OnAfterLineCreated(line);

            return line;
        }

        public async Task<Line> CancelLineChanges(Line item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnLineUpdated(Line item);
        partial void OnAfterLineUpdated(Line item);

        public async Task<Line> UpdateLine(short lineid, Line line)
        {
            OnLineUpdated(line);

            var itemToUpdate = Context.Lines
                              .Where(i => i.LINE_ID == line.LINE_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(line);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterLineUpdated(line);

            return line;
        }

        partial void OnLineDeleted(Line item);
        partial void OnAfterLineDeleted(Line item);

        public async Task<Line> DeleteLine(short lineid)
        {
            var itemToDelete = Context.Lines
                              .Where(i => i.LINE_ID == lineid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnLineDeleted(itemToDelete);

            Context.Lines.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterLineDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportMeasureUnitsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/measureunits/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/measureunits/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportMeasureUnitsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/measureunits/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/measureunits/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnMeasureUnitsRead(ref IQueryable<MeasureUnit> items);

        public async Task<IQueryable<MeasureUnit>> GetMeasureUnits(Query query = null)
        {
            var items = Context.MeasureUnits.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnMeasureUnitsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnMeasureUnitGet(MeasureUnit item);
        partial void OnGetMeasureUnitByMeasureUnitId(ref IQueryable<MeasureUnit> items);

        public async Task<MeasureUnit> GetMeasureUnitByMeasureUnitId(short measureunitid)
        {
            var items = Context.MeasureUnits
                              .AsNoTracking()
                              .Where(i => i.MEASURE_UNIT_ID == measureunitid);

            OnGetMeasureUnitByMeasureUnitId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnMeasureUnitGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnMeasureUnitCreated(MeasureUnit item);
        partial void OnAfterMeasureUnitCreated(MeasureUnit item);

        public async Task<MeasureUnit> CreateMeasureUnit(MeasureUnit measureunit)
        {
            OnMeasureUnitCreated(measureunit);

            var existingItem = Context.MeasureUnits
                              .Where(i => i.MEASURE_UNIT_ID == measureunit.MEASURE_UNIT_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.MeasureUnits.Add(measureunit);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(measureunit).State = EntityState.Detached;
                throw;
            }

            OnAfterMeasureUnitCreated(measureunit);

            return measureunit;
        }

        public async Task<MeasureUnit> CancelMeasureUnitChanges(MeasureUnit item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnMeasureUnitUpdated(MeasureUnit item);
        partial void OnAfterMeasureUnitUpdated(MeasureUnit item);

        public async Task<MeasureUnit> UpdateMeasureUnit(short measureunitid, MeasureUnit measureunit)
        {
            OnMeasureUnitUpdated(measureunit);

            var itemToUpdate = Context.MeasureUnits
                              .Where(i => i.MEASURE_UNIT_ID == measureunit.MEASURE_UNIT_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(measureunit);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterMeasureUnitUpdated(measureunit);

            return measureunit;
        }

        partial void OnMeasureUnitDeleted(MeasureUnit item);
        partial void OnAfterMeasureUnitDeleted(MeasureUnit item);

        public async Task<MeasureUnit> DeleteMeasureUnit(short measureunitid)
        {
            var itemToDelete = Context.MeasureUnits
                              .Where(i => i.MEASURE_UNIT_ID == measureunitid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnMeasureUnitDeleted(itemToDelete);

            Context.MeasureUnits.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterMeasureUnitDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportProviderReferencesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/providerreferences/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/providerreferences/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportProviderReferencesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/providerreferences/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/providerreferences/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnProviderReferencesRead(ref IQueryable<ProviderReference> items);

        public async Task<IQueryable<ProviderReference>> GetProviderReferences(Query query = null)
        {
            var items = Context.ProviderReferences.AsQueryable();

            items = items.Include(i => i.Provider);
            items = items.Include(i => i.ItemReference);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnProviderReferencesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnProviderReferenceGet(ProviderReference item);
        partial void OnGetProviderReferenceByReferenceIdAndProviderId(ref IQueryable<ProviderReference> items);

        public async Task<ProviderReference> GetProviderReferenceByReferenceIdAndProviderId(int referenceid, int providerid)
        {
            var items = Context.ProviderReferences
                              .AsNoTracking()
                              .Where(i => i.REFERENCE_ID == referenceid && i.PROVIDER_ID == providerid);

            items = items.Include(i => i.Provider);
            items = items.Include(i => i.ItemReference);

            OnGetProviderReferenceByReferenceIdAndProviderId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnProviderReferenceGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnProviderReferenceCreated(ProviderReference item);
        partial void OnAfterProviderReferenceCreated(ProviderReference item);

        public async Task<ProviderReference> CreateProviderReference(ProviderReference providerreference)
        {
            OnProviderReferenceCreated(providerreference);

            var existingItem = Context.ProviderReferences
                              .Where(i => i.REFERENCE_ID == providerreference.REFERENCE_ID && i.PROVIDER_ID == providerreference.PROVIDER_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.ProviderReferences.Add(providerreference);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(providerreference).State = EntityState.Detached;
                throw;
            }

            OnAfterProviderReferenceCreated(providerreference);

            return providerreference;
        }

        public async Task<ProviderReference> CancelProviderReferenceChanges(ProviderReference item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnProviderReferenceUpdated(ProviderReference item);
        partial void OnAfterProviderReferenceUpdated(ProviderReference item);

        public async Task<ProviderReference> UpdateProviderReference(int referenceid, int providerid, ProviderReference providerreference)
        {
            OnProviderReferenceUpdated(providerreference);

            var itemToUpdate = Context.ProviderReferences
                              .Where(i => i.REFERENCE_ID == providerreference.REFERENCE_ID && i.PROVIDER_ID == providerreference.PROVIDER_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(providerreference);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterProviderReferenceUpdated(providerreference);

            return providerreference;
        }

        partial void OnProviderReferenceDeleted(ProviderReference item);
        partial void OnAfterProviderReferenceDeleted(ProviderReference item);

        public async Task<ProviderReference> DeleteProviderReference(int referenceid, int providerid)
        {
            var itemToDelete = Context.ProviderReferences
                              .Where(i => i.REFERENCE_ID == referenceid && i.PROVIDER_ID == providerid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnProviderReferenceDeleted(itemToDelete);

            Context.ProviderReferences.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterProviderReferenceDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportProvidersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/providers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/providers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportProvidersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/providers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/providers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnProvidersRead(ref IQueryable<Provider> items);

        public async Task<IQueryable<Provider>> GetProviders(Query query = null)
        {
            var items = Context.Providers.AsQueryable();

            items = items.Include(i => i.City);
            items = items.Include(i => i.IdentityType);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnProvidersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnProviderGet(Provider item);
        partial void OnGetProviderByProviderId(ref IQueryable<Provider> items);

        public async Task<Provider> GetProviderByProviderId(int providerid)
        {
            var items = Context.Providers
                              .AsNoTracking()
                              .Where(i => i.PROVIDER_ID == providerid);

            items = items.Include(i => i.City);
            items = items.Include(i => i.IdentityType);

            OnGetProviderByProviderId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnProviderGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnProviderCreated(Provider item);
        partial void OnAfterProviderCreated(Provider item);

        public async Task<Provider> CreateProvider(Provider provider)
        {
            OnProviderCreated(provider);

            var existingItem = Context.Providers
                              .Where(i => i.PROVIDER_ID == provider.PROVIDER_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Providers.Add(provider);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(provider).State = EntityState.Detached;
                throw;
            }

            OnAfterProviderCreated(provider);

            return provider;
        }

        public async Task<Provider> CancelProviderChanges(Provider item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnProviderUpdated(Provider item);
        partial void OnAfterProviderUpdated(Provider item);

        public async Task<Provider> UpdateProvider(int providerid, Provider provider)
        {
            OnProviderUpdated(provider);

            var itemToUpdate = Context.Providers
                              .Where(i => i.PROVIDER_ID == provider.PROVIDER_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(provider);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterProviderUpdated(provider);

            return provider;
        }

        partial void OnProviderDeleted(Provider item);
        partial void OnAfterProviderDeleted(Provider item);

        public async Task<Provider> DeleteProvider(int providerid)
        {
            var itemToDelete = Context.Providers
                              .Where(i => i.PROVIDER_ID == providerid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnProviderDeleted(itemToDelete);

            Context.Providers.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterProviderDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportPurchaseOrderActivitiesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/purchaseorderactivities/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/purchaseorderactivities/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportPurchaseOrderActivitiesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/purchaseorderactivities/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/purchaseorderactivities/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnPurchaseOrderActivitiesRead(ref IQueryable<PurchaseOrderActivity> items);

        public async Task<IQueryable<PurchaseOrderActivity>> GetPurchaseOrderActivities(Query query = null)
        {
            var items = Context.PurchaseOrderActivities.AsQueryable();

            items = items.Include(i => i.ActivityEmployee);
            items = items.Include(i => i.Employee);
            items = items.Include(i => i.PurchaseOrder);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnPurchaseOrderActivitiesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnPurchaseOrderActivityGet(PurchaseOrderActivity item);
        partial void OnGetPurchaseOrderActivityByPurchaseOrderActivityId(ref IQueryable<PurchaseOrderActivity> items);

        public async Task<PurchaseOrderActivity> GetPurchaseOrderActivityByPurchaseOrderActivityId(int purchaseorderactivityid)
        {
            var items = Context.PurchaseOrderActivities
                              .AsNoTracking()
                              .Where(i => i.PURCHASE_ORDER_ACTIVITY_ID == purchaseorderactivityid);

            items = items.Include(i => i.ActivityEmployee);
            items = items.Include(i => i.Employee);
            items = items.Include(i => i.PurchaseOrder);

            OnGetPurchaseOrderActivityByPurchaseOrderActivityId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnPurchaseOrderActivityGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnPurchaseOrderActivityCreated(PurchaseOrderActivity item);
        partial void OnAfterPurchaseOrderActivityCreated(PurchaseOrderActivity item);

        public async Task<PurchaseOrderActivity> CreatePurchaseOrderActivity(PurchaseOrderActivity purchaseorderactivity)
        {
            OnPurchaseOrderActivityCreated(purchaseorderactivity);

            var existingItem = Context.PurchaseOrderActivities
                              .Where(i => i.PURCHASE_ORDER_ACTIVITY_ID == purchaseorderactivity.PURCHASE_ORDER_ACTIVITY_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.PurchaseOrderActivities.Add(purchaseorderactivity);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(purchaseorderactivity).State = EntityState.Detached;
                throw;
            }

            OnAfterPurchaseOrderActivityCreated(purchaseorderactivity);

            return purchaseorderactivity;
        }

        public async Task<PurchaseOrderActivity> CancelPurchaseOrderActivityChanges(PurchaseOrderActivity item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnPurchaseOrderActivityUpdated(PurchaseOrderActivity item);
        partial void OnAfterPurchaseOrderActivityUpdated(PurchaseOrderActivity item);

        public async Task<PurchaseOrderActivity> UpdatePurchaseOrderActivity(int purchaseorderactivityid, PurchaseOrderActivity purchaseorderactivity)
        {
            OnPurchaseOrderActivityUpdated(purchaseorderactivity);

            var itemToUpdate = Context.PurchaseOrderActivities
                              .Where(i => i.PURCHASE_ORDER_ACTIVITY_ID == purchaseorderactivity.PURCHASE_ORDER_ACTIVITY_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(purchaseorderactivity);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterPurchaseOrderActivityUpdated(purchaseorderactivity);

            return purchaseorderactivity;
        }

        partial void OnPurchaseOrderActivityDeleted(PurchaseOrderActivity item);
        partial void OnAfterPurchaseOrderActivityDeleted(PurchaseOrderActivity item);

        public async Task<PurchaseOrderActivity> DeletePurchaseOrderActivity(int purchaseorderactivityid)
        {
            var itemToDelete = Context.PurchaseOrderActivities
                              .Where(i => i.PURCHASE_ORDER_ACTIVITY_ID == purchaseorderactivityid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnPurchaseOrderActivityDeleted(itemToDelete);

            Context.PurchaseOrderActivities.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterPurchaseOrderActivityDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportPurchaseOrderDetailsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/purchaseorderdetails/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/purchaseorderdetails/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportPurchaseOrderDetailsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/purchaseorderdetails/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/purchaseorderdetails/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnPurchaseOrderDetailsRead(ref IQueryable<PurchaseOrderDetail> items);

        public async Task<IQueryable<PurchaseOrderDetail>> GetPurchaseOrderDetails(Query query = null)
        {
            var items = Context.PurchaseOrderDetails.AsQueryable();

            items = items.Include(i => i.PurchaseOrder);
            items = items.Include(i => i.ItemReference);
            items = items.Include(i => i.Warehouse);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnPurchaseOrderDetailsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnPurchaseOrderDetailGet(PurchaseOrderDetail item);
        partial void OnGetPurchaseOrderDetailByPurchaseOrderDetailId(ref IQueryable<PurchaseOrderDetail> items);

        public async Task<PurchaseOrderDetail> GetPurchaseOrderDetailByPurchaseOrderDetailId(int purchaseorderdetailid)
        {
            var items = Context.PurchaseOrderDetails
                              .AsNoTracking()
                              .Where(i => i.PURCHASE_ORDER_DETAIL_ID == purchaseorderdetailid);

            items = items.Include(i => i.PurchaseOrder);
            items = items.Include(i => i.ItemReference);
            items = items.Include(i => i.Warehouse);

            OnGetPurchaseOrderDetailByPurchaseOrderDetailId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnPurchaseOrderDetailGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnPurchaseOrderDetailCreated(PurchaseOrderDetail item);
        partial void OnAfterPurchaseOrderDetailCreated(PurchaseOrderDetail item);

        public async Task<PurchaseOrderDetail> CreatePurchaseOrderDetail(PurchaseOrderDetail purchaseorderdetail)
        {
            OnPurchaseOrderDetailCreated(purchaseorderdetail);

            var existingItem = Context.PurchaseOrderDetails
                              .Where(i => i.PURCHASE_ORDER_DETAIL_ID == purchaseorderdetail.PURCHASE_ORDER_DETAIL_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.PurchaseOrderDetails.Add(purchaseorderdetail);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(purchaseorderdetail).State = EntityState.Detached;
                throw;
            }

            OnAfterPurchaseOrderDetailCreated(purchaseorderdetail);

            return purchaseorderdetail;
        }

        public async Task<PurchaseOrderDetail> CancelPurchaseOrderDetailChanges(PurchaseOrderDetail item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnPurchaseOrderDetailUpdated(PurchaseOrderDetail item);
        partial void OnAfterPurchaseOrderDetailUpdated(PurchaseOrderDetail item);

        public async Task<PurchaseOrderDetail> UpdatePurchaseOrderDetail(int purchaseorderdetailid, PurchaseOrderDetail purchaseorderdetail)
        {
            OnPurchaseOrderDetailUpdated(purchaseorderdetail);

            var itemToUpdate = Context.PurchaseOrderDetails
                              .Where(i => i.PURCHASE_ORDER_DETAIL_ID == purchaseorderdetail.PURCHASE_ORDER_DETAIL_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(purchaseorderdetail);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterPurchaseOrderDetailUpdated(purchaseorderdetail);

            return purchaseorderdetail;
        }

        partial void OnPurchaseOrderDetailDeleted(PurchaseOrderDetail item);
        partial void OnAfterPurchaseOrderDetailDeleted(PurchaseOrderDetail item);

        public async Task<PurchaseOrderDetail> DeletePurchaseOrderDetail(int purchaseorderdetailid)
        {
            var itemToDelete = Context.PurchaseOrderDetails
                              .Where(i => i.PURCHASE_ORDER_DETAIL_ID == purchaseorderdetailid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnPurchaseOrderDetailDeleted(itemToDelete);

            Context.PurchaseOrderDetails.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterPurchaseOrderDetailDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportPurchaseOrdersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/purchaseorders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/purchaseorders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportPurchaseOrdersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/purchaseorders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/purchaseorders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnPurchaseOrdersRead(ref IQueryable<PurchaseOrder> items);

        public async Task<IQueryable<PurchaseOrder>> GetPurchaseOrders(Query query = null)
        {
            var items = Context.PurchaseOrders.AsQueryable();

            items = items.Include(i => i.Employee);
            items = items.Include(i => i.ForwarderAgent);
            items = items.Include(i => i.Provider);
            items = items.Include(i => i.ShipmentForwarderAgentMethod);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnPurchaseOrdersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnPurchaseOrderGet(PurchaseOrder item);
        partial void OnGetPurchaseOrderByPurchaseOrderId(ref IQueryable<PurchaseOrder> items);

        public async Task<PurchaseOrder> GetPurchaseOrderByPurchaseOrderId(int purchaseorderid)
        {
            var items = Context.PurchaseOrders
                              .AsNoTracking()
                              .Where(i => i.PURCHASE_ORDER_ID == purchaseorderid);

            items = items.Include(i => i.Employee);
            items = items.Include(i => i.ForwarderAgent);
            items = items.Include(i => i.Provider);
            items = items.Include(i => i.ShipmentForwarderAgentMethod);

            OnGetPurchaseOrderByPurchaseOrderId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnPurchaseOrderGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnPurchaseOrderCreated(PurchaseOrder item);
        partial void OnAfterPurchaseOrderCreated(PurchaseOrder item);

        public async Task<PurchaseOrder> CreatePurchaseOrder(PurchaseOrder purchaseorder)
        {
            OnPurchaseOrderCreated(purchaseorder);

            var existingItem = Context.PurchaseOrders
                              .Where(i => i.PURCHASE_ORDER_ID == purchaseorder.PURCHASE_ORDER_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.PurchaseOrders.Add(purchaseorder);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(purchaseorder).State = EntityState.Detached;
                throw;
            }

            OnAfterPurchaseOrderCreated(purchaseorder);

            return purchaseorder;
        }

        public async Task<PurchaseOrder> CancelPurchaseOrderChanges(PurchaseOrder item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnPurchaseOrderUpdated(PurchaseOrder item);
        partial void OnAfterPurchaseOrderUpdated(PurchaseOrder item);

        public async Task<PurchaseOrder> UpdatePurchaseOrder(int purchaseorderid, PurchaseOrder purchaseorder)
        {
            OnPurchaseOrderUpdated(purchaseorder);

            var itemToUpdate = Context.PurchaseOrders
                              .Where(i => i.PURCHASE_ORDER_ID == purchaseorder.PURCHASE_ORDER_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(purchaseorder);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterPurchaseOrderUpdated(purchaseorder);

            return purchaseorder;
        }

        partial void OnPurchaseOrderDeleted(PurchaseOrder item);
        partial void OnAfterPurchaseOrderDeleted(PurchaseOrder item);

        public async Task<PurchaseOrder> DeletePurchaseOrder(int purchaseorderid)
        {
            var itemToDelete = Context.PurchaseOrders
                              .Where(i => i.PURCHASE_ORDER_ID == purchaseorderid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnPurchaseOrderDeleted(itemToDelete);

            Context.PurchaseOrders.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterPurchaseOrderDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportReferencesWarehousesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/referenceswarehouses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/referenceswarehouses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportReferencesWarehousesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/referenceswarehouses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/referenceswarehouses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnReferencesWarehousesRead(ref IQueryable<ReferencesWarehouse> items);

        public async Task<IQueryable<ReferencesWarehouse>> GetReferencesWarehouses(Query query = null)
        {
            var items = Context.ReferencesWarehouses.AsQueryable();

            items = items.Include(i => i.ItemReference);
            items = items.Include(i => i.Warehouse);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnReferencesWarehousesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnReferencesWarehouseGet(ReferencesWarehouse item);
        partial void OnGetReferencesWarehouseByReferenceIdAndWarehouseId(ref IQueryable<ReferencesWarehouse> items);

        public async Task<ReferencesWarehouse> GetReferencesWarehouseByReferenceIdAndWarehouseId(int referenceid, short warehouseid)
        {
            var items = Context.ReferencesWarehouses
                              .AsNoTracking()
                              .Where(i => i.REFERENCE_ID == referenceid && i.WAREHOUSE_ID == warehouseid);

            items = items.Include(i => i.ItemReference);
            items = items.Include(i => i.Warehouse);

            OnGetReferencesWarehouseByReferenceIdAndWarehouseId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnReferencesWarehouseGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnReferencesWarehouseCreated(ReferencesWarehouse item);
        partial void OnAfterReferencesWarehouseCreated(ReferencesWarehouse item);

        public async Task<ReferencesWarehouse> CreateReferencesWarehouse(ReferencesWarehouse referenceswarehouse)
        {
            OnReferencesWarehouseCreated(referenceswarehouse);

            var existingItem = Context.ReferencesWarehouses
                              .Where(i => i.REFERENCE_ID == referenceswarehouse.REFERENCE_ID && i.WAREHOUSE_ID == referenceswarehouse.WAREHOUSE_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.ReferencesWarehouses.Add(referenceswarehouse);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(referenceswarehouse).State = EntityState.Detached;
                throw;
            }

            OnAfterReferencesWarehouseCreated(referenceswarehouse);

            return referenceswarehouse;
        }

        public async Task<ReferencesWarehouse> CancelReferencesWarehouseChanges(ReferencesWarehouse item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnReferencesWarehouseUpdated(ReferencesWarehouse item);
        partial void OnAfterReferencesWarehouseUpdated(ReferencesWarehouse item);

        public async Task<ReferencesWarehouse> UpdateReferencesWarehouse(int referenceid, short warehouseid, ReferencesWarehouse referenceswarehouse)
        {
            OnReferencesWarehouseUpdated(referenceswarehouse);

            var itemToUpdate = Context.ReferencesWarehouses
                              .Where(i => i.REFERENCE_ID == referenceswarehouse.REFERENCE_ID && i.WAREHOUSE_ID == referenceswarehouse.WAREHOUSE_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(referenceswarehouse);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterReferencesWarehouseUpdated(referenceswarehouse);

            return referenceswarehouse;
        }

        partial void OnReferencesWarehouseDeleted(ReferencesWarehouse item);
        partial void OnAfterReferencesWarehouseDeleted(ReferencesWarehouse item);

        public async Task<ReferencesWarehouse> DeleteReferencesWarehouse(int referenceid, short warehouseid)
        {
            var itemToDelete = Context.ReferencesWarehouses
                              .Where(i => i.REFERENCE_ID == referenceid && i.WAREHOUSE_ID == warehouseid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnReferencesWarehouseDeleted(itemToDelete);

            Context.ReferencesWarehouses.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterReferencesWarehouseDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportShipmentForwarderAgentMethodsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/shipmentforwarderagentmethods/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/shipmentforwarderagentmethods/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportShipmentForwarderAgentMethodsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/shipmentforwarderagentmethods/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/shipmentforwarderagentmethods/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnShipmentForwarderAgentMethodsRead(ref IQueryable<ShipmentForwarderAgentMethod> items);

        public async Task<IQueryable<ShipmentForwarderAgentMethod>> GetShipmentForwarderAgentMethods(Query query = null)
        {
            var items = Context.ShipmentForwarderAgentMethods.AsQueryable();

            items = items.Include(i => i.ForwarderAgent);
            items = items.Include(i => i.ShipmentMethod);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnShipmentForwarderAgentMethodsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnShipmentForwarderAgentMethodGet(ShipmentForwarderAgentMethod item);
        partial void OnGetShipmentForwarderAgentMethodByShipmentForwarderAgentMethodId(ref IQueryable<ShipmentForwarderAgentMethod> items);

        public async Task<ShipmentForwarderAgentMethod> GetShipmentForwarderAgentMethodByShipmentForwarderAgentMethodId(short shipmentforwarderagentmethodid)
        {
            var items = Context.ShipmentForwarderAgentMethods
                              .AsNoTracking()
                              .Where(i => i.SHIPMENT_FORWARDER_AGENT_METHOD_ID == shipmentforwarderagentmethodid);

            items = items.Include(i => i.ForwarderAgent);
            items = items.Include(i => i.ShipmentMethod);

            OnGetShipmentForwarderAgentMethodByShipmentForwarderAgentMethodId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnShipmentForwarderAgentMethodGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnShipmentForwarderAgentMethodCreated(ShipmentForwarderAgentMethod item);
        partial void OnAfterShipmentForwarderAgentMethodCreated(ShipmentForwarderAgentMethod item);

        public async Task<ShipmentForwarderAgentMethod> CreateShipmentForwarderAgentMethod(ShipmentForwarderAgentMethod shipmentforwarderagentmethod)
        {
            OnShipmentForwarderAgentMethodCreated(shipmentforwarderagentmethod);

            var existingItem = Context.ShipmentForwarderAgentMethods
                              .Where(i => i.SHIPMENT_FORWARDER_AGENT_METHOD_ID == shipmentforwarderagentmethod.SHIPMENT_FORWARDER_AGENT_METHOD_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.ShipmentForwarderAgentMethods.Add(shipmentforwarderagentmethod);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(shipmentforwarderagentmethod).State = EntityState.Detached;
                throw;
            }

            OnAfterShipmentForwarderAgentMethodCreated(shipmentforwarderagentmethod);

            return shipmentforwarderagentmethod;
        }

        public async Task<ShipmentForwarderAgentMethod> CancelShipmentForwarderAgentMethodChanges(ShipmentForwarderAgentMethod item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnShipmentForwarderAgentMethodUpdated(ShipmentForwarderAgentMethod item);
        partial void OnAfterShipmentForwarderAgentMethodUpdated(ShipmentForwarderAgentMethod item);

        public async Task<ShipmentForwarderAgentMethod> UpdateShipmentForwarderAgentMethod(short shipmentforwarderagentmethodid, ShipmentForwarderAgentMethod shipmentforwarderagentmethod)
        {
            OnShipmentForwarderAgentMethodUpdated(shipmentforwarderagentmethod);

            var itemToUpdate = Context.ShipmentForwarderAgentMethods
                              .Where(i => i.SHIPMENT_FORWARDER_AGENT_METHOD_ID == shipmentforwarderagentmethod.SHIPMENT_FORWARDER_AGENT_METHOD_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(shipmentforwarderagentmethod);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterShipmentForwarderAgentMethodUpdated(shipmentforwarderagentmethod);

            return shipmentforwarderagentmethod;
        }

        partial void OnShipmentForwarderAgentMethodDeleted(ShipmentForwarderAgentMethod item);
        partial void OnAfterShipmentForwarderAgentMethodDeleted(ShipmentForwarderAgentMethod item);

        public async Task<ShipmentForwarderAgentMethod> DeleteShipmentForwarderAgentMethod(short shipmentforwarderagentmethodid)
        {
            var itemToDelete = Context.ShipmentForwarderAgentMethods
                              .Where(i => i.SHIPMENT_FORWARDER_AGENT_METHOD_ID == shipmentforwarderagentmethodid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnShipmentForwarderAgentMethodDeleted(itemToDelete);

            Context.ShipmentForwarderAgentMethods.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterShipmentForwarderAgentMethodDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportShipmentMethodsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/shipmentmethods/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/shipmentmethods/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportShipmentMethodsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/shipmentmethods/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/shipmentmethods/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnShipmentMethodsRead(ref IQueryable<ShipmentMethod> items);

        public async Task<IQueryable<ShipmentMethod>> GetShipmentMethods(Query query = null)
        {
            var items = Context.ShipmentMethods.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnShipmentMethodsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnShipmentMethodGet(ShipmentMethod item);
        partial void OnGetShipmentMethodByShipmentMethodId(ref IQueryable<ShipmentMethod> items);

        public async Task<ShipmentMethod> GetShipmentMethodByShipmentMethodId(short shipmentmethodid)
        {
            var items = Context.ShipmentMethods
                              .AsNoTracking()
                              .Where(i => i.SHIPMENT_METHOD_ID == shipmentmethodid);

            OnGetShipmentMethodByShipmentMethodId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnShipmentMethodGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnShipmentMethodCreated(ShipmentMethod item);
        partial void OnAfterShipmentMethodCreated(ShipmentMethod item);

        public async Task<ShipmentMethod> CreateShipmentMethod(ShipmentMethod shipmentmethod)
        {
            OnShipmentMethodCreated(shipmentmethod);

            var existingItem = Context.ShipmentMethods
                              .Where(i => i.SHIPMENT_METHOD_ID == shipmentmethod.SHIPMENT_METHOD_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.ShipmentMethods.Add(shipmentmethod);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(shipmentmethod).State = EntityState.Detached;
                throw;
            }

            OnAfterShipmentMethodCreated(shipmentmethod);

            return shipmentmethod;
        }

        public async Task<ShipmentMethod> CancelShipmentMethodChanges(ShipmentMethod item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnShipmentMethodUpdated(ShipmentMethod item);
        partial void OnAfterShipmentMethodUpdated(ShipmentMethod item);

        public async Task<ShipmentMethod> UpdateShipmentMethod(short shipmentmethodid, ShipmentMethod shipmentmethod)
        {
            OnShipmentMethodUpdated(shipmentmethod);

            var itemToUpdate = Context.ShipmentMethods
                              .Where(i => i.SHIPMENT_METHOD_ID == shipmentmethod.SHIPMENT_METHOD_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(shipmentmethod);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterShipmentMethodUpdated(shipmentmethod);

            return shipmentmethod;
        }

        partial void OnShipmentMethodDeleted(ShipmentMethod item);
        partial void OnAfterShipmentMethodDeleted(ShipmentMethod item);

        public async Task<ShipmentMethod> DeleteShipmentMethod(short shipmentmethodid)
        {
            var itemToDelete = Context.ShipmentMethods
                              .Where(i => i.SHIPMENT_METHOD_ID == shipmentmethodid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnShipmentMethodDeleted(itemToDelete);

            Context.ShipmentMethods.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterShipmentMethodDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportShippingMethodsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/shippingmethods/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/shippingmethods/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportShippingMethodsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/shippingmethods/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/shippingmethods/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnShippingMethodsRead(ref IQueryable<ShippingMethod> items);

        public async Task<IQueryable<ShippingMethod>> GetShippingMethods(Query query = null)
        {
            var items = Context.ShippingMethods.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnShippingMethodsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnShippingMethodGet(ShippingMethod item);
        partial void OnGetShippingMethodByShippingMethodId(ref IQueryable<ShippingMethod> items);

        public async Task<ShippingMethod> GetShippingMethodByShippingMethodId(short shippingmethodid)
        {
            var items = Context.ShippingMethods
                              .AsNoTracking()
                              .Where(i => i.SHIPPING_METHOD_ID == shippingmethodid);

            OnGetShippingMethodByShippingMethodId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnShippingMethodGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnShippingMethodCreated(ShippingMethod item);
        partial void OnAfterShippingMethodCreated(ShippingMethod item);

        public async Task<ShippingMethod> CreateShippingMethod(ShippingMethod shippingmethod)
        {
            OnShippingMethodCreated(shippingmethod);

            var existingItem = Context.ShippingMethods
                              .Where(i => i.SHIPPING_METHOD_ID == shippingmethod.SHIPPING_METHOD_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.ShippingMethods.Add(shippingmethod);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(shippingmethod).State = EntityState.Detached;
                throw;
            }

            OnAfterShippingMethodCreated(shippingmethod);

            return shippingmethod;
        }

        public async Task<ShippingMethod> CancelShippingMethodChanges(ShippingMethod item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnShippingMethodUpdated(ShippingMethod item);
        partial void OnAfterShippingMethodUpdated(ShippingMethod item);

        public async Task<ShippingMethod> UpdateShippingMethod(short shippingmethodid, ShippingMethod shippingmethod)
        {
            OnShippingMethodUpdated(shippingmethod);

            var itemToUpdate = Context.ShippingMethods
                              .Where(i => i.SHIPPING_METHOD_ID == shippingmethod.SHIPPING_METHOD_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(shippingmethod);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterShippingMethodUpdated(shippingmethod);

            return shippingmethod;
        }

        partial void OnShippingMethodDeleted(ShippingMethod item);
        partial void OnAfterShippingMethodDeleted(ShippingMethod item);

        public async Task<ShippingMethod> DeleteShippingMethod(short shippingmethodid)
        {
            var itemToDelete = Context.ShippingMethods
                              .Where(i => i.SHIPPING_METHOD_ID == shippingmethodid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnShippingMethodDeleted(itemToDelete);

            Context.ShippingMethods.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterShippingMethodDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportWarehousesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/warehouses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/warehouses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportWarehousesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/warehouses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/warehouses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnWarehousesRead(ref IQueryable<Warehouse> items);

        public async Task<IQueryable<Warehouse>> GetWarehouses(Query query = null)
        {
            var items = Context.Warehouses.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnWarehousesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnWarehouseGet(Warehouse item);
        partial void OnGetWarehouseByWarehouseId(ref IQueryable<Warehouse> items);

        public async Task<Warehouse> GetWarehouseByWarehouseId(short warehouseid)
        {
            var items = Context.Warehouses
                              .AsNoTracking()
                              .Where(i => i.WAREHOUSE_ID == warehouseid);

            OnGetWarehouseByWarehouseId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnWarehouseGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnWarehouseCreated(Warehouse item);
        partial void OnAfterWarehouseCreated(Warehouse item);

        public async Task<Warehouse> CreateWarehouse(Warehouse warehouse)
        {
            OnWarehouseCreated(warehouse);

            var existingItem = Context.Warehouses
                              .Where(i => i.WAREHOUSE_ID == warehouse.WAREHOUSE_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Warehouses.Add(warehouse);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(warehouse).State = EntityState.Detached;
                throw;
            }

            OnAfterWarehouseCreated(warehouse);

            return warehouse;
        }

        public async Task<Warehouse> CancelWarehouseChanges(Warehouse item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnWarehouseUpdated(Warehouse item);
        partial void OnAfterWarehouseUpdated(Warehouse item);

        public async Task<Warehouse> UpdateWarehouse(short warehouseid, Warehouse warehouse)
        {
            OnWarehouseUpdated(warehouse);

            var itemToUpdate = Context.Warehouses
                              .Where(i => i.WAREHOUSE_ID == warehouse.WAREHOUSE_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(warehouse);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterWarehouseUpdated(warehouse);

            return warehouse;
        }

        partial void OnWarehouseDeleted(Warehouse item);
        partial void OnAfterWarehouseDeleted(Warehouse item);

        public async Task<Warehouse> DeleteWarehouse(short warehouseid)
        {
            var itemToDelete = Context.Warehouses
                              .Where(i => i.WAREHOUSE_ID == warehouseid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnWarehouseDeleted(itemToDelete);

            Context.Warehouses.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterWarehouseDeleted(itemToDelete);

            return itemToDelete;
        }

        partial void OnDocumentTypeGet(DocumentType item);
        partial void OnGetDocumentTypeByCode(ref IQueryable<DocumentType> items);

        public async Task<DocumentType> GetDocumentTypeByCode(string documentTypeCode)
        {
            var items = Context.DocumentTypes
                              .AsNoTracking()
                              .Where(i => i.DOCUMENT_TYPE_CODE.Equals(documentTypeCode));

            OnGetDocumentTypeByCode(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnDocumentTypeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnStatusDocumentTypesRead(ref IQueryable<StatusDocumentType> items);

        partial void OnStatusDocumentTypeGet(StatusDocumentType item);

        partial void OnGetStatusDocumentTypeByDocumentAndOrder(ref IQueryable<StatusDocumentType> items);

        public async Task<StatusDocumentType> GetStatusDocumentTypeByDocumentAndOrder(DocumentType documentType, short orderId)
        {

            var items = Context.StatusDocumentTypes
                    .AsNoTracking()
                    .Where(i => i.DOCUMENT_TYPE_ID.Equals(documentType.DOCUMENT_TYPE_ID) && i.STATUS_ORDER.Equals(orderId));

            OnGetStatusDocumentTypeByDocumentAndOrder(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnStatusDocumentTypeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);

        }

        public async Task<IQueryable<StatusDocumentType>> GetStatusDocumentTypes(Query query = null)
        {
            var items = Context.StatusDocumentTypes.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnStatusDocumentTypesRead(ref items);

            return await Task.FromResult(items);
        }

        public async Task<Employee> GetLoggedEmployee(SecurityService security)
        {

            var userId = security.User.Id;

            var employee = context.Employees
                .AsNoTracking()
                .FirstOrDefault(e => e.LOGIN_USER_ID.Equals(userId));
            return employee;
        }

        /* Customer Reservations */

        public async Task<IQueryable<CustomerReservation>> GetCustomerReservations(Query query = null)
        {
            var items = Context.CustomerReservations.AsQueryable();

            items = items.Include(i => i.Customer);
            items = items.Include(i => i.StatusDocumentType);
            items = items.Include(i => i.Employee);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCustomerReservationRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCustomerReservationGet(CustomerReservation item);
        partial void OnGetCustomerReservationByCustomerReservationId(ref IQueryable<CustomerReservation> items);

        public async Task<CustomerReservation> GetCustomerReservationByCustomerReservationId(int customerReservationid)
        {
            var items = Context.CustomerReservations
                              .AsNoTracking()
                              .Where(i => i.CUSTOMER_RESERVATION_ID == customerReservationid);

            items = items.Include(i => i.Customer);
            items = items.Include(i => i.StatusDocumentType);
            items = items.Include(i => i.Employee);
            items = items.Include(i => i.CustomerOrder);
            items = items.Include(i => i.CustomerReservationDetails);

            OnGetCustomerReservationByCustomerReservationId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCustomerReservationGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCustomerReservationRead(ref IQueryable<CustomerReservation> items);

        partial void OnCustomerReservationCreated(CustomerReservation item);

        partial void OnAfterCustomerReservationCreated(CustomerReservation item);

        public async Task<CustomerReservation> CancelCustomerReservationChanges(CustomerReservation item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCustomerReservationDeleted(CustomerReservation item);

        partial void OnAfterCustomerReservationDeleted(CustomerReservation item);

        public async Task<IQueryable<CustomerReservationDetail>> GetCustomerReservationDetails(Query query = null)
        {
            var items = Context.CustomerReservationDetails.AsQueryable();

            items = items.Include(i => i.CustomerReservation);
            items = items.Include(i => i.ItemReference);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCustomerReservationDetailsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCustomerReservationDetailsRead(ref IQueryable<CustomerReservationDetail> items);

        partial void OnCustomerReservationUpdated(CustomerReservation item);

        partial void OnAfterCustomerReservationUpdated(CustomerReservation item);

        public async Task<ICollection<CustomerReservationDetail>> UpdateCustomerReservationDetails(ICollection<CustomerReservationDetail> customerReservationDetails)
        {
            foreach (var customerReservationDetail in customerReservationDetails)
            {
                var itemToUpdate = context.CustomerReservationDetails.FirstOrDefault(i => i.CUSTOMER_RESERVATION_DETAIL_ID.Equals(customerReservationDetail.CUSTOMER_RESERVATION_DETAIL_ID));
                itemToUpdate.SEND_TO_CUSTOMER_ORDER = customerReservationDetail.SEND_TO_CUSTOMER_ORDER;
                Context.CustomerReservationDetails.Update(itemToUpdate);
            }
            Context.SaveChanges();

            return customerReservationDetails;
        }

        /* Customer Order */

        partial void OnCustomerOrderUpdated(CustomerOrder customerOrder);

        partial void OnAfterCustomerOrderUpdated(CustomerOrder customerOrder);

        public async Task<CustomerReservation> AssignReservationNumber(CustomerReservation customerReservation)
        {
            var itemToUpdate = Context.CustomerReservations
                              .FirstOrDefault(i => i.CUSTOMER_RESERVATION_ID == customerReservation.CUSTOMER_RESERVATION_ID);

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            itemToUpdate.RESERVATION_NUMBER = customerReservation.RESERVATION_NUMBER;

            context.CustomerReservations.Update(itemToUpdate);
            Context.SaveChanges();

            return customerReservation;
        }

        public async Task<CustomerOrder> AssignOrderNumber(CustomerOrder customerOrder)
        {
            var itemToUpdate = Context.CustomerOrders
                              .FirstOrDefault(i => i.CUSTOMER_ORDER_ID == customerOrder.CUSTOMER_ORDER_ID);

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            itemToUpdate.ORDER_NUMBER = customerOrder.ORDER_NUMBER;

            context.CustomerOrders.Update(itemToUpdate);
            Context.SaveChanges();

            return customerOrder;
        }

        partial void OnCustomerOrderCreated(CustomerOrder customerOrder);
        partial void OnAfterCustomerOrderCreated(CustomerOrder customerOrder);

        public async Task<CustomerReservation> CreateCustomerReservation(CustomerReservation customerReservation)
        {
            OnCustomerReservationCreated(customerReservation);

            var existingItem = Context.CustomerReservations
                              .Where(i => i.CUSTOMER_RESERVATION_ID == customerReservation.CUSTOMER_RESERVATION_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }
            try
            {
                var customerReservationToSave = new CustomerReservation()
                {
                    CREATION_DATE = DateTime.Now,
                    CUSTOMER_ID = customerReservation.CUSTOMER_ID,
                    EXPIRATION_DATE = customerReservation.EXPIRATION_DATE,
                    NOTES = customerReservation.NOTES,
                    RESERVATION_NUMBER = customerReservation.RESERVATION_NUMBER,
                    RESERVATION_DATE = customerReservation.RESERVATION_DATE,
                    STATUS_DOCUMENT_TYPE_ID = customerReservation.StatusDocumentType.STATUS_DOCUMENT_TYPE_ID,
                    EMPLOYEE_ID = customerReservation.Employee.EMPLOYEE_ID,
                    CustomerReservationDetails = new List<CustomerReservationDetail>()
                };

                foreach (var customerReservationDetail in customerReservation.CustomerReservationDetails)
                {
                    customerReservationToSave.CustomerReservationDetails.Add(new CustomerReservationDetail()
                    {
                        CUSTOMER_RESERVATION_ID = customerReservationDetail.CUSTOMER_RESERVATION_ID,
                        RESERVED_QUANTITY = customerReservationDetail.RESERVED_QUANTITY,
                        REFERENCE_ID = customerReservationDetail.REFERENCE_ID,
                        BRAND = customerReservationDetail.BRAND
                    });
                }
                try
                {
                    Context.CustomerReservations.Add(customerReservationToSave);

                    Context.SaveChanges();

                    customerReservation.CUSTOMER_RESERVATION_ID = customerReservationToSave.CUSTOMER_RESERVATION_ID;
                }
                catch
                {
                    Context.Entry(customerReservationToSave).State = EntityState.Detached;
                    throw;
                }

                OnAfterCustomerReservationCreated(customerReservation);

                return customerReservation;
            }
            catch
            {
                throw;
            }
        }

        public async Task<CustomerOrder> CreateCustomerOrder(CustomerOrder customerOrder)
        {
            OnCustomerOrderCreated(customerOrder);

            var existingItem = Context.CustomerOrders
                              .Where(i => i.CUSTOMER_ORDER_ID == customerOrder.CUSTOMER_ORDER_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                var customerOrderToSave = new CustomerOrder()
                {
                    CREATION_DATE = DateTime.Now,
                    CUSTOMER_ID = customerOrder.CUSTOMER_ID,
                    ESTIMATED_DELIVERY_DATE = customerOrder.ESTIMATED_DELIVERY_DATE,
                    CUSTOMER_NOTES = customerOrder.CUSTOMER_NOTES,
                    INTERNAL_NOTES = customerOrder.INTERNAL_NOTES,
                    ORDER_DATE = customerOrder.ORDER_DATE,
                    STATUS_DOCUMENT_TYPE_ID = customerOrder.STATUS_DOCUMENT_TYPE_ID,
                    ORDER_NUMBER = customerOrder.ORDER_NUMBER,
                    EMPLOYEE_ID = customerOrder.EMPLOYEE_ID,
                    CustomerOrderDetails = new List<CustomerOrderDetail>()
                };

                foreach (var customerOrderDetail in customerOrder.CustomerOrderDetails)
                {
                    customerOrderToSave.CustomerOrderDetails.Add(new CustomerOrderDetail()
                    {
                        CUSTOMER_ORDER_ID = customerOrderDetail.CUSTOMER_ORDER_ID,
                        DELIVERED_QUANTITY = 0,
                        CUSTOMER_ORDER_DETAIL_ID = 0,
                        BRAND = customerOrderDetail.BRAND,
                        PROCESSED_QUANTITY = 0,
                        REFERENCE_ID = customerOrderDetail.REFERENCE_ID,
                        REQUESTED_QUANTITY = customerOrderDetail.REQUESTED_QUANTITY
                    });
                }
                try
                {
                    Context.CustomerOrders.Add(customerOrderToSave);

                    Context.SaveChanges();
                }
                catch
                {
                    Context.Entry(customerOrderToSave).State = EntityState.Detached;
                    throw;
                }

                OnAfterCustomerOrderCreated(customerOrder);
                customerOrder.CUSTOMER_ORDER_ID = customerOrderToSave.CUSTOMER_ORDER_ID;
                return customerOrder;
            }
            catch
            {
                throw;
            }
        }

        public async Task<ICollection<GroupPurchaseOrderDetail>> GetTotalTransitOrdersPurchaseByReferenceId(int referenceId)
        {
            var documentType = await GetDocumentTypeByCode("O");
            var statusOrder = await GetStatusDocumentTypeByDocumentAndOrder(documentType, 1);

            return context.PurchaseOrderDetails
                    .Where(det => det.PurchaseOrder.STATUS_DOCUMENT_TYPE_ID.Equals(statusOrder.STATUS_DOCUMENT_TYPE_ID))
                    .GroupBy(group => group.PurchaseOrder.REQUEST_DATE)
                    .Select(c => new GroupPurchaseOrderDetail() { Request_Date = c.Key, Quantity = c.Sum(p => p.REQUESTED_QUANTITY) }).ToList();
        }

        partial void OnCustomerOrderDetailsRead(ref IQueryable<CustomerOrderDetail> items);

        partial void OnCustomerOrdersRead(ref IQueryable<CustomerOrder> items);

        public async Task<IQueryable<CustomerOrderDetail>> GetCustomerOrderDetails(Query query = null)
        {
            var items = Context.CustomerOrderDetails.AsQueryable();

            items = items.Include(i => i.CustomerOrder);
            items = items.Include(i => i.ItemReference);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCustomerOrderDetailsRead(ref items);

            return await Task.FromResult(items);

        }

        partial void OnCustomerOrderActivitiesRead(ref IQueryable<CustomerOrderActivity> items);

        public async Task<IQueryable<CustomerOrderActivity>> GetCustomerOrderActivities(Query query = null)
        {
            var items = Context.CustomerOrderActivities.AsQueryable();

            items = items.Include(i => i.CustomerOrder);
            items = items.Include(i => i.Area);
            items = items.Include(i => i.Employee);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCustomerOrderActivitiesRead(ref items);

            return await Task.FromResult(items);

        }

        partial void OnCustomerOrderActivityDetailsRead(ref IQueryable<CustomerOrderActivityDetail> items);

        public async Task<IQueryable<CustomerOrderActivityDetail>> GetCustomerOrderActivityDetails(Query query = null)
        {
            var items = Context.CustomerOrderActivityDetails.AsQueryable();

            items = items.Include(i => i.CustomerOrderActivity);
            items = items.Include(i => i.ActivityType);
            items = items.Include(i => i.Employee);
            items = items.Include(i => i.EmployeeActivity);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCustomerOrderActivityDetailsRead(ref items);

            return await Task.FromResult(items);

        }

        public async Task<IQueryable<CustomerOrder>> GetCustomerOrders(Query query = null)
        {
            var items = Context.CustomerOrders.AsQueryable();

            items = items.Include(i => i.Customer);
            items = items.Include(i => i.Employee);
            items = items.Include(i => i.StatusDocumentType);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCustomerOrdersRead(ref items);

            return await Task.FromResult(items);

        }

        public async Task<CustomerOrder> UpdateCustomerOrderStatus(CustomerOrder customerOrder, short newDocumentStatusId)
        {
            var itemToUpdate = Context.CustomerOrders
                              .Where(i => i.CUSTOMER_ORDER_ID == customerOrder.CUSTOMER_ORDER_ID && i.STATUS_DOCUMENT_TYPE_ID.Equals(customerOrder.STATUS_DOCUMENT_TYPE_ID))
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            itemToUpdate.STATUS_DOCUMENT_TYPE_ID = newDocumentStatusId;

            Context.CustomerOrders.Update(itemToUpdate);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToUpdate).State = EntityState.Unchanged;
                throw;
            }

            return itemToUpdate;
        }

        public async Task<CustomerReservation> UpdateCustomerReservationStatus(CustomerReservation customerReservation, short newDocumentStatusId)
        {
            var itemToUpdate = Context.CustomerReservations
                              .Where(i => i.CUSTOMER_RESERVATION_ID == customerReservation.CUSTOMER_RESERVATION_ID && i.STATUS_DOCUMENT_TYPE_ID.Equals(customerReservation.STATUS_DOCUMENT_TYPE_ID))
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            itemToUpdate.STATUS_DOCUMENT_TYPE_ID = newDocumentStatusId;

            Context.CustomerReservations.Update(itemToUpdate);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToUpdate).State = EntityState.Unchanged;
                throw;
            }

            return itemToUpdate;
        }

        partial void OnOrderActivityDeleted(CustomerOrderActivity customerOrderActivity);
        partial void OnAfterOrderActivityDeleted(CustomerOrderActivity customerOrderActivity);

        public async Task<CustomerOrderActivity> DeleteCustomerOrderActivity(CustomerOrderActivity customerOrderActivity)
        {
            var itemToDelete = Context.CustomerOrderActivities
                                          .Where(i => i.CUSTOMER_ORDER_ACTIVITY_ID == customerOrderActivity.CUSTOMER_ORDER_ACTIVITY_ID)
                                          .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            var childItemToDelete = Context.CustomerOrderActivityDetails
                .Where(i => i.CUSTOMER_ORDER_ACTIVITY_ID == customerOrderActivity.CUSTOMER_ORDER_ACTIVITY_ID);

            OnOrderActivityDeleted(itemToDelete);

            foreach (var item in childItemToDelete)
            {
                Context.CustomerOrderActivityDetails.Remove(item);
            }

            Context.CustomerOrderActivities.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterOrderActivityDeleted(itemToDelete);

            return itemToDelete;
        }

        partial void OnCustomerOrderActivityCreated(CustomerOrderActivity customerOrderActivity);
        partial void OnAfterCustomerOrderActivityCreated(CustomerOrderActivity customerOrderActivity);

        public async Task<CustomerOrderActivity> CreateCustomerOrderActivity(CustomerOrderActivity customerOrderActivity)
        {
            OnCustomerOrderActivityCreated(customerOrderActivity);

            var existingItem = Context.CustomerOrderActivities
                              .Where(i => i.CUSTOMER_ORDER_ACTIVITY_ID == customerOrderActivity.CUSTOMER_ORDER_ACTIVITY_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                var customerOrderActivityToSave = new CustomerOrderActivity()
                {
                    ACTIVITY_DATE = customerOrderActivity.ACTIVITY_DATE,
                    AREA_ID = customerOrderActivity.AREA_ID,
                    CUSTOMER_ORDER_ID = customerOrderActivity.CUSTOMER_ORDER_ID,
                    EMPLOYEE_ID = customerOrderActivity.EMPLOYEE_ID,
                    NOTES = customerOrderActivity.NOTES,
                    CustomerOrderActivityDetails = new List<CustomerOrderActivityDetail>()
                };

                foreach (var customerOrderActivityDetail in customerOrderActivity.CustomerOrderActivityDetails)
                {
                    customerOrderActivityToSave.CustomerOrderActivityDetails.Add(new CustomerOrderActivityDetail()
                    {
                        ACTIVITY_TYPE_ID = customerOrderActivityDetail.ACTIVITY_TYPE_ID,
                        ACTIVITY_EMPLOYEE_ID = customerOrderActivityDetail.ACTIVITY_EMPLOYEE_ID,
                        EMPLOYEE_ID = customerOrderActivityDetail.ACTIVITY_EMPLOYEE_ID
                    });
                }

                Context.CustomerOrderActivities.Add(customerOrderActivityToSave);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(customerOrderActivity).State = EntityState.Detached;
                throw;
            }

            OnAfterCustomerOrderActivityCreated(customerOrderActivity);

            return customerOrderActivity;
        }

        partial void OnGetCustomerOrderActivityByCustomerOrderActivityId(ref IQueryable<CustomerOrderActivity> item);
        partial void OnCustomerOrderActivityGet(CustomerOrderActivity customerOrderActivity);

        public async Task<CustomerOrderActivity> GetCustomerOrderActivityByCustomerOrderActivityId(int customerOrderActivityId)
        {

            var items = Context.CustomerOrderActivities
                              .AsNoTracking()
                              .Where(i => i.CUSTOMER_ORDER_ACTIVITY_ID == customerOrderActivityId);

            items = items.Include(i => i.Area);
            items = items.Include(i => i.Employee);
            items = items.Include(i => i.CustomerOrder);

            OnGetCustomerOrderActivityByCustomerOrderActivityId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCustomerOrderActivityGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCustomerOrderActivityUpdated(CustomerOrderActivity customerOrderActivity);
        partial void OnAfterCustomerOrderActivityUpdated(CustomerOrderActivity customerOrderActivity);

        public async Task<CustomerOrderActivity> UpdateCustomerOrderActivity(CustomerOrderActivity customerOrderActivity)
        {
            OnCustomerOrderActivityUpdated(customerOrderActivity);

            var itemToUpdate = Context.CustomerOrderActivities
                              .Where(i => i.CUSTOMER_ORDER_ACTIVITY_ID == customerOrderActivity.CUSTOMER_ORDER_ACTIVITY_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }
            var a = new CustomerOrderActivity();

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(customerOrderActivity);
            entryToUpdate.State = EntityState.Modified;

            foreach (var item in customerOrderActivity.CustomerOrderActivityDetails)
            {
                var itemDetailToUpdate = Context.CustomerOrderActivityDetails
                              .Where(i => i.CUSTOMER_ORDER_ACTIVITY_DETAIL_ID == item.CUSTOMER_ORDER_ACTIVITY_DETAIL_ID)
                              .FirstOrDefault();
                if (itemDetailToUpdate == null)
                {
                    var itemDetail = new CustomerOrderActivityDetail()
                    {
                        CUSTOMER_ORDER_ACTIVITY_ID = customerOrderActivity.CUSTOMER_ORDER_ACTIVITY_ID,
                        ACTIVITY_TYPE_ID = item.ACTIVITY_TYPE_ID,
                        ACTIVITY_EMPLOYEE_ID = item.ACTIVITY_EMPLOYEE_ID,
                        EMPLOYEE_ID = item.ACTIVITY_EMPLOYEE_ID
                    };
                    Context.CustomerOrderActivityDetails.Add(itemDetail);
                }
                else
                {
                    var entryDetailToUpdate = Context.Entry(itemDetailToUpdate);
                    entryDetailToUpdate.CurrentValues.SetValues(item);
                    entryDetailToUpdate.State = EntityState.Modified;
                }
            }

            foreach (var item in itemToUpdate.CustomerOrderActivityDetails)
            {
                if (!customerOrderActivity.CustomerOrderActivityDetails.Any(x => x.CUSTOMER_ORDER_ACTIVITY_DETAIL_ID.Equals(item.CUSTOMER_ORDER_ACTIVITY_DETAIL_ID)))
                {
                    var entryDetailToDelete = Context.Entry(item);
                    entryDetailToDelete.State = EntityState.Deleted;
                }
            }

            Context.SaveChanges();

            OnAfterCustomerOrderActivityUpdated(customerOrderActivity);
            return customerOrderActivity;
        }

        public async Task<CustomerOrder> UpdateCustomerOrder(CustomerOrder customerOrder)
        {
            OnCustomerOrderUpdated(customerOrder);

            var itemToUpdate = Context.CustomerOrders
                              .Where(i => i.CUSTOMER_ORDER_ID == customerOrder.CUSTOMER_ORDER_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }
            var a = new CustomerOrderActivity();

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(customerOrder);
            entryToUpdate.State = EntityState.Modified;

            foreach (var item in customerOrder.CustomerOrderDetails)
            {
                var itemDetailToUpdate = Context.CustomerOrderDetails
                              .Where(i => i.CUSTOMER_ORDER_DETAIL_ID == item.CUSTOMER_ORDER_DETAIL_ID)
                              .FirstOrDefault();

                if (itemDetailToUpdate == null)
                {
                    var itemDetail = new CustomerOrderDetail()
                    {
                        CUSTOMER_ORDER_ID = customerOrder.CUSTOMER_ORDER_ID,
                        BRAND = item.BRAND,
                        CUSTOMER_ORDER_DETAIL_ID = 0,
                        REFERENCE_ID = item.REFERENCE_ID,
                        DELIVERED_QUANTITY = item.DELIVERED_QUANTITY,
                        PROCESSED_QUANTITY = item.PROCESSED_QUANTITY,
                        REQUESTED_QUANTITY = item.REQUESTED_QUANTITY
                    };
                    Context.CustomerOrderDetails.Add(itemDetail);
                }
                else
                {
                    var entryDetailToUpdate = Context.Entry(itemDetailToUpdate);
                    entryDetailToUpdate.CurrentValues.SetValues(item);
                    entryDetailToUpdate.State = EntityState.Modified;
                }
            }

            foreach (var item in itemToUpdate.CustomerOrderDetails)
            {
                if (!customerOrder.CustomerOrderDetails.Any(x => x.CUSTOMER_ORDER_DETAIL_ID.Equals(item.CUSTOMER_ORDER_DETAIL_ID)))
                {
                    var entryDetailToDelete = Context.Entry(item);
                    entryDetailToDelete.State = EntityState.Deleted;
                }
            }

            Context.SaveChanges();

            OnAfterCustomerOrderUpdated(customerOrder);

            return customerOrder;
        }

        public async Task<CustomerReservation> UpdateCustomerReservation(CustomerReservation customerReservation)
        {
            OnCustomerReservationUpdated(customerReservation);

            var itemToUpdate = Context.CustomerReservations
                              .Where(i => i.CUSTOMER_ORDER_ID == customerReservation.CUSTOMER_ORDER_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }
            var a = new CustomerOrderActivity();

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(customerReservation);
            entryToUpdate.State = EntityState.Modified;

            foreach (var item in customerReservation.CustomerReservationDetails)
            {
                var itemDetailToUpdate = Context.CustomerReservationDetails
                              .Where(i => i.CUSTOMER_RESERVATION_DETAIL_ID == item.CUSTOMER_RESERVATION_DETAIL_ID)
                              .FirstOrDefault();

                if (itemDetailToUpdate == null)
                {
                    var itemDetail = new CustomerReservationDetail()
                    {
                        CUSTOMER_RESERVATION_ID = customerReservation.CUSTOMER_RESERVATION_ID,
                        BRAND = item.BRAND,
                        REFERENCE_ID = item.REFERENCE_ID,
                        RESERVED_QUANTITY = item.RESERVED_QUANTITY
                    };

                    Context.CustomerReservationDetails.Add(itemDetail);
                }
                else
                {
                    var entryDetailToUpdate = Context.Entry(itemDetailToUpdate);
                    entryDetailToUpdate.CurrentValues.SetValues(item);
                    entryDetailToUpdate.State = EntityState.Modified;
                }
            }

            foreach (var item in itemToUpdate.CustomerReservationDetails)
            {
                if (!customerReservation.CustomerReservationDetails.Any(x => x.CUSTOMER_RESERVATION_DETAIL_ID.Equals(item.CUSTOMER_RESERVATION_DETAIL_ID)))
                {
                    var entryDetailToDelete = Context.Entry(item);
                    entryDetailToDelete.State = EntityState.Deleted;
                }
            }
            Context.SaveChanges();

            OnAfterCustomerReservationUpdated(customerReservation);

            return customerReservation;
        }

        public async Task<IQueryable<CustomerOrderInProcess>> GetCustomerOrderInProcesses(Query query = null)
        {
            var items = Context.CustomerOrderInProcesses.AsQueryable();

            items = items.Include(i => i.CustomerOrder);
            items = items.Include(i => i.Employee);
            items = items.Include(i => i.ProcessSatellite);
            items = items.Include(i => i.StatusDocumentType);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }
            return await Task.FromResult(items);
        }

        public async Task<IQueryable<CustomerOrderInProcessDetail>> GetCustomerOrderInProcessDetails(Query query = null)
        {
            var items = Context.CustomerOrderInProcessDetails.AsQueryable();

            items = items.Include(i => i.CustomerOrderInProcess);
            items = items.Include(i => i.CustomerOrderDetail);
            items = items.Include(i => i.Warehouse);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }
            return await Task.FromResult(items);
        }

        public async Task<CustomerOrderInProcess> UpdateCustomerOrderInProgressStatus(CustomerOrderInProcess customerOrderInProcess, short newDocumentStatusId)
        {
            var itemToUpdate = Context.CustomerOrderInProcesses
                              .Where(i => i.CUSTOMER_ORDER_IN_PROCESS_ID == customerOrderInProcess.CUSTOMER_ORDER_IN_PROCESS_ID && i.STATUS_DOCUMENT_TYPE_ID.Equals(customerOrderInProcess.STATUS_DOCUMENT_TYPE_ID))
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            itemToUpdate.STATUS_DOCUMENT_TYPE_ID = newDocumentStatusId;

            Context.CustomerOrderInProcesses.Update(itemToUpdate);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToUpdate).State = EntityState.Unchanged;
                throw;
            }

            return itemToUpdate;
        }

        partial void OnCustomerOrderInProcessCreated(CustomerOrderInProcess customerOrderInProcess);
        partial void OnAfterCustomerOrderInProcessCreated(CustomerOrderInProcess customerOrderInProcess);

        public async Task<CustomerOrderInProcess> CreateCustomerOrderInProcess(CustomerOrderInProcess customerOrderInProcess)
        {

            OnCustomerOrderInProcessCreated(customerOrderInProcess);

            var existingItem = Context.CustomerOrderInProcesses
                              .Where(i => i.CUSTOMER_ORDER_IN_PROCESS_ID == customerOrderInProcess.CUSTOMER_ORDER_IN_PROCESS_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.CustomerOrderInProcesses.Add(customerOrderInProcess);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(customerOrderInProcess).State = EntityState.Detached;
                throw;
            }

            OnAfterCustomerOrderInProcessCreated(customerOrderInProcess);

            return customerOrderInProcess;
        }

        public async Task<IQueryable<ProcessSatellite>> GetProcessSatellites(Query query = null)
        {
            var items = Context.ProcessSatellites.AsQueryable();

            items = items.Include(i => i.IdentityType);
            items = items.Include(i => i.City);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }
            return await Task.FromResult(items);
        }

        partial void OnGetCustomerOrderInProcessByCustomerOrderInProcessId(ref IQueryable<CustomerOrderInProcess> customerOrderInProcesses);

        partial void OnCustomerOrderInProcessGet(CustomerOrderInProcess customerOrderInProcess);

        public async Task<CustomerOrderInProcess> GetCustomerOrderInProcessByCustomerOrderInProcessId(int customerOrderInProcessId)
        {
            var items = Context.CustomerOrderInProcesses
                              .AsNoTracking()
                              .Where(i => i.CUSTOMER_ORDER_IN_PROCESS_ID == customerOrderInProcessId);

            items = items.Include(i => i.CustomerOrder);
            items = items.Include(i => i.Employee);
            items = items.Include(i => i.ProcessSatellite);
            items = items.Include(i => i.StatusDocumentType);

            OnGetCustomerOrderInProcessByCustomerOrderInProcessId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCustomerOrderInProcessGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        public async Task<CustomerOrderInProcess> UpdateCustomerOrderInProcess(CustomerOrderInProcess customerOrderInProcess, IEnumerable<DetailInProcess> detailsInProcess)
        {
            var itemsToUpdate = context.CustomerOrderInProcesses.Where(i => i.CUSTOMER_ORDER_IN_PROCESS_ID.Equals(customerOrderInProcess.CUSTOMER_ORDER_IN_PROCESS_ID));

            if (itemsToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var itemToUpdate = itemsToUpdate.FirstOrDefault();

            itemToUpdate.NOTES = customerOrderInProcess.NOTES;
            itemToUpdate.EMPLOYEE_RECIPIENT_ID = customerOrderInProcess.EMPLOYEE_RECIPIENT_ID;
            itemToUpdate.PROCESS_DATE = customerOrderInProcess.PROCESS_DATE;
            itemToUpdate.TRANSFER_DATETIME = customerOrderInProcess.TRANSFER_DATETIME;
            itemToUpdate.PROCESS_SATELLITE_ID = customerOrderInProcess.PROCESS_SATELLITE_ID;

            var itemDetails = context.CustomerOrderInProcessDetails.Where(i => i.CUSTOMER_ORDER_IN_PROCESS_ID.Equals(customerOrderInProcess.CUSTOMER_ORDER_IN_PROCESS_ID));

            foreach (var item in itemDetails)
                context.CustomerOrderInProcessDetails.Remove(item);

            foreach (var item in detailsInProcess.Where(i => i.THIS_QUANTITY > 0))
            {
                context.CustomerOrderInProcessDetails.Add(new CustomerOrderInProcessDetail()
                {
                    CUSTOMER_ORDER_DETAIL_ID = item.CUSTOMER_ORDER_DETAIL_ID,
                    BRAND = item.BRAND,
                    PROCESSED_QUANTITY = item.THIS_QUANTITY,
                    WAREHOUSE_ID = item.WAREHOUSE_ID,
                    CUSTOMER_ORDER_IN_PROCESS_ID = itemToUpdate.CUSTOMER_ORDER_IN_PROCESS_ID
                });
            }

            try
            {
                Context.CustomerOrderInProcesses.Update(itemToUpdate);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToUpdate).State = EntityState.Detached;
                throw;
            }
            return customerOrderInProcess;
        }

        internal async Task<string> GetDocumentNumber<T>(T entry) where T : class
        {
            context.Entry(entry);
            var documentNumber = context.Set<T>().Count();

            return (documentNumber + 1).ToString().PadLeft(10, '0'); ;
        }
    }
}