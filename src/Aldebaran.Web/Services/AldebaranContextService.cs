using Aldebaran.Web.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Text.Encodings.Web;

namespace Aldebaran.Web
{
    public partial class AldebaranContextService
    {
        AldebaranContext Context
        {
            get
            {
                return this.context;
            }
        }

        private readonly AldebaranContext context;
        private readonly NavigationManager navigationManager;

        public AldebaranContextService(AldebaranContext context, NavigationManager navigationManager)
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


        public async Task ExportActordensToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/actordens/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/actordens/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportActordensToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/actordens/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/actordens/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnActordensRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Actorden> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Actorden>> GetActordens(Query query = null)
        {
            var items = Context.Actordens.AsQueryable();

            items = items.Include(i => i.Ordene);

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

            OnActordensRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnActordenGet(Aldebaran.Web.Models.AldebaranContext.Actorden item);
        partial void OnGetActordenByIdactividadorden(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Actorden> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Actorden> GetActordenByIdactividadorden(int idactividadorden)
        {
            var items = Context.Actordens
                              .AsNoTracking()
                              .Where(i => i.IDACTIVIDADORDEN == idactividadorden);

            items = items.Include(i => i.Ordene);

            OnGetActordenByIdactividadorden(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnActordenGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnActordenCreated(Aldebaran.Web.Models.AldebaranContext.Actorden item);
        partial void OnAfterActordenCreated(Aldebaran.Web.Models.AldebaranContext.Actorden item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Actorden> CreateActorden(Aldebaran.Web.Models.AldebaranContext.Actorden actorden)
        {
            OnActordenCreated(actorden);

            var existingItem = Context.Actordens
                              .Where(i => i.IDACTIVIDADORDEN == actorden.IDACTIVIDADORDEN)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Actordens.Add(actorden);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(actorden).State = EntityState.Detached;
                throw;
            }

            OnAfterActordenCreated(actorden);

            return actorden;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Actorden> CancelActordenChanges(Aldebaran.Web.Models.AldebaranContext.Actorden item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnActordenUpdated(Aldebaran.Web.Models.AldebaranContext.Actorden item);
        partial void OnAfterActordenUpdated(Aldebaran.Web.Models.AldebaranContext.Actorden item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Actorden> UpdateActorden(int idactividadorden, Aldebaran.Web.Models.AldebaranContext.Actorden actorden)
        {
            OnActordenUpdated(actorden);

            var itemToUpdate = Context.Actordens
                              .Where(i => i.IDACTIVIDADORDEN == actorden.IDACTIVIDADORDEN)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(actorden);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterActordenUpdated(actorden);

            return actorden;
        }

        partial void OnActordenDeleted(Aldebaran.Web.Models.AldebaranContext.Actorden item);
        partial void OnAfterActordenDeleted(Aldebaran.Web.Models.AldebaranContext.Actorden item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Actorden> DeleteActorden(int idactividadorden)
        {
            var itemToDelete = Context.Actordens
                              .Where(i => i.IDACTIVIDADORDEN == idactividadorden)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnActordenDeleted(itemToDelete);


            Context.Actordens.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterActordenDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportActpedidosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/actpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/actpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportActpedidosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/actpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/actpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnActpedidosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Actpedido> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Actpedido>> GetActpedidos(Query query = null)
        {
            var items = Context.Actpedidos.AsQueryable();

            items = items.Include(i => i.Area);
            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Pedido);

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

            OnActpedidosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnActpedidoGet(Aldebaran.Web.Models.AldebaranContext.Actpedido item);
        partial void OnGetActpedidoByIdactpedido(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Actpedido> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Actpedido> GetActpedidoByIdactpedido(int idactpedido)
        {
            var items = Context.Actpedidos
                              .AsNoTracking()
                              .Where(i => i.IDACTPEDIDO == idactpedido);

            items = items.Include(i => i.Area);
            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Pedido);

            OnGetActpedidoByIdactpedido(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnActpedidoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnActpedidoCreated(Aldebaran.Web.Models.AldebaranContext.Actpedido item);
        partial void OnAfterActpedidoCreated(Aldebaran.Web.Models.AldebaranContext.Actpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Actpedido> CreateActpedido(Aldebaran.Web.Models.AldebaranContext.Actpedido actpedido)
        {
            OnActpedidoCreated(actpedido);

            var existingItem = Context.Actpedidos
                              .Where(i => i.IDACTPEDIDO == actpedido.IDACTPEDIDO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Actpedidos.Add(actpedido);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(actpedido).State = EntityState.Detached;
                throw;
            }

            OnAfterActpedidoCreated(actpedido);

            return actpedido;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Actpedido> CancelActpedidoChanges(Aldebaran.Web.Models.AldebaranContext.Actpedido item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnActpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.Actpedido item);
        partial void OnAfterActpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.Actpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Actpedido> UpdateActpedido(int idactpedido, Aldebaran.Web.Models.AldebaranContext.Actpedido actpedido)
        {
            OnActpedidoUpdated(actpedido);

            var itemToUpdate = Context.Actpedidos
                              .Where(i => i.IDACTPEDIDO == actpedido.IDACTPEDIDO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(actpedido);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterActpedidoUpdated(actpedido);

            return actpedido;
        }

        partial void OnActpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.Actpedido item);
        partial void OnAfterActpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.Actpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Actpedido> DeleteActpedido(int idactpedido)
        {
            var itemToDelete = Context.Actpedidos
                              .Where(i => i.IDACTPEDIDO == idactpedido)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnActpedidoDeleted(itemToDelete);


            Context.Actpedidos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterActpedidoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportActxactpedidosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/actxactpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/actxactpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportActxactpedidosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/actxactpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/actxactpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnActxactpedidosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Actxactpedido> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Actxactpedido>> GetActxactpedidos(Query query = null)
        {
            var items = Context.Actxactpedidos.AsQueryable();

            items = items.Include(i => i.Actpedido);
            items = items.Include(i => i.Tiposactividad);

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

            OnActxactpedidosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnActxactpedidoGet(Aldebaran.Web.Models.AldebaranContext.Actxactpedido item);
        partial void OnGetActxactpedidoByIdtipoactividadAndIdactpedido(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Actxactpedido> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Actxactpedido> GetActxactpedidoByIdtipoactividadAndIdactpedido(int idtipoactividad, int idactpedido)
        {
            var items = Context.Actxactpedidos
                              .AsNoTracking()
                              .Where(i => i.IDTIPOACTIVIDAD == idtipoactividad && i.IDACTPEDIDO == idactpedido);

            items = items.Include(i => i.Actpedido);
            items = items.Include(i => i.Tiposactividad);

            OnGetActxactpedidoByIdtipoactividadAndIdactpedido(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnActxactpedidoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnActxactpedidoCreated(Aldebaran.Web.Models.AldebaranContext.Actxactpedido item);
        partial void OnAfterActxactpedidoCreated(Aldebaran.Web.Models.AldebaranContext.Actxactpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Actxactpedido> CreateActxactpedido(Aldebaran.Web.Models.AldebaranContext.Actxactpedido actxactpedido)
        {
            OnActxactpedidoCreated(actxactpedido);

            var existingItem = Context.Actxactpedidos
                              .Where(i => i.IDTIPOACTIVIDAD == actxactpedido.IDTIPOACTIVIDAD && i.IDACTPEDIDO == actxactpedido.IDACTPEDIDO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Actxactpedidos.Add(actxactpedido);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(actxactpedido).State = EntityState.Detached;
                throw;
            }

            OnAfterActxactpedidoCreated(actxactpedido);

            return actxactpedido;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Actxactpedido> CancelActxactpedidoChanges(Aldebaran.Web.Models.AldebaranContext.Actxactpedido item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnActxactpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.Actxactpedido item);
        partial void OnAfterActxactpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.Actxactpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Actxactpedido> UpdateActxactpedido(int idtipoactividad, int idactpedido, Aldebaran.Web.Models.AldebaranContext.Actxactpedido actxactpedido)
        {
            OnActxactpedidoUpdated(actxactpedido);

            var itemToUpdate = Context.Actxactpedidos
                              .Where(i => i.IDTIPOACTIVIDAD == actxactpedido.IDTIPOACTIVIDAD && i.IDACTPEDIDO == actxactpedido.IDACTPEDIDO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(actxactpedido);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterActxactpedidoUpdated(actxactpedido);

            return actxactpedido;
        }

        partial void OnActxactpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.Actxactpedido item);
        partial void OnAfterActxactpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.Actxactpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Actxactpedido> DeleteActxactpedido(int idtipoactividad, int idactpedido)
        {
            var itemToDelete = Context.Actxactpedidos
                              .Where(i => i.IDTIPOACTIVIDAD == idtipoactividad && i.IDACTPEDIDO == idactpedido)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnActxactpedidoDeleted(itemToDelete);


            Context.Actxactpedidos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterActxactpedidoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportAgentesforwardersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/agentesforwarders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/agentesforwarders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAgentesforwardersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/agentesforwarders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/agentesforwarders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAgentesforwardersRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Agentesforwarder> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Agentesforwarder>> GetAgentesforwarders(Query query = null)
        {
            var items = Context.Agentesforwarders.AsQueryable();

            items = items.Include(i => i.Forwarder);
            items = items.Include(i => i.Paise);

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

            OnAgentesforwardersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAgentesforwarderGet(Aldebaran.Web.Models.AldebaranContext.Agentesforwarder item);
        partial void OnGetAgentesforwarderByIdagenteforwarder(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Agentesforwarder> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Agentesforwarder> GetAgentesforwarderByIdagenteforwarder(int idagenteforwarder)
        {
            var items = Context.Agentesforwarders
                              .AsNoTracking()
                              .Where(i => i.IDAGENTEFORWARDER == idagenteforwarder);

            items = items.Include(i => i.Forwarder);
            items = items.Include(i => i.Paise);

            OnGetAgentesforwarderByIdagenteforwarder(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAgentesforwarderGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAgentesforwarderCreated(Aldebaran.Web.Models.AldebaranContext.Agentesforwarder item);
        partial void OnAfterAgentesforwarderCreated(Aldebaran.Web.Models.AldebaranContext.Agentesforwarder item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Agentesforwarder> CreateAgentesforwarder(Aldebaran.Web.Models.AldebaranContext.Agentesforwarder agentesforwarder)
        {
            OnAgentesforwarderCreated(agentesforwarder);

            var existingItem = Context.Agentesforwarders
                              .Where(i => i.IDAGENTEFORWARDER == agentesforwarder.IDAGENTEFORWARDER)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Agentesforwarders.Add(agentesforwarder);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(agentesforwarder).State = EntityState.Detached;
                throw;
            }

            OnAfterAgentesforwarderCreated(agentesforwarder);

            return agentesforwarder;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Agentesforwarder> CancelAgentesforwarderChanges(Aldebaran.Web.Models.AldebaranContext.Agentesforwarder item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAgentesforwarderUpdated(Aldebaran.Web.Models.AldebaranContext.Agentesforwarder item);
        partial void OnAfterAgentesforwarderUpdated(Aldebaran.Web.Models.AldebaranContext.Agentesforwarder item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Agentesforwarder> UpdateAgentesforwarder(int idagenteforwarder, Aldebaran.Web.Models.AldebaranContext.Agentesforwarder agentesforwarder)
        {
            OnAgentesforwarderUpdated(agentesforwarder);

            var itemToUpdate = Context.Agentesforwarders
                              .Where(i => i.IDAGENTEFORWARDER == agentesforwarder.IDAGENTEFORWARDER)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(agentesforwarder);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAgentesforwarderUpdated(agentesforwarder);

            return agentesforwarder;
        }

        partial void OnAgentesforwarderDeleted(Aldebaran.Web.Models.AldebaranContext.Agentesforwarder item);
        partial void OnAfterAgentesforwarderDeleted(Aldebaran.Web.Models.AldebaranContext.Agentesforwarder item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Agentesforwarder> DeleteAgentesforwarder(int idagenteforwarder)
        {
            var itemToDelete = Context.Agentesforwarders
                              .Where(i => i.IDAGENTEFORWARDER == idagenteforwarder)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnAgentesforwarderDeleted(itemToDelete);


            Context.Agentesforwarders.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAgentesforwarderDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportAjustesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/ajustes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/ajustes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAjustesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/ajustes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/ajustes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAjustesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Ajuste> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Ajuste>> GetAjustes(Query query = null)
        {
            var items = Context.Ajustes.AsQueryable();

            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Motivajuste);

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

            OnAjustesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAjusteGet(Aldebaran.Web.Models.AldebaranContext.Ajuste item);
        partial void OnGetAjusteByIdajuste(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Ajuste> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Ajuste> GetAjusteByIdajuste(int idajuste)
        {
            var items = Context.Ajustes
                              .AsNoTracking()
                              .Where(i => i.IDAJUSTE == idajuste);

            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Motivajuste);

            OnGetAjusteByIdajuste(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAjusteGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAjusteCreated(Aldebaran.Web.Models.AldebaranContext.Ajuste item);
        partial void OnAfterAjusteCreated(Aldebaran.Web.Models.AldebaranContext.Ajuste item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ajuste> CreateAjuste(Aldebaran.Web.Models.AldebaranContext.Ajuste ajuste)
        {
            OnAjusteCreated(ajuste);

            var existingItem = Context.Ajustes
                              .Where(i => i.IDAJUSTE == ajuste.IDAJUSTE)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Ajustes.Add(ajuste);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(ajuste).State = EntityState.Detached;
                throw;
            }

            OnAfterAjusteCreated(ajuste);

            return ajuste;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ajuste> CancelAjusteChanges(Aldebaran.Web.Models.AldebaranContext.Ajuste item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAjusteUpdated(Aldebaran.Web.Models.AldebaranContext.Ajuste item);
        partial void OnAfterAjusteUpdated(Aldebaran.Web.Models.AldebaranContext.Ajuste item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ajuste> UpdateAjuste(int idajuste, Aldebaran.Web.Models.AldebaranContext.Ajuste ajuste)
        {
            OnAjusteUpdated(ajuste);

            var itemToUpdate = Context.Ajustes
                              .Where(i => i.IDAJUSTE == ajuste.IDAJUSTE)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(ajuste);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAjusteUpdated(ajuste);

            return ajuste;
        }

        partial void OnAjusteDeleted(Aldebaran.Web.Models.AldebaranContext.Ajuste item);
        partial void OnAfterAjusteDeleted(Aldebaran.Web.Models.AldebaranContext.Ajuste item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ajuste> DeleteAjuste(int idajuste)
        {
            var itemToDelete = Context.Ajustes
                              .Where(i => i.IDAJUSTE == idajuste)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnAjusteDeleted(itemToDelete);


            Context.Ajustes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAjusteDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportAjustesinvsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/ajustesinvs/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/ajustesinvs/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAjustesinvsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/ajustesinvs/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/ajustesinvs/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAjustesinvsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Ajustesinv> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Ajustesinv>> GetAjustesinvs(Query query = null)
        {
            var items = Context.Ajustesinvs.AsQueryable();


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

            OnAjustesinvsRead(ref items);

            return await Task.FromResult(items);
        }

        public async Task ExportAjustesxitemsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/ajustesxitems/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/ajustesxitems/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAjustesxitemsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/ajustesxitems/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/ajustesxitems/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAjustesxitemsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Ajustesxitem> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Ajustesxitem>> GetAjustesxitems(Query query = null)
        {
            var items = Context.Ajustesxitems.AsQueryable();

            items = items.Include(i => i.Ajuste);
            items = items.Include(i => i.Item);
            items = items.Include(i => i.Linea);

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

            OnAjustesxitemsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAjustesxitemGet(Aldebaran.Web.Models.AldebaranContext.Ajustesxitem item);
        partial void OnGetAjustesxitemByIddetajuste(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Ajustesxitem> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Ajustesxitem> GetAjustesxitemByIddetajuste(int iddetajuste)
        {
            var items = Context.Ajustesxitems
                              .AsNoTracking()
                              .Where(i => i.IDDETAJUSTE == iddetajuste);

            items = items.Include(i => i.Ajuste);
            items = items.Include(i => i.Item);
            items = items.Include(i => i.Linea);

            OnGetAjustesxitemByIddetajuste(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAjustesxitemGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAjustesxitemCreated(Aldebaran.Web.Models.AldebaranContext.Ajustesxitem item);
        partial void OnAfterAjustesxitemCreated(Aldebaran.Web.Models.AldebaranContext.Ajustesxitem item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ajustesxitem> CreateAjustesxitem(Aldebaran.Web.Models.AldebaranContext.Ajustesxitem ajustesxitem)
        {
            OnAjustesxitemCreated(ajustesxitem);

            var existingItem = Context.Ajustesxitems
                              .Where(i => i.IDDETAJUSTE == ajustesxitem.IDDETAJUSTE)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Ajustesxitems.Add(ajustesxitem);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(ajustesxitem).State = EntityState.Detached;
                throw;
            }

            OnAfterAjustesxitemCreated(ajustesxitem);

            return ajustesxitem;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ajustesxitem> CancelAjustesxitemChanges(Aldebaran.Web.Models.AldebaranContext.Ajustesxitem item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAjustesxitemUpdated(Aldebaran.Web.Models.AldebaranContext.Ajustesxitem item);
        partial void OnAfterAjustesxitemUpdated(Aldebaran.Web.Models.AldebaranContext.Ajustesxitem item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ajustesxitem> UpdateAjustesxitem(int iddetajuste, Aldebaran.Web.Models.AldebaranContext.Ajustesxitem ajustesxitem)
        {
            OnAjustesxitemUpdated(ajustesxitem);

            var itemToUpdate = Context.Ajustesxitems
                              .Where(i => i.IDDETAJUSTE == ajustesxitem.IDDETAJUSTE)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(ajustesxitem);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAjustesxitemUpdated(ajustesxitem);

            return ajustesxitem;
        }

        partial void OnAjustesxitemDeleted(Aldebaran.Web.Models.AldebaranContext.Ajustesxitem item);
        partial void OnAfterAjustesxitemDeleted(Aldebaran.Web.Models.AldebaranContext.Ajustesxitem item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ajustesxitem> DeleteAjustesxitem(int iddetajuste)
        {
            var itemToDelete = Context.Ajustesxitems
                              .Where(i => i.IDDETAJUSTE == iddetajuste)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnAjustesxitemDeleted(itemToDelete);


            Context.Ajustesxitems.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAjustesxitemDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportAlarmasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/alarmas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/alarmas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAlarmasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/alarmas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/alarmas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAlarmasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Alarma> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Alarma>> GetAlarmas(Query query = null)
        {
            var items = Context.Alarmas.AsQueryable();

            items = items.Include(i => i.Tiposalarma);

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

            OnAlarmasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAlarmaGet(Aldebaran.Web.Models.AldebaranContext.Alarma item);
        partial void OnGetAlarmaByIdalarma(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Alarma> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Alarma> GetAlarmaByIdalarma(int idalarma)
        {
            var items = Context.Alarmas
                              .AsNoTracking()
                              .Where(i => i.IDALARMA == idalarma);

            items = items.Include(i => i.Tiposalarma);

            OnGetAlarmaByIdalarma(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAlarmaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAlarmaCreated(Aldebaran.Web.Models.AldebaranContext.Alarma item);
        partial void OnAfterAlarmaCreated(Aldebaran.Web.Models.AldebaranContext.Alarma item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Alarma> CreateAlarma(Aldebaran.Web.Models.AldebaranContext.Alarma alarma)
        {
            OnAlarmaCreated(alarma);

            var existingItem = Context.Alarmas
                              .Where(i => i.IDALARMA == alarma.IDALARMA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Alarmas.Add(alarma);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(alarma).State = EntityState.Detached;
                throw;
            }

            OnAfterAlarmaCreated(alarma);

            return alarma;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Alarma> CancelAlarmaChanges(Aldebaran.Web.Models.AldebaranContext.Alarma item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAlarmaUpdated(Aldebaran.Web.Models.AldebaranContext.Alarma item);
        partial void OnAfterAlarmaUpdated(Aldebaran.Web.Models.AldebaranContext.Alarma item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Alarma> UpdateAlarma(int idalarma, Aldebaran.Web.Models.AldebaranContext.Alarma alarma)
        {
            OnAlarmaUpdated(alarma);

            var itemToUpdate = Context.Alarmas
                              .Where(i => i.IDALARMA == alarma.IDALARMA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(alarma);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAlarmaUpdated(alarma);

            return alarma;
        }

        partial void OnAlarmaDeleted(Aldebaran.Web.Models.AldebaranContext.Alarma item);
        partial void OnAfterAlarmaDeleted(Aldebaran.Web.Models.AldebaranContext.Alarma item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Alarma> DeleteAlarma(int idalarma)
        {
            var itemToDelete = Context.Alarmas
                              .Where(i => i.IDALARMA == idalarma)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnAlarmaDeleted(itemToDelete);


            Context.Alarmas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAlarmaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportAlarmascantidadesminimasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/alarmascantidadesminimas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/alarmascantidadesminimas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAlarmascantidadesminimasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/alarmascantidadesminimas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/alarmascantidadesminimas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAlarmascantidadesminimasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima>> GetAlarmascantidadesminimas(Query query = null)
        {
            var items = Context.Alarmascantidadesminimas.AsQueryable();

            items = items.Include(i => i.Itemsxcolor);

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

            OnAlarmascantidadesminimasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAlarmascantidadesminimaGet(Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima item);
        partial void OnGetAlarmascantidadesminimaByIdalarma(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima> GetAlarmascantidadesminimaByIdalarma(int idalarma)
        {
            var items = Context.Alarmascantidadesminimas
                              .AsNoTracking()
                              .Where(i => i.IDALARMA == idalarma);

            items = items.Include(i => i.Itemsxcolor);

            OnGetAlarmascantidadesminimaByIdalarma(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAlarmascantidadesminimaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAlarmascantidadesminimaCreated(Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima item);
        partial void OnAfterAlarmascantidadesminimaCreated(Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima> CreateAlarmascantidadesminima(Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima alarmascantidadesminima)
        {
            OnAlarmascantidadesminimaCreated(alarmascantidadesminima);

            var existingItem = Context.Alarmascantidadesminimas
                              .Where(i => i.IDALARMA == alarmascantidadesminima.IDALARMA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Alarmascantidadesminimas.Add(alarmascantidadesminima);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(alarmascantidadesminima).State = EntityState.Detached;
                throw;
            }

            OnAfterAlarmascantidadesminimaCreated(alarmascantidadesminima);

            return alarmascantidadesminima;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima> CancelAlarmascantidadesminimaChanges(Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAlarmascantidadesminimaUpdated(Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima item);
        partial void OnAfterAlarmascantidadesminimaUpdated(Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima> UpdateAlarmascantidadesminima(int idalarma, Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima alarmascantidadesminima)
        {
            OnAlarmascantidadesminimaUpdated(alarmascantidadesminima);

            var itemToUpdate = Context.Alarmascantidadesminimas
                              .Where(i => i.IDALARMA == alarmascantidadesminima.IDALARMA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(alarmascantidadesminima);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAlarmascantidadesminimaUpdated(alarmascantidadesminima);

            return alarmascantidadesminima;
        }

        partial void OnAlarmascantidadesminimaDeleted(Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima item);
        partial void OnAfterAlarmascantidadesminimaDeleted(Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima> DeleteAlarmascantidadesminima(int idalarma)
        {
            var itemToDelete = Context.Alarmascantidadesminimas
                              .Where(i => i.IDALARMA == idalarma)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnAlarmascantidadesminimaDeleted(itemToDelete);


            Context.Alarmascantidadesminimas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAlarmascantidadesminimaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportAnulacionreservasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/anulacionreservas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/anulacionreservas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAnulacionreservasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/anulacionreservas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/anulacionreservas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAnulacionreservasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Anulacionreserva> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Anulacionreserva>> GetAnulacionreservas(Query query = null)
        {
            var items = Context.Anulacionreservas.AsQueryable();

            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Motivodevolucion);
            items = items.Include(i => i.Reserva);

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

            OnAnulacionreservasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAnulacionreservaGet(Aldebaran.Web.Models.AldebaranContext.Anulacionreserva item);
        partial void OnGetAnulacionreservaByIdreserva(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Anulacionreserva> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Anulacionreserva> GetAnulacionreservaByIdreserva(int idreserva)
        {
            var items = Context.Anulacionreservas
                              .AsNoTracking()
                              .Where(i => i.IDRESERVA == idreserva);

            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Motivodevolucion);
            items = items.Include(i => i.Reserva);

            OnGetAnulacionreservaByIdreserva(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAnulacionreservaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAnulacionreservaCreated(Aldebaran.Web.Models.AldebaranContext.Anulacionreserva item);
        partial void OnAfterAnulacionreservaCreated(Aldebaran.Web.Models.AldebaranContext.Anulacionreserva item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Anulacionreserva> CreateAnulacionreserva(Aldebaran.Web.Models.AldebaranContext.Anulacionreserva anulacionreserva)
        {
            OnAnulacionreservaCreated(anulacionreserva);

            var existingItem = Context.Anulacionreservas
                              .Where(i => i.IDRESERVA == anulacionreserva.IDRESERVA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Anulacionreservas.Add(anulacionreserva);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(anulacionreserva).State = EntityState.Detached;
                throw;
            }

            OnAfterAnulacionreservaCreated(anulacionreserva);

            return anulacionreserva;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Anulacionreserva> CancelAnulacionreservaChanges(Aldebaran.Web.Models.AldebaranContext.Anulacionreserva item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAnulacionreservaUpdated(Aldebaran.Web.Models.AldebaranContext.Anulacionreserva item);
        partial void OnAfterAnulacionreservaUpdated(Aldebaran.Web.Models.AldebaranContext.Anulacionreserva item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Anulacionreserva> UpdateAnulacionreserva(int idreserva, Aldebaran.Web.Models.AldebaranContext.Anulacionreserva anulacionreserva)
        {
            OnAnulacionreservaUpdated(anulacionreserva);

            var itemToUpdate = Context.Anulacionreservas
                              .Where(i => i.IDRESERVA == anulacionreserva.IDRESERVA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(anulacionreserva);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAnulacionreservaUpdated(anulacionreserva);

            return anulacionreserva;
        }

        partial void OnAnulacionreservaDeleted(Aldebaran.Web.Models.AldebaranContext.Anulacionreserva item);
        partial void OnAfterAnulacionreservaDeleted(Aldebaran.Web.Models.AldebaranContext.Anulacionreserva item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Anulacionreserva> DeleteAnulacionreserva(int idreserva)
        {
            var itemToDelete = Context.Anulacionreservas
                              .Where(i => i.IDRESERVA == idreserva)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnAnulacionreservaDeleted(itemToDelete);


            Context.Anulacionreservas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAnulacionreservaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportAnuladetcantprocesosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/anuladetcantprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/anuladetcantprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAnuladetcantprocesosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/anuladetcantprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/anuladetcantprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAnuladetcantprocesosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso>> GetAnuladetcantprocesos(Query query = null)
        {
            var items = Context.Anuladetcantprocesos.AsQueryable();

            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Linea);
            items = items.Include(i => i.Cantproceso);

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

            OnAnuladetcantprocesosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAnuladetcantprocesoGet(Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso item);
        partial void OnGetAnuladetcantprocesoByIddetanulaproceso(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso> GetAnuladetcantprocesoByIddetanulaproceso(int iddetanulaproceso)
        {
            var items = Context.Anuladetcantprocesos
                              .AsNoTracking()
                              .Where(i => i.IDDETANULAPROCESO == iddetanulaproceso);

            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Linea);
            items = items.Include(i => i.Cantproceso);

            OnGetAnuladetcantprocesoByIddetanulaproceso(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAnuladetcantprocesoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAnuladetcantprocesoCreated(Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso item);
        partial void OnAfterAnuladetcantprocesoCreated(Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso> CreateAnuladetcantproceso(Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso anuladetcantproceso)
        {
            OnAnuladetcantprocesoCreated(anuladetcantproceso);

            var existingItem = Context.Anuladetcantprocesos
                              .Where(i => i.IDDETANULAPROCESO == anuladetcantproceso.IDDETANULAPROCESO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Anuladetcantprocesos.Add(anuladetcantproceso);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(anuladetcantproceso).State = EntityState.Detached;
                throw;
            }

            OnAfterAnuladetcantprocesoCreated(anuladetcantproceso);

            return anuladetcantproceso;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso> CancelAnuladetcantprocesoChanges(Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAnuladetcantprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso item);
        partial void OnAfterAnuladetcantprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso> UpdateAnuladetcantproceso(int iddetanulaproceso, Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso anuladetcantproceso)
        {
            OnAnuladetcantprocesoUpdated(anuladetcantproceso);

            var itemToUpdate = Context.Anuladetcantprocesos
                              .Where(i => i.IDDETANULAPROCESO == anuladetcantproceso.IDDETANULAPROCESO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(anuladetcantproceso);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAnuladetcantprocesoUpdated(anuladetcantproceso);

            return anuladetcantproceso;
        }

        partial void OnAnuladetcantprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso item);
        partial void OnAfterAnuladetcantprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso> DeleteAnuladetcantproceso(int iddetanulaproceso)
        {
            var itemToDelete = Context.Anuladetcantprocesos
                              .Where(i => i.IDDETANULAPROCESO == iddetanulaproceso)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnAnuladetcantprocesoDeleted(itemToDelete);


            Context.Anuladetcantprocesos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAnuladetcantprocesoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportAnulaprocesosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/anulaprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/anulaprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAnulaprocesosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/anulaprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/anulaprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAnulaprocesosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Anulaproceso> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Anulaproceso>> GetAnulaprocesos(Query query = null)
        {
            var items = Context.Anulaprocesos.AsQueryable();

            items = items.Include(i => i.Pedido);
            items = items.Include(i => i.Cantproceso);
            items = items.Include(i => i.Satelite);

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

            OnAnulaprocesosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAnulaprocesoGet(Aldebaran.Web.Models.AldebaranContext.Anulaproceso item);
        partial void OnGetAnulaprocesoByIdanulaproceso(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Anulaproceso> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Anulaproceso> GetAnulaprocesoByIdanulaproceso(int idanulaproceso)
        {
            var items = Context.Anulaprocesos
                              .AsNoTracking()
                              .Where(i => i.IDANULAPROCESO == idanulaproceso);

            items = items.Include(i => i.Pedido);
            items = items.Include(i => i.Cantproceso);
            items = items.Include(i => i.Satelite);

            OnGetAnulaprocesoByIdanulaproceso(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAnulaprocesoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAnulaprocesoCreated(Aldebaran.Web.Models.AldebaranContext.Anulaproceso item);
        partial void OnAfterAnulaprocesoCreated(Aldebaran.Web.Models.AldebaranContext.Anulaproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Anulaproceso> CreateAnulaproceso(Aldebaran.Web.Models.AldebaranContext.Anulaproceso anulaproceso)
        {
            OnAnulaprocesoCreated(anulaproceso);

            var existingItem = Context.Anulaprocesos
                              .Where(i => i.IDANULAPROCESO == anulaproceso.IDANULAPROCESO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Anulaprocesos.Add(anulaproceso);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(anulaproceso).State = EntityState.Detached;
                throw;
            }

            OnAfterAnulaprocesoCreated(anulaproceso);

            return anulaproceso;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Anulaproceso> CancelAnulaprocesoChanges(Aldebaran.Web.Models.AldebaranContext.Anulaproceso item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAnulaprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.Anulaproceso item);
        partial void OnAfterAnulaprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.Anulaproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Anulaproceso> UpdateAnulaproceso(int idanulaproceso, Aldebaran.Web.Models.AldebaranContext.Anulaproceso anulaproceso)
        {
            OnAnulaprocesoUpdated(anulaproceso);

            var itemToUpdate = Context.Anulaprocesos
                              .Where(i => i.IDANULAPROCESO == anulaproceso.IDANULAPROCESO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(anulaproceso);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAnulaprocesoUpdated(anulaproceso);

            return anulaproceso;
        }

        partial void OnAnulaprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.Anulaproceso item);
        partial void OnAfterAnulaprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.Anulaproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Anulaproceso> DeleteAnulaproceso(int idanulaproceso)
        {
            var itemToDelete = Context.Anulaprocesos
                              .Where(i => i.IDANULAPROCESO == idanulaproceso)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnAnulaprocesoDeleted(itemToDelete);


            Context.Anulaprocesos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAnulaprocesoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportAnulasubitemdetprocesosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/anulasubitemdetprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/anulasubitemdetprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAnulasubitemdetprocesosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/anulasubitemdetprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/anulasubitemdetprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAnulasubitemdetprocesosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso>> GetAnulasubitemdetprocesos(Query query = null)
        {
            var items = Context.Anulasubitemdetprocesos.AsQueryable();

            items = items.Include(i => i.Anulaproceso);
            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Anuladetcantproceso);
            items = items.Include(i => i.Detcantproceso);
            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Itemsxcolor1);
            items = items.Include(i => i.Cantproceso);

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

            OnAnulasubitemdetprocesosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAnulasubitemdetprocesoGet(Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso item);
        partial void OnGetAnulasubitemdetprocesoByIdansubitemdetproceso(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso> GetAnulasubitemdetprocesoByIdansubitemdetproceso(int idansubitemdetproceso)
        {
            var items = Context.Anulasubitemdetprocesos
                              .AsNoTracking()
                              .Where(i => i.IDANSUBITEMDETPROCESO == idansubitemdetproceso);

            items = items.Include(i => i.Anulaproceso);
            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Anuladetcantproceso);
            items = items.Include(i => i.Detcantproceso);
            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Itemsxcolor1);
            items = items.Include(i => i.Cantproceso);

            OnGetAnulasubitemdetprocesoByIdansubitemdetproceso(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAnulasubitemdetprocesoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAnulasubitemdetprocesoCreated(Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso item);
        partial void OnAfterAnulasubitemdetprocesoCreated(Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso> CreateAnulasubitemdetproceso(Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso anulasubitemdetproceso)
        {
            OnAnulasubitemdetprocesoCreated(anulasubitemdetproceso);

            var existingItem = Context.Anulasubitemdetprocesos
                              .Where(i => i.IDANSUBITEMDETPROCESO == anulasubitemdetproceso.IDANSUBITEMDETPROCESO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Anulasubitemdetprocesos.Add(anulasubitemdetproceso);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(anulasubitemdetproceso).State = EntityState.Detached;
                throw;
            }

            OnAfterAnulasubitemdetprocesoCreated(anulasubitemdetproceso);

            return anulasubitemdetproceso;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso> CancelAnulasubitemdetprocesoChanges(Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAnulasubitemdetprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso item);
        partial void OnAfterAnulasubitemdetprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso> UpdateAnulasubitemdetproceso(int idansubitemdetproceso, Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso anulasubitemdetproceso)
        {
            OnAnulasubitemdetprocesoUpdated(anulasubitemdetproceso);

            var itemToUpdate = Context.Anulasubitemdetprocesos
                              .Where(i => i.IDANSUBITEMDETPROCESO == anulasubitemdetproceso.IDANSUBITEMDETPROCESO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(anulasubitemdetproceso);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAnulasubitemdetprocesoUpdated(anulasubitemdetproceso);

            return anulasubitemdetproceso;
        }

        partial void OnAnulasubitemdetprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso item);
        partial void OnAfterAnulasubitemdetprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso> DeleteAnulasubitemdetproceso(int idansubitemdetproceso)
        {
            var itemToDelete = Context.Anulasubitemdetprocesos
                              .Where(i => i.IDANSUBITEMDETPROCESO == idansubitemdetproceso)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnAnulasubitemdetprocesoDeleted(itemToDelete);


            Context.Anulasubitemdetprocesos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAnulasubitemdetprocesoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportAreasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/areas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/areas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAreasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/areas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/areas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAreasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Area> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Area>> GetAreas(Query query = null)
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

        partial void OnAreaGet(Aldebaran.Web.Models.AldebaranContext.Area item);
        partial void OnGetAreaByIdarea(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Area> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Area> GetAreaByIdarea(int idarea)
        {
            var items = Context.Areas
                              .AsNoTracking()
                              .Where(i => i.IDAREA == idarea);


            OnGetAreaByIdarea(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAreaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAreaCreated(Aldebaran.Web.Models.AldebaranContext.Area item);
        partial void OnAfterAreaCreated(Aldebaran.Web.Models.AldebaranContext.Area item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Area> CreateArea(Aldebaran.Web.Models.AldebaranContext.Area area)
        {
            OnAreaCreated(area);

            var existingItem = Context.Areas
                              .Where(i => i.IDAREA == area.IDAREA)
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

        public async Task<Aldebaran.Web.Models.AldebaranContext.Area> CancelAreaChanges(Aldebaran.Web.Models.AldebaranContext.Area item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAreaUpdated(Aldebaran.Web.Models.AldebaranContext.Area item);
        partial void OnAfterAreaUpdated(Aldebaran.Web.Models.AldebaranContext.Area item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Area> UpdateArea(int idarea, Aldebaran.Web.Models.AldebaranContext.Area area)
        {
            OnAreaUpdated(area);

            var itemToUpdate = Context.Areas
                              .Where(i => i.IDAREA == area.IDAREA)
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

        partial void OnAreaDeleted(Aldebaran.Web.Models.AldebaranContext.Area item);
        partial void OnAfterAreaDeleted(Aldebaran.Web.Models.AldebaranContext.Area item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Area> DeleteArea(int idarea)
        {
            var itemToDelete = Context.Areas
                              .Where(i => i.IDAREA == idarea)
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

        public async Task ExportAuxactordensToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/auxactordens/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/auxactordens/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAuxactordensToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/auxactordens/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/auxactordens/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAuxactordensRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Auxactorden> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Auxactorden>> GetAuxactordens(Query query = null)
        {
            var items = Context.Auxactordens.AsQueryable();


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

            OnAuxactordensRead(ref items);

            return await Task.FromResult(items);
        }

        public async Task ExportAuxitemsxcolorsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/auxitemsxcolors/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/auxitemsxcolors/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAuxitemsxcolorsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/auxitemsxcolors/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/auxitemsxcolors/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAuxitemsxcolorsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Auxitemsxcolor> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Auxitemsxcolor>> GetAuxitemsxcolors(Query query = null)
        {
            var items = Context.Auxitemsxcolors.AsQueryable();


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

            OnAuxitemsxcolorsRead(ref items);

            return await Task.FromResult(items);
        }

        public async Task ExportAuxordenesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/auxordenes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/auxordenes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAuxordenesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/auxordenes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/auxordenes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAuxordenesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Auxordene> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Auxordene>> GetAuxordenes(Query query = null)
        {
            var items = Context.Auxordenes.AsQueryable();


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

            OnAuxordenesRead(ref items);

            return await Task.FromResult(items);
        }

        public async Task ExportBodegasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/bodegas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/bodegas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportBodegasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/bodegas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/bodegas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnBodegasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Bodega> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Bodega>> GetBodegas(Query query = null)
        {
            var items = Context.Bodegas.AsQueryable();


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

            OnBodegasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnBodegaGet(Aldebaran.Web.Models.AldebaranContext.Bodega item);
        partial void OnGetBodegaByIdbodega(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Bodega> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Bodega> GetBodegaByIdbodega(int idbodega)
        {
            var items = Context.Bodegas
                              .AsNoTracking()
                              .Where(i => i.IDBODEGA == idbodega);


            OnGetBodegaByIdbodega(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnBodegaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnBodegaCreated(Aldebaran.Web.Models.AldebaranContext.Bodega item);
        partial void OnAfterBodegaCreated(Aldebaran.Web.Models.AldebaranContext.Bodega item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Bodega> CreateBodega(Aldebaran.Web.Models.AldebaranContext.Bodega bodega)
        {
            OnBodegaCreated(bodega);

            var existingItem = Context.Bodegas
                              .Where(i => i.IDBODEGA == bodega.IDBODEGA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Bodegas.Add(bodega);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(bodega).State = EntityState.Detached;
                throw;
            }

            OnAfterBodegaCreated(bodega);

            return bodega;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Bodega> CancelBodegaChanges(Aldebaran.Web.Models.AldebaranContext.Bodega item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnBodegaUpdated(Aldebaran.Web.Models.AldebaranContext.Bodega item);
        partial void OnAfterBodegaUpdated(Aldebaran.Web.Models.AldebaranContext.Bodega item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Bodega> UpdateBodega(int idbodega, Aldebaran.Web.Models.AldebaranContext.Bodega bodega)
        {
            OnBodegaUpdated(bodega);

            var itemToUpdate = Context.Bodegas
                              .Where(i => i.IDBODEGA == bodega.IDBODEGA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(bodega);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterBodegaUpdated(bodega);

            return bodega;
        }

        partial void OnBodegaDeleted(Aldebaran.Web.Models.AldebaranContext.Bodega item);
        partial void OnAfterBodegaDeleted(Aldebaran.Web.Models.AldebaranContext.Bodega item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Bodega> DeleteBodega(int idbodega)
        {
            var itemToDelete = Context.Bodegas
                              .Where(i => i.IDBODEGA == idbodega)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnBodegaDeleted(itemToDelete);


            Context.Bodegas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterBodegaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportCancelpedidosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/cancelpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/cancelpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCancelpedidosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/cancelpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/cancelpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCancelpedidosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Cancelpedido> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Cancelpedido>> GetCancelpedidos(Query query = null)
        {
            var items = Context.Cancelpedidos.AsQueryable();

            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Motivodevolucion);
            items = items.Include(i => i.Pedido);

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

            OnCancelpedidosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCancelpedidoGet(Aldebaran.Web.Models.AldebaranContext.Cancelpedido item);
        partial void OnGetCancelpedidoByIdcancelpedido(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Cancelpedido> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Cancelpedido> GetCancelpedidoByIdcancelpedido(int idcancelpedido)
        {
            var items = Context.Cancelpedidos
                              .AsNoTracking()
                              .Where(i => i.IDCANCELPEDIDO == idcancelpedido);

            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Motivodevolucion);
            items = items.Include(i => i.Pedido);

            OnGetCancelpedidoByIdcancelpedido(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCancelpedidoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCancelpedidoCreated(Aldebaran.Web.Models.AldebaranContext.Cancelpedido item);
        partial void OnAfterCancelpedidoCreated(Aldebaran.Web.Models.AldebaranContext.Cancelpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Cancelpedido> CreateCancelpedido(Aldebaran.Web.Models.AldebaranContext.Cancelpedido cancelpedido)
        {
            OnCancelpedidoCreated(cancelpedido);

            var existingItem = Context.Cancelpedidos
                              .Where(i => i.IDCANCELPEDIDO == cancelpedido.IDCANCELPEDIDO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Cancelpedidos.Add(cancelpedido);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(cancelpedido).State = EntityState.Detached;
                throw;
            }

            OnAfterCancelpedidoCreated(cancelpedido);

            return cancelpedido;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Cancelpedido> CancelCancelpedidoChanges(Aldebaran.Web.Models.AldebaranContext.Cancelpedido item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCancelpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.Cancelpedido item);
        partial void OnAfterCancelpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.Cancelpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Cancelpedido> UpdateCancelpedido(int idcancelpedido, Aldebaran.Web.Models.AldebaranContext.Cancelpedido cancelpedido)
        {
            OnCancelpedidoUpdated(cancelpedido);

            var itemToUpdate = Context.Cancelpedidos
                              .Where(i => i.IDCANCELPEDIDO == cancelpedido.IDCANCELPEDIDO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(cancelpedido);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCancelpedidoUpdated(cancelpedido);

            return cancelpedido;
        }

        partial void OnCancelpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.Cancelpedido item);
        partial void OnAfterCancelpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.Cancelpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Cancelpedido> DeleteCancelpedido(int idcancelpedido)
        {
            var itemToDelete = Context.Cancelpedidos
                              .Where(i => i.IDCANCELPEDIDO == idcancelpedido)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnCancelpedidoDeleted(itemToDelete);


            Context.Cancelpedidos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCancelpedidoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportCantprocesosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/cantprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/cantprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCantprocesosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/cantprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/cantprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCantprocesosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Cantproceso> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Cantproceso>> GetCantprocesos(Query query = null)
        {
            var items = Context.Cantprocesos.AsQueryable();

            items = items.Include(i => i.Pedido);
            items = items.Include(i => i.Satelite);

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

            OnCantprocesosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCantprocesoGet(Aldebaran.Web.Models.AldebaranContext.Cantproceso item);
        partial void OnGetCantprocesoByIdproceso(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Cantproceso> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Cantproceso> GetCantprocesoByIdproceso(int idproceso)
        {
            var items = Context.Cantprocesos
                              .AsNoTracking()
                              .Where(i => i.IDPROCESO == idproceso);

            items = items.Include(i => i.Pedido);
            items = items.Include(i => i.Satelite);

            OnGetCantprocesoByIdproceso(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCantprocesoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCantprocesoCreated(Aldebaran.Web.Models.AldebaranContext.Cantproceso item);
        partial void OnAfterCantprocesoCreated(Aldebaran.Web.Models.AldebaranContext.Cantproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Cantproceso> CreateCantproceso(Aldebaran.Web.Models.AldebaranContext.Cantproceso cantproceso)
        {
            OnCantprocesoCreated(cantproceso);

            var existingItem = Context.Cantprocesos
                              .Where(i => i.IDPROCESO == cantproceso.IDPROCESO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Cantprocesos.Add(cantproceso);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(cantproceso).State = EntityState.Detached;
                throw;
            }

            OnAfterCantprocesoCreated(cantproceso);

            return cantproceso;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Cantproceso> CancelCantprocesoChanges(Aldebaran.Web.Models.AldebaranContext.Cantproceso item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCantprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.Cantproceso item);
        partial void OnAfterCantprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.Cantproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Cantproceso> UpdateCantproceso(int idproceso, Aldebaran.Web.Models.AldebaranContext.Cantproceso cantproceso)
        {
            OnCantprocesoUpdated(cantproceso);

            var itemToUpdate = Context.Cantprocesos
                              .Where(i => i.IDPROCESO == cantproceso.IDPROCESO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(cantproceso);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCantprocesoUpdated(cantproceso);

            return cantproceso;
        }

        partial void OnCantprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.Cantproceso item);
        partial void OnAfterCantprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.Cantproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Cantproceso> DeleteCantproceso(int idproceso)
        {
            var itemToDelete = Context.Cantprocesos
                              .Where(i => i.IDPROCESO == idproceso)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnCantprocesoDeleted(itemToDelete);


            Context.Cantprocesos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCantprocesoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportCierrepedidosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/cierrepedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/cierrepedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCierrepedidosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/cierrepedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/cierrepedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCierrepedidosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Cierrepedido> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Cierrepedido>> GetCierrepedidos(Query query = null)
        {
            var items = Context.Cierrepedidos.AsQueryable();

            items = items.Include(i => i.Pedido);

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

            OnCierrepedidosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCierrepedidoGet(Aldebaran.Web.Models.AldebaranContext.Cierrepedido item);
        partial void OnGetCierrepedidoById(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Cierrepedido> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Cierrepedido> GetCierrepedidoById(int id)
        {
            var items = Context.Cierrepedidos
                              .AsNoTracking()
                              .Where(i => i.ID == id);

            items = items.Include(i => i.Pedido);

            OnGetCierrepedidoById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCierrepedidoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCierrepedidoCreated(Aldebaran.Web.Models.AldebaranContext.Cierrepedido item);
        partial void OnAfterCierrepedidoCreated(Aldebaran.Web.Models.AldebaranContext.Cierrepedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Cierrepedido> CreateCierrepedido(Aldebaran.Web.Models.AldebaranContext.Cierrepedido cierrepedido)
        {
            OnCierrepedidoCreated(cierrepedido);

            var existingItem = Context.Cierrepedidos
                              .Where(i => i.ID == cierrepedido.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Cierrepedidos.Add(cierrepedido);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(cierrepedido).State = EntityState.Detached;
                throw;
            }

            OnAfterCierrepedidoCreated(cierrepedido);

            return cierrepedido;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Cierrepedido> CancelCierrepedidoChanges(Aldebaran.Web.Models.AldebaranContext.Cierrepedido item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCierrepedidoUpdated(Aldebaran.Web.Models.AldebaranContext.Cierrepedido item);
        partial void OnAfterCierrepedidoUpdated(Aldebaran.Web.Models.AldebaranContext.Cierrepedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Cierrepedido> UpdateCierrepedido(int id, Aldebaran.Web.Models.AldebaranContext.Cierrepedido cierrepedido)
        {
            OnCierrepedidoUpdated(cierrepedido);

            var itemToUpdate = Context.Cierrepedidos
                              .Where(i => i.ID == cierrepedido.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(cierrepedido);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCierrepedidoUpdated(cierrepedido);

            return cierrepedido;
        }

        partial void OnCierrepedidoDeleted(Aldebaran.Web.Models.AldebaranContext.Cierrepedido item);
        partial void OnAfterCierrepedidoDeleted(Aldebaran.Web.Models.AldebaranContext.Cierrepedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Cierrepedido> DeleteCierrepedido(int id)
        {
            var itemToDelete = Context.Cierrepedidos
                              .Where(i => i.ID == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnCierrepedidoDeleted(itemToDelete);


            Context.Cierrepedidos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCierrepedidoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportCiudadesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/ciudades/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/ciudades/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCiudadesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/ciudades/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/ciudades/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCiudadesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Ciudade> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Ciudade>> GetCiudades(Query query = null)
        {
            var items = Context.Ciudades.AsQueryable();

            items = items.Include(i => i.Departamento);

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

            OnCiudadesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCiudadeGet(Aldebaran.Web.Models.AldebaranContext.Ciudade item);
        partial void OnGetCiudadeByIdciudad(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Ciudade> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Ciudade> GetCiudadeByIdciudad(int idciudad)
        {
            var items = Context.Ciudades
                              .AsNoTracking()
                              .Where(i => i.IDCIUDAD == idciudad);

            items = items.Include(i => i.Departamento);

            OnGetCiudadeByIdciudad(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCiudadeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCiudadeCreated(Aldebaran.Web.Models.AldebaranContext.Ciudade item);
        partial void OnAfterCiudadeCreated(Aldebaran.Web.Models.AldebaranContext.Ciudade item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ciudade> CreateCiudade(Aldebaran.Web.Models.AldebaranContext.Ciudade ciudade)
        {
            OnCiudadeCreated(ciudade);

            var existingItem = Context.Ciudades
                              .Where(i => i.IDCIUDAD == ciudade.IDCIUDAD)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Ciudades.Add(ciudade);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(ciudade).State = EntityState.Detached;
                throw;
            }

            OnAfterCiudadeCreated(ciudade);

            return ciudade;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ciudade> CancelCiudadeChanges(Aldebaran.Web.Models.AldebaranContext.Ciudade item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCiudadeUpdated(Aldebaran.Web.Models.AldebaranContext.Ciudade item);
        partial void OnAfterCiudadeUpdated(Aldebaran.Web.Models.AldebaranContext.Ciudade item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ciudade> UpdateCiudade(int idciudad, Aldebaran.Web.Models.AldebaranContext.Ciudade ciudade)
        {
            OnCiudadeUpdated(ciudade);

            var itemToUpdate = Context.Ciudades
                              .Where(i => i.IDCIUDAD == ciudade.IDCIUDAD)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(ciudade);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCiudadeUpdated(ciudade);

            return ciudade;
        }

        partial void OnCiudadeDeleted(Aldebaran.Web.Models.AldebaranContext.Ciudade item);
        partial void OnAfterCiudadeDeleted(Aldebaran.Web.Models.AldebaranContext.Ciudade item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ciudade> DeleteCiudade(int idciudad)
        {
            var itemToDelete = Context.Ciudades
                              .Where(i => i.IDCIUDAD == idciudad)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnCiudadeDeleted(itemToDelete);


            Context.Ciudades.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCiudadeDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportClientesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/clientes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/clientes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportClientesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/clientes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/clientes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnClientesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Cliente> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Cliente>> GetClientes(Query query = null)
        {
            var items = Context.Clientes.AsQueryable();

            items = items.Include(i => i.Ciudade);
            items = items.Include(i => i.Tipidentifica);

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

            OnClientesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnClienteGet(Aldebaran.Web.Models.AldebaranContext.Cliente item);
        partial void OnGetClienteByIdcliente(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Cliente> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Cliente> GetClienteByIdcliente(int idcliente)
        {
            var items = Context.Clientes
                              .AsNoTracking()
                              .Where(i => i.IDCLIENTE == idcliente);

            items = items.Include(i => i.Ciudade);
            items = items.Include(i => i.Tipidentifica);

            OnGetClienteByIdcliente(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnClienteGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnClienteCreated(Aldebaran.Web.Models.AldebaranContext.Cliente item);
        partial void OnAfterClienteCreated(Aldebaran.Web.Models.AldebaranContext.Cliente item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Cliente> CreateCliente(Aldebaran.Web.Models.AldebaranContext.Cliente cliente)
        {
            OnClienteCreated(cliente);

            var existingItem = Context.Clientes
                              .Where(i => i.IDCLIENTE == cliente.IDCLIENTE)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Clientes.Add(cliente);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(cliente).State = EntityState.Detached;
                throw;
            }

            OnAfterClienteCreated(cliente);

            return cliente;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Cliente> CancelClienteChanges(Aldebaran.Web.Models.AldebaranContext.Cliente item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnClienteUpdated(Aldebaran.Web.Models.AldebaranContext.Cliente item);
        partial void OnAfterClienteUpdated(Aldebaran.Web.Models.AldebaranContext.Cliente item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Cliente> UpdateCliente(int idcliente, Aldebaran.Web.Models.AldebaranContext.Cliente cliente)
        {
            OnClienteUpdated(cliente);

            var itemToUpdate = Context.Clientes
                              .Where(i => i.IDCLIENTE == cliente.IDCLIENTE)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(cliente);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterClienteUpdated(cliente);

            return cliente;
        }

        partial void OnClienteDeleted(Aldebaran.Web.Models.AldebaranContext.Cliente item);
        partial void OnAfterClienteDeleted(Aldebaran.Web.Models.AldebaranContext.Cliente item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Cliente> DeleteCliente(int idcliente)
        {
            var itemToDelete = Context.Clientes
                              .Where(i => i.IDCLIENTE == idcliente)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnClienteDeleted(itemToDelete);


            Context.Clientes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterClienteDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportConsecutivosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/consecutivos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/consecutivos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportConsecutivosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/consecutivos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/consecutivos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnConsecutivosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Consecutivo> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Consecutivo>> GetConsecutivos(Query query = null)
        {
            var items = Context.Consecutivos.AsQueryable();


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

            OnConsecutivosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnConsecutivoGet(Aldebaran.Web.Models.AldebaranContext.Consecutivo item);
        partial void OnGetConsecutivoByNomtabla(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Consecutivo> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Consecutivo> GetConsecutivoByNomtabla(string nomtabla)
        {
            var items = Context.Consecutivos
                              .AsNoTracking()
                              .Where(i => i.NOMTABLA == nomtabla);


            OnGetConsecutivoByNomtabla(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnConsecutivoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnConsecutivoCreated(Aldebaran.Web.Models.AldebaranContext.Consecutivo item);
        partial void OnAfterConsecutivoCreated(Aldebaran.Web.Models.AldebaranContext.Consecutivo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Consecutivo> CreateConsecutivo(Aldebaran.Web.Models.AldebaranContext.Consecutivo consecutivo)
        {
            OnConsecutivoCreated(consecutivo);

            var existingItem = Context.Consecutivos
                              .Where(i => i.NOMTABLA == consecutivo.NOMTABLA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Consecutivos.Add(consecutivo);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(consecutivo).State = EntityState.Detached;
                throw;
            }

            OnAfterConsecutivoCreated(consecutivo);

            return consecutivo;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Consecutivo> CancelConsecutivoChanges(Aldebaran.Web.Models.AldebaranContext.Consecutivo item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnConsecutivoUpdated(Aldebaran.Web.Models.AldebaranContext.Consecutivo item);
        partial void OnAfterConsecutivoUpdated(Aldebaran.Web.Models.AldebaranContext.Consecutivo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Consecutivo> UpdateConsecutivo(string nomtabla, Aldebaran.Web.Models.AldebaranContext.Consecutivo consecutivo)
        {
            OnConsecutivoUpdated(consecutivo);

            var itemToUpdate = Context.Consecutivos
                              .Where(i => i.NOMTABLA == consecutivo.NOMTABLA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(consecutivo);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterConsecutivoUpdated(consecutivo);

            return consecutivo;
        }

        partial void OnConsecutivoDeleted(Aldebaran.Web.Models.AldebaranContext.Consecutivo item);
        partial void OnAfterConsecutivoDeleted(Aldebaran.Web.Models.AldebaranContext.Consecutivo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Consecutivo> DeleteConsecutivo(string nomtabla)
        {
            var itemToDelete = Context.Consecutivos
                              .Where(i => i.NOMTABLA == nomtabla)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnConsecutivoDeleted(itemToDelete);


            Context.Consecutivos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterConsecutivoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportContactosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/contactos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/contactos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportContactosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/contactos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/contactos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnContactosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Contacto> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Contacto>> GetContactos(Query query = null)
        {
            var items = Context.Contactos.AsQueryable();


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

            OnContactosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnContactoGet(Aldebaran.Web.Models.AldebaranContext.Contacto item);
        partial void OnGetContactoByIdcontacto(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Contacto> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Contacto> GetContactoByIdcontacto(int idcontacto)
        {
            var items = Context.Contactos
                              .AsNoTracking()
                              .Where(i => i.IDCONTACTO == idcontacto);


            OnGetContactoByIdcontacto(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnContactoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnContactoCreated(Aldebaran.Web.Models.AldebaranContext.Contacto item);
        partial void OnAfterContactoCreated(Aldebaran.Web.Models.AldebaranContext.Contacto item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Contacto> CreateContacto(Aldebaran.Web.Models.AldebaranContext.Contacto contacto)
        {
            OnContactoCreated(contacto);

            var existingItem = Context.Contactos
                              .Where(i => i.IDCONTACTO == contacto.IDCONTACTO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Contactos.Add(contacto);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(contacto).State = EntityState.Detached;
                throw;
            }

            OnAfterContactoCreated(contacto);

            return contacto;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Contacto> CancelContactoChanges(Aldebaran.Web.Models.AldebaranContext.Contacto item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnContactoUpdated(Aldebaran.Web.Models.AldebaranContext.Contacto item);
        partial void OnAfterContactoUpdated(Aldebaran.Web.Models.AldebaranContext.Contacto item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Contacto> UpdateContacto(int idcontacto, Aldebaran.Web.Models.AldebaranContext.Contacto contacto)
        {
            OnContactoUpdated(contacto);

            var itemToUpdate = Context.Contactos
                              .Where(i => i.IDCONTACTO == contacto.IDCONTACTO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(contacto);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterContactoUpdated(contacto);

            return contacto;
        }

        partial void OnContactoDeleted(Aldebaran.Web.Models.AldebaranContext.Contacto item);
        partial void OnAfterContactoDeleted(Aldebaran.Web.Models.AldebaranContext.Contacto item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Contacto> DeleteContacto(int idcontacto)
        {
            var itemToDelete = Context.Contactos
                              .Where(i => i.IDCONTACTO == idcontacto)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnContactoDeleted(itemToDelete);


            Context.Contactos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterContactoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportControlconcurrenciaToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/controlconcurrencia/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/controlconcurrencia/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportControlconcurrenciaToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/controlconcurrencia/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/controlconcurrencia/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnControlconcurrenciaRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium>> GetControlconcurrencia(Query query = null)
        {
            var items = Context.Controlconcurrencia.AsQueryable();


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

            OnControlconcurrenciaRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnControlconcurrenciumGet(Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium item);
        partial void OnGetControlconcurrenciumByTipoAndIdtabla(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium> GetControlconcurrenciumByTipoAndIdtabla(string tipo, int idtabla)
        {
            var items = Context.Controlconcurrencia
                              .AsNoTracking()
                              .Where(i => i.TIPO == tipo && i.IDTABLA == idtabla);


            OnGetControlconcurrenciumByTipoAndIdtabla(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnControlconcurrenciumGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnControlconcurrenciumCreated(Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium item);
        partial void OnAfterControlconcurrenciumCreated(Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium> CreateControlconcurrencium(Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium controlconcurrencium)
        {
            OnControlconcurrenciumCreated(controlconcurrencium);

            var existingItem = Context.Controlconcurrencia
                              .Where(i => i.TIPO == controlconcurrencium.TIPO && i.IDTABLA == controlconcurrencium.IDTABLA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Controlconcurrencia.Add(controlconcurrencium);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(controlconcurrencium).State = EntityState.Detached;
                throw;
            }

            OnAfterControlconcurrenciumCreated(controlconcurrencium);

            return controlconcurrencium;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium> CancelControlconcurrenciumChanges(Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnControlconcurrenciumUpdated(Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium item);
        partial void OnAfterControlconcurrenciumUpdated(Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium> UpdateControlconcurrencium(string tipo, int idtabla, Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium controlconcurrencium)
        {
            OnControlconcurrenciumUpdated(controlconcurrencium);

            var itemToUpdate = Context.Controlconcurrencia
                              .Where(i => i.TIPO == controlconcurrencium.TIPO && i.IDTABLA == controlconcurrencium.IDTABLA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(controlconcurrencium);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterControlconcurrenciumUpdated(controlconcurrencium);

            return controlconcurrencium;
        }

        partial void OnControlconcurrenciumDeleted(Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium item);
        partial void OnAfterControlconcurrenciumDeleted(Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium> DeleteControlconcurrencium(string tipo, int idtabla)
        {
            var itemToDelete = Context.Controlconcurrencia
                              .Where(i => i.TIPO == tipo && i.IDTABLA == idtabla)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnControlconcurrenciumDeleted(itemToDelete);


            Context.Controlconcurrencia.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterControlconcurrenciumDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportDepartamentosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/departamentos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/departamentos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportDepartamentosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/departamentos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/departamentos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnDepartamentosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Departamento> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Departamento>> GetDepartamentos(Query query = null)
        {
            var items = Context.Departamentos.AsQueryable();


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

            OnDepartamentosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnDepartamentoGet(Aldebaran.Web.Models.AldebaranContext.Departamento item);
        partial void OnGetDepartamentoByIddepto(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Departamento> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Departamento> GetDepartamentoByIddepto(int iddepto)
        {
            var items = Context.Departamentos
                              .AsNoTracking()
                              .Where(i => i.IDDEPTO == iddepto);


            OnGetDepartamentoByIddepto(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnDepartamentoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnDepartamentoCreated(Aldebaran.Web.Models.AldebaranContext.Departamento item);
        partial void OnAfterDepartamentoCreated(Aldebaran.Web.Models.AldebaranContext.Departamento item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Departamento> CreateDepartamento(Aldebaran.Web.Models.AldebaranContext.Departamento departamento)
        {
            OnDepartamentoCreated(departamento);

            var existingItem = Context.Departamentos
                              .Where(i => i.IDDEPTO == departamento.IDDEPTO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Departamentos.Add(departamento);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(departamento).State = EntityState.Detached;
                throw;
            }

            OnAfterDepartamentoCreated(departamento);

            return departamento;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Departamento> CancelDepartamentoChanges(Aldebaran.Web.Models.AldebaranContext.Departamento item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnDepartamentoUpdated(Aldebaran.Web.Models.AldebaranContext.Departamento item);
        partial void OnAfterDepartamentoUpdated(Aldebaran.Web.Models.AldebaranContext.Departamento item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Departamento> UpdateDepartamento(int iddepto, Aldebaran.Web.Models.AldebaranContext.Departamento departamento)
        {
            OnDepartamentoUpdated(departamento);

            var itemToUpdate = Context.Departamentos
                              .Where(i => i.IDDEPTO == departamento.IDDEPTO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(departamento);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterDepartamentoUpdated(departamento);

            return departamento;
        }

        partial void OnDepartamentoDeleted(Aldebaran.Web.Models.AldebaranContext.Departamento item);
        partial void OnAfterDepartamentoDeleted(Aldebaran.Web.Models.AldebaranContext.Departamento item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Departamento> DeleteDepartamento(int iddepto)
        {
            var itemToDelete = Context.Departamentos
                              .Where(i => i.IDDEPTO == iddepto)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnDepartamentoDeleted(itemToDelete);


            Context.Departamentos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterDepartamentoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportDetcantprocesosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/detcantprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/detcantprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportDetcantprocesosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/detcantprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/detcantprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnDetcantprocesosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Detcantproceso> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Detcantproceso>> GetDetcantprocesos(Query query = null)
        {
            var items = Context.Detcantprocesos.AsQueryable();

            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Linea);
            items = items.Include(i => i.Cantproceso);

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

            OnDetcantprocesosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnDetcantprocesoGet(Aldebaran.Web.Models.AldebaranContext.Detcantproceso item);
        partial void OnGetDetcantprocesoByIddetcantproceso(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Detcantproceso> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Detcantproceso> GetDetcantprocesoByIddetcantproceso(int iddetcantproceso)
        {
            var items = Context.Detcantprocesos
                              .AsNoTracking()
                              .Where(i => i.IDDETCANTPROCESO == iddetcantproceso);

            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Linea);
            items = items.Include(i => i.Cantproceso);

            OnGetDetcantprocesoByIddetcantproceso(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnDetcantprocesoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnDetcantprocesoCreated(Aldebaran.Web.Models.AldebaranContext.Detcantproceso item);
        partial void OnAfterDetcantprocesoCreated(Aldebaran.Web.Models.AldebaranContext.Detcantproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Detcantproceso> CreateDetcantproceso(Aldebaran.Web.Models.AldebaranContext.Detcantproceso detcantproceso)
        {
            OnDetcantprocesoCreated(detcantproceso);

            var existingItem = Context.Detcantprocesos
                              .Where(i => i.IDDETCANTPROCESO == detcantproceso.IDDETCANTPROCESO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Detcantprocesos.Add(detcantproceso);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(detcantproceso).State = EntityState.Detached;
                throw;
            }

            OnAfterDetcantprocesoCreated(detcantproceso);

            return detcantproceso;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Detcantproceso> CancelDetcantprocesoChanges(Aldebaran.Web.Models.AldebaranContext.Detcantproceso item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnDetcantprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.Detcantproceso item);
        partial void OnAfterDetcantprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.Detcantproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Detcantproceso> UpdateDetcantproceso(int iddetcantproceso, Aldebaran.Web.Models.AldebaranContext.Detcantproceso detcantproceso)
        {
            OnDetcantprocesoUpdated(detcantproceso);

            var itemToUpdate = Context.Detcantprocesos
                              .Where(i => i.IDDETCANTPROCESO == detcantproceso.IDDETCANTPROCESO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(detcantproceso);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterDetcantprocesoUpdated(detcantproceso);

            return detcantproceso;
        }

        partial void OnDetcantprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.Detcantproceso item);
        partial void OnAfterDetcantprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.Detcantproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Detcantproceso> DeleteDetcantproceso(int iddetcantproceso)
        {
            var itemToDelete = Context.Detcantprocesos
                              .Where(i => i.IDDETCANTPROCESO == iddetcantproceso)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnDetcantprocesoDeleted(itemToDelete);


            Context.Detcantprocesos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterDetcantprocesoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportDetdevolpedidosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/detdevolpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/detdevolpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportDetdevolpedidosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/detdevolpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/detdevolpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnDetdevolpedidosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Detdevolpedido> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Detdevolpedido>> GetDetdevolpedidos(Query query = null)
        {
            var items = Context.Detdevolpedidos.AsQueryable();

            items = items.Include(i => i.Devolpedido);
            items = items.Include(i => i.Motivodevolucion);
            items = items.Include(i => i.Itemsxbodega);

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

            OnDetdevolpedidosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnDetdevolpedidoGet(Aldebaran.Web.Models.AldebaranContext.Detdevolpedido item);
        partial void OnGetDetdevolpedidoByIddetdevolpedido(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Detdevolpedido> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Detdevolpedido> GetDetdevolpedidoByIddetdevolpedido(int iddetdevolpedido)
        {
            var items = Context.Detdevolpedidos
                              .AsNoTracking()
                              .Where(i => i.IDDETDEVOLPEDIDO == iddetdevolpedido);

            items = items.Include(i => i.Devolpedido);
            items = items.Include(i => i.Motivodevolucion);
            items = items.Include(i => i.Itemsxbodega);

            OnGetDetdevolpedidoByIddetdevolpedido(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnDetdevolpedidoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnDetdevolpedidoCreated(Aldebaran.Web.Models.AldebaranContext.Detdevolpedido item);
        partial void OnAfterDetdevolpedidoCreated(Aldebaran.Web.Models.AldebaranContext.Detdevolpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Detdevolpedido> CreateDetdevolpedido(Aldebaran.Web.Models.AldebaranContext.Detdevolpedido detdevolpedido)
        {
            OnDetdevolpedidoCreated(detdevolpedido);

            var existingItem = Context.Detdevolpedidos
                              .Where(i => i.IDDETDEVOLPEDIDO == detdevolpedido.IDDETDEVOLPEDIDO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Detdevolpedidos.Add(detdevolpedido);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(detdevolpedido).State = EntityState.Detached;
                throw;
            }

            OnAfterDetdevolpedidoCreated(detdevolpedido);

            return detdevolpedido;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Detdevolpedido> CancelDetdevolpedidoChanges(Aldebaran.Web.Models.AldebaranContext.Detdevolpedido item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnDetdevolpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.Detdevolpedido item);
        partial void OnAfterDetdevolpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.Detdevolpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Detdevolpedido> UpdateDetdevolpedido(int iddetdevolpedido, Aldebaran.Web.Models.AldebaranContext.Detdevolpedido detdevolpedido)
        {
            OnDetdevolpedidoUpdated(detdevolpedido);

            var itemToUpdate = Context.Detdevolpedidos
                              .Where(i => i.IDDETDEVOLPEDIDO == detdevolpedido.IDDETDEVOLPEDIDO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(detdevolpedido);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterDetdevolpedidoUpdated(detdevolpedido);

            return detdevolpedido;
        }

        partial void OnDetdevolpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.Detdevolpedido item);
        partial void OnAfterDetdevolpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.Detdevolpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Detdevolpedido> DeleteDetdevolpedido(int iddetdevolpedido)
        {
            var itemToDelete = Context.Detdevolpedidos
                              .Where(i => i.IDDETDEVOLPEDIDO == iddetdevolpedido)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnDetdevolpedidoDeleted(itemToDelete);


            Context.Detdevolpedidos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterDetdevolpedidoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportDetentregaspactsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/detentregaspacts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/detentregaspacts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportDetentregaspactsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/detentregaspacts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/detentregaspacts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnDetentregaspactsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Detentregaspact> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Detentregaspact>> GetDetentregaspacts(Query query = null)
        {
            var items = Context.Detentregaspacts.AsQueryable();

            items = items.Include(i => i.Entregaspact);
            items = items.Include(i => i.Itemsxcolor);

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

            OnDetentregaspactsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnDetentregaspactGet(Aldebaran.Web.Models.AldebaranContext.Detentregaspact item);
        partial void OnGetDetentregaspactByIddetentregapact(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Detentregaspact> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Detentregaspact> GetDetentregaspactByIddetentregapact(int iddetentregapact)
        {
            var items = Context.Detentregaspacts
                              .AsNoTracking()
                              .Where(i => i.IDDETENTREGAPACT == iddetentregapact);

            items = items.Include(i => i.Entregaspact);
            items = items.Include(i => i.Itemsxcolor);

            OnGetDetentregaspactByIddetentregapact(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnDetentregaspactGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnDetentregaspactCreated(Aldebaran.Web.Models.AldebaranContext.Detentregaspact item);
        partial void OnAfterDetentregaspactCreated(Aldebaran.Web.Models.AldebaranContext.Detentregaspact item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Detentregaspact> CreateDetentregaspact(Aldebaran.Web.Models.AldebaranContext.Detentregaspact detentregaspact)
        {
            OnDetentregaspactCreated(detentregaspact);

            var existingItem = Context.Detentregaspacts
                              .Where(i => i.IDDETENTREGAPACT == detentregaspact.IDDETENTREGAPACT)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Detentregaspacts.Add(detentregaspact);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(detentregaspact).State = EntityState.Detached;
                throw;
            }

            OnAfterDetentregaspactCreated(detentregaspact);

            return detentregaspact;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Detentregaspact> CancelDetentregaspactChanges(Aldebaran.Web.Models.AldebaranContext.Detentregaspact item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnDetentregaspactUpdated(Aldebaran.Web.Models.AldebaranContext.Detentregaspact item);
        partial void OnAfterDetentregaspactUpdated(Aldebaran.Web.Models.AldebaranContext.Detentregaspact item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Detentregaspact> UpdateDetentregaspact(int iddetentregapact, Aldebaran.Web.Models.AldebaranContext.Detentregaspact detentregaspact)
        {
            OnDetentregaspactUpdated(detentregaspact);

            var itemToUpdate = Context.Detentregaspacts
                              .Where(i => i.IDDETENTREGAPACT == detentregaspact.IDDETENTREGAPACT)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(detentregaspact);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterDetentregaspactUpdated(detentregaspact);

            return detentregaspact;
        }

        partial void OnDetentregaspactDeleted(Aldebaran.Web.Models.AldebaranContext.Detentregaspact item);
        partial void OnAfterDetentregaspactDeleted(Aldebaran.Web.Models.AldebaranContext.Detentregaspact item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Detentregaspact> DeleteDetentregaspact(int iddetentregapact)
        {
            var itemToDelete = Context.Detentregaspacts
                              .Where(i => i.IDDETENTREGAPACT == iddetentregapact)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnDetentregaspactDeleted(itemToDelete);


            Context.Detentregaspacts.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterDetentregaspactDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportDetenviosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/detenvios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/detenvios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportDetenviosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/detenvios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/detenvios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnDetenviosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Detenvio> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Detenvio>> GetDetenvios(Query query = null)
        {
            var items = Context.Detenvios.AsQueryable();

            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Envio);
            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Linea);

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

            OnDetenviosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnDetenvioGet(Aldebaran.Web.Models.AldebaranContext.Detenvio item);
        partial void OnGetDetenvioByIddetenvio(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Detenvio> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Detenvio> GetDetenvioByIddetenvio(int iddetenvio)
        {
            var items = Context.Detenvios
                              .AsNoTracking()
                              .Where(i => i.IDDETENVIO == iddetenvio);

            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Envio);
            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Linea);

            OnGetDetenvioByIddetenvio(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnDetenvioGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnDetenvioCreated(Aldebaran.Web.Models.AldebaranContext.Detenvio item);
        partial void OnAfterDetenvioCreated(Aldebaran.Web.Models.AldebaranContext.Detenvio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Detenvio> CreateDetenvio(Aldebaran.Web.Models.AldebaranContext.Detenvio detenvio)
        {
            OnDetenvioCreated(detenvio);

            var existingItem = Context.Detenvios
                              .Where(i => i.IDDETENVIO == detenvio.IDDETENVIO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Detenvios.Add(detenvio);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(detenvio).State = EntityState.Detached;
                throw;
            }

            OnAfterDetenvioCreated(detenvio);

            return detenvio;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Detenvio> CancelDetenvioChanges(Aldebaran.Web.Models.AldebaranContext.Detenvio item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnDetenvioUpdated(Aldebaran.Web.Models.AldebaranContext.Detenvio item);
        partial void OnAfterDetenvioUpdated(Aldebaran.Web.Models.AldebaranContext.Detenvio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Detenvio> UpdateDetenvio(int iddetenvio, Aldebaran.Web.Models.AldebaranContext.Detenvio detenvio)
        {
            OnDetenvioUpdated(detenvio);

            var itemToUpdate = Context.Detenvios
                              .Where(i => i.IDDETENVIO == detenvio.IDDETENVIO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(detenvio);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterDetenvioUpdated(detenvio);

            return detenvio;
        }

        partial void OnDetenvioDeleted(Aldebaran.Web.Models.AldebaranContext.Detenvio item);
        partial void OnAfterDetenvioDeleted(Aldebaran.Web.Models.AldebaranContext.Detenvio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Detenvio> DeleteDetenvio(int iddetenvio)
        {
            var itemToDelete = Context.Detenvios
                              .Where(i => i.IDDETENVIO == iddetenvio)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnDetenvioDeleted(itemToDelete);


            Context.Detenvios.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterDetenvioDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportDevolordensToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/devolordens/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/devolordens/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportDevolordensToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/devolordens/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/devolordens/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnDevolordensRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Devolorden> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Devolorden>> GetDevolordens(Query query = null)
        {
            var items = Context.Devolordens.AsQueryable();

            items = items.Include(i => i.Motivodevolucion);
            items = items.Include(i => i.Ordene);

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

            OnDevolordensRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnDevolordenGet(Aldebaran.Web.Models.AldebaranContext.Devolorden item);
        partial void OnGetDevolordenByIddevolorden(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Devolorden> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Devolorden> GetDevolordenByIddevolorden(int iddevolorden)
        {
            var items = Context.Devolordens
                              .AsNoTracking()
                              .Where(i => i.IDDEVOLORDEN == iddevolorden);

            items = items.Include(i => i.Motivodevolucion);
            items = items.Include(i => i.Ordene);

            OnGetDevolordenByIddevolorden(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnDevolordenGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnDevolordenCreated(Aldebaran.Web.Models.AldebaranContext.Devolorden item);
        partial void OnAfterDevolordenCreated(Aldebaran.Web.Models.AldebaranContext.Devolorden item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Devolorden> CreateDevolorden(Aldebaran.Web.Models.AldebaranContext.Devolorden devolorden)
        {
            OnDevolordenCreated(devolorden);

            var existingItem = Context.Devolordens
                              .Where(i => i.IDDEVOLORDEN == devolorden.IDDEVOLORDEN)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Devolordens.Add(devolorden);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(devolorden).State = EntityState.Detached;
                throw;
            }

            OnAfterDevolordenCreated(devolorden);

            return devolorden;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Devolorden> CancelDevolordenChanges(Aldebaran.Web.Models.AldebaranContext.Devolorden item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnDevolordenUpdated(Aldebaran.Web.Models.AldebaranContext.Devolorden item);
        partial void OnAfterDevolordenUpdated(Aldebaran.Web.Models.AldebaranContext.Devolorden item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Devolorden> UpdateDevolorden(int iddevolorden, Aldebaran.Web.Models.AldebaranContext.Devolorden devolorden)
        {
            OnDevolordenUpdated(devolorden);

            var itemToUpdate = Context.Devolordens
                              .Where(i => i.IDDEVOLORDEN == devolorden.IDDEVOLORDEN)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(devolorden);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterDevolordenUpdated(devolorden);

            return devolorden;
        }

        partial void OnDevolordenDeleted(Aldebaran.Web.Models.AldebaranContext.Devolorden item);
        partial void OnAfterDevolordenDeleted(Aldebaran.Web.Models.AldebaranContext.Devolorden item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Devolorden> DeleteDevolorden(int iddevolorden)
        {
            var itemToDelete = Context.Devolordens
                              .Where(i => i.IDDEVOLORDEN == iddevolorden)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnDevolordenDeleted(itemToDelete);


            Context.Devolordens.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterDevolordenDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportDevolpedidosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/devolpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/devolpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportDevolpedidosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/devolpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/devolpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnDevolpedidosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Devolpedido> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Devolpedido>> GetDevolpedidos(Query query = null)
        {
            var items = Context.Devolpedidos.AsQueryable();

            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Motivodevolucion);
            items = items.Include(i => i.Pedido);

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

            OnDevolpedidosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnDevolpedidoGet(Aldebaran.Web.Models.AldebaranContext.Devolpedido item);
        partial void OnGetDevolpedidoByIddevolpedido(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Devolpedido> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Devolpedido> GetDevolpedidoByIddevolpedido(int iddevolpedido)
        {
            var items = Context.Devolpedidos
                              .AsNoTracking()
                              .Where(i => i.IDDEVOLPEDIDO == iddevolpedido);

            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Motivodevolucion);
            items = items.Include(i => i.Pedido);

            OnGetDevolpedidoByIddevolpedido(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnDevolpedidoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnDevolpedidoCreated(Aldebaran.Web.Models.AldebaranContext.Devolpedido item);
        partial void OnAfterDevolpedidoCreated(Aldebaran.Web.Models.AldebaranContext.Devolpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Devolpedido> CreateDevolpedido(Aldebaran.Web.Models.AldebaranContext.Devolpedido devolpedido)
        {
            OnDevolpedidoCreated(devolpedido);

            var existingItem = Context.Devolpedidos
                              .Where(i => i.IDDEVOLPEDIDO == devolpedido.IDDEVOLPEDIDO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Devolpedidos.Add(devolpedido);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(devolpedido).State = EntityState.Detached;
                throw;
            }

            OnAfterDevolpedidoCreated(devolpedido);

            return devolpedido;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Devolpedido> CancelDevolpedidoChanges(Aldebaran.Web.Models.AldebaranContext.Devolpedido item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnDevolpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.Devolpedido item);
        partial void OnAfterDevolpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.Devolpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Devolpedido> UpdateDevolpedido(int iddevolpedido, Aldebaran.Web.Models.AldebaranContext.Devolpedido devolpedido)
        {
            OnDevolpedidoUpdated(devolpedido);

            var itemToUpdate = Context.Devolpedidos
                              .Where(i => i.IDDEVOLPEDIDO == devolpedido.IDDEVOLPEDIDO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(devolpedido);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterDevolpedidoUpdated(devolpedido);

            return devolpedido;
        }

        partial void OnDevolpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.Devolpedido item);
        partial void OnAfterDevolpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.Devolpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Devolpedido> DeleteDevolpedido(int iddevolpedido)
        {
            var itemToDelete = Context.Devolpedidos
                              .Where(i => i.IDDEVOLPEDIDO == iddevolpedido)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnDevolpedidoDeleted(itemToDelete);


            Context.Devolpedidos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterDevolpedidoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportEmbalajesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/embalajes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/embalajes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportEmbalajesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/embalajes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/embalajes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnEmbalajesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Embalaje> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Embalaje>> GetEmbalajes(Query query = null)
        {
            var items = Context.Embalajes.AsQueryable();

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

            OnEmbalajesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnEmbalajeGet(Aldebaran.Web.Models.AldebaranContext.Embalaje item);
        partial void OnGetEmbalajeByIdembalaje(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Embalaje> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Embalaje> GetEmbalajeByIdembalaje(int idembalaje)
        {
            var items = Context.Embalajes
                              .AsNoTracking()
                              .Where(i => i.IDEMBALAJE == idembalaje);

            items = items.Include(i => i.Item);

            OnGetEmbalajeByIdembalaje(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnEmbalajeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnEmbalajeCreated(Aldebaran.Web.Models.AldebaranContext.Embalaje item);
        partial void OnAfterEmbalajeCreated(Aldebaran.Web.Models.AldebaranContext.Embalaje item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Embalaje> CreateEmbalaje(Aldebaran.Web.Models.AldebaranContext.Embalaje embalaje)
        {
            OnEmbalajeCreated(embalaje);

            var existingItem = Context.Embalajes
                              .Where(i => i.IDEMBALAJE == embalaje.IDEMBALAJE)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Embalajes.Add(embalaje);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(embalaje).State = EntityState.Detached;
                throw;
            }

            OnAfterEmbalajeCreated(embalaje);

            return embalaje;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Embalaje> CancelEmbalajeChanges(Aldebaran.Web.Models.AldebaranContext.Embalaje item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnEmbalajeUpdated(Aldebaran.Web.Models.AldebaranContext.Embalaje item);
        partial void OnAfterEmbalajeUpdated(Aldebaran.Web.Models.AldebaranContext.Embalaje item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Embalaje> UpdateEmbalaje(int idembalaje, Aldebaran.Web.Models.AldebaranContext.Embalaje embalaje)
        {
            OnEmbalajeUpdated(embalaje);

            var itemToUpdate = Context.Embalajes
                              .Where(i => i.IDEMBALAJE == embalaje.IDEMBALAJE)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(embalaje);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterEmbalajeUpdated(embalaje);

            return embalaje;
        }

        partial void OnEmbalajeDeleted(Aldebaran.Web.Models.AldebaranContext.Embalaje item);
        partial void OnAfterEmbalajeDeleted(Aldebaran.Web.Models.AldebaranContext.Embalaje item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Embalaje> DeleteEmbalaje(int idembalaje)
        {
            var itemToDelete = Context.Embalajes
                              .Where(i => i.IDEMBALAJE == idembalaje)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnEmbalajeDeleted(itemToDelete);


            Context.Embalajes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterEmbalajeDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportEmbarqueagentesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/embarqueagentes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/embarqueagentes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportEmbarqueagentesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/embarqueagentes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/embarqueagentes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnEmbarqueagentesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Embarqueagente> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Embarqueagente>> GetEmbarqueagentes(Query query = null)
        {
            var items = Context.Embarqueagentes.AsQueryable();

            items = items.Include(i => i.Agentesforwarder);
            items = items.Include(i => i.Metodoembarque);

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

            OnEmbarqueagentesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnEmbarqueagenteGet(Aldebaran.Web.Models.AldebaranContext.Embarqueagente item);
        partial void OnGetEmbarqueagenteByIdmetagente(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Embarqueagente> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Embarqueagente> GetEmbarqueagenteByIdmetagente(int idmetagente)
        {
            var items = Context.Embarqueagentes
                              .AsNoTracking()
                              .Where(i => i.IDMETAGENTE == idmetagente);

            items = items.Include(i => i.Agentesforwarder);
            items = items.Include(i => i.Metodoembarque);

            OnGetEmbarqueagenteByIdmetagente(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnEmbarqueagenteGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnEmbarqueagenteCreated(Aldebaran.Web.Models.AldebaranContext.Embarqueagente item);
        partial void OnAfterEmbarqueagenteCreated(Aldebaran.Web.Models.AldebaranContext.Embarqueagente item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Embarqueagente> CreateEmbarqueagente(Aldebaran.Web.Models.AldebaranContext.Embarqueagente embarqueagente)
        {
            OnEmbarqueagenteCreated(embarqueagente);

            var existingItem = Context.Embarqueagentes
                              .Where(i => i.IDMETAGENTE == embarqueagente.IDMETAGENTE)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Embarqueagentes.Add(embarqueagente);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(embarqueagente).State = EntityState.Detached;
                throw;
            }

            OnAfterEmbarqueagenteCreated(embarqueagente);

            return embarqueagente;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Embarqueagente> CancelEmbarqueagenteChanges(Aldebaran.Web.Models.AldebaranContext.Embarqueagente item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnEmbarqueagenteUpdated(Aldebaran.Web.Models.AldebaranContext.Embarqueagente item);
        partial void OnAfterEmbarqueagenteUpdated(Aldebaran.Web.Models.AldebaranContext.Embarqueagente item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Embarqueagente> UpdateEmbarqueagente(int idmetagente, Aldebaran.Web.Models.AldebaranContext.Embarqueagente embarqueagente)
        {
            OnEmbarqueagenteUpdated(embarqueagente);

            var itemToUpdate = Context.Embarqueagentes
                              .Where(i => i.IDMETAGENTE == embarqueagente.IDMETAGENTE)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(embarqueagente);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterEmbarqueagenteUpdated(embarqueagente);

            return embarqueagente;
        }

        partial void OnEmbarqueagenteDeleted(Aldebaran.Web.Models.AldebaranContext.Embarqueagente item);
        partial void OnAfterEmbarqueagenteDeleted(Aldebaran.Web.Models.AldebaranContext.Embarqueagente item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Embarqueagente> DeleteEmbarqueagente(int idmetagente)
        {
            var itemToDelete = Context.Embarqueagentes
                              .Where(i => i.IDMETAGENTE == idmetagente)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnEmbarqueagenteDeleted(itemToDelete);


            Context.Embarqueagentes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterEmbarqueagenteDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportEmpresasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/empresas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/empresas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportEmpresasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/empresas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/empresas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnEmpresasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Empresa> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Empresa>> GetEmpresas(Query query = null)
        {
            var items = Context.Empresas.AsQueryable();


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

            OnEmpresasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnEmpresaGet(Aldebaran.Web.Models.AldebaranContext.Empresa item);
        partial void OnGetEmpresaByIdempresa(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Empresa> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Empresa> GetEmpresaByIdempresa(int idempresa)
        {
            var items = Context.Empresas
                              .AsNoTracking()
                              .Where(i => i.IDEMPRESA == idempresa);


            OnGetEmpresaByIdempresa(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnEmpresaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnEmpresaCreated(Aldebaran.Web.Models.AldebaranContext.Empresa item);
        partial void OnAfterEmpresaCreated(Aldebaran.Web.Models.AldebaranContext.Empresa item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Empresa> CreateEmpresa(Aldebaran.Web.Models.AldebaranContext.Empresa empresa)
        {
            OnEmpresaCreated(empresa);

            var existingItem = Context.Empresas
                              .Where(i => i.IDEMPRESA == empresa.IDEMPRESA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Empresas.Add(empresa);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(empresa).State = EntityState.Detached;
                throw;
            }

            OnAfterEmpresaCreated(empresa);

            return empresa;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Empresa> CancelEmpresaChanges(Aldebaran.Web.Models.AldebaranContext.Empresa item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnEmpresaUpdated(Aldebaran.Web.Models.AldebaranContext.Empresa item);
        partial void OnAfterEmpresaUpdated(Aldebaran.Web.Models.AldebaranContext.Empresa item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Empresa> UpdateEmpresa(int idempresa, Aldebaran.Web.Models.AldebaranContext.Empresa empresa)
        {
            OnEmpresaUpdated(empresa);

            var itemToUpdate = Context.Empresas
                              .Where(i => i.IDEMPRESA == empresa.IDEMPRESA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(empresa);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterEmpresaUpdated(empresa);

            return empresa;
        }

        partial void OnEmpresaDeleted(Aldebaran.Web.Models.AldebaranContext.Empresa item);
        partial void OnAfterEmpresaDeleted(Aldebaran.Web.Models.AldebaranContext.Empresa item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Empresa> DeleteEmpresa(int idempresa)
        {
            var itemToDelete = Context.Empresas
                              .Where(i => i.IDEMPRESA == idempresa)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnEmpresaDeleted(itemToDelete);


            Context.Empresas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterEmpresaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportEntregaspactsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/entregaspacts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/entregaspacts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportEntregaspactsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/entregaspacts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/entregaspacts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnEntregaspactsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Entregaspact> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Entregaspact>> GetEntregaspacts(Query query = null)
        {
            var items = Context.Entregaspacts.AsQueryable();

            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Pedido);

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

            OnEntregaspactsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnEntregaspactGet(Aldebaran.Web.Models.AldebaranContext.Entregaspact item);
        partial void OnGetEntregaspactByIdentregapact(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Entregaspact> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Entregaspact> GetEntregaspactByIdentregapact(int identregapact)
        {
            var items = Context.Entregaspacts
                              .AsNoTracking()
                              .Where(i => i.IDENTREGAPACT == identregapact);

            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Pedido);

            OnGetEntregaspactByIdentregapact(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnEntregaspactGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnEntregaspactCreated(Aldebaran.Web.Models.AldebaranContext.Entregaspact item);
        partial void OnAfterEntregaspactCreated(Aldebaran.Web.Models.AldebaranContext.Entregaspact item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Entregaspact> CreateEntregaspact(Aldebaran.Web.Models.AldebaranContext.Entregaspact entregaspact)
        {
            OnEntregaspactCreated(entregaspact);

            var existingItem = Context.Entregaspacts
                              .Where(i => i.IDENTREGAPACT == entregaspact.IDENTREGAPACT)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Entregaspacts.Add(entregaspact);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(entregaspact).State = EntityState.Detached;
                throw;
            }

            OnAfterEntregaspactCreated(entregaspact);

            return entregaspact;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Entregaspact> CancelEntregaspactChanges(Aldebaran.Web.Models.AldebaranContext.Entregaspact item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnEntregaspactUpdated(Aldebaran.Web.Models.AldebaranContext.Entregaspact item);
        partial void OnAfterEntregaspactUpdated(Aldebaran.Web.Models.AldebaranContext.Entregaspact item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Entregaspact> UpdateEntregaspact(int identregapact, Aldebaran.Web.Models.AldebaranContext.Entregaspact entregaspact)
        {
            OnEntregaspactUpdated(entregaspact);

            var itemToUpdate = Context.Entregaspacts
                              .Where(i => i.IDENTREGAPACT == entregaspact.IDENTREGAPACT)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(entregaspact);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterEntregaspactUpdated(entregaspact);

            return entregaspact;
        }

        partial void OnEntregaspactDeleted(Aldebaran.Web.Models.AldebaranContext.Entregaspact item);
        partial void OnAfterEntregaspactDeleted(Aldebaran.Web.Models.AldebaranContext.Entregaspact item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Entregaspact> DeleteEntregaspact(int identregapact)
        {
            var itemToDelete = Context.Entregaspacts
                              .Where(i => i.IDENTREGAPACT == identregapact)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnEntregaspactDeleted(itemToDelete);


            Context.Entregaspacts.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterEntregaspactDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportEnviosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/envios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/envios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportEnviosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/envios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/envios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnEnviosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Envio> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Envio>> GetEnvios(Query query = null)
        {
            var items = Context.Envios.AsQueryable();

            items = items.Include(i => i.Metodosenvio);
            items = items.Include(i => i.Pedido);

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

            OnEnviosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnEnvioGet(Aldebaran.Web.Models.AldebaranContext.Envio item);
        partial void OnGetEnvioByIdenvio(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Envio> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Envio> GetEnvioByIdenvio(int idenvio)
        {
            var items = Context.Envios
                              .AsNoTracking()
                              .Where(i => i.IDENVIO == idenvio);

            items = items.Include(i => i.Metodosenvio);
            items = items.Include(i => i.Pedido);

            OnGetEnvioByIdenvio(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnEnvioGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnEnvioCreated(Aldebaran.Web.Models.AldebaranContext.Envio item);
        partial void OnAfterEnvioCreated(Aldebaran.Web.Models.AldebaranContext.Envio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Envio> CreateEnvio(Aldebaran.Web.Models.AldebaranContext.Envio envio)
        {
            OnEnvioCreated(envio);

            var existingItem = Context.Envios
                              .Where(i => i.IDENVIO == envio.IDENVIO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Envios.Add(envio);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(envio).State = EntityState.Detached;
                throw;
            }

            OnAfterEnvioCreated(envio);

            return envio;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Envio> CancelEnvioChanges(Aldebaran.Web.Models.AldebaranContext.Envio item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnEnvioUpdated(Aldebaran.Web.Models.AldebaranContext.Envio item);
        partial void OnAfterEnvioUpdated(Aldebaran.Web.Models.AldebaranContext.Envio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Envio> UpdateEnvio(int idenvio, Aldebaran.Web.Models.AldebaranContext.Envio envio)
        {
            OnEnvioUpdated(envio);

            var itemToUpdate = Context.Envios
                              .Where(i => i.IDENVIO == envio.IDENVIO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(envio);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterEnvioUpdated(envio);

            return envio;
        }

        partial void OnEnvioDeleted(Aldebaran.Web.Models.AldebaranContext.Envio item);
        partial void OnAfterEnvioDeleted(Aldebaran.Web.Models.AldebaranContext.Envio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Envio> DeleteEnvio(int idenvio)
        {
            var itemToDelete = Context.Envios
                              .Where(i => i.IDENVIO == idenvio)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnEnvioDeleted(itemToDelete);


            Context.Envios.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterEnvioDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportEnvioscorreosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/envioscorreos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/envioscorreos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportEnvioscorreosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/envioscorreos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/envioscorreos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnEnvioscorreosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Envioscorreo> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Envioscorreo>> GetEnvioscorreos(Query query = null)
        {
            var items = Context.Envioscorreos.AsQueryable();

            items = items.Include(i => i.Funcionario);

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

            OnEnvioscorreosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnEnvioscorreoGet(Aldebaran.Web.Models.AldebaranContext.Envioscorreo item);
        partial void OnGetEnvioscorreoByIdenvio(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Envioscorreo> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Envioscorreo> GetEnvioscorreoByIdenvio(int idenvio)
        {
            var items = Context.Envioscorreos
                              .AsNoTracking()
                              .Where(i => i.IDENVIO == idenvio);

            items = items.Include(i => i.Funcionario);

            OnGetEnvioscorreoByIdenvio(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnEnvioscorreoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnEnvioscorreoCreated(Aldebaran.Web.Models.AldebaranContext.Envioscorreo item);
        partial void OnAfterEnvioscorreoCreated(Aldebaran.Web.Models.AldebaranContext.Envioscorreo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Envioscorreo> CreateEnvioscorreo(Aldebaran.Web.Models.AldebaranContext.Envioscorreo envioscorreo)
        {
            OnEnvioscorreoCreated(envioscorreo);

            var existingItem = Context.Envioscorreos
                              .Where(i => i.IDENVIO == envioscorreo.IDENVIO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Envioscorreos.Add(envioscorreo);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(envioscorreo).State = EntityState.Detached;
                throw;
            }

            OnAfterEnvioscorreoCreated(envioscorreo);

            return envioscorreo;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Envioscorreo> CancelEnvioscorreoChanges(Aldebaran.Web.Models.AldebaranContext.Envioscorreo item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnEnvioscorreoUpdated(Aldebaran.Web.Models.AldebaranContext.Envioscorreo item);
        partial void OnAfterEnvioscorreoUpdated(Aldebaran.Web.Models.AldebaranContext.Envioscorreo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Envioscorreo> UpdateEnvioscorreo(int idenvio, Aldebaran.Web.Models.AldebaranContext.Envioscorreo envioscorreo)
        {
            OnEnvioscorreoUpdated(envioscorreo);

            var itemToUpdate = Context.Envioscorreos
                              .Where(i => i.IDENVIO == envioscorreo.IDENVIO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(envioscorreo);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterEnvioscorreoUpdated(envioscorreo);

            return envioscorreo;
        }

        partial void OnEnvioscorreoDeleted(Aldebaran.Web.Models.AldebaranContext.Envioscorreo item);
        partial void OnAfterEnvioscorreoDeleted(Aldebaran.Web.Models.AldebaranContext.Envioscorreo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Envioscorreo> DeleteEnvioscorreo(int idenvio)
        {
            var itemToDelete = Context.Envioscorreos
                              .Where(i => i.IDENVIO == idenvio)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnEnvioscorreoDeleted(itemToDelete);


            Context.Envioscorreos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterEnvioscorreoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportErpdocumenttypesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/erpdocumenttypes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/erpdocumenttypes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportErpdocumenttypesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/erpdocumenttypes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/erpdocumenttypes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnErpdocumenttypesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype>> GetErpdocumenttypes(Query query = null)
        {
            var items = Context.Erpdocumenttypes.AsQueryable();


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

            OnErpdocumenttypesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnErpdocumenttypeGet(Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype item);
        partial void OnGetErpdocumenttypeById(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype> GetErpdocumenttypeById(int id)
        {
            var items = Context.Erpdocumenttypes
                              .AsNoTracking()
                              .Where(i => i.ID == id);


            OnGetErpdocumenttypeById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnErpdocumenttypeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnErpdocumenttypeCreated(Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype item);
        partial void OnAfterErpdocumenttypeCreated(Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype> CreateErpdocumenttype(Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype erpdocumenttype)
        {
            OnErpdocumenttypeCreated(erpdocumenttype);

            var existingItem = Context.Erpdocumenttypes
                              .Where(i => i.ID == erpdocumenttype.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Erpdocumenttypes.Add(erpdocumenttype);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(erpdocumenttype).State = EntityState.Detached;
                throw;
            }

            OnAfterErpdocumenttypeCreated(erpdocumenttype);

            return erpdocumenttype;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype> CancelErpdocumenttypeChanges(Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnErpdocumenttypeUpdated(Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype item);
        partial void OnAfterErpdocumenttypeUpdated(Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype> UpdateErpdocumenttype(int id, Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype erpdocumenttype)
        {
            OnErpdocumenttypeUpdated(erpdocumenttype);

            var itemToUpdate = Context.Erpdocumenttypes
                              .Where(i => i.ID == erpdocumenttype.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(erpdocumenttype);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterErpdocumenttypeUpdated(erpdocumenttype);

            return erpdocumenttype;
        }

        partial void OnErpdocumenttypeDeleted(Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype item);
        partial void OnAfterErpdocumenttypeDeleted(Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype> DeleteErpdocumenttype(int id)
        {
            var itemToDelete = Context.Erpdocumenttypes
                              .Where(i => i.ID == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnErpdocumenttypeDeleted(itemToDelete);


            Context.Erpdocumenttypes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterErpdocumenttypeDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportErpshippingprocessesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/erpshippingprocesses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/erpshippingprocesses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportErpshippingprocessesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/erpshippingprocesses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/erpshippingprocesses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnErpshippingprocessesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess>> GetErpshippingprocesses(Query query = null)
        {
            var items = Context.Erpshippingprocesses.AsQueryable();


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

            OnErpshippingprocessesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnErpshippingprocessGet(Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess item);
        partial void OnGetErpshippingprocessById(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess> GetErpshippingprocessById(int id)
        {
            var items = Context.Erpshippingprocesses
                              .AsNoTracking()
                              .Where(i => i.ID == id);


            OnGetErpshippingprocessById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnErpshippingprocessGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnErpshippingprocessCreated(Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess item);
        partial void OnAfterErpshippingprocessCreated(Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess> CreateErpshippingprocess(Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess erpshippingprocess)
        {
            OnErpshippingprocessCreated(erpshippingprocess);

            var existingItem = Context.Erpshippingprocesses
                              .Where(i => i.ID == erpshippingprocess.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Erpshippingprocesses.Add(erpshippingprocess);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(erpshippingprocess).State = EntityState.Detached;
                throw;
            }

            OnAfterErpshippingprocessCreated(erpshippingprocess);

            return erpshippingprocess;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess> CancelErpshippingprocessChanges(Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnErpshippingprocessUpdated(Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess item);
        partial void OnAfterErpshippingprocessUpdated(Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess> UpdateErpshippingprocess(int id, Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess erpshippingprocess)
        {
            OnErpshippingprocessUpdated(erpshippingprocess);

            var itemToUpdate = Context.Erpshippingprocesses
                              .Where(i => i.ID == erpshippingprocess.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(erpshippingprocess);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterErpshippingprocessUpdated(erpshippingprocess);

            return erpshippingprocess;
        }

        partial void OnErpshippingprocessDeleted(Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess item);
        partial void OnAfterErpshippingprocessDeleted(Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess> DeleteErpshippingprocess(int id)
        {
            var itemToDelete = Context.Erpshippingprocesses
                              .Where(i => i.ID == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnErpshippingprocessDeleted(itemToDelete);


            Context.Erpshippingprocesses.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterErpshippingprocessDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportErpshippingprocessdetailsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/erpshippingprocessdetails/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/erpshippingprocessdetails/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportErpshippingprocessdetailsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/erpshippingprocessdetails/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/erpshippingprocessdetails/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnErpshippingprocessdetailsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail>> GetErpshippingprocessdetails(Query query = null)
        {
            var items = Context.Erpshippingprocessdetails.AsQueryable();

            items = items.Include(i => i.Erpdocumenttype);
            items = items.Include(i => i.Erpshippingprocess);

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

            OnErpshippingprocessdetailsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnErpshippingprocessdetailGet(Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail item);
        partial void OnGetErpshippingprocessdetailById(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail> GetErpshippingprocessdetailById(int id)
        {
            var items = Context.Erpshippingprocessdetails
                              .AsNoTracking()
                              .Where(i => i.ID == id);

            items = items.Include(i => i.Erpdocumenttype);
            items = items.Include(i => i.Erpshippingprocess);

            OnGetErpshippingprocessdetailById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnErpshippingprocessdetailGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnErpshippingprocessdetailCreated(Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail item);
        partial void OnAfterErpshippingprocessdetailCreated(Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail> CreateErpshippingprocessdetail(Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail erpshippingprocessdetail)
        {
            OnErpshippingprocessdetailCreated(erpshippingprocessdetail);

            var existingItem = Context.Erpshippingprocessdetails
                              .Where(i => i.ID == erpshippingprocessdetail.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Erpshippingprocessdetails.Add(erpshippingprocessdetail);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(erpshippingprocessdetail).State = EntityState.Detached;
                throw;
            }

            OnAfterErpshippingprocessdetailCreated(erpshippingprocessdetail);

            return erpshippingprocessdetail;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail> CancelErpshippingprocessdetailChanges(Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnErpshippingprocessdetailUpdated(Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail item);
        partial void OnAfterErpshippingprocessdetailUpdated(Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail> UpdateErpshippingprocessdetail(int id, Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail erpshippingprocessdetail)
        {
            OnErpshippingprocessdetailUpdated(erpshippingprocessdetail);

            var itemToUpdate = Context.Erpshippingprocessdetails
                              .Where(i => i.ID == erpshippingprocessdetail.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(erpshippingprocessdetail);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterErpshippingprocessdetailUpdated(erpshippingprocessdetail);

            return erpshippingprocessdetail;
        }

        partial void OnErpshippingprocessdetailDeleted(Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail item);
        partial void OnAfterErpshippingprocessdetailDeleted(Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail> DeleteErpshippingprocessdetail(int id)
        {
            var itemToDelete = Context.Erpshippingprocessdetails
                              .Where(i => i.ID == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnErpshippingprocessdetailDeleted(itemToDelete);


            Context.Erpshippingprocessdetails.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterErpshippingprocessdetailDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportErroresenvioscorreosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/erroresenvioscorreos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/erroresenvioscorreos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportErroresenvioscorreosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/erroresenvioscorreos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/erroresenvioscorreos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnErroresenvioscorreosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo>> GetErroresenvioscorreos(Query query = null)
        {
            var items = Context.Erroresenvioscorreos.AsQueryable();


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

            OnErroresenvioscorreosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnErroresenvioscorreoGet(Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo item);
        partial void OnGetErroresenvioscorreoByIderrorenvio(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo> GetErroresenvioscorreoByIderrorenvio(int iderrorenvio)
        {
            var items = Context.Erroresenvioscorreos
                              .AsNoTracking()
                              .Where(i => i.IDERRORENVIO == iderrorenvio);


            OnGetErroresenvioscorreoByIderrorenvio(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnErroresenvioscorreoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnErroresenvioscorreoCreated(Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo item);
        partial void OnAfterErroresenvioscorreoCreated(Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo> CreateErroresenvioscorreo(Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo erroresenvioscorreo)
        {
            OnErroresenvioscorreoCreated(erroresenvioscorreo);

            var existingItem = Context.Erroresenvioscorreos
                              .Where(i => i.IDERRORENVIO == erroresenvioscorreo.IDERRORENVIO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Erroresenvioscorreos.Add(erroresenvioscorreo);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(erroresenvioscorreo).State = EntityState.Detached;
                throw;
            }

            OnAfterErroresenvioscorreoCreated(erroresenvioscorreo);

            return erroresenvioscorreo;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo> CancelErroresenvioscorreoChanges(Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnErroresenvioscorreoUpdated(Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo item);
        partial void OnAfterErroresenvioscorreoUpdated(Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo> UpdateErroresenvioscorreo(int iderrorenvio, Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo erroresenvioscorreo)
        {
            OnErroresenvioscorreoUpdated(erroresenvioscorreo);

            var itemToUpdate = Context.Erroresenvioscorreos
                              .Where(i => i.IDERRORENVIO == erroresenvioscorreo.IDERRORENVIO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(erroresenvioscorreo);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterErroresenvioscorreoUpdated(erroresenvioscorreo);

            return erroresenvioscorreo;
        }

        partial void OnErroresenvioscorreoDeleted(Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo item);
        partial void OnAfterErroresenvioscorreoDeleted(Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo> DeleteErroresenvioscorreo(int iderrorenvio)
        {
            var itemToDelete = Context.Erroresenvioscorreos
                              .Where(i => i.IDERRORENVIO == iderrorenvio)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnErroresenvioscorreoDeleted(itemToDelete);


            Context.Erroresenvioscorreos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterErroresenvioscorreoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportEstadoinvinicialsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/estadoinvinicials/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/estadoinvinicials/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportEstadoinvinicialsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/estadoinvinicials/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/estadoinvinicials/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnEstadoinvinicialsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Estadoinvinicial> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Estadoinvinicial>> GetEstadoinvinicials(Query query = null)
        {
            var items = Context.Estadoinvinicials.AsQueryable();


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

            OnEstadoinvinicialsRead(ref items);

            return await Task.FromResult(items);
        }

        public async Task ExportFestivosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/festivos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/festivos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportFestivosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/festivos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/festivos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnFestivosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Festivo> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Festivo>> GetFestivos(Query query = null)
        {
            var items = Context.Festivos.AsQueryable();


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

            OnFestivosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnFestivoGet(Aldebaran.Web.Models.AldebaranContext.Festivo item);
        partial void OnGetFestivoByFecha(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Festivo> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Festivo> GetFestivoByFecha(DateTime fecha)
        {
            var items = Context.Festivos
                              .AsNoTracking()
                              .Where(i => i.FECHA == fecha);


            OnGetFestivoByFecha(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnFestivoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnFestivoCreated(Aldebaran.Web.Models.AldebaranContext.Festivo item);
        partial void OnAfterFestivoCreated(Aldebaran.Web.Models.AldebaranContext.Festivo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Festivo> CreateFestivo(Aldebaran.Web.Models.AldebaranContext.Festivo festivo)
        {
            OnFestivoCreated(festivo);

            var existingItem = Context.Festivos
                              .Where(i => i.FECHA == festivo.FECHA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Festivos.Add(festivo);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(festivo).State = EntityState.Detached;
                throw;
            }

            OnAfterFestivoCreated(festivo);

            return festivo;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Festivo> CancelFestivoChanges(Aldebaran.Web.Models.AldebaranContext.Festivo item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnFestivoUpdated(Aldebaran.Web.Models.AldebaranContext.Festivo item);
        partial void OnAfterFestivoUpdated(Aldebaran.Web.Models.AldebaranContext.Festivo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Festivo> UpdateFestivo(DateTime fecha, Aldebaran.Web.Models.AldebaranContext.Festivo festivo)
        {
            OnFestivoUpdated(festivo);

            var itemToUpdate = Context.Festivos
                              .Where(i => i.FECHA == festivo.FECHA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(festivo);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterFestivoUpdated(festivo);

            return festivo;
        }

        partial void OnFestivoDeleted(Aldebaran.Web.Models.AldebaranContext.Festivo item);
        partial void OnAfterFestivoDeleted(Aldebaran.Web.Models.AldebaranContext.Festivo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Festivo> DeleteFestivo(DateTime fecha)
        {
            var itemToDelete = Context.Festivos
                              .Where(i => i.FECHA == fecha)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnFestivoDeleted(itemToDelete);


            Context.Festivos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterFestivoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportForwardersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/forwarders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/forwarders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportForwardersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/forwarders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/forwarders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnForwardersRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Forwarder> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Forwarder>> GetForwarders(Query query = null)
        {
            var items = Context.Forwarders.AsQueryable();

            items = items.Include(i => i.Ciudade);

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

        partial void OnForwarderGet(Aldebaran.Web.Models.AldebaranContext.Forwarder item);
        partial void OnGetForwarderByIdforwarder(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Forwarder> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Forwarder> GetForwarderByIdforwarder(int idforwarder)
        {
            var items = Context.Forwarders
                              .AsNoTracking()
                              .Where(i => i.IDFORWARDER == idforwarder);

            items = items.Include(i => i.Ciudade);

            OnGetForwarderByIdforwarder(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnForwarderGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnForwarderCreated(Aldebaran.Web.Models.AldebaranContext.Forwarder item);
        partial void OnAfterForwarderCreated(Aldebaran.Web.Models.AldebaranContext.Forwarder item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Forwarder> CreateForwarder(Aldebaran.Web.Models.AldebaranContext.Forwarder forwarder)
        {
            OnForwarderCreated(forwarder);

            var existingItem = Context.Forwarders
                              .Where(i => i.IDFORWARDER == forwarder.IDFORWARDER)
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

        public async Task<Aldebaran.Web.Models.AldebaranContext.Forwarder> CancelForwarderChanges(Aldebaran.Web.Models.AldebaranContext.Forwarder item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnForwarderUpdated(Aldebaran.Web.Models.AldebaranContext.Forwarder item);
        partial void OnAfterForwarderUpdated(Aldebaran.Web.Models.AldebaranContext.Forwarder item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Forwarder> UpdateForwarder(int idforwarder, Aldebaran.Web.Models.AldebaranContext.Forwarder forwarder)
        {
            OnForwarderUpdated(forwarder);

            var itemToUpdate = Context.Forwarders
                              .Where(i => i.IDFORWARDER == forwarder.IDFORWARDER)
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

        partial void OnForwarderDeleted(Aldebaran.Web.Models.AldebaranContext.Forwarder item);
        partial void OnAfterForwarderDeleted(Aldebaran.Web.Models.AldebaranContext.Forwarder item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Forwarder> DeleteForwarder(int idforwarder)
        {
            var itemToDelete = Context.Forwarders
                              .Where(i => i.IDFORWARDER == idforwarder)
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

        public async Task ExportFuncionariosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/funcionarios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/funcionarios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportFuncionariosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/funcionarios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/funcionarios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnFuncionariosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Funcionario> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Funcionario>> GetFuncionarios(Query query = null)
        {
            var items = Context.Funcionarios.AsQueryable();

            items = items.Include(i => i.Area);
            items = items.Include(i => i.Tipidentifica);

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

            OnFuncionariosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnFuncionarioGet(Aldebaran.Web.Models.AldebaranContext.Funcionario item);
        partial void OnGetFuncionarioByIdfuncionario(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Funcionario> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Funcionario> GetFuncionarioByIdfuncionario(int idfuncionario)
        {
            var items = Context.Funcionarios
                              .AsNoTracking()
                              .Where(i => i.IDFUNCIONARIO == idfuncionario);

            items = items.Include(i => i.Area);
            items = items.Include(i => i.Tipidentifica);

            OnGetFuncionarioByIdfuncionario(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnFuncionarioGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnFuncionarioCreated(Aldebaran.Web.Models.AldebaranContext.Funcionario item);
        partial void OnAfterFuncionarioCreated(Aldebaran.Web.Models.AldebaranContext.Funcionario item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Funcionario> CreateFuncionario(Aldebaran.Web.Models.AldebaranContext.Funcionario funcionario)
        {
            OnFuncionarioCreated(funcionario);

            var existingItem = Context.Funcionarios
                              .Where(i => i.IDFUNCIONARIO == funcionario.IDFUNCIONARIO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Funcionarios.Add(funcionario);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(funcionario).State = EntityState.Detached;
                throw;
            }

            OnAfterFuncionarioCreated(funcionario);

            return funcionario;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Funcionario> CancelFuncionarioChanges(Aldebaran.Web.Models.AldebaranContext.Funcionario item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnFuncionarioUpdated(Aldebaran.Web.Models.AldebaranContext.Funcionario item);
        partial void OnAfterFuncionarioUpdated(Aldebaran.Web.Models.AldebaranContext.Funcionario item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Funcionario> UpdateFuncionario(int idfuncionario, Aldebaran.Web.Models.AldebaranContext.Funcionario funcionario)
        {
            OnFuncionarioUpdated(funcionario);

            var itemToUpdate = Context.Funcionarios
                              .Where(i => i.IDFUNCIONARIO == funcionario.IDFUNCIONARIO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(funcionario);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterFuncionarioUpdated(funcionario);

            return funcionario;
        }

        partial void OnFuncionarioDeleted(Aldebaran.Web.Models.AldebaranContext.Funcionario item);
        partial void OnAfterFuncionarioDeleted(Aldebaran.Web.Models.AldebaranContext.Funcionario item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Funcionario> DeleteFuncionario(int idfuncionario)
        {
            var itemToDelete = Context.Funcionarios
                              .Where(i => i.IDFUNCIONARIO == idfuncionario)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnFuncionarioDeleted(itemToDelete);


            Context.Funcionarios.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterFuncionarioDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportGrupopcsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/grupopcs/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/grupopcs/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportGrupopcsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/grupopcs/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/grupopcs/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGrupopcsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Grupopc> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Grupopc>> GetGrupopcs(Query query = null)
        {
            var items = Context.Grupopcs.AsQueryable();

            items = items.Include(i => i.Grupo);
            items = items.Include(i => i.Opcione);

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

            OnGrupopcsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnGrupopcGet(Aldebaran.Web.Models.AldebaranContext.Grupopc item);
        partial void OnGetGrupopcByIdgrupoAndIdopcion(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Grupopc> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Grupopc> GetGrupopcByIdgrupoAndIdopcion(int idgrupo, int idopcion)
        {
            var items = Context.Grupopcs
                              .AsNoTracking()
                              .Where(i => i.IDGRUPO == idgrupo && i.IDOPCION == idopcion);

            items = items.Include(i => i.Grupo);
            items = items.Include(i => i.Opcione);

            OnGetGrupopcByIdgrupoAndIdopcion(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnGrupopcGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnGrupopcCreated(Aldebaran.Web.Models.AldebaranContext.Grupopc item);
        partial void OnAfterGrupopcCreated(Aldebaran.Web.Models.AldebaranContext.Grupopc item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Grupopc> CreateGrupopc(Aldebaran.Web.Models.AldebaranContext.Grupopc grupopc)
        {
            OnGrupopcCreated(grupopc);

            var existingItem = Context.Grupopcs
                              .Where(i => i.IDGRUPO == grupopc.IDGRUPO && i.IDOPCION == grupopc.IDOPCION)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Grupopcs.Add(grupopc);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(grupopc).State = EntityState.Detached;
                throw;
            }

            OnAfterGrupopcCreated(grupopc);

            return grupopc;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Grupopc> CancelGrupopcChanges(Aldebaran.Web.Models.AldebaranContext.Grupopc item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnGrupopcUpdated(Aldebaran.Web.Models.AldebaranContext.Grupopc item);
        partial void OnAfterGrupopcUpdated(Aldebaran.Web.Models.AldebaranContext.Grupopc item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Grupopc> UpdateGrupopc(int idgrupo, int idopcion, Aldebaran.Web.Models.AldebaranContext.Grupopc grupopc)
        {
            OnGrupopcUpdated(grupopc);

            var itemToUpdate = Context.Grupopcs
                              .Where(i => i.IDGRUPO == grupopc.IDGRUPO && i.IDOPCION == grupopc.IDOPCION)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(grupopc);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterGrupopcUpdated(grupopc);

            return grupopc;
        }

        partial void OnGrupopcDeleted(Aldebaran.Web.Models.AldebaranContext.Grupopc item);
        partial void OnAfterGrupopcDeleted(Aldebaran.Web.Models.AldebaranContext.Grupopc item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Grupopc> DeleteGrupopc(int idgrupo, int idopcion)
        {
            var itemToDelete = Context.Grupopcs
                              .Where(i => i.IDGRUPO == idgrupo && i.IDOPCION == idopcion)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnGrupopcDeleted(itemToDelete);


            Context.Grupopcs.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterGrupopcDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportGruposToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/grupos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/grupos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportGruposToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/grupos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/grupos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGruposRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Grupo> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Grupo>> GetGrupos(Query query = null)
        {
            var items = Context.Grupos.AsQueryable();


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

            OnGruposRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnGrupoGet(Aldebaran.Web.Models.AldebaranContext.Grupo item);
        partial void OnGetGrupoByIdgrupo(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Grupo> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Grupo> GetGrupoByIdgrupo(int idgrupo)
        {
            var items = Context.Grupos
                              .AsNoTracking()
                              .Where(i => i.IDGRUPO == idgrupo);


            OnGetGrupoByIdgrupo(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnGrupoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnGrupoCreated(Aldebaran.Web.Models.AldebaranContext.Grupo item);
        partial void OnAfterGrupoCreated(Aldebaran.Web.Models.AldebaranContext.Grupo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Grupo> CreateGrupo(Aldebaran.Web.Models.AldebaranContext.Grupo grupo)
        {
            OnGrupoCreated(grupo);

            var existingItem = Context.Grupos
                              .Where(i => i.IDGRUPO == grupo.IDGRUPO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Grupos.Add(grupo);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(grupo).State = EntityState.Detached;
                throw;
            }

            OnAfterGrupoCreated(grupo);

            return grupo;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Grupo> CancelGrupoChanges(Aldebaran.Web.Models.AldebaranContext.Grupo item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnGrupoUpdated(Aldebaran.Web.Models.AldebaranContext.Grupo item);
        partial void OnAfterGrupoUpdated(Aldebaran.Web.Models.AldebaranContext.Grupo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Grupo> UpdateGrupo(int idgrupo, Aldebaran.Web.Models.AldebaranContext.Grupo grupo)
        {
            OnGrupoUpdated(grupo);

            var itemToUpdate = Context.Grupos
                              .Where(i => i.IDGRUPO == grupo.IDGRUPO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(grupo);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterGrupoUpdated(grupo);

            return grupo;
        }

        partial void OnGrupoDeleted(Aldebaran.Web.Models.AldebaranContext.Grupo item);
        partial void OnAfterGrupoDeleted(Aldebaran.Web.Models.AldebaranContext.Grupo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Grupo> DeleteGrupo(int idgrupo)
        {
            var itemToDelete = Context.Grupos
                              .Where(i => i.IDGRUPO == idgrupo)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnGrupoDeleted(itemToDelete);


            Context.Grupos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterGrupoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportGrupususToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/grupusus/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/grupusus/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportGrupususToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/grupusus/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/grupusus/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGrupususRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Grupusu> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Grupusu>> GetGrupusus(Query query = null)
        {
            var items = Context.Grupusus.AsQueryable();

            items = items.Include(i => i.Grupo);
            items = items.Include(i => i.Usuario);

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

            OnGrupususRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnGrupusuGet(Aldebaran.Web.Models.AldebaranContext.Grupusu item);
        partial void OnGetGrupusuByIdgrupoAndIdusuario(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Grupusu> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Grupusu> GetGrupusuByIdgrupoAndIdusuario(int idgrupo, int idusuario)
        {
            var items = Context.Grupusus
                              .AsNoTracking()
                              .Where(i => i.IDGRUPO == idgrupo && i.IDUSUARIO == idusuario);

            items = items.Include(i => i.Grupo);
            items = items.Include(i => i.Usuario);

            OnGetGrupusuByIdgrupoAndIdusuario(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnGrupusuGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnGrupusuCreated(Aldebaran.Web.Models.AldebaranContext.Grupusu item);
        partial void OnAfterGrupusuCreated(Aldebaran.Web.Models.AldebaranContext.Grupusu item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Grupusu> CreateGrupusu(Aldebaran.Web.Models.AldebaranContext.Grupusu grupusu)
        {
            OnGrupusuCreated(grupusu);

            var existingItem = Context.Grupusus
                              .Where(i => i.IDGRUPO == grupusu.IDGRUPO && i.IDUSUARIO == grupusu.IDUSUARIO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Grupusus.Add(grupusu);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(grupusu).State = EntityState.Detached;
                throw;
            }

            OnAfterGrupusuCreated(grupusu);

            return grupusu;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Grupusu> CancelGrupusuChanges(Aldebaran.Web.Models.AldebaranContext.Grupusu item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnGrupusuUpdated(Aldebaran.Web.Models.AldebaranContext.Grupusu item);
        partial void OnAfterGrupusuUpdated(Aldebaran.Web.Models.AldebaranContext.Grupusu item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Grupusu> UpdateGrupusu(int idgrupo, int idusuario, Aldebaran.Web.Models.AldebaranContext.Grupusu grupusu)
        {
            OnGrupusuUpdated(grupusu);

            var itemToUpdate = Context.Grupusus
                              .Where(i => i.IDGRUPO == grupusu.IDGRUPO && i.IDUSUARIO == grupusu.IDUSUARIO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(grupusu);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterGrupusuUpdated(grupusu);

            return grupusu;
        }

        partial void OnGrupusuDeleted(Aldebaran.Web.Models.AldebaranContext.Grupusu item);
        partial void OnAfterGrupusuDeleted(Aldebaran.Web.Models.AldebaranContext.Grupusu item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Grupusu> DeleteGrupusu(int idgrupo, int idusuario)
        {
            var itemToDelete = Context.Grupusus
                              .Where(i => i.IDGRUPO == idgrupo && i.IDUSUARIO == idusuario)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnGrupusuDeleted(itemToDelete);


            Context.Grupusus.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterGrupusuDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHisActpedidosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisactpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisactpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHisActpedidosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisactpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisactpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHisActpedidosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisActpedido> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.HisActpedido>> GetHisActpedidos(Query query = null)
        {
            var items = Context.HisActpedidos.AsQueryable();


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

            OnHisActpedidosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHisActpedidoGet(Aldebaran.Web.Models.AldebaranContext.HisActpedido item);
        partial void OnGetHisActpedidoByIdactpedido(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisActpedido> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.HisActpedido> GetHisActpedidoByIdactpedido(int idactpedido)
        {
            var items = Context.HisActpedidos
                              .AsNoTracking()
                              .Where(i => i.IDACTPEDIDO == idactpedido);


            OnGetHisActpedidoByIdactpedido(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHisActpedidoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHisActpedidoCreated(Aldebaran.Web.Models.AldebaranContext.HisActpedido item);
        partial void OnAfterHisActpedidoCreated(Aldebaran.Web.Models.AldebaranContext.HisActpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisActpedido> CreateHisActpedido(Aldebaran.Web.Models.AldebaranContext.HisActpedido hisactpedido)
        {
            OnHisActpedidoCreated(hisactpedido);

            var existingItem = Context.HisActpedidos
                              .Where(i => i.IDACTPEDIDO == hisactpedido.IDACTPEDIDO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.HisActpedidos.Add(hisactpedido);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(hisactpedido).State = EntityState.Detached;
                throw;
            }

            OnAfterHisActpedidoCreated(hisactpedido);

            return hisactpedido;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisActpedido> CancelHisActpedidoChanges(Aldebaran.Web.Models.AldebaranContext.HisActpedido item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHisActpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.HisActpedido item);
        partial void OnAfterHisActpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.HisActpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisActpedido> UpdateHisActpedido(int idactpedido, Aldebaran.Web.Models.AldebaranContext.HisActpedido hisactpedido)
        {
            OnHisActpedidoUpdated(hisactpedido);

            var itemToUpdate = Context.HisActpedidos
                              .Where(i => i.IDACTPEDIDO == hisactpedido.IDACTPEDIDO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(hisactpedido);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHisActpedidoUpdated(hisactpedido);

            return hisactpedido;
        }

        partial void OnHisActpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.HisActpedido item);
        partial void OnAfterHisActpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.HisActpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisActpedido> DeleteHisActpedido(int idactpedido)
        {
            var itemToDelete = Context.HisActpedidos
                              .Where(i => i.IDACTPEDIDO == idactpedido)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHisActpedidoDeleted(itemToDelete);


            Context.HisActpedidos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHisActpedidoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHisActxactpedidosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisactxactpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisactxactpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHisActxactpedidosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisactxactpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisactxactpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHisActxactpedidosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisActxactpedido> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.HisActxactpedido>> GetHisActxactpedidos(Query query = null)
        {
            var items = Context.HisActxactpedidos.AsQueryable();


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

            OnHisActxactpedidosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHisActxactpedidoGet(Aldebaran.Web.Models.AldebaranContext.HisActxactpedido item);
        partial void OnGetHisActxactpedidoByIdtipoactividadAndIdactpedido(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisActxactpedido> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.HisActxactpedido> GetHisActxactpedidoByIdtipoactividadAndIdactpedido(int idtipoactividad, int idactpedido)
        {
            var items = Context.HisActxactpedidos
                              .AsNoTracking()
                              .Where(i => i.IDTIPOACTIVIDAD == idtipoactividad && i.IDACTPEDIDO == idactpedido);


            OnGetHisActxactpedidoByIdtipoactividadAndIdactpedido(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHisActxactpedidoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHisActxactpedidoCreated(Aldebaran.Web.Models.AldebaranContext.HisActxactpedido item);
        partial void OnAfterHisActxactpedidoCreated(Aldebaran.Web.Models.AldebaranContext.HisActxactpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisActxactpedido> CreateHisActxactpedido(Aldebaran.Web.Models.AldebaranContext.HisActxactpedido hisactxactpedido)
        {
            OnHisActxactpedidoCreated(hisactxactpedido);

            var existingItem = Context.HisActxactpedidos
                              .Where(i => i.IDTIPOACTIVIDAD == hisactxactpedido.IDTIPOACTIVIDAD && i.IDACTPEDIDO == hisactxactpedido.IDACTPEDIDO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.HisActxactpedidos.Add(hisactxactpedido);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(hisactxactpedido).State = EntityState.Detached;
                throw;
            }

            OnAfterHisActxactpedidoCreated(hisactxactpedido);

            return hisactxactpedido;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisActxactpedido> CancelHisActxactpedidoChanges(Aldebaran.Web.Models.AldebaranContext.HisActxactpedido item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHisActxactpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.HisActxactpedido item);
        partial void OnAfterHisActxactpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.HisActxactpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisActxactpedido> UpdateHisActxactpedido(int idtipoactividad, int idactpedido, Aldebaran.Web.Models.AldebaranContext.HisActxactpedido hisactxactpedido)
        {
            OnHisActxactpedidoUpdated(hisactxactpedido);

            var itemToUpdate = Context.HisActxactpedidos
                              .Where(i => i.IDTIPOACTIVIDAD == hisactxactpedido.IDTIPOACTIVIDAD && i.IDACTPEDIDO == hisactxactpedido.IDACTPEDIDO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(hisactxactpedido);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHisActxactpedidoUpdated(hisactxactpedido);

            return hisactxactpedido;
        }

        partial void OnHisActxactpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.HisActxactpedido item);
        partial void OnAfterHisActxactpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.HisActxactpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisActxactpedido> DeleteHisActxactpedido(int idtipoactividad, int idactpedido)
        {
            var itemToDelete = Context.HisActxactpedidos
                              .Where(i => i.IDTIPOACTIVIDAD == idtipoactividad && i.IDACTPEDIDO == idactpedido)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHisActxactpedidoDeleted(itemToDelete);


            Context.HisActxactpedidos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHisActxactpedidoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHisAnuladetcantprocesosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisanuladetcantprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisanuladetcantprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHisAnuladetcantprocesosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisanuladetcantprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisanuladetcantprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHisAnuladetcantprocesosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisAnuladetcantproceso> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.HisAnuladetcantproceso>> GetHisAnuladetcantprocesos(Query query = null)
        {
            var items = Context.HisAnuladetcantprocesos.AsQueryable();


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

            OnHisAnuladetcantprocesosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHisAnuladetcantprocesoGet(Aldebaran.Web.Models.AldebaranContext.HisAnuladetcantproceso item);
        partial void OnGetHisAnuladetcantprocesoByIddetanulaproceso(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisAnuladetcantproceso> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.HisAnuladetcantproceso> GetHisAnuladetcantprocesoByIddetanulaproceso(int iddetanulaproceso)
        {
            var items = Context.HisAnuladetcantprocesos
                              .AsNoTracking()
                              .Where(i => i.IDDETANULAPROCESO == iddetanulaproceso);


            OnGetHisAnuladetcantprocesoByIddetanulaproceso(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHisAnuladetcantprocesoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHisAnuladetcantprocesoCreated(Aldebaran.Web.Models.AldebaranContext.HisAnuladetcantproceso item);
        partial void OnAfterHisAnuladetcantprocesoCreated(Aldebaran.Web.Models.AldebaranContext.HisAnuladetcantproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisAnuladetcantproceso> CreateHisAnuladetcantproceso(Aldebaran.Web.Models.AldebaranContext.HisAnuladetcantproceso hisanuladetcantproceso)
        {
            OnHisAnuladetcantprocesoCreated(hisanuladetcantproceso);

            var existingItem = Context.HisAnuladetcantprocesos
                              .Where(i => i.IDDETANULAPROCESO == hisanuladetcantproceso.IDDETANULAPROCESO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.HisAnuladetcantprocesos.Add(hisanuladetcantproceso);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(hisanuladetcantproceso).State = EntityState.Detached;
                throw;
            }

            OnAfterHisAnuladetcantprocesoCreated(hisanuladetcantproceso);

            return hisanuladetcantproceso;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisAnuladetcantproceso> CancelHisAnuladetcantprocesoChanges(Aldebaran.Web.Models.AldebaranContext.HisAnuladetcantproceso item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHisAnuladetcantprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.HisAnuladetcantproceso item);
        partial void OnAfterHisAnuladetcantprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.HisAnuladetcantproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisAnuladetcantproceso> UpdateHisAnuladetcantproceso(int iddetanulaproceso, Aldebaran.Web.Models.AldebaranContext.HisAnuladetcantproceso hisanuladetcantproceso)
        {
            OnHisAnuladetcantprocesoUpdated(hisanuladetcantproceso);

            var itemToUpdate = Context.HisAnuladetcantprocesos
                              .Where(i => i.IDDETANULAPROCESO == hisanuladetcantproceso.IDDETANULAPROCESO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(hisanuladetcantproceso);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHisAnuladetcantprocesoUpdated(hisanuladetcantproceso);

            return hisanuladetcantproceso;
        }

        partial void OnHisAnuladetcantprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.HisAnuladetcantproceso item);
        partial void OnAfterHisAnuladetcantprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.HisAnuladetcantproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisAnuladetcantproceso> DeleteHisAnuladetcantproceso(int iddetanulaproceso)
        {
            var itemToDelete = Context.HisAnuladetcantprocesos
                              .Where(i => i.IDDETANULAPROCESO == iddetanulaproceso)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHisAnuladetcantprocesoDeleted(itemToDelete);


            Context.HisAnuladetcantprocesos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHisAnuladetcantprocesoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHisAnulaprocesosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisanulaprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisanulaprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHisAnulaprocesosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisanulaprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisanulaprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHisAnulaprocesosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso>> GetHisAnulaprocesos(Query query = null)
        {
            var items = Context.HisAnulaprocesos.AsQueryable();


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

            OnHisAnulaprocesosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHisAnulaprocesoGet(Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso item);
        partial void OnGetHisAnulaprocesoByIdanulaproceso(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso> GetHisAnulaprocesoByIdanulaproceso(int idanulaproceso)
        {
            var items = Context.HisAnulaprocesos
                              .AsNoTracking()
                              .Where(i => i.IDANULAPROCESO == idanulaproceso);


            OnGetHisAnulaprocesoByIdanulaproceso(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHisAnulaprocesoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHisAnulaprocesoCreated(Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso item);
        partial void OnAfterHisAnulaprocesoCreated(Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso> CreateHisAnulaproceso(Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso hisanulaproceso)
        {
            OnHisAnulaprocesoCreated(hisanulaproceso);

            var existingItem = Context.HisAnulaprocesos
                              .Where(i => i.IDANULAPROCESO == hisanulaproceso.IDANULAPROCESO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.HisAnulaprocesos.Add(hisanulaproceso);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(hisanulaproceso).State = EntityState.Detached;
                throw;
            }

            OnAfterHisAnulaprocesoCreated(hisanulaproceso);

            return hisanulaproceso;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso> CancelHisAnulaprocesoChanges(Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHisAnulaprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso item);
        partial void OnAfterHisAnulaprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso> UpdateHisAnulaproceso(int idanulaproceso, Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso hisanulaproceso)
        {
            OnHisAnulaprocesoUpdated(hisanulaproceso);

            var itemToUpdate = Context.HisAnulaprocesos
                              .Where(i => i.IDANULAPROCESO == hisanulaproceso.IDANULAPROCESO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(hisanulaproceso);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHisAnulaprocesoUpdated(hisanulaproceso);

            return hisanulaproceso;
        }

        partial void OnHisAnulaprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso item);
        partial void OnAfterHisAnulaprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso> DeleteHisAnulaproceso(int idanulaproceso)
        {
            var itemToDelete = Context.HisAnulaprocesos
                              .Where(i => i.IDANULAPROCESO == idanulaproceso)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHisAnulaprocesoDeleted(itemToDelete);


            Context.HisAnulaprocesos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHisAnulaprocesoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHisAnulasubitemdetprocesosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisanulasubitemdetprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisanulasubitemdetprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHisAnulasubitemdetprocesosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisanulasubitemdetprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisanulasubitemdetprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHisAnulasubitemdetprocesosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisAnulasubitemdetproceso> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.HisAnulasubitemdetproceso>> GetHisAnulasubitemdetprocesos(Query query = null)
        {
            var items = Context.HisAnulasubitemdetprocesos.AsQueryable();


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

            OnHisAnulasubitemdetprocesosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHisAnulasubitemdetprocesoGet(Aldebaran.Web.Models.AldebaranContext.HisAnulasubitemdetproceso item);
        partial void OnGetHisAnulasubitemdetprocesoByIdansubitemdetproceso(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisAnulasubitemdetproceso> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.HisAnulasubitemdetproceso> GetHisAnulasubitemdetprocesoByIdansubitemdetproceso(int idansubitemdetproceso)
        {
            var items = Context.HisAnulasubitemdetprocesos
                              .AsNoTracking()
                              .Where(i => i.IDANSUBITEMDETPROCESO == idansubitemdetproceso);


            OnGetHisAnulasubitemdetprocesoByIdansubitemdetproceso(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHisAnulasubitemdetprocesoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHisAnulasubitemdetprocesoCreated(Aldebaran.Web.Models.AldebaranContext.HisAnulasubitemdetproceso item);
        partial void OnAfterHisAnulasubitemdetprocesoCreated(Aldebaran.Web.Models.AldebaranContext.HisAnulasubitemdetproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisAnulasubitemdetproceso> CreateHisAnulasubitemdetproceso(Aldebaran.Web.Models.AldebaranContext.HisAnulasubitemdetproceso hisanulasubitemdetproceso)
        {
            OnHisAnulasubitemdetprocesoCreated(hisanulasubitemdetproceso);

            var existingItem = Context.HisAnulasubitemdetprocesos
                              .Where(i => i.IDANSUBITEMDETPROCESO == hisanulasubitemdetproceso.IDANSUBITEMDETPROCESO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.HisAnulasubitemdetprocesos.Add(hisanulasubitemdetproceso);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(hisanulasubitemdetproceso).State = EntityState.Detached;
                throw;
            }

            OnAfterHisAnulasubitemdetprocesoCreated(hisanulasubitemdetproceso);

            return hisanulasubitemdetproceso;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisAnulasubitemdetproceso> CancelHisAnulasubitemdetprocesoChanges(Aldebaran.Web.Models.AldebaranContext.HisAnulasubitemdetproceso item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHisAnulasubitemdetprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.HisAnulasubitemdetproceso item);
        partial void OnAfterHisAnulasubitemdetprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.HisAnulasubitemdetproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisAnulasubitemdetproceso> UpdateHisAnulasubitemdetproceso(int idansubitemdetproceso, Aldebaran.Web.Models.AldebaranContext.HisAnulasubitemdetproceso hisanulasubitemdetproceso)
        {
            OnHisAnulasubitemdetprocesoUpdated(hisanulasubitemdetproceso);

            var itemToUpdate = Context.HisAnulasubitemdetprocesos
                              .Where(i => i.IDANSUBITEMDETPROCESO == hisanulasubitemdetproceso.IDANSUBITEMDETPROCESO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(hisanulasubitemdetproceso);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHisAnulasubitemdetprocesoUpdated(hisanulasubitemdetproceso);

            return hisanulasubitemdetproceso;
        }

        partial void OnHisAnulasubitemdetprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.HisAnulasubitemdetproceso item);
        partial void OnAfterHisAnulasubitemdetprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.HisAnulasubitemdetproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisAnulasubitemdetproceso> DeleteHisAnulasubitemdetproceso(int idansubitemdetproceso)
        {
            var itemToDelete = Context.HisAnulasubitemdetprocesos
                              .Where(i => i.IDANSUBITEMDETPROCESO == idansubitemdetproceso)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHisAnulasubitemdetprocesoDeleted(itemToDelete);


            Context.HisAnulasubitemdetprocesos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHisAnulasubitemdetprocesoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHisCancelpedidosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hiscancelpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hiscancelpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHisCancelpedidosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hiscancelpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hiscancelpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHisCancelpedidosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisCancelpedido> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.HisCancelpedido>> GetHisCancelpedidos(Query query = null)
        {
            var items = Context.HisCancelpedidos.AsQueryable();


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

            OnHisCancelpedidosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHisCancelpedidoGet(Aldebaran.Web.Models.AldebaranContext.HisCancelpedido item);
        partial void OnGetHisCancelpedidoByIdcancelpedido(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisCancelpedido> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.HisCancelpedido> GetHisCancelpedidoByIdcancelpedido(int idcancelpedido)
        {
            var items = Context.HisCancelpedidos
                              .AsNoTracking()
                              .Where(i => i.IDCANCELPEDIDO == idcancelpedido);


            OnGetHisCancelpedidoByIdcancelpedido(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHisCancelpedidoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHisCancelpedidoCreated(Aldebaran.Web.Models.AldebaranContext.HisCancelpedido item);
        partial void OnAfterHisCancelpedidoCreated(Aldebaran.Web.Models.AldebaranContext.HisCancelpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisCancelpedido> CreateHisCancelpedido(Aldebaran.Web.Models.AldebaranContext.HisCancelpedido hiscancelpedido)
        {
            OnHisCancelpedidoCreated(hiscancelpedido);

            var existingItem = Context.HisCancelpedidos
                              .Where(i => i.IDCANCELPEDIDO == hiscancelpedido.IDCANCELPEDIDO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.HisCancelpedidos.Add(hiscancelpedido);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(hiscancelpedido).State = EntityState.Detached;
                throw;
            }

            OnAfterHisCancelpedidoCreated(hiscancelpedido);

            return hiscancelpedido;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisCancelpedido> CancelHisCancelpedidoChanges(Aldebaran.Web.Models.AldebaranContext.HisCancelpedido item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHisCancelpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.HisCancelpedido item);
        partial void OnAfterHisCancelpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.HisCancelpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisCancelpedido> UpdateHisCancelpedido(int idcancelpedido, Aldebaran.Web.Models.AldebaranContext.HisCancelpedido hiscancelpedido)
        {
            OnHisCancelpedidoUpdated(hiscancelpedido);

            var itemToUpdate = Context.HisCancelpedidos
                              .Where(i => i.IDCANCELPEDIDO == hiscancelpedido.IDCANCELPEDIDO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(hiscancelpedido);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHisCancelpedidoUpdated(hiscancelpedido);

            return hiscancelpedido;
        }

        partial void OnHisCancelpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.HisCancelpedido item);
        partial void OnAfterHisCancelpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.HisCancelpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisCancelpedido> DeleteHisCancelpedido(int idcancelpedido)
        {
            var itemToDelete = Context.HisCancelpedidos
                              .Where(i => i.IDCANCELPEDIDO == idcancelpedido)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHisCancelpedidoDeleted(itemToDelete);


            Context.HisCancelpedidos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHisCancelpedidoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHisCantprocesosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hiscantprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hiscantprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHisCantprocesosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hiscantprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hiscantprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHisCantprocesosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisCantproceso> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.HisCantproceso>> GetHisCantprocesos(Query query = null)
        {
            var items = Context.HisCantprocesos.AsQueryable();


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

            OnHisCantprocesosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHisCantprocesoGet(Aldebaran.Web.Models.AldebaranContext.HisCantproceso item);
        partial void OnGetHisCantprocesoByIdproceso(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisCantproceso> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.HisCantproceso> GetHisCantprocesoByIdproceso(int idproceso)
        {
            var items = Context.HisCantprocesos
                              .AsNoTracking()
                              .Where(i => i.IDPROCESO == idproceso);


            OnGetHisCantprocesoByIdproceso(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHisCantprocesoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHisCantprocesoCreated(Aldebaran.Web.Models.AldebaranContext.HisCantproceso item);
        partial void OnAfterHisCantprocesoCreated(Aldebaran.Web.Models.AldebaranContext.HisCantproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisCantproceso> CreateHisCantproceso(Aldebaran.Web.Models.AldebaranContext.HisCantproceso hiscantproceso)
        {
            OnHisCantprocesoCreated(hiscantproceso);

            var existingItem = Context.HisCantprocesos
                              .Where(i => i.IDPROCESO == hiscantproceso.IDPROCESO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.HisCantprocesos.Add(hiscantproceso);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(hiscantproceso).State = EntityState.Detached;
                throw;
            }

            OnAfterHisCantprocesoCreated(hiscantproceso);

            return hiscantproceso;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisCantproceso> CancelHisCantprocesoChanges(Aldebaran.Web.Models.AldebaranContext.HisCantproceso item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHisCantprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.HisCantproceso item);
        partial void OnAfterHisCantprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.HisCantproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisCantproceso> UpdateHisCantproceso(int idproceso, Aldebaran.Web.Models.AldebaranContext.HisCantproceso hiscantproceso)
        {
            OnHisCantprocesoUpdated(hiscantproceso);

            var itemToUpdate = Context.HisCantprocesos
                              .Where(i => i.IDPROCESO == hiscantproceso.IDPROCESO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(hiscantproceso);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHisCantprocesoUpdated(hiscantproceso);

            return hiscantproceso;
        }

        partial void OnHisCantprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.HisCantproceso item);
        partial void OnAfterHisCantprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.HisCantproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisCantproceso> DeleteHisCantproceso(int idproceso)
        {
            var itemToDelete = Context.HisCantprocesos
                              .Where(i => i.IDPROCESO == idproceso)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHisCantprocesoDeleted(itemToDelete);


            Context.HisCantprocesos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHisCantprocesoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHisDetcantprocesosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisdetcantprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisdetcantprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHisDetcantprocesosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisdetcantprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisdetcantprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHisDetcantprocesosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisDetcantproceso> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.HisDetcantproceso>> GetHisDetcantprocesos(Query query = null)
        {
            var items = Context.HisDetcantprocesos.AsQueryable();


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

            OnHisDetcantprocesosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHisDetcantprocesoGet(Aldebaran.Web.Models.AldebaranContext.HisDetcantproceso item);
        partial void OnGetHisDetcantprocesoByIddetcantproceso(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisDetcantproceso> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.HisDetcantproceso> GetHisDetcantprocesoByIddetcantproceso(int iddetcantproceso)
        {
            var items = Context.HisDetcantprocesos
                              .AsNoTracking()
                              .Where(i => i.IDDETCANTPROCESO == iddetcantproceso);


            OnGetHisDetcantprocesoByIddetcantproceso(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHisDetcantprocesoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHisDetcantprocesoCreated(Aldebaran.Web.Models.AldebaranContext.HisDetcantproceso item);
        partial void OnAfterHisDetcantprocesoCreated(Aldebaran.Web.Models.AldebaranContext.HisDetcantproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisDetcantproceso> CreateHisDetcantproceso(Aldebaran.Web.Models.AldebaranContext.HisDetcantproceso hisdetcantproceso)
        {
            OnHisDetcantprocesoCreated(hisdetcantproceso);

            var existingItem = Context.HisDetcantprocesos
                              .Where(i => i.IDDETCANTPROCESO == hisdetcantproceso.IDDETCANTPROCESO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.HisDetcantprocesos.Add(hisdetcantproceso);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(hisdetcantproceso).State = EntityState.Detached;
                throw;
            }

            OnAfterHisDetcantprocesoCreated(hisdetcantproceso);

            return hisdetcantproceso;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisDetcantproceso> CancelHisDetcantprocesoChanges(Aldebaran.Web.Models.AldebaranContext.HisDetcantproceso item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHisDetcantprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.HisDetcantproceso item);
        partial void OnAfterHisDetcantprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.HisDetcantproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisDetcantproceso> UpdateHisDetcantproceso(int iddetcantproceso, Aldebaran.Web.Models.AldebaranContext.HisDetcantproceso hisdetcantproceso)
        {
            OnHisDetcantprocesoUpdated(hisdetcantproceso);

            var itemToUpdate = Context.HisDetcantprocesos
                              .Where(i => i.IDDETCANTPROCESO == hisdetcantproceso.IDDETCANTPROCESO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(hisdetcantproceso);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHisDetcantprocesoUpdated(hisdetcantproceso);

            return hisdetcantproceso;
        }

        partial void OnHisDetcantprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.HisDetcantproceso item);
        partial void OnAfterHisDetcantprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.HisDetcantproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisDetcantproceso> DeleteHisDetcantproceso(int iddetcantproceso)
        {
            var itemToDelete = Context.HisDetcantprocesos
                              .Where(i => i.IDDETCANTPROCESO == iddetcantproceso)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHisDetcantprocesoDeleted(itemToDelete);


            Context.HisDetcantprocesos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHisDetcantprocesoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHisDetenviosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisdetenvios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisdetenvios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHisDetenviosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisdetenvios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisdetenvios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHisDetenviosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisDetenvio> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.HisDetenvio>> GetHisDetenvios(Query query = null)
        {
            var items = Context.HisDetenvios.AsQueryable();


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

            OnHisDetenviosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHisDetenvioGet(Aldebaran.Web.Models.AldebaranContext.HisDetenvio item);
        partial void OnGetHisDetenvioByIddetenvio(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisDetenvio> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.HisDetenvio> GetHisDetenvioByIddetenvio(int iddetenvio)
        {
            var items = Context.HisDetenvios
                              .AsNoTracking()
                              .Where(i => i.IDDETENVIO == iddetenvio);


            OnGetHisDetenvioByIddetenvio(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHisDetenvioGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHisDetenvioCreated(Aldebaran.Web.Models.AldebaranContext.HisDetenvio item);
        partial void OnAfterHisDetenvioCreated(Aldebaran.Web.Models.AldebaranContext.HisDetenvio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisDetenvio> CreateHisDetenvio(Aldebaran.Web.Models.AldebaranContext.HisDetenvio hisdetenvio)
        {
            OnHisDetenvioCreated(hisdetenvio);

            var existingItem = Context.HisDetenvios
                              .Where(i => i.IDDETENVIO == hisdetenvio.IDDETENVIO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.HisDetenvios.Add(hisdetenvio);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(hisdetenvio).State = EntityState.Detached;
                throw;
            }

            OnAfterHisDetenvioCreated(hisdetenvio);

            return hisdetenvio;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisDetenvio> CancelHisDetenvioChanges(Aldebaran.Web.Models.AldebaranContext.HisDetenvio item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHisDetenvioUpdated(Aldebaran.Web.Models.AldebaranContext.HisDetenvio item);
        partial void OnAfterHisDetenvioUpdated(Aldebaran.Web.Models.AldebaranContext.HisDetenvio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisDetenvio> UpdateHisDetenvio(int iddetenvio, Aldebaran.Web.Models.AldebaranContext.HisDetenvio hisdetenvio)
        {
            OnHisDetenvioUpdated(hisdetenvio);

            var itemToUpdate = Context.HisDetenvios
                              .Where(i => i.IDDETENVIO == hisdetenvio.IDDETENVIO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(hisdetenvio);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHisDetenvioUpdated(hisdetenvio);

            return hisdetenvio;
        }

        partial void OnHisDetenvioDeleted(Aldebaran.Web.Models.AldebaranContext.HisDetenvio item);
        partial void OnAfterHisDetenvioDeleted(Aldebaran.Web.Models.AldebaranContext.HisDetenvio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisDetenvio> DeleteHisDetenvio(int iddetenvio)
        {
            var itemToDelete = Context.HisDetenvios
                              .Where(i => i.IDDETENVIO == iddetenvio)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHisDetenvioDeleted(itemToDelete);


            Context.HisDetenvios.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHisDetenvioDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHisEnviosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisenvios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisenvios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHisEnviosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisenvios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisenvios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHisEnviosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisEnvio> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.HisEnvio>> GetHisEnvios(Query query = null)
        {
            var items = Context.HisEnvios.AsQueryable();


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

            OnHisEnviosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHisEnvioGet(Aldebaran.Web.Models.AldebaranContext.HisEnvio item);
        partial void OnGetHisEnvioByIdenvio(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisEnvio> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.HisEnvio> GetHisEnvioByIdenvio(int idenvio)
        {
            var items = Context.HisEnvios
                              .AsNoTracking()
                              .Where(i => i.IDENVIO == idenvio);


            OnGetHisEnvioByIdenvio(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHisEnvioGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHisEnvioCreated(Aldebaran.Web.Models.AldebaranContext.HisEnvio item);
        partial void OnAfterHisEnvioCreated(Aldebaran.Web.Models.AldebaranContext.HisEnvio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisEnvio> CreateHisEnvio(Aldebaran.Web.Models.AldebaranContext.HisEnvio hisenvio)
        {
            OnHisEnvioCreated(hisenvio);

            var existingItem = Context.HisEnvios
                              .Where(i => i.IDENVIO == hisenvio.IDENVIO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.HisEnvios.Add(hisenvio);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(hisenvio).State = EntityState.Detached;
                throw;
            }

            OnAfterHisEnvioCreated(hisenvio);

            return hisenvio;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisEnvio> CancelHisEnvioChanges(Aldebaran.Web.Models.AldebaranContext.HisEnvio item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHisEnvioUpdated(Aldebaran.Web.Models.AldebaranContext.HisEnvio item);
        partial void OnAfterHisEnvioUpdated(Aldebaran.Web.Models.AldebaranContext.HisEnvio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisEnvio> UpdateHisEnvio(int idenvio, Aldebaran.Web.Models.AldebaranContext.HisEnvio hisenvio)
        {
            OnHisEnvioUpdated(hisenvio);

            var itemToUpdate = Context.HisEnvios
                              .Where(i => i.IDENVIO == hisenvio.IDENVIO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(hisenvio);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHisEnvioUpdated(hisenvio);

            return hisenvio;
        }

        partial void OnHisEnvioDeleted(Aldebaran.Web.Models.AldebaranContext.HisEnvio item);
        partial void OnAfterHisEnvioDeleted(Aldebaran.Web.Models.AldebaranContext.HisEnvio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisEnvio> DeleteHisEnvio(int idenvio)
        {
            var itemToDelete = Context.HisEnvios
                              .Where(i => i.IDENVIO == idenvio)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHisEnvioDeleted(itemToDelete);


            Context.HisEnvios.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHisEnvioDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHisItempedidosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisitempedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisitempedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHisItempedidosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisitempedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisitempedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHisItempedidosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisItempedido> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.HisItempedido>> GetHisItempedidos(Query query = null)
        {
            var items = Context.HisItempedidos.AsQueryable();


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

            OnHisItempedidosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHisItempedidoGet(Aldebaran.Web.Models.AldebaranContext.HisItempedido item);
        partial void OnGetHisItempedidoByIditempedido(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisItempedido> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.HisItempedido> GetHisItempedidoByIditempedido(int iditempedido)
        {
            var items = Context.HisItempedidos
                              .AsNoTracking()
                              .Where(i => i.IDITEMPEDIDO == iditempedido);


            OnGetHisItempedidoByIditempedido(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHisItempedidoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHisItempedidoCreated(Aldebaran.Web.Models.AldebaranContext.HisItempedido item);
        partial void OnAfterHisItempedidoCreated(Aldebaran.Web.Models.AldebaranContext.HisItempedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisItempedido> CreateHisItempedido(Aldebaran.Web.Models.AldebaranContext.HisItempedido hisitempedido)
        {
            OnHisItempedidoCreated(hisitempedido);

            var existingItem = Context.HisItempedidos
                              .Where(i => i.IDITEMPEDIDO == hisitempedido.IDITEMPEDIDO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.HisItempedidos.Add(hisitempedido);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(hisitempedido).State = EntityState.Detached;
                throw;
            }

            OnAfterHisItempedidoCreated(hisitempedido);

            return hisitempedido;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisItempedido> CancelHisItempedidoChanges(Aldebaran.Web.Models.AldebaranContext.HisItempedido item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHisItempedidoUpdated(Aldebaran.Web.Models.AldebaranContext.HisItempedido item);
        partial void OnAfterHisItempedidoUpdated(Aldebaran.Web.Models.AldebaranContext.HisItempedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisItempedido> UpdateHisItempedido(int iditempedido, Aldebaran.Web.Models.AldebaranContext.HisItempedido hisitempedido)
        {
            OnHisItempedidoUpdated(hisitempedido);

            var itemToUpdate = Context.HisItempedidos
                              .Where(i => i.IDITEMPEDIDO == hisitempedido.IDITEMPEDIDO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(hisitempedido);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHisItempedidoUpdated(hisitempedido);

            return hisitempedido;
        }

        partial void OnHisItempedidoDeleted(Aldebaran.Web.Models.AldebaranContext.HisItempedido item);
        partial void OnAfterHisItempedidoDeleted(Aldebaran.Web.Models.AldebaranContext.HisItempedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisItempedido> DeleteHisItempedido(int iditempedido)
        {
            var itemToDelete = Context.HisItempedidos
                              .Where(i => i.IDITEMPEDIDO == iditempedido)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHisItempedidoDeleted(itemToDelete);


            Context.HisItempedidos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHisItempedidoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHisItempedidoagotadosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisitempedidoagotados/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisitempedidoagotados/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHisItempedidoagotadosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisitempedidoagotados/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisitempedidoagotados/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHisItempedidoagotadosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisItempedidoagotado> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.HisItempedidoagotado>> GetHisItempedidoagotados(Query query = null)
        {
            var items = Context.HisItempedidoagotados.AsQueryable();


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

            OnHisItempedidoagotadosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHisItempedidoagotadoGet(Aldebaran.Web.Models.AldebaranContext.HisItempedidoagotado item);
        partial void OnGetHisItempedidoagotadoByIditempedido(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisItempedidoagotado> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.HisItempedidoagotado> GetHisItempedidoagotadoByIditempedido(int iditempedido)
        {
            var items = Context.HisItempedidoagotados
                              .AsNoTracking()
                              .Where(i => i.IDITEMPEDIDO == iditempedido);


            OnGetHisItempedidoagotadoByIditempedido(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHisItempedidoagotadoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHisItempedidoagotadoCreated(Aldebaran.Web.Models.AldebaranContext.HisItempedidoagotado item);
        partial void OnAfterHisItempedidoagotadoCreated(Aldebaran.Web.Models.AldebaranContext.HisItempedidoagotado item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisItempedidoagotado> CreateHisItempedidoagotado(Aldebaran.Web.Models.AldebaranContext.HisItempedidoagotado hisitempedidoagotado)
        {
            OnHisItempedidoagotadoCreated(hisitempedidoagotado);

            var existingItem = Context.HisItempedidoagotados
                              .Where(i => i.IDITEMPEDIDO == hisitempedidoagotado.IDITEMPEDIDO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.HisItempedidoagotados.Add(hisitempedidoagotado);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(hisitempedidoagotado).State = EntityState.Detached;
                throw;
            }

            OnAfterHisItempedidoagotadoCreated(hisitempedidoagotado);

            return hisitempedidoagotado;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisItempedidoagotado> CancelHisItempedidoagotadoChanges(Aldebaran.Web.Models.AldebaranContext.HisItempedidoagotado item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHisItempedidoagotadoUpdated(Aldebaran.Web.Models.AldebaranContext.HisItempedidoagotado item);
        partial void OnAfterHisItempedidoagotadoUpdated(Aldebaran.Web.Models.AldebaranContext.HisItempedidoagotado item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisItempedidoagotado> UpdateHisItempedidoagotado(int iditempedido, Aldebaran.Web.Models.AldebaranContext.HisItempedidoagotado hisitempedidoagotado)
        {
            OnHisItempedidoagotadoUpdated(hisitempedidoagotado);

            var itemToUpdate = Context.HisItempedidoagotados
                              .Where(i => i.IDITEMPEDIDO == hisitempedidoagotado.IDITEMPEDIDO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(hisitempedidoagotado);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHisItempedidoagotadoUpdated(hisitempedidoagotado);

            return hisitempedidoagotado;
        }

        partial void OnHisItempedidoagotadoDeleted(Aldebaran.Web.Models.AldebaranContext.HisItempedidoagotado item);
        partial void OnAfterHisItempedidoagotadoDeleted(Aldebaran.Web.Models.AldebaranContext.HisItempedidoagotado item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisItempedidoagotado> DeleteHisItempedidoagotado(int iditempedido)
        {
            var itemToDelete = Context.HisItempedidoagotados
                              .Where(i => i.IDITEMPEDIDO == iditempedido)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHisItempedidoagotadoDeleted(itemToDelete);


            Context.HisItempedidoagotados.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHisItempedidoagotadoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHisItemreservasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisitemreservas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisitemreservas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHisItemreservasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisitemreservas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisitemreservas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHisItemreservasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisItemreserva> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.HisItemreserva>> GetHisItemreservas(Query query = null)
        {
            var items = Context.HisItemreservas.AsQueryable();


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

            OnHisItemreservasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHisItemreservaGet(Aldebaran.Web.Models.AldebaranContext.HisItemreserva item);
        partial void OnGetHisItemreservaByIddetreserva(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisItemreserva> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.HisItemreserva> GetHisItemreservaByIddetreserva(int iddetreserva)
        {
            var items = Context.HisItemreservas
                              .AsNoTracking()
                              .Where(i => i.IDDETRESERVA == iddetreserva);


            OnGetHisItemreservaByIddetreserva(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHisItemreservaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHisItemreservaCreated(Aldebaran.Web.Models.AldebaranContext.HisItemreserva item);
        partial void OnAfterHisItemreservaCreated(Aldebaran.Web.Models.AldebaranContext.HisItemreserva item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisItemreserva> CreateHisItemreserva(Aldebaran.Web.Models.AldebaranContext.HisItemreserva hisitemreserva)
        {
            OnHisItemreservaCreated(hisitemreserva);

            var existingItem = Context.HisItemreservas
                              .Where(i => i.IDDETRESERVA == hisitemreserva.IDDETRESERVA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.HisItemreservas.Add(hisitemreserva);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(hisitemreserva).State = EntityState.Detached;
                throw;
            }

            OnAfterHisItemreservaCreated(hisitemreserva);

            return hisitemreserva;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisItemreserva> CancelHisItemreservaChanges(Aldebaran.Web.Models.AldebaranContext.HisItemreserva item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHisItemreservaUpdated(Aldebaran.Web.Models.AldebaranContext.HisItemreserva item);
        partial void OnAfterHisItemreservaUpdated(Aldebaran.Web.Models.AldebaranContext.HisItemreserva item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisItemreserva> UpdateHisItemreserva(int iddetreserva, Aldebaran.Web.Models.AldebaranContext.HisItemreserva hisitemreserva)
        {
            OnHisItemreservaUpdated(hisitemreserva);

            var itemToUpdate = Context.HisItemreservas
                              .Where(i => i.IDDETRESERVA == hisitemreserva.IDDETRESERVA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(hisitemreserva);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHisItemreservaUpdated(hisitemreserva);

            return hisitemreserva;
        }

        partial void OnHisItemreservaDeleted(Aldebaran.Web.Models.AldebaranContext.HisItemreserva item);
        partial void OnAfterHisItemreservaDeleted(Aldebaran.Web.Models.AldebaranContext.HisItemreserva item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisItemreserva> DeleteHisItemreserva(int iddetreserva)
        {
            var itemToDelete = Context.HisItemreservas
                              .Where(i => i.IDDETRESERVA == iddetreserva)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHisItemreservaDeleted(itemToDelete);


            Context.HisItemreservas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHisItemreservaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHisModpedidosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hismodpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hismodpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHisModpedidosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hismodpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hismodpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHisModpedidosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisModpedido> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.HisModpedido>> GetHisModpedidos(Query query = null)
        {
            var items = Context.HisModpedidos.AsQueryable();


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

            OnHisModpedidosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHisModpedidoGet(Aldebaran.Web.Models.AldebaranContext.HisModpedido item);
        partial void OnGetHisModpedidoByIdpedidoAndIdfuncionarioAndFecha(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisModpedido> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.HisModpedido> GetHisModpedidoByIdpedidoAndIdfuncionarioAndFecha(int idpedido, int idfuncionario, DateTime fecha)
        {
            var items = Context.HisModpedidos
                              .AsNoTracking()
                              .Where(i => i.IDPEDIDO == idpedido && i.IDFUNCIONARIO == idfuncionario && i.FECHA == fecha);


            OnGetHisModpedidoByIdpedidoAndIdfuncionarioAndFecha(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHisModpedidoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHisModpedidoCreated(Aldebaran.Web.Models.AldebaranContext.HisModpedido item);
        partial void OnAfterHisModpedidoCreated(Aldebaran.Web.Models.AldebaranContext.HisModpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisModpedido> CreateHisModpedido(Aldebaran.Web.Models.AldebaranContext.HisModpedido hismodpedido)
        {
            OnHisModpedidoCreated(hismodpedido);

            var existingItem = Context.HisModpedidos
                              .Where(i => i.IDPEDIDO == hismodpedido.IDPEDIDO && i.IDFUNCIONARIO == hismodpedido.IDFUNCIONARIO && i.FECHA == hismodpedido.FECHA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.HisModpedidos.Add(hismodpedido);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(hismodpedido).State = EntityState.Detached;
                throw;
            }

            OnAfterHisModpedidoCreated(hismodpedido);

            return hismodpedido;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisModpedido> CancelHisModpedidoChanges(Aldebaran.Web.Models.AldebaranContext.HisModpedido item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHisModpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.HisModpedido item);
        partial void OnAfterHisModpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.HisModpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisModpedido> UpdateHisModpedido(int idpedido, int idfuncionario, DateTime fecha, Aldebaran.Web.Models.AldebaranContext.HisModpedido hismodpedido)
        {
            OnHisModpedidoUpdated(hismodpedido);

            var itemToUpdate = Context.HisModpedidos
                              .Where(i => i.IDPEDIDO == hismodpedido.IDPEDIDO && i.IDFUNCIONARIO == hismodpedido.IDFUNCIONARIO && i.FECHA == hismodpedido.FECHA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(hismodpedido);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHisModpedidoUpdated(hismodpedido);

            return hismodpedido;
        }

        partial void OnHisModpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.HisModpedido item);
        partial void OnAfterHisModpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.HisModpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisModpedido> DeleteHisModpedido(int idpedido, int idfuncionario, DateTime fecha)
        {
            var itemToDelete = Context.HisModpedidos
                              .Where(i => i.IDPEDIDO == idpedido && i.IDFUNCIONARIO == idfuncionario && i.FECHA == fecha)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHisModpedidoDeleted(itemToDelete);


            Context.HisModpedidos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHisModpedidoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHisPedidosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hispedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hispedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHisPedidosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hispedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hispedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHisPedidosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisPedido> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.HisPedido>> GetHisPedidos(Query query = null)
        {
            var items = Context.HisPedidos.AsQueryable();


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

            OnHisPedidosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHisPedidoGet(Aldebaran.Web.Models.AldebaranContext.HisPedido item);
        partial void OnGetHisPedidoByIdpedido(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisPedido> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.HisPedido> GetHisPedidoByIdpedido(int idpedido)
        {
            var items = Context.HisPedidos
                              .AsNoTracking()
                              .Where(i => i.IDPEDIDO == idpedido);


            OnGetHisPedidoByIdpedido(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHisPedidoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHisPedidoCreated(Aldebaran.Web.Models.AldebaranContext.HisPedido item);
        partial void OnAfterHisPedidoCreated(Aldebaran.Web.Models.AldebaranContext.HisPedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisPedido> CreateHisPedido(Aldebaran.Web.Models.AldebaranContext.HisPedido hispedido)
        {
            OnHisPedidoCreated(hispedido);

            var existingItem = Context.HisPedidos
                              .Where(i => i.IDPEDIDO == hispedido.IDPEDIDO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.HisPedidos.Add(hispedido);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(hispedido).State = EntityState.Detached;
                throw;
            }

            OnAfterHisPedidoCreated(hispedido);

            return hispedido;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisPedido> CancelHisPedidoChanges(Aldebaran.Web.Models.AldebaranContext.HisPedido item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHisPedidoUpdated(Aldebaran.Web.Models.AldebaranContext.HisPedido item);
        partial void OnAfterHisPedidoUpdated(Aldebaran.Web.Models.AldebaranContext.HisPedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisPedido> UpdateHisPedido(int idpedido, Aldebaran.Web.Models.AldebaranContext.HisPedido hispedido)
        {
            OnHisPedidoUpdated(hispedido);

            var itemToUpdate = Context.HisPedidos
                              .Where(i => i.IDPEDIDO == hispedido.IDPEDIDO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(hispedido);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHisPedidoUpdated(hispedido);

            return hispedido;
        }

        partial void OnHisPedidoDeleted(Aldebaran.Web.Models.AldebaranContext.HisPedido item);
        partial void OnAfterHisPedidoDeleted(Aldebaran.Web.Models.AldebaranContext.HisPedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisPedido> DeleteHisPedido(int idpedido)
        {
            var itemToDelete = Context.HisPedidos
                              .Where(i => i.IDPEDIDO == idpedido)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHisPedidoDeleted(itemToDelete);


            Context.HisPedidos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHisPedidoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHisReservasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisreservas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisreservas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHisReservasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hisreservas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hisreservas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHisReservasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisReserva> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.HisReserva>> GetHisReservas(Query query = null)
        {
            var items = Context.HisReservas.AsQueryable();


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

            OnHisReservasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHisReservaGet(Aldebaran.Web.Models.AldebaranContext.HisReserva item);
        partial void OnGetHisReservaByIdreserva(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisReserva> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.HisReserva> GetHisReservaByIdreserva(int idreserva)
        {
            var items = Context.HisReservas
                              .AsNoTracking()
                              .Where(i => i.IDRESERVA == idreserva);


            OnGetHisReservaByIdreserva(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHisReservaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHisReservaCreated(Aldebaran.Web.Models.AldebaranContext.HisReserva item);
        partial void OnAfterHisReservaCreated(Aldebaran.Web.Models.AldebaranContext.HisReserva item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisReserva> CreateHisReserva(Aldebaran.Web.Models.AldebaranContext.HisReserva hisreserva)
        {
            OnHisReservaCreated(hisreserva);

            var existingItem = Context.HisReservas
                              .Where(i => i.IDRESERVA == hisreserva.IDRESERVA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.HisReservas.Add(hisreserva);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(hisreserva).State = EntityState.Detached;
                throw;
            }

            OnAfterHisReservaCreated(hisreserva);

            return hisreserva;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisReserva> CancelHisReservaChanges(Aldebaran.Web.Models.AldebaranContext.HisReserva item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHisReservaUpdated(Aldebaran.Web.Models.AldebaranContext.HisReserva item);
        partial void OnAfterHisReservaUpdated(Aldebaran.Web.Models.AldebaranContext.HisReserva item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisReserva> UpdateHisReserva(int idreserva, Aldebaran.Web.Models.AldebaranContext.HisReserva hisreserva)
        {
            OnHisReservaUpdated(hisreserva);

            var itemToUpdate = Context.HisReservas
                              .Where(i => i.IDRESERVA == hisreserva.IDRESERVA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(hisreserva);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHisReservaUpdated(hisreserva);

            return hisreserva;
        }

        partial void OnHisReservaDeleted(Aldebaran.Web.Models.AldebaranContext.HisReserva item);
        partial void OnAfterHisReservaDeleted(Aldebaran.Web.Models.AldebaranContext.HisReserva item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisReserva> DeleteHisReserva(int idreserva)
        {
            var itemToDelete = Context.HisReservas
                              .Where(i => i.IDRESERVA == idreserva)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHisReservaDeleted(itemToDelete);


            Context.HisReservas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHisReservaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHisSubitemdetenviosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hissubitemdetenvios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hissubitemdetenvios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHisSubitemdetenviosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hissubitemdetenvios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hissubitemdetenvios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHisSubitemdetenviosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisSubitemdetenvio> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.HisSubitemdetenvio>> GetHisSubitemdetenvios(Query query = null)
        {
            var items = Context.HisSubitemdetenvios.AsQueryable();


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

            OnHisSubitemdetenviosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHisSubitemdetenvioGet(Aldebaran.Web.Models.AldebaranContext.HisSubitemdetenvio item);
        partial void OnGetHisSubitemdetenvioByIdsubitemdetenvio(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisSubitemdetenvio> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.HisSubitemdetenvio> GetHisSubitemdetenvioByIdsubitemdetenvio(int idsubitemdetenvio)
        {
            var items = Context.HisSubitemdetenvios
                              .AsNoTracking()
                              .Where(i => i.IDSUBITEMDETENVIO == idsubitemdetenvio);


            OnGetHisSubitemdetenvioByIdsubitemdetenvio(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHisSubitemdetenvioGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHisSubitemdetenvioCreated(Aldebaran.Web.Models.AldebaranContext.HisSubitemdetenvio item);
        partial void OnAfterHisSubitemdetenvioCreated(Aldebaran.Web.Models.AldebaranContext.HisSubitemdetenvio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisSubitemdetenvio> CreateHisSubitemdetenvio(Aldebaran.Web.Models.AldebaranContext.HisSubitemdetenvio hissubitemdetenvio)
        {
            OnHisSubitemdetenvioCreated(hissubitemdetenvio);

            var existingItem = Context.HisSubitemdetenvios
                              .Where(i => i.IDSUBITEMDETENVIO == hissubitemdetenvio.IDSUBITEMDETENVIO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.HisSubitemdetenvios.Add(hissubitemdetenvio);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(hissubitemdetenvio).State = EntityState.Detached;
                throw;
            }

            OnAfterHisSubitemdetenvioCreated(hissubitemdetenvio);

            return hissubitemdetenvio;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisSubitemdetenvio> CancelHisSubitemdetenvioChanges(Aldebaran.Web.Models.AldebaranContext.HisSubitemdetenvio item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHisSubitemdetenvioUpdated(Aldebaran.Web.Models.AldebaranContext.HisSubitemdetenvio item);
        partial void OnAfterHisSubitemdetenvioUpdated(Aldebaran.Web.Models.AldebaranContext.HisSubitemdetenvio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisSubitemdetenvio> UpdateHisSubitemdetenvio(int idsubitemdetenvio, Aldebaran.Web.Models.AldebaranContext.HisSubitemdetenvio hissubitemdetenvio)
        {
            OnHisSubitemdetenvioUpdated(hissubitemdetenvio);

            var itemToUpdate = Context.HisSubitemdetenvios
                              .Where(i => i.IDSUBITEMDETENVIO == hissubitemdetenvio.IDSUBITEMDETENVIO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(hissubitemdetenvio);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHisSubitemdetenvioUpdated(hissubitemdetenvio);

            return hissubitemdetenvio;
        }

        partial void OnHisSubitemdetenvioDeleted(Aldebaran.Web.Models.AldebaranContext.HisSubitemdetenvio item);
        partial void OnAfterHisSubitemdetenvioDeleted(Aldebaran.Web.Models.AldebaranContext.HisSubitemdetenvio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisSubitemdetenvio> DeleteHisSubitemdetenvio(int idsubitemdetenvio)
        {
            var itemToDelete = Context.HisSubitemdetenvios
                              .Where(i => i.IDSUBITEMDETENVIO == idsubitemdetenvio)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHisSubitemdetenvioDeleted(itemToDelete);


            Context.HisSubitemdetenvios.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHisSubitemdetenvioDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHisSubitemdetprocesosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hissubitemdetprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hissubitemdetprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHisSubitemdetprocesosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/hissubitemdetprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/hissubitemdetprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHisSubitemdetprocesosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisSubitemdetproceso> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.HisSubitemdetproceso>> GetHisSubitemdetprocesos(Query query = null)
        {
            var items = Context.HisSubitemdetprocesos.AsQueryable();


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

            OnHisSubitemdetprocesosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHisSubitemdetprocesoGet(Aldebaran.Web.Models.AldebaranContext.HisSubitemdetproceso item);
        partial void OnGetHisSubitemdetprocesoByIdsubitemdetproceso(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.HisSubitemdetproceso> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.HisSubitemdetproceso> GetHisSubitemdetprocesoByIdsubitemdetproceso(int idsubitemdetproceso)
        {
            var items = Context.HisSubitemdetprocesos
                              .AsNoTracking()
                              .Where(i => i.IDSUBITEMDETPROCESO == idsubitemdetproceso);


            OnGetHisSubitemdetprocesoByIdsubitemdetproceso(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHisSubitemdetprocesoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHisSubitemdetprocesoCreated(Aldebaran.Web.Models.AldebaranContext.HisSubitemdetproceso item);
        partial void OnAfterHisSubitemdetprocesoCreated(Aldebaran.Web.Models.AldebaranContext.HisSubitemdetproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisSubitemdetproceso> CreateHisSubitemdetproceso(Aldebaran.Web.Models.AldebaranContext.HisSubitemdetproceso hissubitemdetproceso)
        {
            OnHisSubitemdetprocesoCreated(hissubitemdetproceso);

            var existingItem = Context.HisSubitemdetprocesos
                              .Where(i => i.IDSUBITEMDETPROCESO == hissubitemdetproceso.IDSUBITEMDETPROCESO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.HisSubitemdetprocesos.Add(hissubitemdetproceso);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(hissubitemdetproceso).State = EntityState.Detached;
                throw;
            }

            OnAfterHisSubitemdetprocesoCreated(hissubitemdetproceso);

            return hissubitemdetproceso;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisSubitemdetproceso> CancelHisSubitemdetprocesoChanges(Aldebaran.Web.Models.AldebaranContext.HisSubitemdetproceso item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHisSubitemdetprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.HisSubitemdetproceso item);
        partial void OnAfterHisSubitemdetprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.HisSubitemdetproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisSubitemdetproceso> UpdateHisSubitemdetproceso(int idsubitemdetproceso, Aldebaran.Web.Models.AldebaranContext.HisSubitemdetproceso hissubitemdetproceso)
        {
            OnHisSubitemdetprocesoUpdated(hissubitemdetproceso);

            var itemToUpdate = Context.HisSubitemdetprocesos
                              .Where(i => i.IDSUBITEMDETPROCESO == hissubitemdetproceso.IDSUBITEMDETPROCESO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(hissubitemdetproceso);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHisSubitemdetprocesoUpdated(hissubitemdetproceso);

            return hissubitemdetproceso;
        }

        partial void OnHisSubitemdetprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.HisSubitemdetproceso item);
        partial void OnAfterHisSubitemdetprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.HisSubitemdetproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.HisSubitemdetproceso> DeleteHisSubitemdetproceso(int idsubitemdetproceso)
        {
            var itemToDelete = Context.HisSubitemdetprocesos
                              .Where(i => i.IDSUBITEMDETPROCESO == idsubitemdetproceso)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHisSubitemdetprocesoDeleted(itemToDelete);


            Context.HisSubitemdetprocesos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHisSubitemdetprocesoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHisterroresenvioscorreosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/histerroresenvioscorreos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/histerroresenvioscorreos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHisterroresenvioscorreosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/histerroresenvioscorreos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/histerroresenvioscorreos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHisterroresenvioscorreosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo>> GetHisterroresenvioscorreos(Query query = null)
        {
            var items = Context.Histerroresenvioscorreos.AsQueryable();


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

            OnHisterroresenvioscorreosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHisterroresenvioscorreoGet(Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo item);
        partial void OnGetHisterroresenvioscorreoByIderrorenvio(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo> GetHisterroresenvioscorreoByIderrorenvio(int iderrorenvio)
        {
            var items = Context.Histerroresenvioscorreos
                              .AsNoTracking()
                              .Where(i => i.IDERRORENVIO == iderrorenvio);


            OnGetHisterroresenvioscorreoByIderrorenvio(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHisterroresenvioscorreoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHisterroresenvioscorreoCreated(Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo item);
        partial void OnAfterHisterroresenvioscorreoCreated(Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo> CreateHisterroresenvioscorreo(Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo histerroresenvioscorreo)
        {
            OnHisterroresenvioscorreoCreated(histerroresenvioscorreo);

            var existingItem = Context.Histerroresenvioscorreos
                              .Where(i => i.IDERRORENVIO == histerroresenvioscorreo.IDERRORENVIO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Histerroresenvioscorreos.Add(histerroresenvioscorreo);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(histerroresenvioscorreo).State = EntityState.Detached;
                throw;
            }

            OnAfterHisterroresenvioscorreoCreated(histerroresenvioscorreo);

            return histerroresenvioscorreo;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo> CancelHisterroresenvioscorreoChanges(Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHisterroresenvioscorreoUpdated(Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo item);
        partial void OnAfterHisterroresenvioscorreoUpdated(Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo> UpdateHisterroresenvioscorreo(int iderrorenvio, Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo histerroresenvioscorreo)
        {
            OnHisterroresenvioscorreoUpdated(histerroresenvioscorreo);

            var itemToUpdate = Context.Histerroresenvioscorreos
                              .Where(i => i.IDERRORENVIO == histerroresenvioscorreo.IDERRORENVIO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(histerroresenvioscorreo);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHisterroresenvioscorreoUpdated(histerroresenvioscorreo);

            return histerroresenvioscorreo;
        }

        partial void OnHisterroresenvioscorreoDeleted(Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo item);
        partial void OnAfterHisterroresenvioscorreoDeleted(Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo> DeleteHisterroresenvioscorreo(int iderrorenvio)
        {
            var itemToDelete = Context.Histerroresenvioscorreos
                              .Where(i => i.IDERRORENVIO == iderrorenvio)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHisterroresenvioscorreoDeleted(itemToDelete);


            Context.Histerroresenvioscorreos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHisterroresenvioscorreoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHistfinanosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/histfinanos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/histfinanos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHistfinanosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/histfinanos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/histfinanos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHistfinanosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Histfinano> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Histfinano>> GetHistfinanos(Query query = null)
        {
            var items = Context.Histfinanos.AsQueryable();


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

            OnHistfinanosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHistfinanoGet(Aldebaran.Web.Models.AldebaranContext.Histfinano item);
        partial void OnGetHistfinanoByAnnoAndSemestreAndIditemxcolorAndIdbodega(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Histfinano> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Histfinano> GetHistfinanoByAnnoAndSemestreAndIditemxcolorAndIdbodega(int anno, int semestre, int iditemxcolor, int idbodega)
        {
            var items = Context.Histfinanos
                              .AsNoTracking()
                              .Where(i => i.ANNO == anno && i.SEMESTRE == semestre && i.IDITEMXCOLOR == iditemxcolor && i.IDBODEGA == idbodega);


            OnGetHistfinanoByAnnoAndSemestreAndIditemxcolorAndIdbodega(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHistfinanoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHistfinanoCreated(Aldebaran.Web.Models.AldebaranContext.Histfinano item);
        partial void OnAfterHistfinanoCreated(Aldebaran.Web.Models.AldebaranContext.Histfinano item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Histfinano> CreateHistfinano(Aldebaran.Web.Models.AldebaranContext.Histfinano histfinano)
        {
            OnHistfinanoCreated(histfinano);

            var existingItem = Context.Histfinanos
                              .Where(i => i.ANNO == histfinano.ANNO && i.SEMESTRE == histfinano.SEMESTRE && i.IDITEMXCOLOR == histfinano.IDITEMXCOLOR && i.IDBODEGA == histfinano.IDBODEGA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Histfinanos.Add(histfinano);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(histfinano).State = EntityState.Detached;
                throw;
            }

            OnAfterHistfinanoCreated(histfinano);

            return histfinano;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Histfinano> CancelHistfinanoChanges(Aldebaran.Web.Models.AldebaranContext.Histfinano item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHistfinanoUpdated(Aldebaran.Web.Models.AldebaranContext.Histfinano item);
        partial void OnAfterHistfinanoUpdated(Aldebaran.Web.Models.AldebaranContext.Histfinano item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Histfinano> UpdateHistfinano(int anno, int semestre, int iditemxcolor, int idbodega, Aldebaran.Web.Models.AldebaranContext.Histfinano histfinano)
        {
            OnHistfinanoUpdated(histfinano);

            var itemToUpdate = Context.Histfinanos
                              .Where(i => i.ANNO == histfinano.ANNO && i.SEMESTRE == histfinano.SEMESTRE && i.IDITEMXCOLOR == histfinano.IDITEMXCOLOR && i.IDBODEGA == histfinano.IDBODEGA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(histfinano);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHistfinanoUpdated(histfinano);

            return histfinano;
        }

        partial void OnHistfinanoDeleted(Aldebaran.Web.Models.AldebaranContext.Histfinano item);
        partial void OnAfterHistfinanoDeleted(Aldebaran.Web.Models.AldebaranContext.Histfinano item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Histfinano> DeleteHistfinano(int anno, int semestre, int iditemxcolor, int idbodega)
        {
            var itemToDelete = Context.Histfinanos
                              .Where(i => i.ANNO == anno && i.SEMESTRE == semestre && i.IDITEMXCOLOR == iditemxcolor && i.IDBODEGA == idbodega)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHistfinanoDeleted(itemToDelete);


            Context.Histfinanos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHistfinanoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHistinianosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/histinianos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/histinianos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHistinianosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/histinianos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/histinianos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHistinianosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Histiniano> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Histiniano>> GetHistinianos(Query query = null)
        {
            var items = Context.Histinianos.AsQueryable();


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

            OnHistinianosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHistinianoGet(Aldebaran.Web.Models.AldebaranContext.Histiniano item);
        partial void OnGetHistinianoByAnnoAndSemestreAndIditemxcolorAndIdbodega(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Histiniano> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Histiniano> GetHistinianoByAnnoAndSemestreAndIditemxcolorAndIdbodega(int anno, int semestre, int iditemxcolor, int idbodega)
        {
            var items = Context.Histinianos
                              .AsNoTracking()
                              .Where(i => i.ANNO == anno && i.SEMESTRE == semestre && i.IDITEMXCOLOR == iditemxcolor && i.IDBODEGA == idbodega);


            OnGetHistinianoByAnnoAndSemestreAndIditemxcolorAndIdbodega(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHistinianoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHistinianoCreated(Aldebaran.Web.Models.AldebaranContext.Histiniano item);
        partial void OnAfterHistinianoCreated(Aldebaran.Web.Models.AldebaranContext.Histiniano item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Histiniano> CreateHistiniano(Aldebaran.Web.Models.AldebaranContext.Histiniano histiniano)
        {
            OnHistinianoCreated(histiniano);

            var existingItem = Context.Histinianos
                              .Where(i => i.ANNO == histiniano.ANNO && i.SEMESTRE == histiniano.SEMESTRE && i.IDITEMXCOLOR == histiniano.IDITEMXCOLOR && i.IDBODEGA == histiniano.IDBODEGA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Histinianos.Add(histiniano);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(histiniano).State = EntityState.Detached;
                throw;
            }

            OnAfterHistinianoCreated(histiniano);

            return histiniano;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Histiniano> CancelHistinianoChanges(Aldebaran.Web.Models.AldebaranContext.Histiniano item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHistinianoUpdated(Aldebaran.Web.Models.AldebaranContext.Histiniano item);
        partial void OnAfterHistinianoUpdated(Aldebaran.Web.Models.AldebaranContext.Histiniano item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Histiniano> UpdateHistiniano(int anno, int semestre, int iditemxcolor, int idbodega, Aldebaran.Web.Models.AldebaranContext.Histiniano histiniano)
        {
            OnHistinianoUpdated(histiniano);

            var itemToUpdate = Context.Histinianos
                              .Where(i => i.ANNO == histiniano.ANNO && i.SEMESTRE == histiniano.SEMESTRE && i.IDITEMXCOLOR == histiniano.IDITEMXCOLOR && i.IDBODEGA == histiniano.IDBODEGA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(histiniano);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHistinianoUpdated(histiniano);

            return histiniano;
        }

        partial void OnHistinianoDeleted(Aldebaran.Web.Models.AldebaranContext.Histiniano item);
        partial void OnAfterHistinianoDeleted(Aldebaran.Web.Models.AldebaranContext.Histiniano item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Histiniano> DeleteHistiniano(int anno, int semestre, int iditemxcolor, int idbodega)
        {
            var itemToDelete = Context.Histinianos
                              .Where(i => i.ANNO == anno && i.SEMESTRE == semestre && i.IDITEMXCOLOR == iditemxcolor && i.IDBODEGA == idbodega)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHistinianoDeleted(itemToDelete);


            Context.Histinianos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHistinianoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportHorariosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/horarios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/horarios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHorariosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/horarios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/horarios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHorariosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Horario> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Horario>> GetHorarios(Query query = null)
        {
            var items = Context.Horarios.AsQueryable();

            items = items.Include(i => i.Grupo);

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

            OnHorariosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHorarioGet(Aldebaran.Web.Models.AldebaranContext.Horario item);
        partial void OnGetHorarioByIdhorario(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Horario> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Horario> GetHorarioByIdhorario(int idhorario)
        {
            var items = Context.Horarios
                              .AsNoTracking()
                              .Where(i => i.IDHORARIO == idhorario);

            items = items.Include(i => i.Grupo);

            OnGetHorarioByIdhorario(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHorarioGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHorarioCreated(Aldebaran.Web.Models.AldebaranContext.Horario item);
        partial void OnAfterHorarioCreated(Aldebaran.Web.Models.AldebaranContext.Horario item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Horario> CreateHorario(Aldebaran.Web.Models.AldebaranContext.Horario horario)
        {
            OnHorarioCreated(horario);

            var existingItem = Context.Horarios
                              .Where(i => i.IDHORARIO == horario.IDHORARIO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Horarios.Add(horario);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(horario).State = EntityState.Detached;
                throw;
            }

            OnAfterHorarioCreated(horario);

            return horario;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Horario> CancelHorarioChanges(Aldebaran.Web.Models.AldebaranContext.Horario item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHorarioUpdated(Aldebaran.Web.Models.AldebaranContext.Horario item);
        partial void OnAfterHorarioUpdated(Aldebaran.Web.Models.AldebaranContext.Horario item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Horario> UpdateHorario(int idhorario, Aldebaran.Web.Models.AldebaranContext.Horario horario)
        {
            OnHorarioUpdated(horario);

            var itemToUpdate = Context.Horarios
                              .Where(i => i.IDHORARIO == horario.IDHORARIO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(horario);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHorarioUpdated(horario);

            return horario;
        }

        partial void OnHorarioDeleted(Aldebaran.Web.Models.AldebaranContext.Horario item);
        partial void OnAfterHorarioDeleted(Aldebaran.Web.Models.AldebaranContext.Horario item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Horario> DeleteHorario(int idhorario)
        {
            var itemToDelete = Context.Horarios
                              .Where(i => i.IDHORARIO == idhorario)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnHorarioDeleted(itemToDelete);


            Context.Horarios.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHorarioDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportIntegrasaldosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/integrasaldos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/integrasaldos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportIntegrasaldosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/integrasaldos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/integrasaldos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnIntegrasaldosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Integrasaldo> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Integrasaldo>> GetIntegrasaldos(Query query = null)
        {
            var items = Context.Integrasaldos.AsQueryable();

            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);

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

            OnIntegrasaldosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnIntegrasaldoGet(Aldebaran.Web.Models.AldebaranContext.Integrasaldo item);
        partial void OnGetIntegrasaldoByIditemxcolorAndIdbodega(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Integrasaldo> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Integrasaldo> GetIntegrasaldoByIditemxcolorAndIdbodega(int iditemxcolor, int idbodega)
        {
            var items = Context.Integrasaldos
                              .AsNoTracking()
                              .Where(i => i.IDITEMXCOLOR == iditemxcolor && i.IDBODEGA == idbodega);

            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);

            OnGetIntegrasaldoByIditemxcolorAndIdbodega(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnIntegrasaldoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnIntegrasaldoCreated(Aldebaran.Web.Models.AldebaranContext.Integrasaldo item);
        partial void OnAfterIntegrasaldoCreated(Aldebaran.Web.Models.AldebaranContext.Integrasaldo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Integrasaldo> CreateIntegrasaldo(Aldebaran.Web.Models.AldebaranContext.Integrasaldo integrasaldo)
        {
            OnIntegrasaldoCreated(integrasaldo);

            var existingItem = Context.Integrasaldos
                              .Where(i => i.IDITEMXCOLOR == integrasaldo.IDITEMXCOLOR && i.IDBODEGA == integrasaldo.IDBODEGA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Integrasaldos.Add(integrasaldo);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(integrasaldo).State = EntityState.Detached;
                throw;
            }

            OnAfterIntegrasaldoCreated(integrasaldo);

            return integrasaldo;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Integrasaldo> CancelIntegrasaldoChanges(Aldebaran.Web.Models.AldebaranContext.Integrasaldo item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnIntegrasaldoUpdated(Aldebaran.Web.Models.AldebaranContext.Integrasaldo item);
        partial void OnAfterIntegrasaldoUpdated(Aldebaran.Web.Models.AldebaranContext.Integrasaldo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Integrasaldo> UpdateIntegrasaldo(int iditemxcolor, int idbodega, Aldebaran.Web.Models.AldebaranContext.Integrasaldo integrasaldo)
        {
            OnIntegrasaldoUpdated(integrasaldo);

            var itemToUpdate = Context.Integrasaldos
                              .Where(i => i.IDITEMXCOLOR == integrasaldo.IDITEMXCOLOR && i.IDBODEGA == integrasaldo.IDBODEGA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(integrasaldo);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterIntegrasaldoUpdated(integrasaldo);

            return integrasaldo;
        }

        partial void OnIntegrasaldoDeleted(Aldebaran.Web.Models.AldebaranContext.Integrasaldo item);
        partial void OnAfterIntegrasaldoDeleted(Aldebaran.Web.Models.AldebaranContext.Integrasaldo item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Integrasaldo> DeleteIntegrasaldo(int iditemxcolor, int idbodega)
        {
            var itemToDelete = Context.Integrasaldos
                              .Where(i => i.IDITEMXCOLOR == iditemxcolor && i.IDBODEGA == idbodega)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnIntegrasaldoDeleted(itemToDelete);


            Context.Integrasaldos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterIntegrasaldoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportItemordensToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itemordens/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itemordens/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportItemordensToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itemordens/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itemordens/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnItemordensRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemorden> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemorden>> GetItemordens(Query query = null)
        {
            var items = Context.Itemordens.AsQueryable();

            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Ordene);

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

            OnItemordensRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnItemordenGet(Aldebaran.Web.Models.AldebaranContext.Itemorden item);
        partial void OnGetItemordenByIditemorden(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemorden> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemorden> GetItemordenByIditemorden(int iditemorden)
        {
            var items = Context.Itemordens
                              .AsNoTracking()
                              .Where(i => i.IDITEMORDEN == iditemorden);

            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Ordene);

            OnGetItemordenByIditemorden(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnItemordenGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnItemordenCreated(Aldebaran.Web.Models.AldebaranContext.Itemorden item);
        partial void OnAfterItemordenCreated(Aldebaran.Web.Models.AldebaranContext.Itemorden item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemorden> CreateItemorden(Aldebaran.Web.Models.AldebaranContext.Itemorden itemorden)
        {
            OnItemordenCreated(itemorden);

            var existingItem = Context.Itemordens
                              .Where(i => i.IDITEMORDEN == itemorden.IDITEMORDEN)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Itemordens.Add(itemorden);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemorden).State = EntityState.Detached;
                throw;
            }

            OnAfterItemordenCreated(itemorden);

            return itemorden;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemorden> CancelItemordenChanges(Aldebaran.Web.Models.AldebaranContext.Itemorden item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItemordenUpdated(Aldebaran.Web.Models.AldebaranContext.Itemorden item);
        partial void OnAfterItemordenUpdated(Aldebaran.Web.Models.AldebaranContext.Itemorden item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemorden> UpdateItemorden(int iditemorden, Aldebaran.Web.Models.AldebaranContext.Itemorden itemorden)
        {
            OnItemordenUpdated(itemorden);

            var itemToUpdate = Context.Itemordens
                              .Where(i => i.IDITEMORDEN == itemorden.IDITEMORDEN)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(itemorden);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterItemordenUpdated(itemorden);

            return itemorden;
        }

        partial void OnItemordenDeleted(Aldebaran.Web.Models.AldebaranContext.Itemorden item);
        partial void OnAfterItemordenDeleted(Aldebaran.Web.Models.AldebaranContext.Itemorden item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemorden> DeleteItemorden(int iditemorden)
        {
            var itemToDelete = Context.Itemordens
                              .Where(i => i.IDITEMORDEN == iditemorden)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnItemordenDeleted(itemToDelete);


            Context.Itemordens.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterItemordenDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportItempedidosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itempedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itempedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportItempedidosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itempedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itempedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnItempedidosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itempedido> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Itempedido>> GetItempedidos(Query query = null)
        {
            var items = Context.Itempedidos.AsQueryable();

            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Linea);
            items = items.Include(i => i.Pedido);

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

            OnItempedidosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnItempedidoGet(Aldebaran.Web.Models.AldebaranContext.Itempedido item);
        partial void OnGetItempedidoByIditempedido(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itempedido> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Itempedido> GetItempedidoByIditempedido(int iditempedido)
        {
            var items = Context.Itempedidos
                              .AsNoTracking()
                              .Where(i => i.IDITEMPEDIDO == iditempedido);

            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Linea);
            items = items.Include(i => i.Pedido);

            OnGetItempedidoByIditempedido(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnItempedidoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnItempedidoCreated(Aldebaran.Web.Models.AldebaranContext.Itempedido item);
        partial void OnAfterItempedidoCreated(Aldebaran.Web.Models.AldebaranContext.Itempedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itempedido> CreateItempedido(Aldebaran.Web.Models.AldebaranContext.Itempedido itempedido)
        {
            OnItempedidoCreated(itempedido);

            var existingItem = Context.Itempedidos
                              .Where(i => i.IDITEMPEDIDO == itempedido.IDITEMPEDIDO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Itempedidos.Add(itempedido);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itempedido).State = EntityState.Detached;
                throw;
            }

            OnAfterItempedidoCreated(itempedido);

            return itempedido;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itempedido> CancelItempedidoChanges(Aldebaran.Web.Models.AldebaranContext.Itempedido item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItempedidoUpdated(Aldebaran.Web.Models.AldebaranContext.Itempedido item);
        partial void OnAfterItempedidoUpdated(Aldebaran.Web.Models.AldebaranContext.Itempedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itempedido> UpdateItempedido(int iditempedido, Aldebaran.Web.Models.AldebaranContext.Itempedido itempedido)
        {
            OnItempedidoUpdated(itempedido);

            var itemToUpdate = Context.Itempedidos
                              .Where(i => i.IDITEMPEDIDO == itempedido.IDITEMPEDIDO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(itempedido);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterItempedidoUpdated(itempedido);

            return itempedido;
        }

        partial void OnItempedidoDeleted(Aldebaran.Web.Models.AldebaranContext.Itempedido item);
        partial void OnAfterItempedidoDeleted(Aldebaran.Web.Models.AldebaranContext.Itempedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itempedido> DeleteItempedido(int iditempedido)
        {
            var itemToDelete = Context.Itempedidos
                              .Where(i => i.IDITEMPEDIDO == iditempedido)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnItempedidoDeleted(itemToDelete);


            Context.Itempedidos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterItempedidoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportItempedidoagotadosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itempedidoagotados/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itempedidoagotados/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportItempedidoagotadosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itempedidoagotados/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itempedidoagotados/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnItempedidoagotadosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado>> GetItempedidoagotados(Query query = null)
        {
            var items = Context.Itempedidoagotados.AsQueryable();

            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Linea);
            items = items.Include(i => i.Pedido);

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

            OnItempedidoagotadosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnItempedidoagotadoGet(Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado item);
        partial void OnGetItempedidoagotadoByIditempedido(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado> GetItempedidoagotadoByIditempedido(int iditempedido)
        {
            var items = Context.Itempedidoagotados
                              .AsNoTracking()
                              .Where(i => i.IDITEMPEDIDO == iditempedido);

            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Linea);
            items = items.Include(i => i.Pedido);

            OnGetItempedidoagotadoByIditempedido(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnItempedidoagotadoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnItempedidoagotadoCreated(Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado item);
        partial void OnAfterItempedidoagotadoCreated(Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado> CreateItempedidoagotado(Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado itempedidoagotado)
        {
            OnItempedidoagotadoCreated(itempedidoagotado);

            var existingItem = Context.Itempedidoagotados
                              .Where(i => i.IDITEMPEDIDO == itempedidoagotado.IDITEMPEDIDO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Itempedidoagotados.Add(itempedidoagotado);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itempedidoagotado).State = EntityState.Detached;
                throw;
            }

            OnAfterItempedidoagotadoCreated(itempedidoagotado);

            return itempedidoagotado;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado> CancelItempedidoagotadoChanges(Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItempedidoagotadoUpdated(Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado item);
        partial void OnAfterItempedidoagotadoUpdated(Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado> UpdateItempedidoagotado(int iditempedido, Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado itempedidoagotado)
        {
            OnItempedidoagotadoUpdated(itempedidoagotado);

            var itemToUpdate = Context.Itempedidoagotados
                              .Where(i => i.IDITEMPEDIDO == itempedidoagotado.IDITEMPEDIDO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(itempedidoagotado);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterItempedidoagotadoUpdated(itempedidoagotado);

            return itempedidoagotado;
        }

        partial void OnItempedidoagotadoDeleted(Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado item);
        partial void OnAfterItempedidoagotadoDeleted(Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado> DeleteItempedidoagotado(int iditempedido)
        {
            var itemToDelete = Context.Itempedidoagotados
                              .Where(i => i.IDITEMPEDIDO == iditempedido)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnItempedidoagotadoDeleted(itemToDelete);


            Context.Itempedidoagotados.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterItempedidoagotadoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportItemreservasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itemreservas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itemreservas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportItemreservasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itemreservas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itemreservas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnItemreservasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemreserva> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemreserva>> GetItemreservas(Query query = null)
        {
            var items = Context.Itemreservas.AsQueryable();

            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Linea);
            items = items.Include(i => i.Reserva);

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

            OnItemreservasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnItemreservaGet(Aldebaran.Web.Models.AldebaranContext.Itemreserva item);
        partial void OnGetItemreservaByIddetreserva(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemreserva> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemreserva> GetItemreservaByIddetreserva(int iddetreserva)
        {
            var items = Context.Itemreservas
                              .AsNoTracking()
                              .Where(i => i.IDDETRESERVA == iddetreserva);

            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Linea);
            items = items.Include(i => i.Reserva);

            OnGetItemreservaByIddetreserva(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnItemreservaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnItemreservaCreated(Aldebaran.Web.Models.AldebaranContext.Itemreserva item);
        partial void OnAfterItemreservaCreated(Aldebaran.Web.Models.AldebaranContext.Itemreserva item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemreserva> CreateItemreserva(Aldebaran.Web.Models.AldebaranContext.Itemreserva itemreserva)
        {
            OnItemreservaCreated(itemreserva);

            var existingItem = Context.Itemreservas
                              .Where(i => i.IDDETRESERVA == itemreserva.IDDETRESERVA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Itemreservas.Add(itemreserva);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemreserva).State = EntityState.Detached;
                throw;
            }

            OnAfterItemreservaCreated(itemreserva);

            return itemreserva;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemreserva> CancelItemreservaChanges(Aldebaran.Web.Models.AldebaranContext.Itemreserva item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItemreservaUpdated(Aldebaran.Web.Models.AldebaranContext.Itemreserva item);
        partial void OnAfterItemreservaUpdated(Aldebaran.Web.Models.AldebaranContext.Itemreserva item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemreserva> UpdateItemreserva(int iddetreserva, Aldebaran.Web.Models.AldebaranContext.Itemreserva itemreserva)
        {
            OnItemreservaUpdated(itemreserva);

            var itemToUpdate = Context.Itemreservas
                              .Where(i => i.IDDETRESERVA == itemreserva.IDDETRESERVA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(itemreserva);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterItemreservaUpdated(itemreserva);

            return itemreserva;
        }

        partial void OnItemreservaDeleted(Aldebaran.Web.Models.AldebaranContext.Itemreserva item);
        partial void OnAfterItemreservaDeleted(Aldebaran.Web.Models.AldebaranContext.Itemreserva item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemreserva> DeleteItemreserva(int iddetreserva)
        {
            var itemToDelete = Context.Itemreservas
                              .Where(i => i.IDDETRESERVA == iddetreserva)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnItemreservaDeleted(itemToDelete);


            Context.Itemreservas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterItemreservaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportItemsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/items/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/items/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportItemsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/items/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/items/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnItemsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Item> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Item>> GetItems(Query query = null)
        {
            var items = Context.Items.AsQueryable();

            items = items.Include(i => i.Linea);
            items = items.Include(i => i.Moneda);
            items = items.Include(i => i.Unidadesmedidum);
            items = items.Include(i => i.Unidadesmedidum1);

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

        partial void OnItemGet(Aldebaran.Web.Models.AldebaranContext.Item item);
        partial void OnGetItemByIditem(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Item> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Item> GetItemByIditem(int iditem)
        {
            var items = Context.Items
                              .AsNoTracking()
                              .Where(i => i.IDITEM == iditem);

            items = items.Include(i => i.Linea);
            items = items.Include(i => i.Moneda);
            items = items.Include(i => i.Unidadesmedidum);
            items = items.Include(i => i.Unidadesmedidum1);

            OnGetItemByIditem(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnItemGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnItemCreated(Aldebaran.Web.Models.AldebaranContext.Item item);
        partial void OnAfterItemCreated(Aldebaran.Web.Models.AldebaranContext.Item item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Item> CreateItem(Aldebaran.Web.Models.AldebaranContext.Item item)
        {
            OnItemCreated(item);

            var existingItem = Context.Items
                              .Where(i => i.IDITEM == item.IDITEM)
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

        public async Task<Aldebaran.Web.Models.AldebaranContext.Item> CancelItemChanges(Aldebaran.Web.Models.AldebaranContext.Item item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItemUpdated(Aldebaran.Web.Models.AldebaranContext.Item item);
        partial void OnAfterItemUpdated(Aldebaran.Web.Models.AldebaranContext.Item item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Item> UpdateItem(int iditem, Aldebaran.Web.Models.AldebaranContext.Item item)
        {
            OnItemUpdated(item);

            var itemToUpdate = Context.Items
                              .Where(i => i.IDITEM == item.IDITEM)
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

        partial void OnItemDeleted(Aldebaran.Web.Models.AldebaranContext.Item item);
        partial void OnAfterItemDeleted(Aldebaran.Web.Models.AldebaranContext.Item item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Item> DeleteItem(int iditem)
        {
            var itemToDelete = Context.Items
                              .Where(i => i.IDITEM == iditem)
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

        public async Task ExportItemsxareasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itemsxareas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itemsxareas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportItemsxareasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itemsxareas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itemsxareas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnItemsxareasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemsxarea> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemsxarea>> GetItemsxareas(Query query = null)
        {
            var items = Context.Itemsxareas.AsQueryable();

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

            OnItemsxareasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnItemsxareaGet(Aldebaran.Web.Models.AldebaranContext.Itemsxarea item);
        partial void OnGetItemsxareaByIditemAndIdarea(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemsxarea> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxarea> GetItemsxareaByIditemAndIdarea(int iditem, int idarea)
        {
            var items = Context.Itemsxareas
                              .AsNoTracking()
                              .Where(i => i.IDITEM == iditem && i.IDAREA == idarea);

            items = items.Include(i => i.Area);
            items = items.Include(i => i.Item);

            OnGetItemsxareaByIditemAndIdarea(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnItemsxareaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnItemsxareaCreated(Aldebaran.Web.Models.AldebaranContext.Itemsxarea item);
        partial void OnAfterItemsxareaCreated(Aldebaran.Web.Models.AldebaranContext.Itemsxarea item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxarea> CreateItemsxarea(Aldebaran.Web.Models.AldebaranContext.Itemsxarea itemsxarea)
        {
            OnItemsxareaCreated(itemsxarea);

            var existingItem = Context.Itemsxareas
                              .Where(i => i.IDITEM == itemsxarea.IDITEM && i.IDAREA == itemsxarea.IDAREA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Itemsxareas.Add(itemsxarea);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemsxarea).State = EntityState.Detached;
                throw;
            }

            OnAfterItemsxareaCreated(itemsxarea);

            return itemsxarea;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxarea> CancelItemsxareaChanges(Aldebaran.Web.Models.AldebaranContext.Itemsxarea item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItemsxareaUpdated(Aldebaran.Web.Models.AldebaranContext.Itemsxarea item);
        partial void OnAfterItemsxareaUpdated(Aldebaran.Web.Models.AldebaranContext.Itemsxarea item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxarea> UpdateItemsxarea(int iditem, int idarea, Aldebaran.Web.Models.AldebaranContext.Itemsxarea itemsxarea)
        {
            OnItemsxareaUpdated(itemsxarea);

            var itemToUpdate = Context.Itemsxareas
                              .Where(i => i.IDITEM == itemsxarea.IDITEM && i.IDAREA == itemsxarea.IDAREA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(itemsxarea);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterItemsxareaUpdated(itemsxarea);

            return itemsxarea;
        }

        partial void OnItemsxareaDeleted(Aldebaran.Web.Models.AldebaranContext.Itemsxarea item);
        partial void OnAfterItemsxareaDeleted(Aldebaran.Web.Models.AldebaranContext.Itemsxarea item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxarea> DeleteItemsxarea(int iditem, int idarea)
        {
            var itemToDelete = Context.Itemsxareas
                              .Where(i => i.IDITEM == iditem && i.IDAREA == idarea)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnItemsxareaDeleted(itemToDelete);


            Context.Itemsxareas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterItemsxareaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportItemsxbodegasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itemsxbodegas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itemsxbodegas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportItemsxbodegasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itemsxbodegas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itemsxbodegas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnItemsxbodegasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemsxbodega> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemsxbodega>> GetItemsxbodegas(Query query = null)
        {
            var items = Context.Itemsxbodegas.AsQueryable();

            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Itemsxcolor);

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

            OnItemsxbodegasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnItemsxbodegaGet(Aldebaran.Web.Models.AldebaranContext.Itemsxbodega item);
        partial void OnGetItemsxbodegaByIditemxcolorAndIdbodega(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemsxbodega> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxbodega> GetItemsxbodegaByIditemxcolorAndIdbodega(int iditemxcolor, int idbodega)
        {
            var items = Context.Itemsxbodegas
                              .AsNoTracking()
                              .Where(i => i.IDITEMXCOLOR == iditemxcolor && i.IDBODEGA == idbodega);

            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Itemsxcolor);

            OnGetItemsxbodegaByIditemxcolorAndIdbodega(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnItemsxbodegaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnItemsxbodegaCreated(Aldebaran.Web.Models.AldebaranContext.Itemsxbodega item);
        partial void OnAfterItemsxbodegaCreated(Aldebaran.Web.Models.AldebaranContext.Itemsxbodega item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxbodega> CreateItemsxbodega(Aldebaran.Web.Models.AldebaranContext.Itemsxbodega itemsxbodega)
        {
            OnItemsxbodegaCreated(itemsxbodega);

            var existingItem = Context.Itemsxbodegas
                              .Where(i => i.IDITEMXCOLOR == itemsxbodega.IDITEMXCOLOR && i.IDBODEGA == itemsxbodega.IDBODEGA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Itemsxbodegas.Add(itemsxbodega);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemsxbodega).State = EntityState.Detached;
                throw;
            }

            OnAfterItemsxbodegaCreated(itemsxbodega);

            return itemsxbodega;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxbodega> CancelItemsxbodegaChanges(Aldebaran.Web.Models.AldebaranContext.Itemsxbodega item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItemsxbodegaUpdated(Aldebaran.Web.Models.AldebaranContext.Itemsxbodega item);
        partial void OnAfterItemsxbodegaUpdated(Aldebaran.Web.Models.AldebaranContext.Itemsxbodega item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxbodega> UpdateItemsxbodega(int iditemxcolor, int idbodega, Aldebaran.Web.Models.AldebaranContext.Itemsxbodega itemsxbodega)
        {
            OnItemsxbodegaUpdated(itemsxbodega);

            var itemToUpdate = Context.Itemsxbodegas
                              .Where(i => i.IDITEMXCOLOR == itemsxbodega.IDITEMXCOLOR && i.IDBODEGA == itemsxbodega.IDBODEGA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(itemsxbodega);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterItemsxbodegaUpdated(itemsxbodega);

            return itemsxbodega;
        }

        partial void OnItemsxbodegaDeleted(Aldebaran.Web.Models.AldebaranContext.Itemsxbodega item);
        partial void OnAfterItemsxbodegaDeleted(Aldebaran.Web.Models.AldebaranContext.Itemsxbodega item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxbodega> DeleteItemsxbodega(int iditemxcolor, int idbodega)
        {
            var itemToDelete = Context.Itemsxbodegas
                              .Where(i => i.IDITEMXCOLOR == iditemxcolor && i.IDBODEGA == idbodega)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnItemsxbodegaDeleted(itemToDelete);


            Context.Itemsxbodegas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterItemsxbodegaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportItemsxcolorsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itemsxcolors/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itemsxcolors/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportItemsxcolorsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itemsxcolors/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itemsxcolors/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnItemsxcolorsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemsxcolor> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemsxcolor>> GetItemsxcolors(Query query = null)
        {
            var items = Context.Itemsxcolors.AsQueryable();

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

            OnItemsxcolorsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnItemsxcolorGet(Aldebaran.Web.Models.AldebaranContext.Itemsxcolor item);
        partial void OnGetItemsxcolorByIditemxcolor(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemsxcolor> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxcolor> GetItemsxcolorByIditemxcolor(int iditemxcolor)
        {
            var items = Context.Itemsxcolors
                              .AsNoTracking()
                              .Where(i => i.IDITEMXCOLOR == iditemxcolor);

            items = items.Include(i => i.Item);

            OnGetItemsxcolorByIditemxcolor(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnItemsxcolorGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnItemsxcolorCreated(Aldebaran.Web.Models.AldebaranContext.Itemsxcolor item);
        partial void OnAfterItemsxcolorCreated(Aldebaran.Web.Models.AldebaranContext.Itemsxcolor item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxcolor> CreateItemsxcolor(Aldebaran.Web.Models.AldebaranContext.Itemsxcolor itemsxcolor)
        {
            OnItemsxcolorCreated(itemsxcolor);

            var existingItem = Context.Itemsxcolors
                              .Where(i => i.IDITEMXCOLOR == itemsxcolor.IDITEMXCOLOR)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Itemsxcolors.Add(itemsxcolor);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemsxcolor).State = EntityState.Detached;
                throw;
            }

            OnAfterItemsxcolorCreated(itemsxcolor);

            return itemsxcolor;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxcolor> CancelItemsxcolorChanges(Aldebaran.Web.Models.AldebaranContext.Itemsxcolor item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItemsxcolorUpdated(Aldebaran.Web.Models.AldebaranContext.Itemsxcolor item);
        partial void OnAfterItemsxcolorUpdated(Aldebaran.Web.Models.AldebaranContext.Itemsxcolor item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxcolor> UpdateItemsxcolor(int iditemxcolor, Aldebaran.Web.Models.AldebaranContext.Itemsxcolor itemsxcolor)
        {
            OnItemsxcolorUpdated(itemsxcolor);

            var itemToUpdate = Context.Itemsxcolors
                              .Where(i => i.IDITEMXCOLOR == itemsxcolor.IDITEMXCOLOR)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(itemsxcolor);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterItemsxcolorUpdated(itemsxcolor);

            return itemsxcolor;
        }

        partial void OnItemsxcolorDeleted(Aldebaran.Web.Models.AldebaranContext.Itemsxcolor item);
        partial void OnAfterItemsxcolorDeleted(Aldebaran.Web.Models.AldebaranContext.Itemsxcolor item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxcolor> DeleteItemsxcolor(int iditemxcolor)
        {
            var itemToDelete = Context.Itemsxcolors
                              .Where(i => i.IDITEMXCOLOR == iditemxcolor)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnItemsxcolorDeleted(itemToDelete);


            Context.Itemsxcolors.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterItemsxcolorDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportItemsxproveedorsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itemsxproveedors/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itemsxproveedors/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportItemsxproveedorsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itemsxproveedors/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itemsxproveedors/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnItemsxproveedorsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor>> GetItemsxproveedors(Query query = null)
        {
            var items = Context.Itemsxproveedors.AsQueryable();

            items = items.Include(i => i.Item);
            items = items.Include(i => i.Proveedore);

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

            OnItemsxproveedorsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnItemsxproveedorGet(Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor item);
        partial void OnGetItemsxproveedorByIditemAndIdproveedor(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor> GetItemsxproveedorByIditemAndIdproveedor(int iditem, int idproveedor)
        {
            var items = Context.Itemsxproveedors
                              .AsNoTracking()
                              .Where(i => i.IDITEM == iditem && i.IDPROVEEDOR == idproveedor);

            items = items.Include(i => i.Item);
            items = items.Include(i => i.Proveedore);

            OnGetItemsxproveedorByIditemAndIdproveedor(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnItemsxproveedorGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnItemsxproveedorCreated(Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor item);
        partial void OnAfterItemsxproveedorCreated(Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor> CreateItemsxproveedor(Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor itemsxproveedor)
        {
            OnItemsxproveedorCreated(itemsxproveedor);

            var existingItem = Context.Itemsxproveedors
                              .Where(i => i.IDITEM == itemsxproveedor.IDITEM && i.IDPROVEEDOR == itemsxproveedor.IDPROVEEDOR)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Itemsxproveedors.Add(itemsxproveedor);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemsxproveedor).State = EntityState.Detached;
                throw;
            }

            OnAfterItemsxproveedorCreated(itemsxproveedor);

            return itemsxproveedor;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor> CancelItemsxproveedorChanges(Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItemsxproveedorUpdated(Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor item);
        partial void OnAfterItemsxproveedorUpdated(Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor> UpdateItemsxproveedor(int iditem, int idproveedor, Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor itemsxproveedor)
        {
            OnItemsxproveedorUpdated(itemsxproveedor);

            var itemToUpdate = Context.Itemsxproveedors
                              .Where(i => i.IDITEM == itemsxproveedor.IDITEM && i.IDPROVEEDOR == itemsxproveedor.IDPROVEEDOR)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(itemsxproveedor);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterItemsxproveedorUpdated(itemsxproveedor);

            return itemsxproveedor;
        }

        partial void OnItemsxproveedorDeleted(Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor item);
        partial void OnAfterItemsxproveedorDeleted(Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor> DeleteItemsxproveedor(int iditem, int idproveedor)
        {
            var itemToDelete = Context.Itemsxproveedors
                              .Where(i => i.IDITEM == iditem && i.IDPROVEEDOR == idproveedor)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnItemsxproveedorDeleted(itemToDelete);


            Context.Itemsxproveedors.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterItemsxproveedorDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportItemsxtrasladosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itemsxtraslados/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itemsxtraslados/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportItemsxtrasladosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itemsxtraslados/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itemsxtraslados/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnItemsxtrasladosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado>> GetItemsxtraslados(Query query = null)
        {
            var items = Context.Itemsxtraslados.AsQueryable();

            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Linea);
            items = items.Include(i => i.Traslado);

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

            OnItemsxtrasladosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnItemsxtrasladoGet(Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado item);
        partial void OnGetItemsxtrasladoByIditemsxtraslado(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado> GetItemsxtrasladoByIditemsxtraslado(int iditemsxtraslado)
        {
            var items = Context.Itemsxtraslados
                              .AsNoTracking()
                              .Where(i => i.IDITEMSXTRASLADO == iditemsxtraslado);

            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Linea);
            items = items.Include(i => i.Traslado);

            OnGetItemsxtrasladoByIditemsxtraslado(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnItemsxtrasladoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnItemsxtrasladoCreated(Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado item);
        partial void OnAfterItemsxtrasladoCreated(Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado> CreateItemsxtraslado(Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado itemsxtraslado)
        {
            OnItemsxtrasladoCreated(itemsxtraslado);

            var existingItem = Context.Itemsxtraslados
                              .Where(i => i.IDITEMSXTRASLADO == itemsxtraslado.IDITEMSXTRASLADO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Itemsxtraslados.Add(itemsxtraslado);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemsxtraslado).State = EntityState.Detached;
                throw;
            }

            OnAfterItemsxtrasladoCreated(itemsxtraslado);

            return itemsxtraslado;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado> CancelItemsxtrasladoChanges(Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItemsxtrasladoUpdated(Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado item);
        partial void OnAfterItemsxtrasladoUpdated(Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado> UpdateItemsxtraslado(int iditemsxtraslado, Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado itemsxtraslado)
        {
            OnItemsxtrasladoUpdated(itemsxtraslado);

            var itemToUpdate = Context.Itemsxtraslados
                              .Where(i => i.IDITEMSXTRASLADO == itemsxtraslado.IDITEMSXTRASLADO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(itemsxtraslado);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterItemsxtrasladoUpdated(itemsxtraslado);

            return itemsxtraslado;
        }

        partial void OnItemsxtrasladoDeleted(Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado item);
        partial void OnAfterItemsxtrasladoDeleted(Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado> DeleteItemsxtraslado(int iditemsxtraslado)
        {
            var itemToDelete = Context.Itemsxtraslados
                              .Where(i => i.IDITEMSXTRASLADO == iditemsxtraslado)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnItemsxtrasladoDeleted(itemToDelete);


            Context.Itemsxtraslados.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterItemsxtrasladoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportItemxitemsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itemxitems/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itemxitems/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportItemxitemsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/itemxitems/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/itemxitems/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnItemxitemsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemxitem> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemxitem>> GetItemxitems(Query query = null)
        {
            var items = Context.Itemxitems.AsQueryable();

            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Itemsxcolor1);

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

            OnItemxitemsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnItemxitemGet(Aldebaran.Web.Models.AldebaranContext.Itemxitem item);
        partial void OnGetItemxitemByIditemxitem(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Itemxitem> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemxitem> GetItemxitemByIditemxitem(int iditemxitem)
        {
            var items = Context.Itemxitems
                              .AsNoTracking()
                              .Where(i => i.IDITEMXITEM == iditemxitem);

            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Itemsxcolor1);

            OnGetItemxitemByIditemxitem(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnItemxitemGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnItemxitemCreated(Aldebaran.Web.Models.AldebaranContext.Itemxitem item);
        partial void OnAfterItemxitemCreated(Aldebaran.Web.Models.AldebaranContext.Itemxitem item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemxitem> CreateItemxitem(Aldebaran.Web.Models.AldebaranContext.Itemxitem itemxitem)
        {
            OnItemxitemCreated(itemxitem);

            var existingItem = Context.Itemxitems
                              .Where(i => i.IDITEMXITEM == itemxitem.IDITEMXITEM)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Itemxitems.Add(itemxitem);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemxitem).State = EntityState.Detached;
                throw;
            }

            OnAfterItemxitemCreated(itemxitem);

            return itemxitem;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemxitem> CancelItemxitemChanges(Aldebaran.Web.Models.AldebaranContext.Itemxitem item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItemxitemUpdated(Aldebaran.Web.Models.AldebaranContext.Itemxitem item);
        partial void OnAfterItemxitemUpdated(Aldebaran.Web.Models.AldebaranContext.Itemxitem item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemxitem> UpdateItemxitem(int iditemxitem, Aldebaran.Web.Models.AldebaranContext.Itemxitem itemxitem)
        {
            OnItemxitemUpdated(itemxitem);

            var itemToUpdate = Context.Itemxitems
                              .Where(i => i.IDITEMXITEM == itemxitem.IDITEMXITEM)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(itemxitem);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterItemxitemUpdated(itemxitem);

            return itemxitem;
        }

        partial void OnItemxitemDeleted(Aldebaran.Web.Models.AldebaranContext.Itemxitem item);
        partial void OnAfterItemxitemDeleted(Aldebaran.Web.Models.AldebaranContext.Itemxitem item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Itemxitem> DeleteItemxitem(int iditemxitem)
        {
            var itemToDelete = Context.Itemxitems
                              .Where(i => i.IDITEMXITEM == iditemxitem)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnItemxitemDeleted(itemToDelete);


            Context.Itemxitems.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterItemxitemDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportLineasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/lineas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/lineas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportLineasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/lineas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/lineas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnLineasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Linea> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Linea>> GetLineas(Query query = null)
        {
            var items = Context.Lineas.AsQueryable();


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

            OnLineasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnLineaGet(Aldebaran.Web.Models.AldebaranContext.Linea item);
        partial void OnGetLineaByIdlinea(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Linea> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Linea> GetLineaByIdlinea(int idlinea)
        {
            var items = Context.Lineas
                              .AsNoTracking()
                              .Where(i => i.IDLINEA == idlinea);


            OnGetLineaByIdlinea(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnLineaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnLineaCreated(Aldebaran.Web.Models.AldebaranContext.Linea item);
        partial void OnAfterLineaCreated(Aldebaran.Web.Models.AldebaranContext.Linea item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Linea> CreateLinea(Aldebaran.Web.Models.AldebaranContext.Linea linea)
        {
            OnLineaCreated(linea);

            var existingItem = Context.Lineas
                              .Where(i => i.IDLINEA == linea.IDLINEA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Lineas.Add(linea);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(linea).State = EntityState.Detached;
                throw;
            }

            OnAfterLineaCreated(linea);

            return linea;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Linea> CancelLineaChanges(Aldebaran.Web.Models.AldebaranContext.Linea item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnLineaUpdated(Aldebaran.Web.Models.AldebaranContext.Linea item);
        partial void OnAfterLineaUpdated(Aldebaran.Web.Models.AldebaranContext.Linea item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Linea> UpdateLinea(int idlinea, Aldebaran.Web.Models.AldebaranContext.Linea linea)
        {
            OnLineaUpdated(linea);

            var itemToUpdate = Context.Lineas
                              .Where(i => i.IDLINEA == linea.IDLINEA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(linea);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterLineaUpdated(linea);

            return linea;
        }

        partial void OnLineaDeleted(Aldebaran.Web.Models.AldebaranContext.Linea item);
        partial void OnAfterLineaDeleted(Aldebaran.Web.Models.AldebaranContext.Linea item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Linea> DeleteLinea(int idlinea)
        {
            var itemToDelete = Context.Lineas
                              .Where(i => i.IDLINEA == idlinea)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnLineaDeleted(itemToDelete);


            Context.Lineas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterLineaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportLogalarmascantidadesminimasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/logalarmascantidadesminimas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/logalarmascantidadesminimas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportLogalarmascantidadesminimasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/logalarmascantidadesminimas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/logalarmascantidadesminimas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnLogalarmascantidadesminimasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima>> GetLogalarmascantidadesminimas(Query query = null)
        {
            var items = Context.Logalarmascantidadesminimas.AsQueryable();

            items = items.Include(i => i.Funcionario);

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

            OnLogalarmascantidadesminimasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnLogalarmascantidadesminimaGet(Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima item);
        partial void OnGetLogalarmascantidadesminimaByIdalarmaAndIdfuncionario(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima> GetLogalarmascantidadesminimaByIdalarmaAndIdfuncionario(int idalarma, int idfuncionario)
        {
            var items = Context.Logalarmascantidadesminimas
                              .AsNoTracking()
                              .Where(i => i.IDALARMA == idalarma && i.IDFUNCIONARIO == idfuncionario);

            items = items.Include(i => i.Funcionario);

            OnGetLogalarmascantidadesminimaByIdalarmaAndIdfuncionario(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnLogalarmascantidadesminimaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnLogalarmascantidadesminimaCreated(Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima item);
        partial void OnAfterLogalarmascantidadesminimaCreated(Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima> CreateLogalarmascantidadesminima(Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima logalarmascantidadesminima)
        {
            OnLogalarmascantidadesminimaCreated(logalarmascantidadesminima);

            var existingItem = Context.Logalarmascantidadesminimas
                              .Where(i => i.IDALARMA == logalarmascantidadesminima.IDALARMA && i.IDFUNCIONARIO == logalarmascantidadesminima.IDFUNCIONARIO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Logalarmascantidadesminimas.Add(logalarmascantidadesminima);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(logalarmascantidadesminima).State = EntityState.Detached;
                throw;
            }

            OnAfterLogalarmascantidadesminimaCreated(logalarmascantidadesminima);

            return logalarmascantidadesminima;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima> CancelLogalarmascantidadesminimaChanges(Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnLogalarmascantidadesminimaUpdated(Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima item);
        partial void OnAfterLogalarmascantidadesminimaUpdated(Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima> UpdateLogalarmascantidadesminima(int idalarma, int idfuncionario, Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima logalarmascantidadesminima)
        {
            OnLogalarmascantidadesminimaUpdated(logalarmascantidadesminima);

            var itemToUpdate = Context.Logalarmascantidadesminimas
                              .Where(i => i.IDALARMA == logalarmascantidadesminima.IDALARMA && i.IDFUNCIONARIO == logalarmascantidadesminima.IDFUNCIONARIO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(logalarmascantidadesminima);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterLogalarmascantidadesminimaUpdated(logalarmascantidadesminima);

            return logalarmascantidadesminima;
        }

        partial void OnLogalarmascantidadesminimaDeleted(Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima item);
        partial void OnAfterLogalarmascantidadesminimaDeleted(Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima> DeleteLogalarmascantidadesminima(int idalarma, int idfuncionario)
        {
            var itemToDelete = Context.Logalarmascantidadesminimas
                              .Where(i => i.IDALARMA == idalarma && i.IDFUNCIONARIO == idfuncionario)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnLogalarmascantidadesminimaDeleted(itemToDelete);


            Context.Logalarmascantidadesminimas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterLogalarmascantidadesminimaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportMensajesalarmasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/mensajesalarmas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/mensajesalarmas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportMensajesalarmasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/mensajesalarmas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/mensajesalarmas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnMensajesalarmasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Mensajesalarma> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Mensajesalarma>> GetMensajesalarmas(Query query = null)
        {
            var items = Context.Mensajesalarmas.AsQueryable();


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

            OnMensajesalarmasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnMensajesalarmaGet(Aldebaran.Web.Models.AldebaranContext.Mensajesalarma item);
        partial void OnGetMensajesalarmaByIdmensajeAndIdtipoalarma(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Mensajesalarma> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Mensajesalarma> GetMensajesalarmaByIdmensajeAndIdtipoalarma(int idmensaje, int idtipoalarma)
        {
            var items = Context.Mensajesalarmas
                              .AsNoTracking()
                              .Where(i => i.IDMENSAJE == idmensaje && i.IDTIPOALARMA == idtipoalarma);


            OnGetMensajesalarmaByIdmensajeAndIdtipoalarma(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnMensajesalarmaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnMensajesalarmaCreated(Aldebaran.Web.Models.AldebaranContext.Mensajesalarma item);
        partial void OnAfterMensajesalarmaCreated(Aldebaran.Web.Models.AldebaranContext.Mensajesalarma item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Mensajesalarma> CreateMensajesalarma(Aldebaran.Web.Models.AldebaranContext.Mensajesalarma mensajesalarma)
        {
            OnMensajesalarmaCreated(mensajesalarma);

            var existingItem = Context.Mensajesalarmas
                              .Where(i => i.IDMENSAJE == mensajesalarma.IDMENSAJE && i.IDTIPOALARMA == mensajesalarma.IDTIPOALARMA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Mensajesalarmas.Add(mensajesalarma);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(mensajesalarma).State = EntityState.Detached;
                throw;
            }

            OnAfterMensajesalarmaCreated(mensajesalarma);

            return mensajesalarma;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Mensajesalarma> CancelMensajesalarmaChanges(Aldebaran.Web.Models.AldebaranContext.Mensajesalarma item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnMensajesalarmaUpdated(Aldebaran.Web.Models.AldebaranContext.Mensajesalarma item);
        partial void OnAfterMensajesalarmaUpdated(Aldebaran.Web.Models.AldebaranContext.Mensajesalarma item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Mensajesalarma> UpdateMensajesalarma(int idmensaje, int idtipoalarma, Aldebaran.Web.Models.AldebaranContext.Mensajesalarma mensajesalarma)
        {
            OnMensajesalarmaUpdated(mensajesalarma);

            var itemToUpdate = Context.Mensajesalarmas
                              .Where(i => i.IDMENSAJE == mensajesalarma.IDMENSAJE && i.IDTIPOALARMA == mensajesalarma.IDTIPOALARMA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(mensajesalarma);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterMensajesalarmaUpdated(mensajesalarma);

            return mensajesalarma;
        }

        partial void OnMensajesalarmaDeleted(Aldebaran.Web.Models.AldebaranContext.Mensajesalarma item);
        partial void OnAfterMensajesalarmaDeleted(Aldebaran.Web.Models.AldebaranContext.Mensajesalarma item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Mensajesalarma> DeleteMensajesalarma(int idmensaje, int idtipoalarma)
        {
            var itemToDelete = Context.Mensajesalarmas
                              .Where(i => i.IDMENSAJE == idmensaje && i.IDTIPOALARMA == idtipoalarma)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnMensajesalarmaDeleted(itemToDelete);


            Context.Mensajesalarmas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterMensajesalarmaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportMetodoembarquesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/metodoembarques/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/metodoembarques/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportMetodoembarquesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/metodoembarques/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/metodoembarques/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnMetodoembarquesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Metodoembarque> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Metodoembarque>> GetMetodoembarques(Query query = null)
        {
            var items = Context.Metodoembarques.AsQueryable();


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

            OnMetodoembarquesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnMetodoembarqueGet(Aldebaran.Web.Models.AldebaranContext.Metodoembarque item);
        partial void OnGetMetodoembarqueByIdmetembarque(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Metodoembarque> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Metodoembarque> GetMetodoembarqueByIdmetembarque(int idmetembarque)
        {
            var items = Context.Metodoembarques
                              .AsNoTracking()
                              .Where(i => i.IDMETEMBARQUE == idmetembarque);


            OnGetMetodoembarqueByIdmetembarque(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnMetodoembarqueGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnMetodoembarqueCreated(Aldebaran.Web.Models.AldebaranContext.Metodoembarque item);
        partial void OnAfterMetodoembarqueCreated(Aldebaran.Web.Models.AldebaranContext.Metodoembarque item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Metodoembarque> CreateMetodoembarque(Aldebaran.Web.Models.AldebaranContext.Metodoembarque metodoembarque)
        {
            OnMetodoembarqueCreated(metodoembarque);

            var existingItem = Context.Metodoembarques
                              .Where(i => i.IDMETEMBARQUE == metodoembarque.IDMETEMBARQUE)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Metodoembarques.Add(metodoembarque);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(metodoembarque).State = EntityState.Detached;
                throw;
            }

            OnAfterMetodoembarqueCreated(metodoembarque);

            return metodoembarque;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Metodoembarque> CancelMetodoembarqueChanges(Aldebaran.Web.Models.AldebaranContext.Metodoembarque item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnMetodoembarqueUpdated(Aldebaran.Web.Models.AldebaranContext.Metodoembarque item);
        partial void OnAfterMetodoembarqueUpdated(Aldebaran.Web.Models.AldebaranContext.Metodoembarque item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Metodoembarque> UpdateMetodoembarque(int idmetembarque, Aldebaran.Web.Models.AldebaranContext.Metodoembarque metodoembarque)
        {
            OnMetodoembarqueUpdated(metodoembarque);

            var itemToUpdate = Context.Metodoembarques
                              .Where(i => i.IDMETEMBARQUE == metodoembarque.IDMETEMBARQUE)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(metodoembarque);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterMetodoembarqueUpdated(metodoembarque);

            return metodoembarque;
        }

        partial void OnMetodoembarqueDeleted(Aldebaran.Web.Models.AldebaranContext.Metodoembarque item);
        partial void OnAfterMetodoembarqueDeleted(Aldebaran.Web.Models.AldebaranContext.Metodoembarque item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Metodoembarque> DeleteMetodoembarque(int idmetembarque)
        {
            var itemToDelete = Context.Metodoembarques
                              .Where(i => i.IDMETEMBARQUE == idmetembarque)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnMetodoembarqueDeleted(itemToDelete);


            Context.Metodoembarques.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterMetodoembarqueDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportMetodosenviosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/metodosenvios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/metodosenvios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportMetodosenviosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/metodosenvios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/metodosenvios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnMetodosenviosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Metodosenvio> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Metodosenvio>> GetMetodosenvios(Query query = null)
        {
            var items = Context.Metodosenvios.AsQueryable();


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

            OnMetodosenviosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnMetodosenvioGet(Aldebaran.Web.Models.AldebaranContext.Metodosenvio item);
        partial void OnGetMetodosenvioByIdmetodoenv(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Metodosenvio> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Metodosenvio> GetMetodosenvioByIdmetodoenv(int idmetodoenv)
        {
            var items = Context.Metodosenvios
                              .AsNoTracking()
                              .Where(i => i.IDMETODOENV == idmetodoenv);


            OnGetMetodosenvioByIdmetodoenv(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnMetodosenvioGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnMetodosenvioCreated(Aldebaran.Web.Models.AldebaranContext.Metodosenvio item);
        partial void OnAfterMetodosenvioCreated(Aldebaran.Web.Models.AldebaranContext.Metodosenvio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Metodosenvio> CreateMetodosenvio(Aldebaran.Web.Models.AldebaranContext.Metodosenvio metodosenvio)
        {
            OnMetodosenvioCreated(metodosenvio);

            var existingItem = Context.Metodosenvios
                              .Where(i => i.IDMETODOENV == metodosenvio.IDMETODOENV)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Metodosenvios.Add(metodosenvio);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(metodosenvio).State = EntityState.Detached;
                throw;
            }

            OnAfterMetodosenvioCreated(metodosenvio);

            return metodosenvio;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Metodosenvio> CancelMetodosenvioChanges(Aldebaran.Web.Models.AldebaranContext.Metodosenvio item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnMetodosenvioUpdated(Aldebaran.Web.Models.AldebaranContext.Metodosenvio item);
        partial void OnAfterMetodosenvioUpdated(Aldebaran.Web.Models.AldebaranContext.Metodosenvio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Metodosenvio> UpdateMetodosenvio(int idmetodoenv, Aldebaran.Web.Models.AldebaranContext.Metodosenvio metodosenvio)
        {
            OnMetodosenvioUpdated(metodosenvio);

            var itemToUpdate = Context.Metodosenvios
                              .Where(i => i.IDMETODOENV == metodosenvio.IDMETODOENV)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(metodosenvio);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterMetodosenvioUpdated(metodosenvio);

            return metodosenvio;
        }

        partial void OnMetodosenvioDeleted(Aldebaran.Web.Models.AldebaranContext.Metodosenvio item);
        partial void OnAfterMetodosenvioDeleted(Aldebaran.Web.Models.AldebaranContext.Metodosenvio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Metodosenvio> DeleteMetodosenvio(int idmetodoenv)
        {
            var itemToDelete = Context.Metodosenvios
                              .Where(i => i.IDMETODOENV == idmetodoenv)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnMetodosenvioDeleted(itemToDelete);


            Context.Metodosenvios.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterMetodosenvioDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportModordenesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/modordenes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/modordenes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportModordenesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/modordenes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/modordenes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnModordenesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Modordene> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Modordene>> GetModordenes(Query query = null)
        {
            var items = Context.Modordenes.AsQueryable();

            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Ordene);

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

            OnModordenesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnModordeneGet(Aldebaran.Web.Models.AldebaranContext.Modordene item);
        partial void OnGetModordeneByIdmodorden(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Modordene> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Modordene> GetModordeneByIdmodorden(int idmodorden)
        {
            var items = Context.Modordenes
                              .AsNoTracking()
                              .Where(i => i.IDMODORDEN == idmodorden);

            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Ordene);

            OnGetModordeneByIdmodorden(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnModordeneGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnModordeneCreated(Aldebaran.Web.Models.AldebaranContext.Modordene item);
        partial void OnAfterModordeneCreated(Aldebaran.Web.Models.AldebaranContext.Modordene item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Modordene> CreateModordene(Aldebaran.Web.Models.AldebaranContext.Modordene modordene)
        {
            OnModordeneCreated(modordene);

            var existingItem = Context.Modordenes
                              .Where(i => i.IDMODORDEN == modordene.IDMODORDEN)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Modordenes.Add(modordene);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(modordene).State = EntityState.Detached;
                throw;
            }

            OnAfterModordeneCreated(modordene);

            return modordene;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Modordene> CancelModordeneChanges(Aldebaran.Web.Models.AldebaranContext.Modordene item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnModordeneUpdated(Aldebaran.Web.Models.AldebaranContext.Modordene item);
        partial void OnAfterModordeneUpdated(Aldebaran.Web.Models.AldebaranContext.Modordene item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Modordene> UpdateModordene(int idmodorden, Aldebaran.Web.Models.AldebaranContext.Modordene modordene)
        {
            OnModordeneUpdated(modordene);

            var itemToUpdate = Context.Modordenes
                              .Where(i => i.IDMODORDEN == modordene.IDMODORDEN)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(modordene);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterModordeneUpdated(modordene);

            return modordene;
        }

        partial void OnModordeneDeleted(Aldebaran.Web.Models.AldebaranContext.Modordene item);
        partial void OnAfterModordeneDeleted(Aldebaran.Web.Models.AldebaranContext.Modordene item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Modordene> DeleteModordene(int idmodorden)
        {
            var itemToDelete = Context.Modordenes
                              .Where(i => i.IDMODORDEN == idmodorden)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnModordeneDeleted(itemToDelete);


            Context.Modordenes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterModordeneDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportModpedidosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/modpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/modpedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportModpedidosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/modpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/modpedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnModpedidosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Modpedido> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Modpedido>> GetModpedidos(Query query = null)
        {
            var items = Context.Modpedidos.AsQueryable();

            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Pedido);

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

            OnModpedidosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnModpedidoGet(Aldebaran.Web.Models.AldebaranContext.Modpedido item);
        partial void OnGetModpedidoByIdpedidoAndIdfuncionarioAndFecha(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Modpedido> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Modpedido> GetModpedidoByIdpedidoAndIdfuncionarioAndFecha(int idpedido, int idfuncionario, DateTime fecha)
        {
            var items = Context.Modpedidos
                              .AsNoTracking()
                              .Where(i => i.IDPEDIDO == idpedido && i.IDFUNCIONARIO == idfuncionario && i.FECHA == fecha);

            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Pedido);

            OnGetModpedidoByIdpedidoAndIdfuncionarioAndFecha(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnModpedidoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnModpedidoCreated(Aldebaran.Web.Models.AldebaranContext.Modpedido item);
        partial void OnAfterModpedidoCreated(Aldebaran.Web.Models.AldebaranContext.Modpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Modpedido> CreateModpedido(Aldebaran.Web.Models.AldebaranContext.Modpedido modpedido)
        {
            OnModpedidoCreated(modpedido);

            var existingItem = Context.Modpedidos
                              .Where(i => i.IDPEDIDO == modpedido.IDPEDIDO && i.IDFUNCIONARIO == modpedido.IDFUNCIONARIO && i.FECHA == modpedido.FECHA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Modpedidos.Add(modpedido);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(modpedido).State = EntityState.Detached;
                throw;
            }

            OnAfterModpedidoCreated(modpedido);

            return modpedido;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Modpedido> CancelModpedidoChanges(Aldebaran.Web.Models.AldebaranContext.Modpedido item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnModpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.Modpedido item);
        partial void OnAfterModpedidoUpdated(Aldebaran.Web.Models.AldebaranContext.Modpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Modpedido> UpdateModpedido(int idpedido, int idfuncionario, DateTime fecha, Aldebaran.Web.Models.AldebaranContext.Modpedido modpedido)
        {
            OnModpedidoUpdated(modpedido);

            var itemToUpdate = Context.Modpedidos
                              .Where(i => i.IDPEDIDO == modpedido.IDPEDIDO && i.IDFUNCIONARIO == modpedido.IDFUNCIONARIO && i.FECHA == modpedido.FECHA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(modpedido);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterModpedidoUpdated(modpedido);

            return modpedido;
        }

        partial void OnModpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.Modpedido item);
        partial void OnAfterModpedidoDeleted(Aldebaran.Web.Models.AldebaranContext.Modpedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Modpedido> DeleteModpedido(int idpedido, int idfuncionario, DateTime fecha)
        {
            var itemToDelete = Context.Modpedidos
                              .Where(i => i.IDPEDIDO == idpedido && i.IDFUNCIONARIO == idfuncionario && i.FECHA == fecha)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnModpedidoDeleted(itemToDelete);


            Context.Modpedidos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterModpedidoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportModreservasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/modreservas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/modreservas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportModreservasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/modreservas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/modreservas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnModreservasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Modreserva> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Modreserva>> GetModreservas(Query query = null)
        {
            var items = Context.Modreservas.AsQueryable();

            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Reserva);

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

            OnModreservasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnModreservaGet(Aldebaran.Web.Models.AldebaranContext.Modreserva item);
        partial void OnGetModreservaByIdreservaAndIdfuncionarioAndFecha(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Modreserva> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Modreserva> GetModreservaByIdreservaAndIdfuncionarioAndFecha(int idreserva, int idfuncionario, DateTime fecha)
        {
            var items = Context.Modreservas
                              .AsNoTracking()
                              .Where(i => i.IDRESERVA == idreserva && i.IDFUNCIONARIO == idfuncionario && i.FECHA == fecha);

            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Reserva);

            OnGetModreservaByIdreservaAndIdfuncionarioAndFecha(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnModreservaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnModreservaCreated(Aldebaran.Web.Models.AldebaranContext.Modreserva item);
        partial void OnAfterModreservaCreated(Aldebaran.Web.Models.AldebaranContext.Modreserva item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Modreserva> CreateModreserva(Aldebaran.Web.Models.AldebaranContext.Modreserva modreserva)
        {
            OnModreservaCreated(modreserva);

            var existingItem = Context.Modreservas
                              .Where(i => i.IDRESERVA == modreserva.IDRESERVA && i.IDFUNCIONARIO == modreserva.IDFUNCIONARIO && i.FECHA == modreserva.FECHA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Modreservas.Add(modreserva);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(modreserva).State = EntityState.Detached;
                throw;
            }

            OnAfterModreservaCreated(modreserva);

            return modreserva;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Modreserva> CancelModreservaChanges(Aldebaran.Web.Models.AldebaranContext.Modreserva item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnModreservaUpdated(Aldebaran.Web.Models.AldebaranContext.Modreserva item);
        partial void OnAfterModreservaUpdated(Aldebaran.Web.Models.AldebaranContext.Modreserva item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Modreserva> UpdateModreserva(int idreserva, int idfuncionario, DateTime fecha, Aldebaran.Web.Models.AldebaranContext.Modreserva modreserva)
        {
            OnModreservaUpdated(modreserva);

            var itemToUpdate = Context.Modreservas
                              .Where(i => i.IDRESERVA == modreserva.IDRESERVA && i.IDFUNCIONARIO == modreserva.IDFUNCIONARIO && i.FECHA == modreserva.FECHA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(modreserva);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterModreservaUpdated(modreserva);

            return modreserva;
        }

        partial void OnModreservaDeleted(Aldebaran.Web.Models.AldebaranContext.Modreserva item);
        partial void OnAfterModreservaDeleted(Aldebaran.Web.Models.AldebaranContext.Modreserva item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Modreserva> DeleteModreserva(int idreserva, int idfuncionario, DateTime fecha)
        {
            var itemToDelete = Context.Modreservas
                              .Where(i => i.IDRESERVA == idreserva && i.IDFUNCIONARIO == idfuncionario && i.FECHA == fecha)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnModreservaDeleted(itemToDelete);


            Context.Modreservas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterModreservaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportMonedaToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/moneda/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/moneda/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportMonedaToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/moneda/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/moneda/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnMonedaRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Moneda> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Moneda>> GetMoneda(Query query = null)
        {
            var items = Context.Moneda.AsQueryable();


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

            OnMonedaRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnMonedaGet(Aldebaran.Web.Models.AldebaranContext.Moneda item);
        partial void OnGetMonedaByIdmoneda(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Moneda> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Moneda> GetMonedaByIdmoneda(int idmoneda)
        {
            var items = Context.Moneda
                              .AsNoTracking()
                              .Where(i => i.IDMONEDA == idmoneda);


            OnGetMonedaByIdmoneda(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnMonedaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnMonedaCreated(Aldebaran.Web.Models.AldebaranContext.Moneda item);
        partial void OnAfterMonedaCreated(Aldebaran.Web.Models.AldebaranContext.Moneda item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Moneda> CreateMoneda(Aldebaran.Web.Models.AldebaranContext.Moneda moneda)
        {
            OnMonedaCreated(moneda);

            var existingItem = Context.Moneda
                              .Where(i => i.IDMONEDA == moneda.IDMONEDA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Moneda.Add(moneda);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(moneda).State = EntityState.Detached;
                throw;
            }

            OnAfterMonedaCreated(moneda);

            return moneda;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Moneda> CancelMonedaChanges(Aldebaran.Web.Models.AldebaranContext.Moneda item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnMonedaUpdated(Aldebaran.Web.Models.AldebaranContext.Moneda item);
        partial void OnAfterMonedaUpdated(Aldebaran.Web.Models.AldebaranContext.Moneda item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Moneda> UpdateMoneda(int idmoneda, Aldebaran.Web.Models.AldebaranContext.Moneda moneda)
        {
            OnMonedaUpdated(moneda);

            var itemToUpdate = Context.Moneda
                              .Where(i => i.IDMONEDA == moneda.IDMONEDA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(moneda);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterMonedaUpdated(moneda);

            return moneda;
        }

        partial void OnMonedaDeleted(Aldebaran.Web.Models.AldebaranContext.Moneda item);
        partial void OnAfterMonedaDeleted(Aldebaran.Web.Models.AldebaranContext.Moneda item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Moneda> DeleteMoneda(int idmoneda)
        {
            var itemToDelete = Context.Moneda
                              .Where(i => i.IDMONEDA == idmoneda)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnMonedaDeleted(itemToDelete);


            Context.Moneda.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterMonedaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportMotivajustesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/motivajustes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/motivajustes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportMotivajustesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/motivajustes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/motivajustes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnMotivajustesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Motivajuste> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Motivajuste>> GetMotivajustes(Query query = null)
        {
            var items = Context.Motivajustes.AsQueryable();


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

            OnMotivajustesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnMotivajusteGet(Aldebaran.Web.Models.AldebaranContext.Motivajuste item);
        partial void OnGetMotivajusteByIdmotivajuste(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Motivajuste> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Motivajuste> GetMotivajusteByIdmotivajuste(int idmotivajuste)
        {
            var items = Context.Motivajustes
                              .AsNoTracking()
                              .Where(i => i.IDMOTIVAJUSTE == idmotivajuste);


            OnGetMotivajusteByIdmotivajuste(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnMotivajusteGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnMotivajusteCreated(Aldebaran.Web.Models.AldebaranContext.Motivajuste item);
        partial void OnAfterMotivajusteCreated(Aldebaran.Web.Models.AldebaranContext.Motivajuste item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Motivajuste> CreateMotivajuste(Aldebaran.Web.Models.AldebaranContext.Motivajuste motivajuste)
        {
            OnMotivajusteCreated(motivajuste);

            var existingItem = Context.Motivajustes
                              .Where(i => i.IDMOTIVAJUSTE == motivajuste.IDMOTIVAJUSTE)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Motivajustes.Add(motivajuste);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(motivajuste).State = EntityState.Detached;
                throw;
            }

            OnAfterMotivajusteCreated(motivajuste);

            return motivajuste;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Motivajuste> CancelMotivajusteChanges(Aldebaran.Web.Models.AldebaranContext.Motivajuste item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnMotivajusteUpdated(Aldebaran.Web.Models.AldebaranContext.Motivajuste item);
        partial void OnAfterMotivajusteUpdated(Aldebaran.Web.Models.AldebaranContext.Motivajuste item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Motivajuste> UpdateMotivajuste(int idmotivajuste, Aldebaran.Web.Models.AldebaranContext.Motivajuste motivajuste)
        {
            OnMotivajusteUpdated(motivajuste);

            var itemToUpdate = Context.Motivajustes
                              .Where(i => i.IDMOTIVAJUSTE == motivajuste.IDMOTIVAJUSTE)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(motivajuste);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterMotivajusteUpdated(motivajuste);

            return motivajuste;
        }

        partial void OnMotivajusteDeleted(Aldebaran.Web.Models.AldebaranContext.Motivajuste item);
        partial void OnAfterMotivajusteDeleted(Aldebaran.Web.Models.AldebaranContext.Motivajuste item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Motivajuste> DeleteMotivajuste(int idmotivajuste)
        {
            var itemToDelete = Context.Motivajustes
                              .Where(i => i.IDMOTIVAJUSTE == idmotivajuste)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnMotivajusteDeleted(itemToDelete);


            Context.Motivajustes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterMotivajusteDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportMotivodevolucionsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/motivodevolucions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/motivodevolucions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportMotivodevolucionsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/motivodevolucions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/motivodevolucions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnMotivodevolucionsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Motivodevolucion> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Motivodevolucion>> GetMotivodevolucions(Query query = null)
        {
            var items = Context.Motivodevolucions.AsQueryable();


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

            OnMotivodevolucionsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnMotivodevolucionGet(Aldebaran.Web.Models.AldebaranContext.Motivodevolucion item);
        partial void OnGetMotivodevolucionByIdmotivodev(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Motivodevolucion> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Motivodevolucion> GetMotivodevolucionByIdmotivodev(int idmotivodev)
        {
            var items = Context.Motivodevolucions
                              .AsNoTracking()
                              .Where(i => i.IDMOTIVODEV == idmotivodev);


            OnGetMotivodevolucionByIdmotivodev(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnMotivodevolucionGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnMotivodevolucionCreated(Aldebaran.Web.Models.AldebaranContext.Motivodevolucion item);
        partial void OnAfterMotivodevolucionCreated(Aldebaran.Web.Models.AldebaranContext.Motivodevolucion item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Motivodevolucion> CreateMotivodevolucion(Aldebaran.Web.Models.AldebaranContext.Motivodevolucion motivodevolucion)
        {
            OnMotivodevolucionCreated(motivodevolucion);

            var existingItem = Context.Motivodevolucions
                              .Where(i => i.IDMOTIVODEV == motivodevolucion.IDMOTIVODEV)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Motivodevolucions.Add(motivodevolucion);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(motivodevolucion).State = EntityState.Detached;
                throw;
            }

            OnAfterMotivodevolucionCreated(motivodevolucion);

            return motivodevolucion;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Motivodevolucion> CancelMotivodevolucionChanges(Aldebaran.Web.Models.AldebaranContext.Motivodevolucion item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnMotivodevolucionUpdated(Aldebaran.Web.Models.AldebaranContext.Motivodevolucion item);
        partial void OnAfterMotivodevolucionUpdated(Aldebaran.Web.Models.AldebaranContext.Motivodevolucion item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Motivodevolucion> UpdateMotivodevolucion(int idmotivodev, Aldebaran.Web.Models.AldebaranContext.Motivodevolucion motivodevolucion)
        {
            OnMotivodevolucionUpdated(motivodevolucion);

            var itemToUpdate = Context.Motivodevolucions
                              .Where(i => i.IDMOTIVODEV == motivodevolucion.IDMOTIVODEV)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(motivodevolucion);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterMotivodevolucionUpdated(motivodevolucion);

            return motivodevolucion;
        }

        partial void OnMotivodevolucionDeleted(Aldebaran.Web.Models.AldebaranContext.Motivodevolucion item);
        partial void OnAfterMotivodevolucionDeleted(Aldebaran.Web.Models.AldebaranContext.Motivodevolucion item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Motivodevolucion> DeleteMotivodevolucion(int idmotivodev)
        {
            var itemToDelete = Context.Motivodevolucions
                              .Where(i => i.IDMOTIVODEV == idmotivodev)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnMotivodevolucionDeleted(itemToDelete);


            Context.Motivodevolucions.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterMotivodevolucionDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportOpcionesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/opciones/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/opciones/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportOpcionesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/opciones/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/opciones/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnOpcionesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Opcione> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Opcione>> GetOpciones(Query query = null)
        {
            var items = Context.Opciones.AsQueryable();


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

            OnOpcionesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnOpcioneGet(Aldebaran.Web.Models.AldebaranContext.Opcione item);
        partial void OnGetOpcioneByIdopcion(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Opcione> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Opcione> GetOpcioneByIdopcion(int idopcion)
        {
            var items = Context.Opciones
                              .AsNoTracking()
                              .Where(i => i.IDOPCION == idopcion);


            OnGetOpcioneByIdopcion(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnOpcioneGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnOpcioneCreated(Aldebaran.Web.Models.AldebaranContext.Opcione item);
        partial void OnAfterOpcioneCreated(Aldebaran.Web.Models.AldebaranContext.Opcione item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Opcione> CreateOpcione(Aldebaran.Web.Models.AldebaranContext.Opcione opcione)
        {
            OnOpcioneCreated(opcione);

            var existingItem = Context.Opciones
                              .Where(i => i.IDOPCION == opcione.IDOPCION)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Opciones.Add(opcione);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(opcione).State = EntityState.Detached;
                throw;
            }

            OnAfterOpcioneCreated(opcione);

            return opcione;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Opcione> CancelOpcioneChanges(Aldebaran.Web.Models.AldebaranContext.Opcione item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnOpcioneUpdated(Aldebaran.Web.Models.AldebaranContext.Opcione item);
        partial void OnAfterOpcioneUpdated(Aldebaran.Web.Models.AldebaranContext.Opcione item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Opcione> UpdateOpcione(int idopcion, Aldebaran.Web.Models.AldebaranContext.Opcione opcione)
        {
            OnOpcioneUpdated(opcione);

            var itemToUpdate = Context.Opciones
                              .Where(i => i.IDOPCION == opcione.IDOPCION)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(opcione);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterOpcioneUpdated(opcione);

            return opcione;
        }

        partial void OnOpcioneDeleted(Aldebaran.Web.Models.AldebaranContext.Opcione item);
        partial void OnAfterOpcioneDeleted(Aldebaran.Web.Models.AldebaranContext.Opcione item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Opcione> DeleteOpcione(int idopcion)
        {
            var itemToDelete = Context.Opciones
                              .Where(i => i.IDOPCION == idopcion)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnOpcioneDeleted(itemToDelete);


            Context.Opciones.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterOpcioneDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportOpcionesftpV1SToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/opcionesftpv1s/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/opcionesftpv1s/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportOpcionesftpV1SToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/opcionesftpv1s/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/opcionesftpv1s/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnOpcionesftpV1SRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.OpcionesftpV1> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.OpcionesftpV1>> GetOpcionesftpV1S(Query query = null)
        {
            var items = Context.OpcionesftpV1S.AsQueryable();


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

            OnOpcionesftpV1SRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnOpcionesftpV1Get(Aldebaran.Web.Models.AldebaranContext.OpcionesftpV1 item);
        partial void OnGetOpcionesftpV1ByIdopciones(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.OpcionesftpV1> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.OpcionesftpV1> GetOpcionesftpV1ByIdopciones(int idopciones)
        {
            var items = Context.OpcionesftpV1S
                              .AsNoTracking()
                              .Where(i => i.IDOPCIONES == idopciones);


            OnGetOpcionesftpV1ByIdopciones(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnOpcionesftpV1Get(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnOpcionesftpV1Created(Aldebaran.Web.Models.AldebaranContext.OpcionesftpV1 item);
        partial void OnAfterOpcionesftpV1Created(Aldebaran.Web.Models.AldebaranContext.OpcionesftpV1 item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.OpcionesftpV1> CreateOpcionesftpV1(Aldebaran.Web.Models.AldebaranContext.OpcionesftpV1 opcionesftpv1)
        {
            OnOpcionesftpV1Created(opcionesftpv1);

            var existingItem = Context.OpcionesftpV1S
                              .Where(i => i.IDOPCIONES == opcionesftpv1.IDOPCIONES)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.OpcionesftpV1S.Add(opcionesftpv1);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(opcionesftpv1).State = EntityState.Detached;
                throw;
            }

            OnAfterOpcionesftpV1Created(opcionesftpv1);

            return opcionesftpv1;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.OpcionesftpV1> CancelOpcionesftpV1Changes(Aldebaran.Web.Models.AldebaranContext.OpcionesftpV1 item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnOpcionesftpV1Updated(Aldebaran.Web.Models.AldebaranContext.OpcionesftpV1 item);
        partial void OnAfterOpcionesftpV1Updated(Aldebaran.Web.Models.AldebaranContext.OpcionesftpV1 item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.OpcionesftpV1> UpdateOpcionesftpV1(int idopciones, Aldebaran.Web.Models.AldebaranContext.OpcionesftpV1 opcionesftpv1)
        {
            OnOpcionesftpV1Updated(opcionesftpv1);

            var itemToUpdate = Context.OpcionesftpV1S
                              .Where(i => i.IDOPCIONES == opcionesftpv1.IDOPCIONES)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(opcionesftpv1);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterOpcionesftpV1Updated(opcionesftpv1);

            return opcionesftpv1;
        }

        partial void OnOpcionesftpV1Deleted(Aldebaran.Web.Models.AldebaranContext.OpcionesftpV1 item);
        partial void OnAfterOpcionesftpV1Deleted(Aldebaran.Web.Models.AldebaranContext.OpcionesftpV1 item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.OpcionesftpV1> DeleteOpcionesftpV1(int idopciones)
        {
            var itemToDelete = Context.OpcionesftpV1S
                              .Where(i => i.IDOPCIONES == idopciones)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnOpcionesftpV1Deleted(itemToDelete);


            Context.OpcionesftpV1S.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterOpcionesftpV1Deleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportOpcionesmailsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/opcionesmails/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/opcionesmails/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportOpcionesmailsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/opcionesmails/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/opcionesmails/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnOpcionesmailsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Opcionesmail> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Opcionesmail>> GetOpcionesmails(Query query = null)
        {
            var items = Context.Opcionesmails.AsQueryable();


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

            OnOpcionesmailsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnOpcionesmailGet(Aldebaran.Web.Models.AldebaranContext.Opcionesmail item);
        partial void OnGetOpcionesmailById(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Opcionesmail> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Opcionesmail> GetOpcionesmailById(int id)
        {
            var items = Context.Opcionesmails
                              .AsNoTracking()
                              .Where(i => i.ID == id);


            OnGetOpcionesmailById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnOpcionesmailGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnOpcionesmailCreated(Aldebaran.Web.Models.AldebaranContext.Opcionesmail item);
        partial void OnAfterOpcionesmailCreated(Aldebaran.Web.Models.AldebaranContext.Opcionesmail item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Opcionesmail> CreateOpcionesmail(Aldebaran.Web.Models.AldebaranContext.Opcionesmail opcionesmail)
        {
            OnOpcionesmailCreated(opcionesmail);

            var existingItem = Context.Opcionesmails
                              .Where(i => i.ID == opcionesmail.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Opcionesmails.Add(opcionesmail);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(opcionesmail).State = EntityState.Detached;
                throw;
            }

            OnAfterOpcionesmailCreated(opcionesmail);

            return opcionesmail;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Opcionesmail> CancelOpcionesmailChanges(Aldebaran.Web.Models.AldebaranContext.Opcionesmail item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnOpcionesmailUpdated(Aldebaran.Web.Models.AldebaranContext.Opcionesmail item);
        partial void OnAfterOpcionesmailUpdated(Aldebaran.Web.Models.AldebaranContext.Opcionesmail item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Opcionesmail> UpdateOpcionesmail(int id, Aldebaran.Web.Models.AldebaranContext.Opcionesmail opcionesmail)
        {
            OnOpcionesmailUpdated(opcionesmail);

            var itemToUpdate = Context.Opcionesmails
                              .Where(i => i.ID == opcionesmail.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(opcionesmail);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterOpcionesmailUpdated(opcionesmail);

            return opcionesmail;
        }

        partial void OnOpcionesmailDeleted(Aldebaran.Web.Models.AldebaranContext.Opcionesmail item);
        partial void OnAfterOpcionesmailDeleted(Aldebaran.Web.Models.AldebaranContext.Opcionesmail item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Opcionesmail> DeleteOpcionesmail(int id)
        {
            var itemToDelete = Context.Opcionesmails
                              .Where(i => i.ID == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnOpcionesmailDeleted(itemToDelete);


            Context.Opcionesmails.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterOpcionesmailDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportOpcionessisToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/opcionessis/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/opcionessis/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportOpcionessisToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/opcionessis/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/opcionessis/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnOpcionessisRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Opcionessi> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Opcionessi>> GetOpcionessis(Query query = null)
        {
            var items = Context.Opcionessis.AsQueryable();


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

            OnOpcionessisRead(ref items);

            return await Task.FromResult(items);
        }

        public async Task ExportOrdenesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/ordenes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/ordenes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportOrdenesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/ordenes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/ordenes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnOrdenesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Ordene> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Ordene>> GetOrdenes(Query query = null)
        {
            var items = Context.Ordenes.AsQueryable();

            items = items.Include(i => i.Agentesforwarder);
            items = items.Include(i => i.Empresa);
            items = items.Include(i => i.Funcionario);

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

            OnOrdenesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnOrdeneGet(Aldebaran.Web.Models.AldebaranContext.Ordene item);
        partial void OnGetOrdeneByIdorden(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Ordene> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Ordene> GetOrdeneByIdorden(int idorden)
        {
            var items = Context.Ordenes
                              .AsNoTracking()
                              .Where(i => i.IDORDEN == idorden);

            items = items.Include(i => i.Agentesforwarder);
            items = items.Include(i => i.Empresa);
            items = items.Include(i => i.Funcionario);

            OnGetOrdeneByIdorden(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnOrdeneGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnOrdeneCreated(Aldebaran.Web.Models.AldebaranContext.Ordene item);
        partial void OnAfterOrdeneCreated(Aldebaran.Web.Models.AldebaranContext.Ordene item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ordene> CreateOrdene(Aldebaran.Web.Models.AldebaranContext.Ordene ordene)
        {
            OnOrdeneCreated(ordene);

            var existingItem = Context.Ordenes
                              .Where(i => i.IDORDEN == ordene.IDORDEN)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Ordenes.Add(ordene);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(ordene).State = EntityState.Detached;
                throw;
            }

            OnAfterOrdeneCreated(ordene);

            return ordene;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ordene> CancelOrdeneChanges(Aldebaran.Web.Models.AldebaranContext.Ordene item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnOrdeneUpdated(Aldebaran.Web.Models.AldebaranContext.Ordene item);
        partial void OnAfterOrdeneUpdated(Aldebaran.Web.Models.AldebaranContext.Ordene item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ordene> UpdateOrdene(int idorden, Aldebaran.Web.Models.AldebaranContext.Ordene ordene)
        {
            OnOrdeneUpdated(ordene);

            var itemToUpdate = Context.Ordenes
                              .Where(i => i.IDORDEN == ordene.IDORDEN)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(ordene);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterOrdeneUpdated(ordene);

            return ordene;
        }

        partial void OnOrdeneDeleted(Aldebaran.Web.Models.AldebaranContext.Ordene item);
        partial void OnAfterOrdeneDeleted(Aldebaran.Web.Models.AldebaranContext.Ordene item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ordene> DeleteOrdene(int idorden)
        {
            var itemToDelete = Context.Ordenes
                              .Where(i => i.IDORDEN == idorden)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnOrdeneDeleted(itemToDelete);


            Context.Ordenes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterOrdeneDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportPaisesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/paises/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/paises/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportPaisesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/paises/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/paises/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnPaisesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Paise> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Paise>> GetPaises(Query query = null)
        {
            var items = Context.Paises.AsQueryable();


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

            OnPaisesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnPaiseGet(Aldebaran.Web.Models.AldebaranContext.Paise item);
        partial void OnGetPaiseByIdpais(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Paise> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Paise> GetPaiseByIdpais(int idpais)
        {
            var items = Context.Paises
                              .AsNoTracking()
                              .Where(i => i.IDPAIS == idpais);


            OnGetPaiseByIdpais(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnPaiseGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnPaiseCreated(Aldebaran.Web.Models.AldebaranContext.Paise item);
        partial void OnAfterPaiseCreated(Aldebaran.Web.Models.AldebaranContext.Paise item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Paise> CreatePaise(Aldebaran.Web.Models.AldebaranContext.Paise paise)
        {
            OnPaiseCreated(paise);

            var existingItem = Context.Paises
                              .Where(i => i.IDPAIS == paise.IDPAIS)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Paises.Add(paise);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(paise).State = EntityState.Detached;
                throw;
            }

            OnAfterPaiseCreated(paise);

            return paise;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Paise> CancelPaiseChanges(Aldebaran.Web.Models.AldebaranContext.Paise item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnPaiseUpdated(Aldebaran.Web.Models.AldebaranContext.Paise item);
        partial void OnAfterPaiseUpdated(Aldebaran.Web.Models.AldebaranContext.Paise item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Paise> UpdatePaise(int idpais, Aldebaran.Web.Models.AldebaranContext.Paise paise)
        {
            OnPaiseUpdated(paise);

            var itemToUpdate = Context.Paises
                              .Where(i => i.IDPAIS == paise.IDPAIS)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(paise);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterPaiseUpdated(paise);

            return paise;
        }

        partial void OnPaiseDeleted(Aldebaran.Web.Models.AldebaranContext.Paise item);
        partial void OnAfterPaiseDeleted(Aldebaran.Web.Models.AldebaranContext.Paise item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Paise> DeletePaise(int idpais)
        {
            var itemToDelete = Context.Paises
                              .Where(i => i.IDPAIS == idpais)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnPaiseDeleted(itemToDelete);


            Context.Paises.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterPaiseDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportPedidosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/pedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/pedidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportPedidosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/pedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/pedidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnPedidosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Pedido> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Pedido>> GetPedidos(Query query = null)
        {
            var items = Context.Pedidos.AsQueryable();

            items = items.Include(i => i.Cliente);
            items = items.Include(i => i.Funcionario);

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

            OnPedidosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnPedidoGet(Aldebaran.Web.Models.AldebaranContext.Pedido item);
        partial void OnGetPedidoByIdpedido(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Pedido> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Pedido> GetPedidoByIdpedido(int idpedido)
        {
            var items = Context.Pedidos
                              .AsNoTracking()
                              .Where(i => i.IDPEDIDO == idpedido);

            items = items.Include(i => i.Cliente);
            items = items.Include(i => i.Funcionario);

            OnGetPedidoByIdpedido(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnPedidoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnPedidoCreated(Aldebaran.Web.Models.AldebaranContext.Pedido item);
        partial void OnAfterPedidoCreated(Aldebaran.Web.Models.AldebaranContext.Pedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Pedido> CreatePedido(Aldebaran.Web.Models.AldebaranContext.Pedido pedido)
        {
            OnPedidoCreated(pedido);

            var existingItem = Context.Pedidos
                              .Where(i => i.IDPEDIDO == pedido.IDPEDIDO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Pedidos.Add(pedido);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(pedido).State = EntityState.Detached;
                throw;
            }

            OnAfterPedidoCreated(pedido);

            return pedido;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Pedido> CancelPedidoChanges(Aldebaran.Web.Models.AldebaranContext.Pedido item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnPedidoUpdated(Aldebaran.Web.Models.AldebaranContext.Pedido item);
        partial void OnAfterPedidoUpdated(Aldebaran.Web.Models.AldebaranContext.Pedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Pedido> UpdatePedido(int idpedido, Aldebaran.Web.Models.AldebaranContext.Pedido pedido)
        {
            OnPedidoUpdated(pedido);

            var itemToUpdate = Context.Pedidos
                              .Where(i => i.IDPEDIDO == pedido.IDPEDIDO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(pedido);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterPedidoUpdated(pedido);

            return pedido;
        }

        partial void OnPedidoDeleted(Aldebaran.Web.Models.AldebaranContext.Pedido item);
        partial void OnAfterPedidoDeleted(Aldebaran.Web.Models.AldebaranContext.Pedido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Pedido> DeletePedido(int idpedido)
        {
            var itemToDelete = Context.Pedidos
                              .Where(i => i.IDPEDIDO == idpedido)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnPedidoDeleted(itemToDelete);


            Context.Pedidos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterPedidoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportPermisosalarmasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/permisosalarmas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/permisosalarmas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportPermisosalarmasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/permisosalarmas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/permisosalarmas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnPermisosalarmasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Permisosalarma> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Permisosalarma>> GetPermisosalarmas(Query query = null)
        {
            var items = Context.Permisosalarmas.AsQueryable();

            items = items.Include(i => i.Tiposalarma);

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

            OnPermisosalarmasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnPermisosalarmaGet(Aldebaran.Web.Models.AldebaranContext.Permisosalarma item);
        partial void OnGetPermisosalarmaByIdtipoalarmaAndIdusuario(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Permisosalarma> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Permisosalarma> GetPermisosalarmaByIdtipoalarmaAndIdusuario(int idtipoalarma, int idusuario)
        {
            var items = Context.Permisosalarmas
                              .AsNoTracking()
                              .Where(i => i.IDTIPOALARMA == idtipoalarma && i.IDUSUARIO == idusuario);

            items = items.Include(i => i.Tiposalarma);

            OnGetPermisosalarmaByIdtipoalarmaAndIdusuario(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnPermisosalarmaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnPermisosalarmaCreated(Aldebaran.Web.Models.AldebaranContext.Permisosalarma item);
        partial void OnAfterPermisosalarmaCreated(Aldebaran.Web.Models.AldebaranContext.Permisosalarma item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Permisosalarma> CreatePermisosalarma(Aldebaran.Web.Models.AldebaranContext.Permisosalarma permisosalarma)
        {
            OnPermisosalarmaCreated(permisosalarma);

            var existingItem = Context.Permisosalarmas
                              .Where(i => i.IDTIPOALARMA == permisosalarma.IDTIPOALARMA && i.IDUSUARIO == permisosalarma.IDUSUARIO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Permisosalarmas.Add(permisosalarma);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(permisosalarma).State = EntityState.Detached;
                throw;
            }

            OnAfterPermisosalarmaCreated(permisosalarma);

            return permisosalarma;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Permisosalarma> CancelPermisosalarmaChanges(Aldebaran.Web.Models.AldebaranContext.Permisosalarma item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnPermisosalarmaUpdated(Aldebaran.Web.Models.AldebaranContext.Permisosalarma item);
        partial void OnAfterPermisosalarmaUpdated(Aldebaran.Web.Models.AldebaranContext.Permisosalarma item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Permisosalarma> UpdatePermisosalarma(int idtipoalarma, int idusuario, Aldebaran.Web.Models.AldebaranContext.Permisosalarma permisosalarma)
        {
            OnPermisosalarmaUpdated(permisosalarma);

            var itemToUpdate = Context.Permisosalarmas
                              .Where(i => i.IDTIPOALARMA == permisosalarma.IDTIPOALARMA && i.IDUSUARIO == permisosalarma.IDUSUARIO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(permisosalarma);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterPermisosalarmaUpdated(permisosalarma);

            return permisosalarma;
        }

        partial void OnPermisosalarmaDeleted(Aldebaran.Web.Models.AldebaranContext.Permisosalarma item);
        partial void OnAfterPermisosalarmaDeleted(Aldebaran.Web.Models.AldebaranContext.Permisosalarma item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Permisosalarma> DeletePermisosalarma(int idtipoalarma, int idusuario)
        {
            var itemToDelete = Context.Permisosalarmas
                              .Where(i => i.IDTIPOALARMA == idtipoalarma && i.IDUSUARIO == idusuario)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnPermisosalarmaDeleted(itemToDelete);


            Context.Permisosalarmas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterPermisosalarmaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportProveedoresToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/proveedores/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/proveedores/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportProveedoresToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/proveedores/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/proveedores/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnProveedoresRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Proveedore> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Proveedore>> GetProveedores(Query query = null)
        {
            var items = Context.Proveedores.AsQueryable();

            items = items.Include(i => i.Tipidentifica);

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

            OnProveedoresRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnProveedoreGet(Aldebaran.Web.Models.AldebaranContext.Proveedore item);
        partial void OnGetProveedoreByIdproveedor(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Proveedore> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Proveedore> GetProveedoreByIdproveedor(int idproveedor)
        {
            var items = Context.Proveedores
                              .AsNoTracking()
                              .Where(i => i.IDPROVEEDOR == idproveedor);

            items = items.Include(i => i.Tipidentifica);

            OnGetProveedoreByIdproveedor(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnProveedoreGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnProveedoreCreated(Aldebaran.Web.Models.AldebaranContext.Proveedore item);
        partial void OnAfterProveedoreCreated(Aldebaran.Web.Models.AldebaranContext.Proveedore item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Proveedore> CreateProveedore(Aldebaran.Web.Models.AldebaranContext.Proveedore proveedore)
        {
            OnProveedoreCreated(proveedore);

            var existingItem = Context.Proveedores
                              .Where(i => i.IDPROVEEDOR == proveedore.IDPROVEEDOR)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Proveedores.Add(proveedore);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(proveedore).State = EntityState.Detached;
                throw;
            }

            OnAfterProveedoreCreated(proveedore);

            return proveedore;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Proveedore> CancelProveedoreChanges(Aldebaran.Web.Models.AldebaranContext.Proveedore item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnProveedoreUpdated(Aldebaran.Web.Models.AldebaranContext.Proveedore item);
        partial void OnAfterProveedoreUpdated(Aldebaran.Web.Models.AldebaranContext.Proveedore item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Proveedore> UpdateProveedore(int idproveedor, Aldebaran.Web.Models.AldebaranContext.Proveedore proveedore)
        {
            OnProveedoreUpdated(proveedore);

            var itemToUpdate = Context.Proveedores
                              .Where(i => i.IDPROVEEDOR == proveedore.IDPROVEEDOR)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(proveedore);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterProveedoreUpdated(proveedore);

            return proveedore;
        }

        partial void OnProveedoreDeleted(Aldebaran.Web.Models.AldebaranContext.Proveedore item);
        partial void OnAfterProveedoreDeleted(Aldebaran.Web.Models.AldebaranContext.Proveedore item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Proveedore> DeleteProveedore(int idproveedor)
        {
            var itemToDelete = Context.Proveedores
                              .Where(i => i.IDPROVEEDOR == idproveedor)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnProveedoreDeleted(itemToDelete);


            Context.Proveedores.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterProveedoreDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportRembalajesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/rembalajes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/rembalajes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportRembalajesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/rembalajes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/rembalajes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnRembalajesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Rembalaje> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Rembalaje>> GetRembalajes(Query query = null)
        {
            var items = Context.Rembalajes.AsQueryable();


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

            OnRembalajesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnRembalajeGet(Aldebaran.Web.Models.AldebaranContext.Rembalaje item);
        partial void OnGetRembalajeById(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Rembalaje> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Rembalaje> GetRembalajeById(int id)
        {
            var items = Context.Rembalajes
                              .AsNoTracking()
                              .Where(i => i.ID == id);


            OnGetRembalajeById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnRembalajeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnRembalajeCreated(Aldebaran.Web.Models.AldebaranContext.Rembalaje item);
        partial void OnAfterRembalajeCreated(Aldebaran.Web.Models.AldebaranContext.Rembalaje item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Rembalaje> CreateRembalaje(Aldebaran.Web.Models.AldebaranContext.Rembalaje rembalaje)
        {
            OnRembalajeCreated(rembalaje);

            var existingItem = Context.Rembalajes
                              .Where(i => i.ID == rembalaje.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Rembalajes.Add(rembalaje);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(rembalaje).State = EntityState.Detached;
                throw;
            }

            OnAfterRembalajeCreated(rembalaje);

            return rembalaje;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Rembalaje> CancelRembalajeChanges(Aldebaran.Web.Models.AldebaranContext.Rembalaje item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnRembalajeUpdated(Aldebaran.Web.Models.AldebaranContext.Rembalaje item);
        partial void OnAfterRembalajeUpdated(Aldebaran.Web.Models.AldebaranContext.Rembalaje item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Rembalaje> UpdateRembalaje(int id, Aldebaran.Web.Models.AldebaranContext.Rembalaje rembalaje)
        {
            OnRembalajeUpdated(rembalaje);

            var itemToUpdate = Context.Rembalajes
                              .Where(i => i.ID == rembalaje.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(rembalaje);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterRembalajeUpdated(rembalaje);

            return rembalaje;
        }

        partial void OnRembalajeDeleted(Aldebaran.Web.Models.AldebaranContext.Rembalaje item);
        partial void OnAfterRembalajeDeleted(Aldebaran.Web.Models.AldebaranContext.Rembalaje item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Rembalaje> DeleteRembalaje(int id)
        {
            var itemToDelete = Context.Rembalajes
                              .Where(i => i.ID == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnRembalajeDeleted(itemToDelete);


            Context.Rembalajes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterRembalajeDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportReservasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/reservas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/reservas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportReservasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/reservas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/reservas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnReservasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Reserva> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Reserva>> GetReservas(Query query = null)
        {
            var items = Context.Reservas.AsQueryable();

            items = items.Include(i => i.Cliente);
            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Pedido);

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

            OnReservasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnReservaGet(Aldebaran.Web.Models.AldebaranContext.Reserva item);
        partial void OnGetReservaByIdreserva(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Reserva> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Reserva> GetReservaByIdreserva(int idreserva)
        {
            var items = Context.Reservas
                              .AsNoTracking()
                              .Where(i => i.IDRESERVA == idreserva);

            items = items.Include(i => i.Cliente);
            items = items.Include(i => i.Funcionario);
            items = items.Include(i => i.Pedido);

            OnGetReservaByIdreserva(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnReservaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnReservaCreated(Aldebaran.Web.Models.AldebaranContext.Reserva item);
        partial void OnAfterReservaCreated(Aldebaran.Web.Models.AldebaranContext.Reserva item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Reserva> CreateReserva(Aldebaran.Web.Models.AldebaranContext.Reserva reserva)
        {
            OnReservaCreated(reserva);

            var existingItem = Context.Reservas
                              .Where(i => i.IDRESERVA == reserva.IDRESERVA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Reservas.Add(reserva);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(reserva).State = EntityState.Detached;
                throw;
            }

            OnAfterReservaCreated(reserva);

            return reserva;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Reserva> CancelReservaChanges(Aldebaran.Web.Models.AldebaranContext.Reserva item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnReservaUpdated(Aldebaran.Web.Models.AldebaranContext.Reserva item);
        partial void OnAfterReservaUpdated(Aldebaran.Web.Models.AldebaranContext.Reserva item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Reserva> UpdateReserva(int idreserva, Aldebaran.Web.Models.AldebaranContext.Reserva reserva)
        {
            OnReservaUpdated(reserva);

            var itemToUpdate = Context.Reservas
                              .Where(i => i.IDRESERVA == reserva.IDRESERVA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(reserva);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterReservaUpdated(reserva);

            return reserva;
        }

        partial void OnReservaDeleted(Aldebaran.Web.Models.AldebaranContext.Reserva item);
        partial void OnAfterReservaDeleted(Aldebaran.Web.Models.AldebaranContext.Reserva item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Reserva> DeleteReserva(int idreserva)
        {
            var itemToDelete = Context.Reservas
                              .Where(i => i.IDRESERVA == idreserva)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnReservaDeleted(itemToDelete);


            Context.Reservas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterReservaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportRexistenciaToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/rexistencia/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/rexistencia/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportRexistenciaToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/rexistencia/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/rexistencia/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnRexistenciaRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Rexistencia> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Rexistencia>> GetRexistencia(Query query = null)
        {
            var items = Context.Rexistencia.AsQueryable();


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

            OnRexistenciaRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnRexistenciaGet(Aldebaran.Web.Models.AldebaranContext.Rexistencia item);
        partial void OnGetRexistenciaById(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Rexistencia> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Rexistencia> GetRexistenciaById(int id)
        {
            var items = Context.Rexistencia
                              .AsNoTracking()
                              .Where(i => i.ID == id);


            OnGetRexistenciaById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnRexistenciaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnRexistenciaCreated(Aldebaran.Web.Models.AldebaranContext.Rexistencia item);
        partial void OnAfterRexistenciaCreated(Aldebaran.Web.Models.AldebaranContext.Rexistencia item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Rexistencia> CreateRexistencia(Aldebaran.Web.Models.AldebaranContext.Rexistencia rexistencia)
        {
            OnRexistenciaCreated(rexistencia);

            var existingItem = Context.Rexistencia
                              .Where(i => i.ID == rexistencia.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Rexistencia.Add(rexistencia);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(rexistencia).State = EntityState.Detached;
                throw;
            }

            OnAfterRexistenciaCreated(rexistencia);

            return rexistencia;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Rexistencia> CancelRexistenciaChanges(Aldebaran.Web.Models.AldebaranContext.Rexistencia item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnRexistenciaUpdated(Aldebaran.Web.Models.AldebaranContext.Rexistencia item);
        partial void OnAfterRexistenciaUpdated(Aldebaran.Web.Models.AldebaranContext.Rexistencia item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Rexistencia> UpdateRexistencia(int id, Aldebaran.Web.Models.AldebaranContext.Rexistencia rexistencia)
        {
            OnRexistenciaUpdated(rexistencia);

            var itemToUpdate = Context.Rexistencia
                              .Where(i => i.ID == rexistencia.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(rexistencia);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterRexistenciaUpdated(rexistencia);

            return rexistencia;
        }

        partial void OnRexistenciaDeleted(Aldebaran.Web.Models.AldebaranContext.Rexistencia item);
        partial void OnAfterRexistenciaDeleted(Aldebaran.Web.Models.AldebaranContext.Rexistencia item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Rexistencia> DeleteRexistencia(int id)
        {
            var itemToDelete = Context.Rexistencia
                              .Where(i => i.ID == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnRexistenciaDeleted(itemToDelete);


            Context.Rexistencia.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterRexistenciaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportRitemsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/ritems/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/ritems/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportRitemsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/ritems/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/ritems/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnRitemsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Ritem> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Ritem>> GetRitems(Query query = null)
        {
            var items = Context.Ritems.AsQueryable();


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

            OnRitemsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnRitemGet(Aldebaran.Web.Models.AldebaranContext.Ritem item);
        partial void OnGetRitemById(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Ritem> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Ritem> GetRitemById(int id)
        {
            var items = Context.Ritems
                              .AsNoTracking()
                              .Where(i => i.ID == id);


            OnGetRitemById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnRitemGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnRitemCreated(Aldebaran.Web.Models.AldebaranContext.Ritem item);
        partial void OnAfterRitemCreated(Aldebaran.Web.Models.AldebaranContext.Ritem item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ritem> CreateRitem(Aldebaran.Web.Models.AldebaranContext.Ritem ritem)
        {
            OnRitemCreated(ritem);

            var existingItem = Context.Ritems
                              .Where(i => i.ID == ritem.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Ritems.Add(ritem);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(ritem).State = EntityState.Detached;
                throw;
            }

            OnAfterRitemCreated(ritem);

            return ritem;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ritem> CancelRitemChanges(Aldebaran.Web.Models.AldebaranContext.Ritem item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnRitemUpdated(Aldebaran.Web.Models.AldebaranContext.Ritem item);
        partial void OnAfterRitemUpdated(Aldebaran.Web.Models.AldebaranContext.Ritem item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ritem> UpdateRitem(int id, Aldebaran.Web.Models.AldebaranContext.Ritem ritem)
        {
            OnRitemUpdated(ritem);

            var itemToUpdate = Context.Ritems
                              .Where(i => i.ID == ritem.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(ritem);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterRitemUpdated(ritem);

            return ritem;
        }

        partial void OnRitemDeleted(Aldebaran.Web.Models.AldebaranContext.Ritem item);
        partial void OnAfterRitemDeleted(Aldebaran.Web.Models.AldebaranContext.Ritem item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ritem> DeleteRitem(int id)
        {
            var itemToDelete = Context.Ritems
                              .Where(i => i.ID == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnRitemDeleted(itemToDelete);


            Context.Ritems.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterRitemDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportRitemsxcolorsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/ritemsxcolors/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/ritemsxcolors/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportRitemsxcolorsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/ritemsxcolors/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/ritemsxcolors/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnRitemsxcolorsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor>> GetRitemsxcolors(Query query = null)
        {
            var items = Context.Ritemsxcolors.AsQueryable();


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

            OnRitemsxcolorsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnRitemsxcolorGet(Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor item);
        partial void OnGetRitemsxcolorById(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor> GetRitemsxcolorById(int id)
        {
            var items = Context.Ritemsxcolors
                              .AsNoTracking()
                              .Where(i => i.ID == id);


            OnGetRitemsxcolorById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnRitemsxcolorGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnRitemsxcolorCreated(Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor item);
        partial void OnAfterRitemsxcolorCreated(Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor> CreateRitemsxcolor(Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor ritemsxcolor)
        {
            OnRitemsxcolorCreated(ritemsxcolor);

            var existingItem = Context.Ritemsxcolors
                              .Where(i => i.ID == ritemsxcolor.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Ritemsxcolors.Add(ritemsxcolor);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(ritemsxcolor).State = EntityState.Detached;
                throw;
            }

            OnAfterRitemsxcolorCreated(ritemsxcolor);

            return ritemsxcolor;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor> CancelRitemsxcolorChanges(Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnRitemsxcolorUpdated(Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor item);
        partial void OnAfterRitemsxcolorUpdated(Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor> UpdateRitemsxcolor(int id, Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor ritemsxcolor)
        {
            OnRitemsxcolorUpdated(ritemsxcolor);

            var itemToUpdate = Context.Ritemsxcolors
                              .Where(i => i.ID == ritemsxcolor.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(ritemsxcolor);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterRitemsxcolorUpdated(ritemsxcolor);

            return ritemsxcolor;
        }

        partial void OnRitemsxcolorDeleted(Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor item);
        partial void OnAfterRitemsxcolorDeleted(Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor> DeleteRitemsxcolor(int id)
        {
            var itemToDelete = Context.Ritemsxcolors
                              .Where(i => i.ID == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnRitemsxcolorDeleted(itemToDelete);


            Context.Ritemsxcolors.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterRitemsxcolorDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportRlineasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/rlineas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/rlineas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportRlineasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/rlineas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/rlineas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnRlineasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Rlinea> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Rlinea>> GetRlineas(Query query = null)
        {
            var items = Context.Rlineas.AsQueryable();


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

            OnRlineasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnRlineaGet(Aldebaran.Web.Models.AldebaranContext.Rlinea item);
        partial void OnGetRlineaById(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Rlinea> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Rlinea> GetRlineaById(int id)
        {
            var items = Context.Rlineas
                              .AsNoTracking()
                              .Where(i => i.ID == id);


            OnGetRlineaById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnRlineaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnRlineaCreated(Aldebaran.Web.Models.AldebaranContext.Rlinea item);
        partial void OnAfterRlineaCreated(Aldebaran.Web.Models.AldebaranContext.Rlinea item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Rlinea> CreateRlinea(Aldebaran.Web.Models.AldebaranContext.Rlinea rlinea)
        {
            OnRlineaCreated(rlinea);

            var existingItem = Context.Rlineas
                              .Where(i => i.ID == rlinea.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Rlineas.Add(rlinea);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(rlinea).State = EntityState.Detached;
                throw;
            }

            OnAfterRlineaCreated(rlinea);

            return rlinea;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Rlinea> CancelRlineaChanges(Aldebaran.Web.Models.AldebaranContext.Rlinea item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnRlineaUpdated(Aldebaran.Web.Models.AldebaranContext.Rlinea item);
        partial void OnAfterRlineaUpdated(Aldebaran.Web.Models.AldebaranContext.Rlinea item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Rlinea> UpdateRlinea(int id, Aldebaran.Web.Models.AldebaranContext.Rlinea rlinea)
        {
            OnRlineaUpdated(rlinea);

            var itemToUpdate = Context.Rlineas
                              .Where(i => i.ID == rlinea.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(rlinea);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterRlineaUpdated(rlinea);

            return rlinea;
        }

        partial void OnRlineaDeleted(Aldebaran.Web.Models.AldebaranContext.Rlinea item);
        partial void OnAfterRlineaDeleted(Aldebaran.Web.Models.AldebaranContext.Rlinea item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Rlinea> DeleteRlinea(int id)
        {
            var itemToDelete = Context.Rlineas
                              .Where(i => i.ID == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnRlineaDeleted(itemToDelete);


            Context.Rlineas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterRlineaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportRmonedaToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/rmoneda/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/rmoneda/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportRmonedaToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/rmoneda/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/rmoneda/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnRmonedaRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Rmoneda> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Rmoneda>> GetRmoneda(Query query = null)
        {
            var items = Context.Rmoneda.AsQueryable();


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

            OnRmonedaRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnRmonedaGet(Aldebaran.Web.Models.AldebaranContext.Rmoneda item);
        partial void OnGetRmonedaById(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Rmoneda> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Rmoneda> GetRmonedaById(int id)
        {
            var items = Context.Rmoneda
                              .AsNoTracking()
                              .Where(i => i.ID == id);


            OnGetRmonedaById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnRmonedaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnRmonedaCreated(Aldebaran.Web.Models.AldebaranContext.Rmoneda item);
        partial void OnAfterRmonedaCreated(Aldebaran.Web.Models.AldebaranContext.Rmoneda item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Rmoneda> CreateRmoneda(Aldebaran.Web.Models.AldebaranContext.Rmoneda rmoneda)
        {
            OnRmonedaCreated(rmoneda);

            var existingItem = Context.Rmoneda
                              .Where(i => i.ID == rmoneda.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Rmoneda.Add(rmoneda);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(rmoneda).State = EntityState.Detached;
                throw;
            }

            OnAfterRmonedaCreated(rmoneda);

            return rmoneda;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Rmoneda> CancelRmonedaChanges(Aldebaran.Web.Models.AldebaranContext.Rmoneda item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnRmonedaUpdated(Aldebaran.Web.Models.AldebaranContext.Rmoneda item);
        partial void OnAfterRmonedaUpdated(Aldebaran.Web.Models.AldebaranContext.Rmoneda item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Rmoneda> UpdateRmoneda(int id, Aldebaran.Web.Models.AldebaranContext.Rmoneda rmoneda)
        {
            OnRmonedaUpdated(rmoneda);

            var itemToUpdate = Context.Rmoneda
                              .Where(i => i.ID == rmoneda.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(rmoneda);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterRmonedaUpdated(rmoneda);

            return rmoneda;
        }

        partial void OnRmonedaDeleted(Aldebaran.Web.Models.AldebaranContext.Rmoneda item);
        partial void OnAfterRmonedaDeleted(Aldebaran.Web.Models.AldebaranContext.Rmoneda item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Rmoneda> DeleteRmoneda(int id)
        {
            var itemToDelete = Context.Rmoneda
                              .Where(i => i.ID == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnRmonedaDeleted(itemToDelete);


            Context.Rmoneda.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterRmonedaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportRunidadesmedidaToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/runidadesmedida/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/runidadesmedida/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportRunidadesmedidaToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/runidadesmedida/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/runidadesmedida/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnRunidadesmedidaRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum>> GetRunidadesmedida(Query query = null)
        {
            var items = Context.Runidadesmedida.AsQueryable();


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

            OnRunidadesmedidaRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnRunidadesmedidumGet(Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum item);
        partial void OnGetRunidadesmedidumById(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum> GetRunidadesmedidumById(int id)
        {
            var items = Context.Runidadesmedida
                              .AsNoTracking()
                              .Where(i => i.ID == id);


            OnGetRunidadesmedidumById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnRunidadesmedidumGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnRunidadesmedidumCreated(Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum item);
        partial void OnAfterRunidadesmedidumCreated(Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum> CreateRunidadesmedidum(Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum runidadesmedidum)
        {
            OnRunidadesmedidumCreated(runidadesmedidum);

            var existingItem = Context.Runidadesmedida
                              .Where(i => i.ID == runidadesmedidum.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Runidadesmedida.Add(runidadesmedidum);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(runidadesmedidum).State = EntityState.Detached;
                throw;
            }

            OnAfterRunidadesmedidumCreated(runidadesmedidum);

            return runidadesmedidum;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum> CancelRunidadesmedidumChanges(Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnRunidadesmedidumUpdated(Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum item);
        partial void OnAfterRunidadesmedidumUpdated(Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum> UpdateRunidadesmedidum(int id, Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum runidadesmedidum)
        {
            OnRunidadesmedidumUpdated(runidadesmedidum);

            var itemToUpdate = Context.Runidadesmedida
                              .Where(i => i.ID == runidadesmedidum.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(runidadesmedidum);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterRunidadesmedidumUpdated(runidadesmedidum);

            return runidadesmedidum;
        }

        partial void OnRunidadesmedidumDeleted(Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum item);
        partial void OnAfterRunidadesmedidumDeleted(Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum> DeleteRunidadesmedidum(int id)
        {
            var itemToDelete = Context.Runidadesmedida
                              .Where(i => i.ID == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnRunidadesmedidumDeleted(itemToDelete);


            Context.Runidadesmedida.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterRunidadesmedidumDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportSatelitesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/satelites/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/satelites/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportSatelitesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/satelites/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/satelites/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnSatelitesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Satelite> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Satelite>> GetSatelites(Query query = null)
        {
            var items = Context.Satelites.AsQueryable();

            items = items.Include(i => i.Ciudade);
            items = items.Include(i => i.Tipidentifica);

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

            OnSatelitesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnSateliteGet(Aldebaran.Web.Models.AldebaranContext.Satelite item);
        partial void OnGetSateliteByIdsatelite(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Satelite> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Satelite> GetSateliteByIdsatelite(int idsatelite)
        {
            var items = Context.Satelites
                              .AsNoTracking()
                              .Where(i => i.IDSATELITE == idsatelite);

            items = items.Include(i => i.Ciudade);
            items = items.Include(i => i.Tipidentifica);

            OnGetSateliteByIdsatelite(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnSateliteGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnSateliteCreated(Aldebaran.Web.Models.AldebaranContext.Satelite item);
        partial void OnAfterSateliteCreated(Aldebaran.Web.Models.AldebaranContext.Satelite item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Satelite> CreateSatelite(Aldebaran.Web.Models.AldebaranContext.Satelite satelite)
        {
            OnSateliteCreated(satelite);

            var existingItem = Context.Satelites
                              .Where(i => i.IDSATELITE == satelite.IDSATELITE)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Satelites.Add(satelite);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(satelite).State = EntityState.Detached;
                throw;
            }

            OnAfterSateliteCreated(satelite);

            return satelite;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Satelite> CancelSateliteChanges(Aldebaran.Web.Models.AldebaranContext.Satelite item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnSateliteUpdated(Aldebaran.Web.Models.AldebaranContext.Satelite item);
        partial void OnAfterSateliteUpdated(Aldebaran.Web.Models.AldebaranContext.Satelite item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Satelite> UpdateSatelite(int idsatelite, Aldebaran.Web.Models.AldebaranContext.Satelite satelite)
        {
            OnSateliteUpdated(satelite);

            var itemToUpdate = Context.Satelites
                              .Where(i => i.IDSATELITE == satelite.IDSATELITE)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(satelite);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterSateliteUpdated(satelite);

            return satelite;
        }

        partial void OnSateliteDeleted(Aldebaran.Web.Models.AldebaranContext.Satelite item);
        partial void OnAfterSateliteDeleted(Aldebaran.Web.Models.AldebaranContext.Satelite item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Satelite> DeleteSatelite(int idsatelite)
        {
            var itemToDelete = Context.Satelites
                              .Where(i => i.IDSATELITE == idsatelite)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnSateliteDeleted(itemToDelete);


            Context.Satelites.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterSateliteDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportStransitosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/stransitos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/stransitos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportStransitosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/stransitos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/stransitos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnStransitosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Stransito> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Stransito>> GetStransitos(Query query = null)
        {
            var items = Context.Stransitos.AsQueryable();


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

            OnStransitosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnStransitoGet(Aldebaran.Web.Models.AldebaranContext.Stransito item);
        partial void OnGetStransitoById(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Stransito> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Stransito> GetStransitoById(int id)
        {
            var items = Context.Stransitos
                              .AsNoTracking()
                              .Where(i => i.ID == id);


            OnGetStransitoById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnStransitoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnStransitoCreated(Aldebaran.Web.Models.AldebaranContext.Stransito item);
        partial void OnAfterStransitoCreated(Aldebaran.Web.Models.AldebaranContext.Stransito item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Stransito> CreateStransito(Aldebaran.Web.Models.AldebaranContext.Stransito stransito)
        {
            OnStransitoCreated(stransito);

            var existingItem = Context.Stransitos
                              .Where(i => i.ID == stransito.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Stransitos.Add(stransito);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(stransito).State = EntityState.Detached;
                throw;
            }

            OnAfterStransitoCreated(stransito);

            return stransito;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Stransito> CancelStransitoChanges(Aldebaran.Web.Models.AldebaranContext.Stransito item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnStransitoUpdated(Aldebaran.Web.Models.AldebaranContext.Stransito item);
        partial void OnAfterStransitoUpdated(Aldebaran.Web.Models.AldebaranContext.Stransito item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Stransito> UpdateStransito(int id, Aldebaran.Web.Models.AldebaranContext.Stransito stransito)
        {
            OnStransitoUpdated(stransito);

            var itemToUpdate = Context.Stransitos
                              .Where(i => i.ID == stransito.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(stransito);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterStransitoUpdated(stransito);

            return stransito;
        }

        partial void OnStransitoDeleted(Aldebaran.Web.Models.AldebaranContext.Stransito item);
        partial void OnAfterStransitoDeleted(Aldebaran.Web.Models.AldebaranContext.Stransito item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Stransito> DeleteStransito(int id)
        {
            var itemToDelete = Context.Stransitos
                              .Where(i => i.ID == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnStransitoDeleted(itemToDelete);


            Context.Stransitos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterStransitoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportSubitemdetenviosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/subitemdetenvios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/subitemdetenvios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportSubitemdetenviosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/subitemdetenvios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/subitemdetenvios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnSubitemdetenviosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio>> GetSubitemdetenvios(Query query = null)
        {
            var items = Context.Subitemdetenvios.AsQueryable();

            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Envio);
            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Itemsxcolor1);

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

            OnSubitemdetenviosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnSubitemdetenvioGet(Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio item);
        partial void OnGetSubitemdetenvioByIdsubitemdetenvio(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio> GetSubitemdetenvioByIdsubitemdetenvio(int idsubitemdetenvio)
        {
            var items = Context.Subitemdetenvios
                              .AsNoTracking()
                              .Where(i => i.IDSUBITEMDETENVIO == idsubitemdetenvio);

            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Envio);
            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Itemsxcolor1);

            OnGetSubitemdetenvioByIdsubitemdetenvio(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnSubitemdetenvioGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnSubitemdetenvioCreated(Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio item);
        partial void OnAfterSubitemdetenvioCreated(Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio> CreateSubitemdetenvio(Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio subitemdetenvio)
        {
            OnSubitemdetenvioCreated(subitemdetenvio);

            var existingItem = Context.Subitemdetenvios
                              .Where(i => i.IDSUBITEMDETENVIO == subitemdetenvio.IDSUBITEMDETENVIO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Subitemdetenvios.Add(subitemdetenvio);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(subitemdetenvio).State = EntityState.Detached;
                throw;
            }

            OnAfterSubitemdetenvioCreated(subitemdetenvio);

            return subitemdetenvio;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio> CancelSubitemdetenvioChanges(Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnSubitemdetenvioUpdated(Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio item);
        partial void OnAfterSubitemdetenvioUpdated(Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio> UpdateSubitemdetenvio(int idsubitemdetenvio, Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio subitemdetenvio)
        {
            OnSubitemdetenvioUpdated(subitemdetenvio);

            var itemToUpdate = Context.Subitemdetenvios
                              .Where(i => i.IDSUBITEMDETENVIO == subitemdetenvio.IDSUBITEMDETENVIO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(subitemdetenvio);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterSubitemdetenvioUpdated(subitemdetenvio);

            return subitemdetenvio;
        }

        partial void OnSubitemdetenvioDeleted(Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio item);
        partial void OnAfterSubitemdetenvioDeleted(Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio> DeleteSubitemdetenvio(int idsubitemdetenvio)
        {
            var itemToDelete = Context.Subitemdetenvios
                              .Where(i => i.IDSUBITEMDETENVIO == idsubitemdetenvio)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnSubitemdetenvioDeleted(itemToDelete);


            Context.Subitemdetenvios.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterSubitemdetenvioDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportSubitemdetprocesosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/subitemdetprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/subitemdetprocesos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportSubitemdetprocesosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/subitemdetprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/subitemdetprocesos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnSubitemdetprocesosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso>> GetSubitemdetprocesos(Query query = null)
        {
            var items = Context.Subitemdetprocesos.AsQueryable();

            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Itemsxcolor1);
            items = items.Include(i => i.Cantproceso);

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

            OnSubitemdetprocesosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnSubitemdetprocesoGet(Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso item);
        partial void OnGetSubitemdetprocesoByIdsubitemdetproceso(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso> GetSubitemdetprocesoByIdsubitemdetproceso(int idsubitemdetproceso)
        {
            var items = Context.Subitemdetprocesos
                              .AsNoTracking()
                              .Where(i => i.IDSUBITEMDETPROCESO == idsubitemdetproceso);

            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Item);
            items = items.Include(i => i.Itemsxcolor);
            items = items.Include(i => i.Itemsxcolor1);
            items = items.Include(i => i.Cantproceso);

            OnGetSubitemdetprocesoByIdsubitemdetproceso(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnSubitemdetprocesoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnSubitemdetprocesoCreated(Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso item);
        partial void OnAfterSubitemdetprocesoCreated(Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso> CreateSubitemdetproceso(Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso subitemdetproceso)
        {
            OnSubitemdetprocesoCreated(subitemdetproceso);

            var existingItem = Context.Subitemdetprocesos
                              .Where(i => i.IDSUBITEMDETPROCESO == subitemdetproceso.IDSUBITEMDETPROCESO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Subitemdetprocesos.Add(subitemdetproceso);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(subitemdetproceso).State = EntityState.Detached;
                throw;
            }

            OnAfterSubitemdetprocesoCreated(subitemdetproceso);

            return subitemdetproceso;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso> CancelSubitemdetprocesoChanges(Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnSubitemdetprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso item);
        partial void OnAfterSubitemdetprocesoUpdated(Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso> UpdateSubitemdetproceso(int idsubitemdetproceso, Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso subitemdetproceso)
        {
            OnSubitemdetprocesoUpdated(subitemdetproceso);

            var itemToUpdate = Context.Subitemdetprocesos
                              .Where(i => i.IDSUBITEMDETPROCESO == subitemdetproceso.IDSUBITEMDETPROCESO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(subitemdetproceso);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterSubitemdetprocesoUpdated(subitemdetproceso);

            return subitemdetproceso;
        }

        partial void OnSubitemdetprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso item);
        partial void OnAfterSubitemdetprocesoDeleted(Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso> DeleteSubitemdetproceso(int idsubitemdetproceso)
        {
            var itemToDelete = Context.Subitemdetprocesos
                              .Where(i => i.IDSUBITEMDETPROCESO == idsubitemdetproceso)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnSubitemdetprocesoDeleted(itemToDelete);


            Context.Subitemdetprocesos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterSubitemdetprocesoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportTempCuadreinventariosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/tempcuadreinventarios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/tempcuadreinventarios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTempCuadreinventariosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/tempcuadreinventarios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/tempcuadreinventarios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTempCuadreinventariosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.TempCuadreinventario> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.TempCuadreinventario>> GetTempCuadreinventarios(Query query = null)
        {
            var items = Context.TempCuadreinventarios.AsQueryable();


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

            OnTempCuadreinventariosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTempCuadreinventarioGet(Aldebaran.Web.Models.AldebaranContext.TempCuadreinventario item);
        partial void OnGetTempCuadreinventarioByIditemxcolor(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.TempCuadreinventario> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.TempCuadreinventario> GetTempCuadreinventarioByIditemxcolor(int iditemxcolor)
        {
            var items = Context.TempCuadreinventarios
                              .AsNoTracking()
                              .Where(i => i.IDITEMXCOLOR == iditemxcolor);


            OnGetTempCuadreinventarioByIditemxcolor(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTempCuadreinventarioGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTempCuadreinventarioCreated(Aldebaran.Web.Models.AldebaranContext.TempCuadreinventario item);
        partial void OnAfterTempCuadreinventarioCreated(Aldebaran.Web.Models.AldebaranContext.TempCuadreinventario item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.TempCuadreinventario> CreateTempCuadreinventario(Aldebaran.Web.Models.AldebaranContext.TempCuadreinventario tempcuadreinventario)
        {
            OnTempCuadreinventarioCreated(tempcuadreinventario);

            var existingItem = Context.TempCuadreinventarios
                              .Where(i => i.IDITEMXCOLOR == tempcuadreinventario.IDITEMXCOLOR)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.TempCuadreinventarios.Add(tempcuadreinventario);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(tempcuadreinventario).State = EntityState.Detached;
                throw;
            }

            OnAfterTempCuadreinventarioCreated(tempcuadreinventario);

            return tempcuadreinventario;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.TempCuadreinventario> CancelTempCuadreinventarioChanges(Aldebaran.Web.Models.AldebaranContext.TempCuadreinventario item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTempCuadreinventarioUpdated(Aldebaran.Web.Models.AldebaranContext.TempCuadreinventario item);
        partial void OnAfterTempCuadreinventarioUpdated(Aldebaran.Web.Models.AldebaranContext.TempCuadreinventario item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.TempCuadreinventario> UpdateTempCuadreinventario(int iditemxcolor, Aldebaran.Web.Models.AldebaranContext.TempCuadreinventario tempcuadreinventario)
        {
            OnTempCuadreinventarioUpdated(tempcuadreinventario);

            var itemToUpdate = Context.TempCuadreinventarios
                              .Where(i => i.IDITEMXCOLOR == tempcuadreinventario.IDITEMXCOLOR)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(tempcuadreinventario);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTempCuadreinventarioUpdated(tempcuadreinventario);

            return tempcuadreinventario;
        }

        partial void OnTempCuadreinventarioDeleted(Aldebaran.Web.Models.AldebaranContext.TempCuadreinventario item);
        partial void OnAfterTempCuadreinventarioDeleted(Aldebaran.Web.Models.AldebaranContext.TempCuadreinventario item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.TempCuadreinventario> DeleteTempCuadreinventario(int iditemxcolor)
        {
            var itemToDelete = Context.TempCuadreinventarios
                              .Where(i => i.IDITEMXCOLOR == iditemxcolor)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnTempCuadreinventarioDeleted(itemToDelete);


            Context.TempCuadreinventarios.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTempCuadreinventarioDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportTipidentificasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/tipidentificas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/tipidentificas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTipidentificasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/tipidentificas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/tipidentificas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTipidentificasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Tipidentifica> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Tipidentifica>> GetTipidentificas(Query query = null)
        {
            var items = Context.Tipidentificas.AsQueryable();


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

            OnTipidentificasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTipidentificaGet(Aldebaran.Web.Models.AldebaranContext.Tipidentifica item);
        partial void OnGetTipidentificaByIdtipidentifica(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Tipidentifica> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Tipidentifica> GetTipidentificaByIdtipidentifica(int idtipidentifica)
        {
            var items = Context.Tipidentificas
                              .AsNoTracking()
                              .Where(i => i.IDTIPIDENTIFICA == idtipidentifica);


            OnGetTipidentificaByIdtipidentifica(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTipidentificaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTipidentificaCreated(Aldebaran.Web.Models.AldebaranContext.Tipidentifica item);
        partial void OnAfterTipidentificaCreated(Aldebaran.Web.Models.AldebaranContext.Tipidentifica item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Tipidentifica> CreateTipidentifica(Aldebaran.Web.Models.AldebaranContext.Tipidentifica tipidentifica)
        {
            OnTipidentificaCreated(tipidentifica);

            var existingItem = Context.Tipidentificas
                              .Where(i => i.IDTIPIDENTIFICA == tipidentifica.IDTIPIDENTIFICA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Tipidentificas.Add(tipidentifica);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(tipidentifica).State = EntityState.Detached;
                throw;
            }

            OnAfterTipidentificaCreated(tipidentifica);

            return tipidentifica;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Tipidentifica> CancelTipidentificaChanges(Aldebaran.Web.Models.AldebaranContext.Tipidentifica item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTipidentificaUpdated(Aldebaran.Web.Models.AldebaranContext.Tipidentifica item);
        partial void OnAfterTipidentificaUpdated(Aldebaran.Web.Models.AldebaranContext.Tipidentifica item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Tipidentifica> UpdateTipidentifica(int idtipidentifica, Aldebaran.Web.Models.AldebaranContext.Tipidentifica tipidentifica)
        {
            OnTipidentificaUpdated(tipidentifica);

            var itemToUpdate = Context.Tipidentificas
                              .Where(i => i.IDTIPIDENTIFICA == tipidentifica.IDTIPIDENTIFICA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(tipidentifica);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTipidentificaUpdated(tipidentifica);

            return tipidentifica;
        }

        partial void OnTipidentificaDeleted(Aldebaran.Web.Models.AldebaranContext.Tipidentifica item);
        partial void OnAfterTipidentificaDeleted(Aldebaran.Web.Models.AldebaranContext.Tipidentifica item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Tipidentifica> DeleteTipidentifica(int idtipidentifica)
        {
            var itemToDelete = Context.Tipidentificas
                              .Where(i => i.IDTIPIDENTIFICA == idtipidentifica)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnTipidentificaDeleted(itemToDelete);


            Context.Tipidentificas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTipidentificaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportTiposactividadsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/tiposactividads/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/tiposactividads/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTiposactividadsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/tiposactividads/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/tiposactividads/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTiposactividadsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Tiposactividad> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Tiposactividad>> GetTiposactividads(Query query = null)
        {
            var items = Context.Tiposactividads.AsQueryable();


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

            OnTiposactividadsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTiposactividadGet(Aldebaran.Web.Models.AldebaranContext.Tiposactividad item);
        partial void OnGetTiposactividadByIdtipoactividad(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Tiposactividad> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Tiposactividad> GetTiposactividadByIdtipoactividad(int idtipoactividad)
        {
            var items = Context.Tiposactividads
                              .AsNoTracking()
                              .Where(i => i.IDTIPOACTIVIDAD == idtipoactividad);


            OnGetTiposactividadByIdtipoactividad(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTiposactividadGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTiposactividadCreated(Aldebaran.Web.Models.AldebaranContext.Tiposactividad item);
        partial void OnAfterTiposactividadCreated(Aldebaran.Web.Models.AldebaranContext.Tiposactividad item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Tiposactividad> CreateTiposactividad(Aldebaran.Web.Models.AldebaranContext.Tiposactividad tiposactividad)
        {
            OnTiposactividadCreated(tiposactividad);

            var existingItem = Context.Tiposactividads
                              .Where(i => i.IDTIPOACTIVIDAD == tiposactividad.IDTIPOACTIVIDAD)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Tiposactividads.Add(tiposactividad);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(tiposactividad).State = EntityState.Detached;
                throw;
            }

            OnAfterTiposactividadCreated(tiposactividad);

            return tiposactividad;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Tiposactividad> CancelTiposactividadChanges(Aldebaran.Web.Models.AldebaranContext.Tiposactividad item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTiposactividadUpdated(Aldebaran.Web.Models.AldebaranContext.Tiposactividad item);
        partial void OnAfterTiposactividadUpdated(Aldebaran.Web.Models.AldebaranContext.Tiposactividad item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Tiposactividad> UpdateTiposactividad(int idtipoactividad, Aldebaran.Web.Models.AldebaranContext.Tiposactividad tiposactividad)
        {
            OnTiposactividadUpdated(tiposactividad);

            var itemToUpdate = Context.Tiposactividads
                              .Where(i => i.IDTIPOACTIVIDAD == tiposactividad.IDTIPOACTIVIDAD)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(tiposactividad);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTiposactividadUpdated(tiposactividad);

            return tiposactividad;
        }

        partial void OnTiposactividadDeleted(Aldebaran.Web.Models.AldebaranContext.Tiposactividad item);
        partial void OnAfterTiposactividadDeleted(Aldebaran.Web.Models.AldebaranContext.Tiposactividad item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Tiposactividad> DeleteTiposactividad(int idtipoactividad)
        {
            var itemToDelete = Context.Tiposactividads
                              .Where(i => i.IDTIPOACTIVIDAD == idtipoactividad)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnTiposactividadDeleted(itemToDelete);


            Context.Tiposactividads.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTiposactividadDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportTiposactxareasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/tiposactxareas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/tiposactxareas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTiposactxareasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/tiposactxareas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/tiposactxareas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTiposactxareasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Tiposactxarea> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Tiposactxarea>> GetTiposactxareas(Query query = null)
        {
            var items = Context.Tiposactxareas.AsQueryable();

            items = items.Include(i => i.Area);
            items = items.Include(i => i.Tiposactividad);

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

            OnTiposactxareasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTiposactxareaGet(Aldebaran.Web.Models.AldebaranContext.Tiposactxarea item);
        partial void OnGetTiposactxareaByIdtipoactividadAndIdarea(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Tiposactxarea> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Tiposactxarea> GetTiposactxareaByIdtipoactividadAndIdarea(int idtipoactividad, int idarea)
        {
            var items = Context.Tiposactxareas
                              .AsNoTracking()
                              .Where(i => i.IDTIPOACTIVIDAD == idtipoactividad && i.IDAREA == idarea);

            items = items.Include(i => i.Area);
            items = items.Include(i => i.Tiposactividad);

            OnGetTiposactxareaByIdtipoactividadAndIdarea(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTiposactxareaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTiposactxareaCreated(Aldebaran.Web.Models.AldebaranContext.Tiposactxarea item);
        partial void OnAfterTiposactxareaCreated(Aldebaran.Web.Models.AldebaranContext.Tiposactxarea item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Tiposactxarea> CreateTiposactxarea(Aldebaran.Web.Models.AldebaranContext.Tiposactxarea tiposactxarea)
        {
            OnTiposactxareaCreated(tiposactxarea);

            var existingItem = Context.Tiposactxareas
                              .Where(i => i.IDTIPOACTIVIDAD == tiposactxarea.IDTIPOACTIVIDAD && i.IDAREA == tiposactxarea.IDAREA)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Tiposactxareas.Add(tiposactxarea);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(tiposactxarea).State = EntityState.Detached;
                throw;
            }

            OnAfterTiposactxareaCreated(tiposactxarea);

            return tiposactxarea;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Tiposactxarea> CancelTiposactxareaChanges(Aldebaran.Web.Models.AldebaranContext.Tiposactxarea item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTiposactxareaUpdated(Aldebaran.Web.Models.AldebaranContext.Tiposactxarea item);
        partial void OnAfterTiposactxareaUpdated(Aldebaran.Web.Models.AldebaranContext.Tiposactxarea item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Tiposactxarea> UpdateTiposactxarea(int idtipoactividad, int idarea, Aldebaran.Web.Models.AldebaranContext.Tiposactxarea tiposactxarea)
        {
            OnTiposactxareaUpdated(tiposactxarea);

            var itemToUpdate = Context.Tiposactxareas
                              .Where(i => i.IDTIPOACTIVIDAD == tiposactxarea.IDTIPOACTIVIDAD && i.IDAREA == tiposactxarea.IDAREA)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(tiposactxarea);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTiposactxareaUpdated(tiposactxarea);

            return tiposactxarea;
        }

        partial void OnTiposactxareaDeleted(Aldebaran.Web.Models.AldebaranContext.Tiposactxarea item);
        partial void OnAfterTiposactxareaDeleted(Aldebaran.Web.Models.AldebaranContext.Tiposactxarea item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Tiposactxarea> DeleteTiposactxarea(int idtipoactividad, int idarea)
        {
            var itemToDelete = Context.Tiposactxareas
                              .Where(i => i.IDTIPOACTIVIDAD == idtipoactividad && i.IDAREA == idarea)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnTiposactxareaDeleted(itemToDelete);


            Context.Tiposactxareas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTiposactxareaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportTiposalarmasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/tiposalarmas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/tiposalarmas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTiposalarmasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/tiposalarmas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/tiposalarmas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTiposalarmasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Tiposalarma> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Tiposalarma>> GetTiposalarmas(Query query = null)
        {
            var items = Context.Tiposalarmas.AsQueryable();


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

            OnTiposalarmasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTiposalarmaGet(Aldebaran.Web.Models.AldebaranContext.Tiposalarma item);
        partial void OnGetTiposalarmaByIdtipo(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Tiposalarma> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Tiposalarma> GetTiposalarmaByIdtipo(int idtipo)
        {
            var items = Context.Tiposalarmas
                              .AsNoTracking()
                              .Where(i => i.IDTIPO == idtipo);


            OnGetTiposalarmaByIdtipo(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTiposalarmaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTiposalarmaCreated(Aldebaran.Web.Models.AldebaranContext.Tiposalarma item);
        partial void OnAfterTiposalarmaCreated(Aldebaran.Web.Models.AldebaranContext.Tiposalarma item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Tiposalarma> CreateTiposalarma(Aldebaran.Web.Models.AldebaranContext.Tiposalarma tiposalarma)
        {
            OnTiposalarmaCreated(tiposalarma);

            var existingItem = Context.Tiposalarmas
                              .Where(i => i.IDTIPO == tiposalarma.IDTIPO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Tiposalarmas.Add(tiposalarma);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(tiposalarma).State = EntityState.Detached;
                throw;
            }

            OnAfterTiposalarmaCreated(tiposalarma);

            return tiposalarma;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Tiposalarma> CancelTiposalarmaChanges(Aldebaran.Web.Models.AldebaranContext.Tiposalarma item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTiposalarmaUpdated(Aldebaran.Web.Models.AldebaranContext.Tiposalarma item);
        partial void OnAfterTiposalarmaUpdated(Aldebaran.Web.Models.AldebaranContext.Tiposalarma item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Tiposalarma> UpdateTiposalarma(int idtipo, Aldebaran.Web.Models.AldebaranContext.Tiposalarma tiposalarma)
        {
            OnTiposalarmaUpdated(tiposalarma);

            var itemToUpdate = Context.Tiposalarmas
                              .Where(i => i.IDTIPO == tiposalarma.IDTIPO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(tiposalarma);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTiposalarmaUpdated(tiposalarma);

            return tiposalarma;
        }

        partial void OnTiposalarmaDeleted(Aldebaran.Web.Models.AldebaranContext.Tiposalarma item);
        partial void OnAfterTiposalarmaDeleted(Aldebaran.Web.Models.AldebaranContext.Tiposalarma item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Tiposalarma> DeleteTiposalarma(int idtipo)
        {
            var itemToDelete = Context.Tiposalarmas
                              .Where(i => i.IDTIPO == idtipo)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnTiposalarmaDeleted(itemToDelete);


            Context.Tiposalarmas.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTiposalarmaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportTrasladosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/traslados/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/traslados/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTrasladosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/traslados/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/traslados/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTrasladosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Traslado> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Traslado>> GetTraslados(Query query = null)
        {
            var items = Context.Traslados.AsQueryable();

            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Bodega1);

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

            OnTrasladosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTrasladoGet(Aldebaran.Web.Models.AldebaranContext.Traslado item);
        partial void OnGetTrasladoByIdtraslado(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Traslado> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Traslado> GetTrasladoByIdtraslado(int idtraslado)
        {
            var items = Context.Traslados
                              .AsNoTracking()
                              .Where(i => i.IDTRASLADO == idtraslado);

            items = items.Include(i => i.Bodega);
            items = items.Include(i => i.Bodega1);

            OnGetTrasladoByIdtraslado(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTrasladoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTrasladoCreated(Aldebaran.Web.Models.AldebaranContext.Traslado item);
        partial void OnAfterTrasladoCreated(Aldebaran.Web.Models.AldebaranContext.Traslado item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Traslado> CreateTraslado(Aldebaran.Web.Models.AldebaranContext.Traslado traslado)
        {
            OnTrasladoCreated(traslado);

            var existingItem = Context.Traslados
                              .Where(i => i.IDTRASLADO == traslado.IDTRASLADO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Traslados.Add(traslado);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(traslado).State = EntityState.Detached;
                throw;
            }

            OnAfterTrasladoCreated(traslado);

            return traslado;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Traslado> CancelTrasladoChanges(Aldebaran.Web.Models.AldebaranContext.Traslado item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTrasladoUpdated(Aldebaran.Web.Models.AldebaranContext.Traslado item);
        partial void OnAfterTrasladoUpdated(Aldebaran.Web.Models.AldebaranContext.Traslado item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Traslado> UpdateTraslado(int idtraslado, Aldebaran.Web.Models.AldebaranContext.Traslado traslado)
        {
            OnTrasladoUpdated(traslado);

            var itemToUpdate = Context.Traslados
                              .Where(i => i.IDTRASLADO == traslado.IDTRASLADO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(traslado);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTrasladoUpdated(traslado);

            return traslado;
        }

        partial void OnTrasladoDeleted(Aldebaran.Web.Models.AldebaranContext.Traslado item);
        partial void OnAfterTrasladoDeleted(Aldebaran.Web.Models.AldebaranContext.Traslado item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Traslado> DeleteTraslado(int idtraslado)
        {
            var itemToDelete = Context.Traslados
                              .Where(i => i.IDTRASLADO == idtraslado)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnTrasladoDeleted(itemToDelete);


            Context.Traslados.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTrasladoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportUnidadesmedidaToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/unidadesmedida/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/unidadesmedida/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportUnidadesmedidaToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/unidadesmedida/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/unidadesmedida/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnUnidadesmedidaRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Unidadesmedidum> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Unidadesmedidum>> GetUnidadesmedida(Query query = null)
        {
            var items = Context.Unidadesmedida.AsQueryable();


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

            OnUnidadesmedidaRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnUnidadesmedidumGet(Aldebaran.Web.Models.AldebaranContext.Unidadesmedidum item);
        partial void OnGetUnidadesmedidumByIdunidad(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Unidadesmedidum> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Unidadesmedidum> GetUnidadesmedidumByIdunidad(int idunidad)
        {
            var items = Context.Unidadesmedida
                              .AsNoTracking()
                              .Where(i => i.IDUNIDAD == idunidad);


            OnGetUnidadesmedidumByIdunidad(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnUnidadesmedidumGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnUnidadesmedidumCreated(Aldebaran.Web.Models.AldebaranContext.Unidadesmedidum item);
        partial void OnAfterUnidadesmedidumCreated(Aldebaran.Web.Models.AldebaranContext.Unidadesmedidum item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Unidadesmedidum> CreateUnidadesmedidum(Aldebaran.Web.Models.AldebaranContext.Unidadesmedidum unidadesmedidum)
        {
            OnUnidadesmedidumCreated(unidadesmedidum);

            var existingItem = Context.Unidadesmedida
                              .Where(i => i.IDUNIDAD == unidadesmedidum.IDUNIDAD)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Unidadesmedida.Add(unidadesmedidum);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(unidadesmedidum).State = EntityState.Detached;
                throw;
            }

            OnAfterUnidadesmedidumCreated(unidadesmedidum);

            return unidadesmedidum;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Unidadesmedidum> CancelUnidadesmedidumChanges(Aldebaran.Web.Models.AldebaranContext.Unidadesmedidum item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnUnidadesmedidumUpdated(Aldebaran.Web.Models.AldebaranContext.Unidadesmedidum item);
        partial void OnAfterUnidadesmedidumUpdated(Aldebaran.Web.Models.AldebaranContext.Unidadesmedidum item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Unidadesmedidum> UpdateUnidadesmedidum(int idunidad, Aldebaran.Web.Models.AldebaranContext.Unidadesmedidum unidadesmedidum)
        {
            OnUnidadesmedidumUpdated(unidadesmedidum);

            var itemToUpdate = Context.Unidadesmedida
                              .Where(i => i.IDUNIDAD == unidadesmedidum.IDUNIDAD)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(unidadesmedidum);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterUnidadesmedidumUpdated(unidadesmedidum);

            return unidadesmedidum;
        }

        partial void OnUnidadesmedidumDeleted(Aldebaran.Web.Models.AldebaranContext.Unidadesmedidum item);
        partial void OnAfterUnidadesmedidumDeleted(Aldebaran.Web.Models.AldebaranContext.Unidadesmedidum item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Unidadesmedidum> DeleteUnidadesmedidum(int idunidad)
        {
            var itemToDelete = Context.Unidadesmedida
                              .Where(i => i.IDUNIDAD == idunidad)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnUnidadesmedidumDeleted(itemToDelete);


            Context.Unidadesmedida.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterUnidadesmedidumDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportUsuariosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/usuarios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/usuarios/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportUsuariosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/usuarios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/usuarios/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnUsuariosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Usuario> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Usuario>> GetUsuarios(Query query = null)
        {
            var items = Context.Usuarios.AsQueryable();

            items = items.Include(i => i.Funcionario);

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

            OnUsuariosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnUsuarioGet(Aldebaran.Web.Models.AldebaranContext.Usuario item);
        partial void OnGetUsuarioByIdusuario(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Usuario> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Usuario> GetUsuarioByIdusuario(int idusuario)
        {
            var items = Context.Usuarios
                              .AsNoTracking()
                              .Where(i => i.IDUSUARIO == idusuario);

            items = items.Include(i => i.Funcionario);

            OnGetUsuarioByIdusuario(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnUsuarioGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnUsuarioCreated(Aldebaran.Web.Models.AldebaranContext.Usuario item);
        partial void OnAfterUsuarioCreated(Aldebaran.Web.Models.AldebaranContext.Usuario item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Usuario> CreateUsuario(Aldebaran.Web.Models.AldebaranContext.Usuario usuario)
        {
            OnUsuarioCreated(usuario);

            var existingItem = Context.Usuarios
                              .Where(i => i.IDUSUARIO == usuario.IDUSUARIO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Usuarios.Add(usuario);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(usuario).State = EntityState.Detached;
                throw;
            }

            OnAfterUsuarioCreated(usuario);

            return usuario;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Usuario> CancelUsuarioChanges(Aldebaran.Web.Models.AldebaranContext.Usuario item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnUsuarioUpdated(Aldebaran.Web.Models.AldebaranContext.Usuario item);
        partial void OnAfterUsuarioUpdated(Aldebaran.Web.Models.AldebaranContext.Usuario item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Usuario> UpdateUsuario(int idusuario, Aldebaran.Web.Models.AldebaranContext.Usuario usuario)
        {
            OnUsuarioUpdated(usuario);

            var itemToUpdate = Context.Usuarios
                              .Where(i => i.IDUSUARIO == usuario.IDUSUARIO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(usuario);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterUsuarioUpdated(usuario);

            return usuario;
        }

        partial void OnUsuarioDeleted(Aldebaran.Web.Models.AldebaranContext.Usuario item);
        partial void OnAfterUsuarioDeleted(Aldebaran.Web.Models.AldebaranContext.Usuario item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Usuario> DeleteUsuario(int idusuario)
        {
            var itemToDelete = Context.Usuarios
                              .Where(i => i.IDUSUARIO == idusuario)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnUsuarioDeleted(itemToDelete);


            Context.Usuarios.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterUsuarioDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportUsuarioscorreoseguimientosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/usuarioscorreoseguimientos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/usuarioscorreoseguimientos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportUsuarioscorreoseguimientosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/usuarioscorreoseguimientos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/usuarioscorreoseguimientos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnUsuarioscorreoseguimientosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Usuarioscorreoseguimiento> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Usuarioscorreoseguimiento>> GetUsuarioscorreoseguimientos(Query query = null)
        {
            var items = Context.Usuarioscorreoseguimientos.AsQueryable();


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

            OnUsuarioscorreoseguimientosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnUsuarioscorreoseguimientoGet(Aldebaran.Web.Models.AldebaranContext.Usuarioscorreoseguimiento item);
        partial void OnGetUsuarioscorreoseguimientoByIdfuncionario(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Usuarioscorreoseguimiento> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Usuarioscorreoseguimiento> GetUsuarioscorreoseguimientoByIdfuncionario(int idfuncionario)
        {
            var items = Context.Usuarioscorreoseguimientos
                              .AsNoTracking()
                              .Where(i => i.IDFUNCIONARIO == idfuncionario);


            OnGetUsuarioscorreoseguimientoByIdfuncionario(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnUsuarioscorreoseguimientoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnUsuarioscorreoseguimientoCreated(Aldebaran.Web.Models.AldebaranContext.Usuarioscorreoseguimiento item);
        partial void OnAfterUsuarioscorreoseguimientoCreated(Aldebaran.Web.Models.AldebaranContext.Usuarioscorreoseguimiento item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Usuarioscorreoseguimiento> CreateUsuarioscorreoseguimiento(Aldebaran.Web.Models.AldebaranContext.Usuarioscorreoseguimiento usuarioscorreoseguimiento)
        {
            OnUsuarioscorreoseguimientoCreated(usuarioscorreoseguimiento);

            var existingItem = Context.Usuarioscorreoseguimientos
                              .Where(i => i.IDFUNCIONARIO == usuarioscorreoseguimiento.IDFUNCIONARIO)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Usuarioscorreoseguimientos.Add(usuarioscorreoseguimiento);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(usuarioscorreoseguimiento).State = EntityState.Detached;
                throw;
            }

            OnAfterUsuarioscorreoseguimientoCreated(usuarioscorreoseguimiento);

            return usuarioscorreoseguimiento;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Usuarioscorreoseguimiento> CancelUsuarioscorreoseguimientoChanges(Aldebaran.Web.Models.AldebaranContext.Usuarioscorreoseguimiento item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnUsuarioscorreoseguimientoUpdated(Aldebaran.Web.Models.AldebaranContext.Usuarioscorreoseguimiento item);
        partial void OnAfterUsuarioscorreoseguimientoUpdated(Aldebaran.Web.Models.AldebaranContext.Usuarioscorreoseguimiento item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Usuarioscorreoseguimiento> UpdateUsuarioscorreoseguimiento(int idfuncionario, Aldebaran.Web.Models.AldebaranContext.Usuarioscorreoseguimiento usuarioscorreoseguimiento)
        {
            OnUsuarioscorreoseguimientoUpdated(usuarioscorreoseguimiento);

            var itemToUpdate = Context.Usuarioscorreoseguimientos
                              .Where(i => i.IDFUNCIONARIO == usuarioscorreoseguimiento.IDFUNCIONARIO)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(usuarioscorreoseguimiento);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterUsuarioscorreoseguimientoUpdated(usuarioscorreoseguimiento);

            return usuarioscorreoseguimiento;
        }

        partial void OnUsuarioscorreoseguimientoDeleted(Aldebaran.Web.Models.AldebaranContext.Usuarioscorreoseguimiento item);
        partial void OnAfterUsuarioscorreoseguimientoDeleted(Aldebaran.Web.Models.AldebaranContext.Usuarioscorreoseguimiento item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Usuarioscorreoseguimiento> DeleteUsuarioscorreoseguimiento(int idfuncionario)
        {
            var itemToDelete = Context.Usuarioscorreoseguimientos
                              .Where(i => i.IDFUNCIONARIO == idfuncionario)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnUsuarioscorreoseguimientoDeleted(itemToDelete);


            Context.Usuarioscorreoseguimientos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterUsuarioscorreoseguimientoDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportValidacioncomprometidosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/validacioncomprometidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/validacioncomprometidos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportValidacioncomprometidosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarancontext/validacioncomprometidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarancontext/validacioncomprometidos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnValidacioncomprometidosRead(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Validacioncomprometido> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranContext.Validacioncomprometido>> GetValidacioncomprometidos(Query query = null)
        {
            var items = Context.Validacioncomprometidos.AsQueryable();


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

            OnValidacioncomprometidosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnValidacioncomprometidoGet(Aldebaran.Web.Models.AldebaranContext.Validacioncomprometido item);
        partial void OnGetValidacioncomprometidoByIditemxcolor(ref IQueryable<Aldebaran.Web.Models.AldebaranContext.Validacioncomprometido> items);


        public async Task<Aldebaran.Web.Models.AldebaranContext.Validacioncomprometido> GetValidacioncomprometidoByIditemxcolor(int iditemxcolor)
        {
            var items = Context.Validacioncomprometidos
                              .AsNoTracking()
                              .Where(i => i.IDITEMXCOLOR == iditemxcolor);


            OnGetValidacioncomprometidoByIditemxcolor(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnValidacioncomprometidoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnValidacioncomprometidoCreated(Aldebaran.Web.Models.AldebaranContext.Validacioncomprometido item);
        partial void OnAfterValidacioncomprometidoCreated(Aldebaran.Web.Models.AldebaranContext.Validacioncomprometido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Validacioncomprometido> CreateValidacioncomprometido(Aldebaran.Web.Models.AldebaranContext.Validacioncomprometido validacioncomprometido)
        {
            OnValidacioncomprometidoCreated(validacioncomprometido);

            var existingItem = Context.Validacioncomprometidos
                              .Where(i => i.IDITEMXCOLOR == validacioncomprometido.IDITEMXCOLOR)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Validacioncomprometidos.Add(validacioncomprometido);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(validacioncomprometido).State = EntityState.Detached;
                throw;
            }

            OnAfterValidacioncomprometidoCreated(validacioncomprometido);

            return validacioncomprometido;
        }

        public async Task<Aldebaran.Web.Models.AldebaranContext.Validacioncomprometido> CancelValidacioncomprometidoChanges(Aldebaran.Web.Models.AldebaranContext.Validacioncomprometido item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnValidacioncomprometidoUpdated(Aldebaran.Web.Models.AldebaranContext.Validacioncomprometido item);
        partial void OnAfterValidacioncomprometidoUpdated(Aldebaran.Web.Models.AldebaranContext.Validacioncomprometido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Validacioncomprometido> UpdateValidacioncomprometido(int iditemxcolor, Aldebaran.Web.Models.AldebaranContext.Validacioncomprometido validacioncomprometido)
        {
            OnValidacioncomprometidoUpdated(validacioncomprometido);

            var itemToUpdate = Context.Validacioncomprometidos
                              .Where(i => i.IDITEMXCOLOR == validacioncomprometido.IDITEMXCOLOR)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(validacioncomprometido);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterValidacioncomprometidoUpdated(validacioncomprometido);

            return validacioncomprometido;
        }

        partial void OnValidacioncomprometidoDeleted(Aldebaran.Web.Models.AldebaranContext.Validacioncomprometido item);
        partial void OnAfterValidacioncomprometidoDeleted(Aldebaran.Web.Models.AldebaranContext.Validacioncomprometido item);

        public async Task<Aldebaran.Web.Models.AldebaranContext.Validacioncomprometido> DeleteValidacioncomprometido(int iditemxcolor)
        {
            var itemToDelete = Context.Validacioncomprometidos
                              .Where(i => i.IDITEMXCOLOR == iditemxcolor)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnValidacioncomprometidoDeleted(itemToDelete);


            Context.Validacioncomprometidos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterValidacioncomprometidoDeleted(itemToDelete);

            return itemToDelete;
        }
    }
}