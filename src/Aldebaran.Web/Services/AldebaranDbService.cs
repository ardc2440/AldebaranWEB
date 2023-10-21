using Aldebaran.Web.Data;
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

        public async Task ExportAreasToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/areas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/areas/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAreasToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/areas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/areas/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAreasRead(ref IQueryable<Models.AldebaranDb.Area> items);

        public async Task<IQueryable<Models.AldebaranDb.Area>> GetAreas(Query query = null)
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

        partial void OnAreaGet(Models.AldebaranDb.Area item);
        partial void OnGetAreaByAreaId(ref IQueryable<Models.AldebaranDb.Area> items);

        public async Task<Models.AldebaranDb.Area> GetAreaByAreaId(short areaid)
        {
            var items = Context.Areas
                              .AsNoTracking()
                              .Where(i => i.AREA_ID == areaid);


            OnGetAreaByAreaId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAreaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAreaCreated(Models.AldebaranDb.Area item);

        partial void OnAfterAreaCreated(Models.AldebaranDb.Area item);

        public async Task<Models.AldebaranDb.Area> CreateArea(Models.AldebaranDb.Area area)
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

        public async Task<Models.AldebaranDb.Area> CancelAreaChanges(Models.AldebaranDb.Area item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAreaUpdated(Models.AldebaranDb.Area item);

        partial void OnAfterAreaUpdated(Models.AldebaranDb.Area item);

        public async Task<Models.AldebaranDb.Area> UpdateArea(short areaid, Models.AldebaranDb.Area area)
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

        partial void OnAreaDeleted(Models.AldebaranDb.Area item);

        partial void OnAdjustmentDeleted(Models.AldebaranDb.Adjustment item);

        partial void OnAdjustmentDetailDeleted(Models.AldebaranDb.AdjustmentDetail item);

        partial void OnAfterAreaDeleted(Models.AldebaranDb.Area item);

        partial void OnAfterAdjustmentDeleted(Models.AldebaranDb.Adjustment item);

        partial void OnAfterAdjustmentDetailDeleted(Models.AldebaranDb.AdjustmentDetail item);

        public async Task<Models.AldebaranDb.Area> DeleteArea(short areaid)
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

        public async Task<Models.AldebaranDb.Adjustment> DeleteAdjustment(int adjustmentId)
        {
            var itemToDelete = Context.Adjustments
                              .Where(i => i.ADJUSTMENT_ID == adjustmentId)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnAdjustmentDeleted(itemToDelete);


            Context.Adjustments.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAdjustmentDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task<Models.AldebaranDb.AdjustmentDetail> DeleteAdjustmentDetail(int adjustmentDetailId)
        {
            var itemToDelete = Context.AdjustmentDetails
                              .Where(i => i.ADJUSTMENT_DETAIL_ID == adjustmentDetailId)
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

        public async Task ExportCitiesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/cities/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/cities/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCitiesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/cities/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/cities/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCitiesRead(ref IQueryable<Models.AldebaranDb.City> items);

        public async Task<IQueryable<Models.AldebaranDb.City>> GetCities(Query query = null)
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

        partial void OnCityGet(Models.AldebaranDb.City item);

        partial void OnGetCityByCityId(ref IQueryable<Models.AldebaranDb.City> items);

        public async Task<Models.AldebaranDb.City> GetCityByCityId(int cityid)
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

        partial void OnCityCreated(Models.AldebaranDb.City item);

        partial void OnAfterCityCreated(Models.AldebaranDb.City item);

        public async Task<Models.AldebaranDb.City> CreateCity(Models.AldebaranDb.City city)
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

        public async Task<Models.AldebaranDb.City> CancelCityChanges(Models.AldebaranDb.City item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCityUpdated(Models.AldebaranDb.City item);

        partial void OnAfterCityUpdated(Models.AldebaranDb.City item);

        public async Task<Models.AldebaranDb.City> UpdateCity(int cityid, Models.AldebaranDb.City city)
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

        partial void OnCityDeleted(Models.AldebaranDb.City item);

        partial void OnAfterCityDeleted(Models.AldebaranDb.City item);

        public async Task<Models.AldebaranDb.City> DeleteCity(int cityid)
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

        partial void OnCountriesRead(ref IQueryable<Models.AldebaranDb.Country> items);

        public async Task<IQueryable<Models.AldebaranDb.Country>> GetCountries(Query query = null)
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

        partial void OnCountryGet(Models.AldebaranDb.Country item);

        partial void OnGetCountryByCountryId(ref IQueryable<Models.AldebaranDb.Country> items);

        public async Task<Models.AldebaranDb.Country> GetCountryByCountryId(short countryid)
        {
            var items = Context.Countries
                              .AsNoTracking()
                              .Where(i => i.COUNTRY_ID == countryid);


            OnGetCountryByCountryId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCountryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCountryCreated(Models.AldebaranDb.Country item);

        partial void OnAfterCountryCreated(Models.AldebaranDb.Country item);

        public async Task<Models.AldebaranDb.Country> CreateCountry(Models.AldebaranDb.Country country)
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

        public async Task<Models.AldebaranDb.Country> CancelCountryChanges(Models.AldebaranDb.Country item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCountryUpdated(Models.AldebaranDb.Country item);

        partial void OnAfterCountryUpdated(Models.AldebaranDb.Country item);

        public async Task<Models.AldebaranDb.Country> UpdateCountry(short countryid, Models.AldebaranDb.Country country)
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

        partial void OnCountryDeleted(Models.AldebaranDb.Country item);

        partial void OnAfterCountryDeleted(Models.AldebaranDb.Country item);

        public async Task<Models.AldebaranDb.Country> DeleteCountry(short countryid)
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

        partial void OnCurrenciesRead(ref IQueryable<Models.AldebaranDb.Currency> items);

        public async Task<IQueryable<Models.AldebaranDb.Currency>> GetCurrencies(Query query = null)
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

        partial void OnCurrencyGet(Models.AldebaranDb.Currency item);

        partial void OnGetCurrencyByCurrencyId(ref IQueryable<Models.AldebaranDb.Currency> items);

        public async Task<Models.AldebaranDb.Currency> GetCurrencyByCurrencyId(short currencyid)
        {
            var items = Context.Currencies
                              .AsNoTracking()
                              .Where(i => i.CURRENCY_ID == currencyid);


            OnGetCurrencyByCurrencyId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCurrencyGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCurrencyCreated(Models.AldebaranDb.Currency item);

        partial void OnAfterCurrencyCreated(Models.AldebaranDb.Currency item);

        public async Task<Models.AldebaranDb.Currency> CreateCurrency(Models.AldebaranDb.Currency currency)
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

        public async Task<Models.AldebaranDb.Currency> CancelCurrencyChanges(Models.AldebaranDb.Currency item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCurrencyUpdated(Models.AldebaranDb.Currency item);

        partial void OnAfterCurrencyUpdated(Models.AldebaranDb.Currency item);

        public async Task<Models.AldebaranDb.Currency> UpdateCurrency(short currencyid, Models.AldebaranDb.Currency currency)
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

        partial void OnCurrencyDeleted(Models.AldebaranDb.Currency item);

        partial void OnAfterCurrencyDeleted(Models.AldebaranDb.Currency item);

        public async Task<Models.AldebaranDb.Currency> DeleteCurrency(short currencyid)
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

        partial void OnCustomerContactsRead(ref IQueryable<Models.AldebaranDb.CustomerContact> items);

        public async Task<IQueryable<Models.AldebaranDb.CustomerContact>> GetCustomerContacts(Query query = null)
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

        partial void OnCustomerContactGet(Models.AldebaranDb.CustomerContact item);

        partial void OnGetCustomerContactByCustomerContactId(ref IQueryable<Models.AldebaranDb.CustomerContact> items);

        public async Task<Models.AldebaranDb.CustomerContact> GetCustomerContactByCustomerContactId(int customercontactid)
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

        partial void OnCustomerContactCreated(Models.AldebaranDb.CustomerContact item);

        partial void OnAfterCustomerContactCreated(Models.AldebaranDb.CustomerContact item);

        public async Task<Models.AldebaranDb.CustomerContact> CreateCustomerContact(Models.AldebaranDb.CustomerContact customercontact)
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

        public async Task<Models.AldebaranDb.CustomerContact> CancelCustomerContactChanges(Models.AldebaranDb.CustomerContact item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCustomerContactUpdated(Models.AldebaranDb.CustomerContact item);

        partial void OnAfterCustomerContactUpdated(Models.AldebaranDb.CustomerContact item);

        public async Task<Models.AldebaranDb.CustomerContact> UpdateCustomerContact(int customercontactid, Models.AldebaranDb.CustomerContact customercontact)
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

        partial void OnCustomerContactDeleted(Models.AldebaranDb.CustomerContact item);

        partial void OnAfterCustomerContactDeleted(Models.AldebaranDb.CustomerContact item);

        public async Task<Models.AldebaranDb.CustomerContact> DeleteCustomerContact(int customercontactid)
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

        partial void OnCustomersRead(ref IQueryable<Models.AldebaranDb.Customer> items);

        public async Task<IQueryable<Models.AldebaranDb.Customer>> GetCustomers(Query query = null)
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

        partial void OnCustomerGet(Models.AldebaranDb.Customer item);

        partial void OnGetCustomerByCustomerId(ref IQueryable<Models.AldebaranDb.Customer> items);

        public async Task<Models.AldebaranDb.Customer> GetCustomerByCustomerId(int customerid)
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

        partial void OnCustomerCreated(Models.AldebaranDb.Customer item);

        partial void OnAfterCustomerCreated(Models.AldebaranDb.Customer item);

        public async Task<Models.AldebaranDb.Customer> CreateCustomer(Models.AldebaranDb.Customer customer)
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

        public async Task<Models.AldebaranDb.Customer> CancelCustomerChanges(Models.AldebaranDb.Customer item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCustomerUpdated(Models.AldebaranDb.Customer item);

        partial void OnAfterCustomerUpdated(Models.AldebaranDb.Customer item);

        public async Task<Models.AldebaranDb.Customer> UpdateCustomer(int customerid, Models.AldebaranDb.Customer customer)
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

        partial void OnCustomerDeleted(Models.AldebaranDb.Customer item);

        partial void OnAfterCustomerDeleted(Models.AldebaranDb.Customer item);

        public async Task<Models.AldebaranDb.Customer> DeleteCustomer(int customerid)
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

        partial void OnDepartmentsRead(ref IQueryable<Models.AldebaranDb.Department> items);

        public async Task<IQueryable<Models.AldebaranDb.Department>> GetDepartments(Query query = null)
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

        partial void OnDepartmentGet(Models.AldebaranDb.Department item);

        partial void OnGetDepartmentByDepartmentId(ref IQueryable<Models.AldebaranDb.Department> items);


        public async Task<Models.AldebaranDb.Department> GetDepartmentByDepartmentId(short departmentid)
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

        partial void OnDepartmentCreated(Models.AldebaranDb.Department item);

        partial void OnAfterDepartmentCreated(Models.AldebaranDb.Department item);

        public async Task<Models.AldebaranDb.Department> CreateDepartment(Models.AldebaranDb.Department department)
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

        public async Task<Models.AldebaranDb.Department> CancelDepartmentChanges(Models.AldebaranDb.Department item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnDepartmentUpdated(Models.AldebaranDb.Department item);

        partial void OnAfterDepartmentUpdated(Models.AldebaranDb.Department item);

        public async Task<Models.AldebaranDb.Department> UpdateDepartment(short departmentid, Models.AldebaranDb.Department department)
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

        partial void OnDepartmentDeleted(Models.AldebaranDb.Department item);
        partial void OnAfterDepartmentDeleted(Models.AldebaranDb.Department item);

        public async Task<Models.AldebaranDb.Department> DeleteDepartment(short departmentid)
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

        public async Task ExportForwarderAgentsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/forwarderagents/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/forwarderagents/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportForwarderAgentsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/forwarderagents/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/forwarderagents/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnForwarderAgentsRead(ref IQueryable<Models.AldebaranDb.ForwarderAgent> items);

        public async Task<IQueryable<Models.AldebaranDb.ForwarderAgent>> GetForwarderAgents(Query query = null)
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

        partial void OnForwarderAgentGet(Models.AldebaranDb.ForwarderAgent item);
        partial void OnGetForwarderAgentByForwarderAgentId(ref IQueryable<Models.AldebaranDb.ForwarderAgent> items);


        public async Task<Models.AldebaranDb.ForwarderAgent> GetForwarderAgentByForwarderAgentId(int forwarderagentid)
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

        partial void OnForwarderAgentCreated(Models.AldebaranDb.ForwarderAgent item);
        partial void OnAfterForwarderAgentCreated(Models.AldebaranDb.ForwarderAgent item);

        public async Task<Models.AldebaranDb.ForwarderAgent> CreateForwarderAgent(Models.AldebaranDb.ForwarderAgent forwarderagent)
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

        public async Task<Models.AldebaranDb.ForwarderAgent> CancelForwarderAgentChanges(Models.AldebaranDb.ForwarderAgent item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnForwarderAgentUpdated(Models.AldebaranDb.ForwarderAgent item);
        partial void OnAfterForwarderAgentUpdated(Models.AldebaranDb.ForwarderAgent item);

        public async Task<Models.AldebaranDb.ForwarderAgent> UpdateForwarderAgent(int forwarderagentid, Models.AldebaranDb.ForwarderAgent forwarderagent)
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

        partial void OnForwarderAgentDeleted(Models.AldebaranDb.ForwarderAgent item);
        partial void OnAfterForwarderAgentDeleted(Models.AldebaranDb.ForwarderAgent item);

        public async Task<Models.AldebaranDb.ForwarderAgent> DeleteForwarderAgent(int forwarderagentid)
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

        partial void OnForwardersRead(ref IQueryable<Models.AldebaranDb.Forwarder> items);

        public async Task<IQueryable<Models.AldebaranDb.Forwarder>> GetForwarders(Query query = null)
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

        partial void OnForwarderGet(Models.AldebaranDb.Forwarder item);
        partial void OnGetForwarderByForwarderId(ref IQueryable<Models.AldebaranDb.Forwarder> items);


        public async Task<Models.AldebaranDb.Forwarder> GetForwarderByForwarderId(int forwarderid)
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

        partial void OnForwarderCreated(Models.AldebaranDb.Forwarder item);
        partial void OnAfterForwarderCreated(Models.AldebaranDb.Forwarder item);

        public async Task<Models.AldebaranDb.Forwarder> CreateForwarder(Models.AldebaranDb.Forwarder forwarder)
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

        public async Task<Models.AldebaranDb.Forwarder> CancelForwarderChanges(Models.AldebaranDb.Forwarder item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnForwarderUpdated(Models.AldebaranDb.Forwarder item);
        partial void OnAfterForwarderUpdated(Models.AldebaranDb.Forwarder item);

        public async Task<Models.AldebaranDb.Forwarder> UpdateForwarder(int forwarderid, Models.AldebaranDb.Forwarder forwarder)
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

        partial void OnForwarderDeleted(Models.AldebaranDb.Forwarder item);
        partial void OnAfterForwarderDeleted(Models.AldebaranDb.Forwarder item);

        public async Task<Models.AldebaranDb.Forwarder> DeleteForwarder(int forwarderid)
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

        partial void OnIdentityTypesRead(ref IQueryable<Models.AldebaranDb.IdentityType> items);
        partial void OnAdjustmentReasonsRead(ref IQueryable<Models.AldebaranDb.AdjustmentReason> items);
        partial void OnAdjustmentTypesRead(ref IQueryable<Models.AldebaranDb.AdjustmentType> items);
        partial void OnAspNetUsersRead(ref IQueryable<Models.AldebaranDb.Aspnetuser> items);

        public async Task<IQueryable<Models.AldebaranDb.IdentityType>> GetIdentityTypes(Query query = null)
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

        public async Task<IQueryable<Models.AldebaranDb.AdjustmentReason>> GetAdjustmentReasons(Query query = null)
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

        public async Task<IQueryable<Models.AldebaranDb.AdjustmentType>> GetAdjustmentTypes(Query query = null)
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

        public async Task<IQueryable<Models.AldebaranDb.Aspnetuser>> GetAspNetUsers(Query query = null)
        {
            var items = Context.Aspnetusers.AsQueryable();


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

            OnAspNetUsersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnIdentityTypeGet(Models.AldebaranDb.IdentityType item);
        partial void OnGetIdentityTypeByIdentityTypeId(ref IQueryable<Models.AldebaranDb.IdentityType> items);

        public async Task<Models.AldebaranDb.IdentityType> GetIdentityTypeByIdentityTypeId(int identitytypeid)
        {
            var items = Context.IdentityTypes
                              .AsNoTracking()
                              .Where(i => i.IDENTITY_TYPE_ID == identitytypeid);


            OnGetIdentityTypeByIdentityTypeId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnIdentityTypeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnIdentityTypeCreated(Models.AldebaranDb.IdentityType item);
        partial void OnAfterIdentityTypeCreated(Models.AldebaranDb.IdentityType item);

        public async Task<Models.AldebaranDb.IdentityType> CreateIdentityType(Models.AldebaranDb.IdentityType identitytype)
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

        public async Task<Models.AldebaranDb.IdentityType> CancelIdentityTypeChanges(Models.AldebaranDb.IdentityType item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnIdentityTypeUpdated(Models.AldebaranDb.IdentityType item);
        partial void OnAfterIdentityTypeUpdated(Models.AldebaranDb.IdentityType item);

        public async Task<Models.AldebaranDb.IdentityType> UpdateIdentityType(int identitytypeid, Models.AldebaranDb.IdentityType identitytype)
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

        partial void OnIdentityTypeDeleted(Models.AldebaranDb.IdentityType item);
        partial void OnAfterIdentityTypeDeleted(Models.AldebaranDb.IdentityType item);

        public async Task<Models.AldebaranDb.IdentityType> DeleteIdentityType(int identitytypeid)
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

        partial void OnItemReferencesRead(ref IQueryable<Models.AldebaranDb.ItemReference> items);

        public async Task<IQueryable<Models.AldebaranDb.ItemReference>> GetItemReferences(Query query = null)
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

        partial void OnItemReferenceGet(Models.AldebaranDb.ItemReference item);
        partial void OnGetItemReferenceByReferenceId(ref IQueryable<Models.AldebaranDb.ItemReference> items);


        public async Task<Models.AldebaranDb.ItemReference> GetItemReferenceByReferenceId(int referenceid)
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

        partial void OnItemReferenceCreated(Models.AldebaranDb.ItemReference item);
        partial void OnAfterItemReferenceCreated(Models.AldebaranDb.ItemReference item);

        public async Task<Models.AldebaranDb.ItemReference> CreateItemReference(Models.AldebaranDb.ItemReference itemreference)
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

        public async Task<Models.AldebaranDb.ItemReference> CancelItemReferenceChanges(Models.AldebaranDb.ItemReference item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItemReferenceUpdated(Models.AldebaranDb.ItemReference item);
        partial void OnAfterItemReferenceUpdated(Models.AldebaranDb.ItemReference item);

        public async Task<Models.AldebaranDb.ItemReference> UpdateItemReference(int referenceid, Models.AldebaranDb.ItemReference itemreference)
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

        partial void OnItemReferenceDeleted(Models.AldebaranDb.ItemReference item);
        partial void OnAfterItemReferenceDeleted(Models.AldebaranDb.ItemReference item);

        public async Task<Models.AldebaranDb.ItemReference> DeleteItemReference(int referenceid)
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

        partial void OnItemsRead(ref IQueryable<Models.AldebaranDb.Item> items);

        public async Task<IQueryable<Models.AldebaranDb.Item>> GetItems(Query query = null)
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

        partial void OnItemGet(Models.AldebaranDb.Item item);
        partial void OnGetItemByItemId(ref IQueryable<Models.AldebaranDb.Item> items);


        public async Task<Models.AldebaranDb.Item> GetItemByItemId(int itemid)
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

        partial void OnItemCreated(Models.AldebaranDb.Item item);
        partial void OnAfterItemCreated(Models.AldebaranDb.Item item);

        public async Task<Models.AldebaranDb.Item> CreateItem(Models.AldebaranDb.Item item)
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

        public async Task<Models.AldebaranDb.Item> CancelItemChanges(Models.AldebaranDb.Item item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItemUpdated(Models.AldebaranDb.Item item);
        partial void OnAfterItemUpdated(Models.AldebaranDb.Item item);

        public async Task<Models.AldebaranDb.Item> UpdateItem(int itemid, Models.AldebaranDb.Item item)
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

        partial void OnItemDeleted(Models.AldebaranDb.Item item);
        partial void OnAfterItemDeleted(Models.AldebaranDb.Item item);

        public async Task<Models.AldebaranDb.Item> DeleteItem(int itemid)
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

        partial void OnItemsAreasRead(ref IQueryable<Models.AldebaranDb.ItemsArea> items);

        public async Task<IQueryable<Models.AldebaranDb.ItemsArea>> GetItemsAreas(Query query = null)
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

        partial void OnItemsAreaGet(Models.AldebaranDb.ItemsArea item);
        partial void OnGetItemsAreaByItemIdAndAreaId(ref IQueryable<Models.AldebaranDb.ItemsArea> items);


        public async Task<Models.AldebaranDb.ItemsArea> GetItemsAreaByItemIdAndAreaId(int itemid, short areaid)
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

        partial void OnItemsAreaCreated(Models.AldebaranDb.ItemsArea item);
        partial void OnAfterItemsAreaCreated(Models.AldebaranDb.ItemsArea item);

        public async Task<Models.AldebaranDb.ItemsArea> CreateItemsArea(Models.AldebaranDb.ItemsArea itemsarea)
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

        public async Task<Models.AldebaranDb.ItemsArea> CancelItemsAreaChanges(Models.AldebaranDb.ItemsArea item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItemsAreaUpdated(Models.AldebaranDb.ItemsArea item);
        partial void OnAfterItemsAreaUpdated(Models.AldebaranDb.ItemsArea item);

        public async Task<Models.AldebaranDb.ItemsArea> UpdateItemsArea(int itemid, short areaid, Models.AldebaranDb.ItemsArea itemsarea)
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

        partial void OnItemsAreaDeleted(Models.AldebaranDb.ItemsArea item);
        partial void OnAfterItemsAreaDeleted(Models.AldebaranDb.ItemsArea item);

        public async Task<Models.AldebaranDb.ItemsArea> DeleteItemsArea(int itemid, short areaid)
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

        partial void OnLinesRead(ref IQueryable<Models.AldebaranDb.Line> items);

        public async Task<IQueryable<Models.AldebaranDb.Line>> GetLines(Query query = null)
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

        partial void OnLineGet(Models.AldebaranDb.Line item);
        partial void OnGetLineByLineId(ref IQueryable<Models.AldebaranDb.Line> items);


        public async Task<Models.AldebaranDb.Line> GetLineByLineId(short lineid)
        {
            var items = Context.Lines
                              .AsNoTracking()
                              .Where(i => i.LINE_ID == lineid);


            OnGetLineByLineId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnLineGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnLineCreated(Models.AldebaranDb.Line item);
        partial void OnAfterLineCreated(Models.AldebaranDb.Line item);

        public async Task<Models.AldebaranDb.Line> CreateLine(Models.AldebaranDb.Line line)
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

        public async Task<Models.AldebaranDb.Line> CancelLineChanges(Models.AldebaranDb.Line item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnLineUpdated(Models.AldebaranDb.Line item);
        partial void OnAfterLineUpdated(Models.AldebaranDb.Line item);

        public async Task<Models.AldebaranDb.Line> UpdateLine(short lineid, Models.AldebaranDb.Line line)
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

        partial void OnLineDeleted(Models.AldebaranDb.Line item);
        partial void OnAfterLineDeleted(Models.AldebaranDb.Line item);

        public async Task<Models.AldebaranDb.Line> DeleteLine(short lineid)
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

        partial void OnMeasureUnitsRead(ref IQueryable<Models.AldebaranDb.MeasureUnit> items);

        public async Task<IQueryable<Models.AldebaranDb.MeasureUnit>> GetMeasureUnits(Query query = null)
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

        partial void OnMeasureUnitGet(Models.AldebaranDb.MeasureUnit item);
        partial void OnGetMeasureUnitByMeasureUnitId(ref IQueryable<Models.AldebaranDb.MeasureUnit> items);


        public async Task<Models.AldebaranDb.MeasureUnit> GetMeasureUnitByMeasureUnitId(short measureunitid)
        {
            var items = Context.MeasureUnits
                              .AsNoTracking()
                              .Where(i => i.MEASURE_UNIT_ID == measureunitid);


            OnGetMeasureUnitByMeasureUnitId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnMeasureUnitGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnMeasureUnitCreated(Models.AldebaranDb.MeasureUnit item);
        partial void OnAfterMeasureUnitCreated(Models.AldebaranDb.MeasureUnit item);

        public async Task<Models.AldebaranDb.MeasureUnit> CreateMeasureUnit(Models.AldebaranDb.MeasureUnit measureunit)
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

        public async Task<Models.AldebaranDb.MeasureUnit> CancelMeasureUnitChanges(Models.AldebaranDb.MeasureUnit item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnMeasureUnitUpdated(Models.AldebaranDb.MeasureUnit item);
        partial void OnAfterMeasureUnitUpdated(Models.AldebaranDb.MeasureUnit item);

        public async Task<Models.AldebaranDb.MeasureUnit> UpdateMeasureUnit(short measureunitid, Models.AldebaranDb.MeasureUnit measureunit)
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

        partial void OnMeasureUnitDeleted(Models.AldebaranDb.MeasureUnit item);
        partial void OnAfterMeasureUnitDeleted(Models.AldebaranDb.MeasureUnit item);

        public async Task<Models.AldebaranDb.MeasureUnit> DeleteMeasureUnit(short measureunitid)
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

        public async Task ExportShipmentForwarderAgentMethodsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/shipmentforwarderagentmethods/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/shipmentforwarderagentmethods/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportShipmentForwarderAgentMethodsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/shipmentforwarderagentmethods/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/shipmentforwarderagentmethods/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnShipmentForwarderAgentMethodsRead(ref IQueryable<Models.AldebaranDb.ShipmentForwarderAgentMethod> items);

        public async Task<IQueryable<Models.AldebaranDb.ShipmentForwarderAgentMethod>> GetShipmentForwarderAgentMethods(Query query = null)
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

        partial void OnShipmentForwarderAgentMethodGet(Models.AldebaranDb.ShipmentForwarderAgentMethod item);
        partial void OnGetShipmentForwarderAgentMethodByShipmentForwarderAgentMethodId(ref IQueryable<Models.AldebaranDb.ShipmentForwarderAgentMethod> items);


        public async Task<Models.AldebaranDb.ShipmentForwarderAgentMethod> GetShipmentForwarderAgentMethodByShipmentForwarderAgentMethodId(short shipmentforwarderagentmethodid)
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

        partial void OnShipmentForwarderAgentMethodCreated(Models.AldebaranDb.ShipmentForwarderAgentMethod item);
        partial void OnAfterShipmentForwarderAgentMethodCreated(Models.AldebaranDb.ShipmentForwarderAgentMethod item);

        public async Task<Models.AldebaranDb.ShipmentForwarderAgentMethod> CreateShipmentForwarderAgentMethod(Models.AldebaranDb.ShipmentForwarderAgentMethod shipmentforwarderagentmethod)
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

        public async Task<Models.AldebaranDb.ShipmentForwarderAgentMethod> CancelShipmentForwarderAgentMethodChanges(Models.AldebaranDb.ShipmentForwarderAgentMethod item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnShipmentForwarderAgentMethodUpdated(Models.AldebaranDb.ShipmentForwarderAgentMethod item);
        partial void OnAfterShipmentForwarderAgentMethodUpdated(Models.AldebaranDb.ShipmentForwarderAgentMethod item);

        public async Task<Models.AldebaranDb.ShipmentForwarderAgentMethod> UpdateShipmentForwarderAgentMethod(short shipmentforwarderagentmethodid, Models.AldebaranDb.ShipmentForwarderAgentMethod shipmentforwarderagentmethod)
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

        partial void OnShipmentForwarderAgentMethodDeleted(Models.AldebaranDb.ShipmentForwarderAgentMethod item);
        partial void OnAfterShipmentForwarderAgentMethodDeleted(Models.AldebaranDb.ShipmentForwarderAgentMethod item);

        public async Task<Models.AldebaranDb.ShipmentForwarderAgentMethod> DeleteShipmentForwarderAgentMethod(short shipmentforwarderagentmethodid)
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

        partial void OnShipmentMethodsRead(ref IQueryable<Models.AldebaranDb.ShipmentMethod> items);

        public async Task<IQueryable<Models.AldebaranDb.ShipmentMethod>> GetShipmentMethods(Query query = null)
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

        partial void OnShipmentMethodGet(Models.AldebaranDb.ShipmentMethod item);
        partial void OnGetShipmentMethodByShipmentMethodId(ref IQueryable<Models.AldebaranDb.ShipmentMethod> items);


        public async Task<Models.AldebaranDb.ShipmentMethod> GetShipmentMethodByShipmentMethodId(short shipmentmethodid)
        {
            var items = Context.ShipmentMethods
                              .AsNoTracking()
                              .Where(i => i.SHIPMENT_METHOD_ID == shipmentmethodid);


            OnGetShipmentMethodByShipmentMethodId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnShipmentMethodGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnShipmentMethodCreated(Models.AldebaranDb.ShipmentMethod item);
        partial void OnAfterShipmentMethodCreated(Models.AldebaranDb.ShipmentMethod item);

        public async Task<Models.AldebaranDb.ShipmentMethod> CreateShipmentMethod(Models.AldebaranDb.ShipmentMethod shipmentmethod)
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

        public async Task<Models.AldebaranDb.ShipmentMethod> CancelShipmentMethodChanges(Models.AldebaranDb.ShipmentMethod item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnShipmentMethodUpdated(Models.AldebaranDb.ShipmentMethod item);
        partial void OnAfterShipmentMethodUpdated(Models.AldebaranDb.ShipmentMethod item);

        public async Task<Models.AldebaranDb.ShipmentMethod> UpdateShipmentMethod(short shipmentmethodid, Models.AldebaranDb.ShipmentMethod shipmentmethod)
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

        partial void OnShipmentMethodDeleted(Models.AldebaranDb.ShipmentMethod item);
        partial void OnAfterShipmentMethodDeleted(Models.AldebaranDb.ShipmentMethod item);

        public async Task<Models.AldebaranDb.ShipmentMethod> DeleteShipmentMethod(short shipmentmethodid)
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

        partial void OnShippingMethodsRead(ref IQueryable<Models.AldebaranDb.ShippingMethod> items);

        public async Task<IQueryable<Models.AldebaranDb.ShippingMethod>> GetShippingMethods(Query query = null)
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

        partial void OnShippingMethodGet(Models.AldebaranDb.ShippingMethod item);
        partial void OnGetShippingMethodByShippingMethodId(ref IQueryable<Models.AldebaranDb.ShippingMethod> items);


        public async Task<Models.AldebaranDb.ShippingMethod> GetShippingMethodByShippingMethodId(short shippingmethodid)
        {
            var items = Context.ShippingMethods
                              .AsNoTracking()
                              .Where(i => i.SHIPPING_METHOD_ID == shippingmethodid);


            OnGetShippingMethodByShippingMethodId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnShippingMethodGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnShippingMethodCreated(Models.AldebaranDb.ShippingMethod item);
        partial void OnAfterShippingMethodCreated(Models.AldebaranDb.ShippingMethod item);

        public async Task<Models.AldebaranDb.ShippingMethod> CreateShippingMethod(Models.AldebaranDb.ShippingMethod shippingmethod)
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

        public async Task<Models.AldebaranDb.ShippingMethod> CancelShippingMethodChanges(Models.AldebaranDb.ShippingMethod item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnShippingMethodUpdated(Models.AldebaranDb.ShippingMethod item);
        partial void OnAfterShippingMethodUpdated(Models.AldebaranDb.ShippingMethod item);

        public async Task<Models.AldebaranDb.ShippingMethod> UpdateShippingMethod(short shippingmethodid, Models.AldebaranDb.ShippingMethod shippingmethod)
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

        partial void OnShippingMethodDeleted(Models.AldebaranDb.ShippingMethod item);
        partial void OnAfterShippingMethodDeleted(Models.AldebaranDb.ShippingMethod item);

        public async Task<Models.AldebaranDb.ShippingMethod> DeleteShippingMethod(short shippingmethodid)
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

        public async Task ExportProvidersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/providers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/providers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportProvidersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/providers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/providers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnProvidersRead(ref IQueryable<Models.AldebaranDb.Provider> items);
        partial void OnAdjustmentsRead(ref IQueryable<Models.AldebaranDb.Adjustment> items);
        partial void OnAdjustmentDetailsRead(ref IQueryable<Models.AldebaranDb.AdjustmentDetail> items);
        partial void OnWarehousesRead(ref IQueryable<Models.AldebaranDb.Warehouse> items);
        partial void OnAspnetusersRead(ref IQueryable<Models.AldebaranDb.Aspnetuser> items);

        public async Task<IQueryable<Models.AldebaranDb.Warehouse>> GetWarehouses(Query query = null)
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

        public async Task<IQueryable<Models.AldebaranDb.Provider>> GetProviders(Query query = null)
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

        public async Task<IQueryable<Models.AldebaranDb.Aspnetuser>> GetAspnetusers(Query query = null)
        {
            var items = Context.Aspnetusers.AsQueryable();

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

            OnAspnetusersRead(ref items);

            return await Task.FromResult(items);
        }

        public async Task<IQueryable<Models.AldebaranDb.Adjustment>> GetAdjustments(Query query = null)
        {
            var items = Context.Adjustments.AsQueryable();

            items = items.Include(i => i.AdjustmentReason);
            items = items.Include(i => i.AdjustmentType);
            items = items.Include(i => i.Aspnetuser);

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

        public async Task<IQueryable<Models.AldebaranDb.AdjustmentDetail>> GetAdjustmentDetails(Query query = null)
        {
            var items = Context.AdjustmentDetails.AsQueryable();

            items = items.Include(i => i.ItemReference);
            items = items.Include(i => i.Adjustment);
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

        partial void OnProviderGet(Models.AldebaranDb.Provider item);
        partial void OnAdjustmentGet(Models.AldebaranDb.Adjustment item);

        partial void OnAdjustmentDetailGet(Models.AldebaranDb.AdjustmentDetail item);

        partial void OnGetProviderByProviderId(ref IQueryable<Models.AldebaranDb.Provider> items);

        partial void OnGetAdjustmentByAdjustmentId(ref IQueryable<Models.AldebaranDb.Adjustment> items);

        partial void OnGetAdjustmentDetailByAdjustmentDetailId(ref IQueryable<Models.AldebaranDb.AdjustmentDetail> items);

        public async Task<Models.AldebaranDb.Provider> GetProviderByProviderId(int providerid)
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

        public async Task<Models.AldebaranDb.Adjustment> GetAdjustmentByAdjustmentId(int adjustmentId)
        {
            var items = Context.Adjustments
                              .AsNoTracking()
                              .Where(i => i.ADJUSTMENT_ID == adjustmentId);

            items = items.Include(i => i.AdjustmentReason);
            items = items.Include(i => i.AdjustmentType);
            items = items.Include(i => i.Aspnetuser);

            OnGetAdjustmentByAdjustmentId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAdjustmentGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        public async Task<Models.AldebaranDb.AdjustmentDetail> GetAdjustmentDetailByAdjustmentDetailId(int adjustmentDetailId)
        {
            var items = Context.AdjustmentDetails
                              .AsNoTracking()
                              .Where(i => i.ADJUSTMENT_DETAIL_ID == adjustmentDetailId);

            items = items.Include(i => i.Adjustment);
            items = items.Include(i => i.ItemReference);
            items = items.Include(i => i.Warehouse);

            OnGetAdjustmentDetailByAdjustmentDetailId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAdjustmentDetailGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }
        
        partial void OnAdjustmentDetailCreated(Models.AldebaranDb.AdjustmentDetail item);
        partial void OnAdjustmentCreated(Models.AldebaranDb.Adjustment item);
        partial void OnProviderCreated(Models.AldebaranDb.Provider item);
        partial void OnAfterProviderCreated(Models.AldebaranDb.Provider item);
        partial void OnAfterAdjustmentCreated(Models.AldebaranDb.Adjustment item);
        partial void OnAfterAdjustmentDetailCreated(Models.AldebaranDb.AdjustmentDetail item);

        public async Task<Models.AldebaranDb.Provider> CreateProvider(Models.AldebaranDb.Provider provider)
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

        public async Task<Models.AldebaranDb.Adjustment> CreateAdjustment(Models.AldebaranDb.Adjustment adjustment)
        {
            OnAdjustmentCreated(adjustment);

            var existingItem = Context.Adjustments
                              .Where(i => i.ADJUSTMENT_ID == adjustment.ADJUSTMENT_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Adjustments.Add(adjustment);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(adjustment).State = EntityState.Detached;
                throw;
            }

            OnAfterAdjustmentCreated(adjustment);

            return adjustment;
        }

        public async Task<Models.AldebaranDb.AdjustmentDetail> CreateAdjustmentDetail(Models.AldebaranDb.AdjustmentDetail adjustmentDetail)
        {
            OnAdjustmentDetailCreated(adjustmentDetail);

            var existingItem = Context.AdjustmentDetails
                              .Where(i => i.ADJUSTMENT_DETAIL_ID == adjustmentDetail.ADJUSTMENT_DETAIL_ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.AdjustmentDetails.Add(adjustmentDetail);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(adjustmentDetail).State = EntityState.Detached;
                throw;
            }

            OnAfterAdjustmentDetailCreated(adjustmentDetail);

            return adjustmentDetail;
        }

        public async Task<Models.AldebaranDb.Provider> CancelProviderChanges(Models.AldebaranDb.Provider item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnProviderUpdated(Models.AldebaranDb.Provider item);
        partial void OnAdjustmentUpdated(Models.AldebaranDb.Adjustment item);
        partial void OnAdjustmentDetailUpdated(Models.AldebaranDb.AdjustmentDetail item);
        partial void OnAfterProviderUpdated(Models.AldebaranDb.Provider item);
        partial void OnAfterAdjustmentUpdated(Models.AldebaranDb.Adjustment item);
        partial void OnAfterAdjustmentDetailUpdated(Models.AldebaranDb.AdjustmentDetail item);

        public async Task<Models.AldebaranDb.Provider> UpdateProvider(int providerid, Models.AldebaranDb.Provider provider)
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

        public async Task<Models.AldebaranDb.Adjustment> UpdateAdjustment(int adjustmentId, Models.AldebaranDb.Adjustment adjustment)
        {
            OnAdjustmentUpdated(adjustment);

            var itemToUpdate = Context.Adjustments
                              .Where(i => i.ADJUSTMENT_ID == adjustment.ADJUSTMENT_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(adjustment);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAdjustmentUpdated(adjustment);

            return adjustment;
        }

        public async Task<Models.AldebaranDb.AdjustmentDetail> UpdateAdjustmentDetail(int adjustmentDetailId, Models.AldebaranDb.AdjustmentDetail adjustmentDetail)
        {
            OnAdjustmentDetailUpdated(adjustmentDetail);

            var itemToUpdate = Context.AdjustmentDetails
                              .Where(i => i.ADJUSTMENT_DETAIL_ID == adjustmentDetail.ADJUSTMENT_DETAIL_ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(adjustmentDetail);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAdjustmentDetailUpdated(adjustmentDetail);

            return adjustmentDetail;
        }

        partial void OnProviderDeleted(Models.AldebaranDb.Provider item);
        partial void OnAfterProviderDeleted(Models.AldebaranDb.Provider item);

        public async Task<Models.AldebaranDb.Provider> DeleteProvider(int providerid)
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

        public async Task ExportProviderReferencesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/providerreferences/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/providerreferences/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportProviderReferencesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/providerreferences/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/providerreferences/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnProviderReferencesRead(ref IQueryable<Models.AldebaranDb.ProviderReference> items);

        public async Task<IQueryable<Models.AldebaranDb.ProviderReference>> GetProviderReferences(Query query = null)
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

        partial void OnProviderReferenceGet(Models.AldebaranDb.ProviderReference item);
        partial void OnGetProviderReferenceByReferenceIdAndProviderId(ref IQueryable<Models.AldebaranDb.ProviderReference> items);


        public async Task<Models.AldebaranDb.ProviderReference> GetProviderReferenceByReferenceIdAndProviderId(int referenceid, int providerid)
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

        partial void OnProviderReferenceCreated(Models.AldebaranDb.ProviderReference item);
        partial void OnAfterProviderReferenceCreated(Models.AldebaranDb.ProviderReference item);

        public async Task<Models.AldebaranDb.ProviderReference> CreateProviderReference(Models.AldebaranDb.ProviderReference providerreference)
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

        public async Task<Models.AldebaranDb.ProviderReference> CancelProviderReferenceChanges(Models.AldebaranDb.ProviderReference item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnProviderReferenceUpdated(Models.AldebaranDb.ProviderReference item);
        partial void OnAfterProviderReferenceUpdated(Models.AldebaranDb.ProviderReference item);

        public async Task<Models.AldebaranDb.ProviderReference> UpdateProviderReference(int referenceid, int providerid, Models.AldebaranDb.ProviderReference providerreference)
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

        partial void OnProviderReferenceDeleted(Models.AldebaranDb.ProviderReference item);
        partial void OnAfterProviderReferenceDeleted(Models.AldebaranDb.ProviderReference item);

        public async Task<Models.AldebaranDb.ProviderReference> DeleteProviderReference(int referenceid, int providerid)
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
    }
}