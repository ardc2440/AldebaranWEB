using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;

using Aldebaran.Web.Data;

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

        partial void OnAreasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.Area> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranDb.Area>> GetAreas(Query query = null)
        {
            var items = Context.Areas.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAreasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAreaGet(Aldebaran.Web.Models.AldebaranDb.Area item);
        partial void OnGetAreaByAreaId(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.Area> items);


        public async Task<Aldebaran.Web.Models.AldebaranDb.Area> GetAreaByAreaId(short areaid)
        {
            var items = Context.Areas
                              .AsNoTracking()
                              .Where(i => i.AREA_ID == areaid);

 
            OnGetAreaByAreaId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAreaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAreaCreated(Aldebaran.Web.Models.AldebaranDb.Area item);
        partial void OnAfterAreaCreated(Aldebaran.Web.Models.AldebaranDb.Area item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Area> CreateArea(Aldebaran.Web.Models.AldebaranDb.Area area)
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

        public async Task<Aldebaran.Web.Models.AldebaranDb.Area> CancelAreaChanges(Aldebaran.Web.Models.AldebaranDb.Area item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAreaUpdated(Aldebaran.Web.Models.AldebaranDb.Area item);
        partial void OnAfterAreaUpdated(Aldebaran.Web.Models.AldebaranDb.Area item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Area> UpdateArea(short areaid, Aldebaran.Web.Models.AldebaranDb.Area area)
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

        partial void OnAreaDeleted(Aldebaran.Web.Models.AldebaranDb.Area item);
        partial void OnAfterAreaDeleted(Aldebaran.Web.Models.AldebaranDb.Area item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Area> DeleteArea(short areaid)
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

        partial void OnCitiesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.City> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranDb.City>> GetCities(Query query = null)
        {
            var items = Context.Cities.AsQueryable();

            items = items.Include(i => i.Department);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCitiesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCityGet(Aldebaran.Web.Models.AldebaranDb.City item);
        partial void OnGetCityByCityId(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.City> items);


        public async Task<Aldebaran.Web.Models.AldebaranDb.City> GetCityByCityId(int cityid)
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

        partial void OnCityCreated(Aldebaran.Web.Models.AldebaranDb.City item);
        partial void OnAfterCityCreated(Aldebaran.Web.Models.AldebaranDb.City item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.City> CreateCity(Aldebaran.Web.Models.AldebaranDb.City city)
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

        public async Task<Aldebaran.Web.Models.AldebaranDb.City> CancelCityChanges(Aldebaran.Web.Models.AldebaranDb.City item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCityUpdated(Aldebaran.Web.Models.AldebaranDb.City item);
        partial void OnAfterCityUpdated(Aldebaran.Web.Models.AldebaranDb.City item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.City> UpdateCity(int cityid, Aldebaran.Web.Models.AldebaranDb.City city)
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

        partial void OnCityDeleted(Aldebaran.Web.Models.AldebaranDb.City item);
        partial void OnAfterCityDeleted(Aldebaran.Web.Models.AldebaranDb.City item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.City> DeleteCity(int cityid)
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

        partial void OnCountriesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.Country> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranDb.Country>> GetCountries(Query query = null)
        {
            var items = Context.Countries.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCountriesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCountryGet(Aldebaran.Web.Models.AldebaranDb.Country item);
        partial void OnGetCountryByCountryId(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.Country> items);


        public async Task<Aldebaran.Web.Models.AldebaranDb.Country> GetCountryByCountryId(short countryid)
        {
            var items = Context.Countries
                              .AsNoTracking()
                              .Where(i => i.COUNTRY_ID == countryid);

 
            OnGetCountryByCountryId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCountryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCountryCreated(Aldebaran.Web.Models.AldebaranDb.Country item);
        partial void OnAfterCountryCreated(Aldebaran.Web.Models.AldebaranDb.Country item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Country> CreateCountry(Aldebaran.Web.Models.AldebaranDb.Country country)
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

        public async Task<Aldebaran.Web.Models.AldebaranDb.Country> CancelCountryChanges(Aldebaran.Web.Models.AldebaranDb.Country item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCountryUpdated(Aldebaran.Web.Models.AldebaranDb.Country item);
        partial void OnAfterCountryUpdated(Aldebaran.Web.Models.AldebaranDb.Country item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Country> UpdateCountry(short countryid, Aldebaran.Web.Models.AldebaranDb.Country country)
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

        partial void OnCountryDeleted(Aldebaran.Web.Models.AldebaranDb.Country item);
        partial void OnAfterCountryDeleted(Aldebaran.Web.Models.AldebaranDb.Country item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Country> DeleteCountry(short countryid)
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

        partial void OnCurrenciesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.Currency> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranDb.Currency>> GetCurrencies(Query query = null)
        {
            var items = Context.Currencies.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCurrenciesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCurrencyGet(Aldebaran.Web.Models.AldebaranDb.Currency item);
        partial void OnGetCurrencyByCurrencyId(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.Currency> items);


        public async Task<Aldebaran.Web.Models.AldebaranDb.Currency> GetCurrencyByCurrencyId(short currencyid)
        {
            var items = Context.Currencies
                              .AsNoTracking()
                              .Where(i => i.CURRENCY_ID == currencyid);

 
            OnGetCurrencyByCurrencyId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCurrencyGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCurrencyCreated(Aldebaran.Web.Models.AldebaranDb.Currency item);
        partial void OnAfterCurrencyCreated(Aldebaran.Web.Models.AldebaranDb.Currency item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Currency> CreateCurrency(Aldebaran.Web.Models.AldebaranDb.Currency currency)
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

        public async Task<Aldebaran.Web.Models.AldebaranDb.Currency> CancelCurrencyChanges(Aldebaran.Web.Models.AldebaranDb.Currency item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCurrencyUpdated(Aldebaran.Web.Models.AldebaranDb.Currency item);
        partial void OnAfterCurrencyUpdated(Aldebaran.Web.Models.AldebaranDb.Currency item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Currency> UpdateCurrency(short currencyid, Aldebaran.Web.Models.AldebaranDb.Currency currency)
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

        partial void OnCurrencyDeleted(Aldebaran.Web.Models.AldebaranDb.Currency item);
        partial void OnAfterCurrencyDeleted(Aldebaran.Web.Models.AldebaranDb.Currency item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Currency> DeleteCurrency(short currencyid)
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
    
        public async Task ExportDepartmentsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/departments/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/departments/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportDepartmentsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/departments/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/departments/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnDepartmentsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.Department> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranDb.Department>> GetDepartments(Query query = null)
        {
            var items = Context.Departments.AsQueryable();

            items = items.Include(i => i.Country);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnDepartmentsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnDepartmentGet(Aldebaran.Web.Models.AldebaranDb.Department item);
        partial void OnGetDepartmentByDepartmentId(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.Department> items);


        public async Task<Aldebaran.Web.Models.AldebaranDb.Department> GetDepartmentByDepartmentId(short departmentid)
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

        partial void OnDepartmentCreated(Aldebaran.Web.Models.AldebaranDb.Department item);
        partial void OnAfterDepartmentCreated(Aldebaran.Web.Models.AldebaranDb.Department item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Department> CreateDepartment(Aldebaran.Web.Models.AldebaranDb.Department department)
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

        public async Task<Aldebaran.Web.Models.AldebaranDb.Department> CancelDepartmentChanges(Aldebaran.Web.Models.AldebaranDb.Department item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnDepartmentUpdated(Aldebaran.Web.Models.AldebaranDb.Department item);
        partial void OnAfterDepartmentUpdated(Aldebaran.Web.Models.AldebaranDb.Department item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Department> UpdateDepartment(short departmentid, Aldebaran.Web.Models.AldebaranDb.Department department)
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

        partial void OnDepartmentDeleted(Aldebaran.Web.Models.AldebaranDb.Department item);
        partial void OnAfterDepartmentDeleted(Aldebaran.Web.Models.AldebaranDb.Department item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Department> DeleteDepartment(short departmentid)
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

        partial void OnForwarderAgentsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.ForwarderAgent> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranDb.ForwarderAgent>> GetForwarderAgents(Query query = null)
        {
            var items = Context.ForwarderAgents.AsQueryable();

            items = items.Include(i => i.City);
            items = items.Include(i => i.Forwarder);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnForwarderAgentsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnForwarderAgentGet(Aldebaran.Web.Models.AldebaranDb.ForwarderAgent item);
        partial void OnGetForwarderAgentByForwarderAgentId(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.ForwarderAgent> items);


        public async Task<Aldebaran.Web.Models.AldebaranDb.ForwarderAgent> GetForwarderAgentByForwarderAgentId(int forwarderagentid)
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

        partial void OnForwarderAgentCreated(Aldebaran.Web.Models.AldebaranDb.ForwarderAgent item);
        partial void OnAfterForwarderAgentCreated(Aldebaran.Web.Models.AldebaranDb.ForwarderAgent item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.ForwarderAgent> CreateForwarderAgent(Aldebaran.Web.Models.AldebaranDb.ForwarderAgent forwarderagent)
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

        public async Task<Aldebaran.Web.Models.AldebaranDb.ForwarderAgent> CancelForwarderAgentChanges(Aldebaran.Web.Models.AldebaranDb.ForwarderAgent item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnForwarderAgentUpdated(Aldebaran.Web.Models.AldebaranDb.ForwarderAgent item);
        partial void OnAfterForwarderAgentUpdated(Aldebaran.Web.Models.AldebaranDb.ForwarderAgent item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.ForwarderAgent> UpdateForwarderAgent(int forwarderagentid, Aldebaran.Web.Models.AldebaranDb.ForwarderAgent forwarderagent)
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

        partial void OnForwarderAgentDeleted(Aldebaran.Web.Models.AldebaranDb.ForwarderAgent item);
        partial void OnAfterForwarderAgentDeleted(Aldebaran.Web.Models.AldebaranDb.ForwarderAgent item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.ForwarderAgent> DeleteForwarderAgent(int forwarderagentid)
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

        partial void OnForwardersRead(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.Forwarder> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranDb.Forwarder>> GetForwarders(Query query = null)
        {
            var items = Context.Forwarders.AsQueryable();

            items = items.Include(i => i.City);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnForwardersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnForwarderGet(Aldebaran.Web.Models.AldebaranDb.Forwarder item);
        partial void OnGetForwarderByForwarderId(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.Forwarder> items);


        public async Task<Aldebaran.Web.Models.AldebaranDb.Forwarder> GetForwarderByForwarderId(int forwarderid)
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

        partial void OnForwarderCreated(Aldebaran.Web.Models.AldebaranDb.Forwarder item);
        partial void OnAfterForwarderCreated(Aldebaran.Web.Models.AldebaranDb.Forwarder item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Forwarder> CreateForwarder(Aldebaran.Web.Models.AldebaranDb.Forwarder forwarder)
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

        public async Task<Aldebaran.Web.Models.AldebaranDb.Forwarder> CancelForwarderChanges(Aldebaran.Web.Models.AldebaranDb.Forwarder item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnForwarderUpdated(Aldebaran.Web.Models.AldebaranDb.Forwarder item);
        partial void OnAfterForwarderUpdated(Aldebaran.Web.Models.AldebaranDb.Forwarder item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Forwarder> UpdateForwarder(int forwarderid, Aldebaran.Web.Models.AldebaranDb.Forwarder forwarder)
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

        partial void OnForwarderDeleted(Aldebaran.Web.Models.AldebaranDb.Forwarder item);
        partial void OnAfterForwarderDeleted(Aldebaran.Web.Models.AldebaranDb.Forwarder item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Forwarder> DeleteForwarder(int forwarderid)
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
    
        public async Task ExportItemReferencesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/itemreferences/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/itemreferences/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportItemReferencesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/aldebarandb/itemreferences/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/aldebarandb/itemreferences/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnItemReferencesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.ItemReference> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranDb.ItemReference>> GetItemReferences(Query query = null)
        {
            var items = Context.ItemReferences.AsQueryable();

            items = items.Include(i => i.Item);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnItemReferencesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnItemReferenceGet(Aldebaran.Web.Models.AldebaranDb.ItemReference item);
        partial void OnGetItemReferenceByReferenceId(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.ItemReference> items);


        public async Task<Aldebaran.Web.Models.AldebaranDb.ItemReference> GetItemReferenceByReferenceId(int referenceid)
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

        partial void OnItemReferenceCreated(Aldebaran.Web.Models.AldebaranDb.ItemReference item);
        partial void OnAfterItemReferenceCreated(Aldebaran.Web.Models.AldebaranDb.ItemReference item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.ItemReference> CreateItemReference(Aldebaran.Web.Models.AldebaranDb.ItemReference itemreference)
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

        public async Task<Aldebaran.Web.Models.AldebaranDb.ItemReference> CancelItemReferenceChanges(Aldebaran.Web.Models.AldebaranDb.ItemReference item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItemReferenceUpdated(Aldebaran.Web.Models.AldebaranDb.ItemReference item);
        partial void OnAfterItemReferenceUpdated(Aldebaran.Web.Models.AldebaranDb.ItemReference item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.ItemReference> UpdateItemReference(int referenceid, Aldebaran.Web.Models.AldebaranDb.ItemReference itemreference)
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

        partial void OnItemReferenceDeleted(Aldebaran.Web.Models.AldebaranDb.ItemReference item);
        partial void OnAfterItemReferenceDeleted(Aldebaran.Web.Models.AldebaranDb.ItemReference item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.ItemReference> DeleteItemReference(int referenceid)
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

        partial void OnItemsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.Item> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranDb.Item>> GetItems(Query query = null)
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
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnItemsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnItemGet(Aldebaran.Web.Models.AldebaranDb.Item item);
        partial void OnGetItemByItemId(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.Item> items);


        public async Task<Aldebaran.Web.Models.AldebaranDb.Item> GetItemByItemId(int itemid)
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

        partial void OnItemCreated(Aldebaran.Web.Models.AldebaranDb.Item item);
        partial void OnAfterItemCreated(Aldebaran.Web.Models.AldebaranDb.Item item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Item> CreateItem(Aldebaran.Web.Models.AldebaranDb.Item item)
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

        public async Task<Aldebaran.Web.Models.AldebaranDb.Item> CancelItemChanges(Aldebaran.Web.Models.AldebaranDb.Item item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItemUpdated(Aldebaran.Web.Models.AldebaranDb.Item item);
        partial void OnAfterItemUpdated(Aldebaran.Web.Models.AldebaranDb.Item item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Item> UpdateItem(int itemid, Aldebaran.Web.Models.AldebaranDb.Item item)
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

        partial void OnItemDeleted(Aldebaran.Web.Models.AldebaranDb.Item item);
        partial void OnAfterItemDeleted(Aldebaran.Web.Models.AldebaranDb.Item item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Item> DeleteItem(int itemid)
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

        partial void OnItemsAreasRead(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.ItemsArea> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranDb.ItemsArea>> GetItemsAreas(Query query = null)
        {
            var items = Context.ItemsAreas.AsQueryable();

            items = items.Include(i => i.Area);
            items = items.Include(i => i.Item);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnItemsAreasRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnItemsAreaGet(Aldebaran.Web.Models.AldebaranDb.ItemsArea item);
        partial void OnGetItemsAreaByItemIdAndAreaId(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.ItemsArea> items);


        public async Task<Aldebaran.Web.Models.AldebaranDb.ItemsArea> GetItemsAreaByItemIdAndAreaId(int itemid, short areaid)
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

        partial void OnItemsAreaCreated(Aldebaran.Web.Models.AldebaranDb.ItemsArea item);
        partial void OnAfterItemsAreaCreated(Aldebaran.Web.Models.AldebaranDb.ItemsArea item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.ItemsArea> CreateItemsArea(Aldebaran.Web.Models.AldebaranDb.ItemsArea itemsarea)
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

        public async Task<Aldebaran.Web.Models.AldebaranDb.ItemsArea> CancelItemsAreaChanges(Aldebaran.Web.Models.AldebaranDb.ItemsArea item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItemsAreaUpdated(Aldebaran.Web.Models.AldebaranDb.ItemsArea item);
        partial void OnAfterItemsAreaUpdated(Aldebaran.Web.Models.AldebaranDb.ItemsArea item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.ItemsArea> UpdateItemsArea(int itemid, short areaid, Aldebaran.Web.Models.AldebaranDb.ItemsArea itemsarea)
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

        partial void OnItemsAreaDeleted(Aldebaran.Web.Models.AldebaranDb.ItemsArea item);
        partial void OnAfterItemsAreaDeleted(Aldebaran.Web.Models.AldebaranDb.ItemsArea item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.ItemsArea> DeleteItemsArea(int itemid, short areaid)
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

        partial void OnLinesRead(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.Line> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranDb.Line>> GetLines(Query query = null)
        {
            var items = Context.Lines.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnLinesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnLineGet(Aldebaran.Web.Models.AldebaranDb.Line item);
        partial void OnGetLineByLineId(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.Line> items);


        public async Task<Aldebaran.Web.Models.AldebaranDb.Line> GetLineByLineId(short lineid)
        {
            var items = Context.Lines
                              .AsNoTracking()
                              .Where(i => i.LINE_ID == lineid);

 
            OnGetLineByLineId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnLineGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnLineCreated(Aldebaran.Web.Models.AldebaranDb.Line item);
        partial void OnAfterLineCreated(Aldebaran.Web.Models.AldebaranDb.Line item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Line> CreateLine(Aldebaran.Web.Models.AldebaranDb.Line line)
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

        public async Task<Aldebaran.Web.Models.AldebaranDb.Line> CancelLineChanges(Aldebaran.Web.Models.AldebaranDb.Line item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnLineUpdated(Aldebaran.Web.Models.AldebaranDb.Line item);
        partial void OnAfterLineUpdated(Aldebaran.Web.Models.AldebaranDb.Line item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Line> UpdateLine(short lineid, Aldebaran.Web.Models.AldebaranDb.Line line)
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

        partial void OnLineDeleted(Aldebaran.Web.Models.AldebaranDb.Line item);
        partial void OnAfterLineDeleted(Aldebaran.Web.Models.AldebaranDb.Line item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.Line> DeleteLine(short lineid)
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

        partial void OnMeasureUnitsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.MeasureUnit> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranDb.MeasureUnit>> GetMeasureUnits(Query query = null)
        {
            var items = Context.MeasureUnits.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnMeasureUnitsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnMeasureUnitGet(Aldebaran.Web.Models.AldebaranDb.MeasureUnit item);
        partial void OnGetMeasureUnitByMeasureUnitId(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.MeasureUnit> items);


        public async Task<Aldebaran.Web.Models.AldebaranDb.MeasureUnit> GetMeasureUnitByMeasureUnitId(short measureunitid)
        {
            var items = Context.MeasureUnits
                              .AsNoTracking()
                              .Where(i => i.MEASURE_UNIT_ID == measureunitid);

 
            OnGetMeasureUnitByMeasureUnitId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnMeasureUnitGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnMeasureUnitCreated(Aldebaran.Web.Models.AldebaranDb.MeasureUnit item);
        partial void OnAfterMeasureUnitCreated(Aldebaran.Web.Models.AldebaranDb.MeasureUnit item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.MeasureUnit> CreateMeasureUnit(Aldebaran.Web.Models.AldebaranDb.MeasureUnit measureunit)
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

        public async Task<Aldebaran.Web.Models.AldebaranDb.MeasureUnit> CancelMeasureUnitChanges(Aldebaran.Web.Models.AldebaranDb.MeasureUnit item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnMeasureUnitUpdated(Aldebaran.Web.Models.AldebaranDb.MeasureUnit item);
        partial void OnAfterMeasureUnitUpdated(Aldebaran.Web.Models.AldebaranDb.MeasureUnit item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.MeasureUnit> UpdateMeasureUnit(short measureunitid, Aldebaran.Web.Models.AldebaranDb.MeasureUnit measureunit)
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

        partial void OnMeasureUnitDeleted(Aldebaran.Web.Models.AldebaranDb.MeasureUnit item);
        partial void OnAfterMeasureUnitDeleted(Aldebaran.Web.Models.AldebaranDb.MeasureUnit item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.MeasureUnit> DeleteMeasureUnit(short measureunitid)
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

        partial void OnShipmentForwarderAgentMethodsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.ShipmentForwarderAgentMethod> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranDb.ShipmentForwarderAgentMethod>> GetShipmentForwarderAgentMethods(Query query = null)
        {
            var items = Context.ShipmentForwarderAgentMethods.AsQueryable();

            items = items.Include(i => i.ForwarderAgent);
            items = items.Include(i => i.ShipmentMethod);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnShipmentForwarderAgentMethodsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnShipmentForwarderAgentMethodGet(Aldebaran.Web.Models.AldebaranDb.ShipmentForwarderAgentMethod item);
        partial void OnGetShipmentForwarderAgentMethodByShipmentForwarderAgentMethodId(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.ShipmentForwarderAgentMethod> items);


        public async Task<Aldebaran.Web.Models.AldebaranDb.ShipmentForwarderAgentMethod> GetShipmentForwarderAgentMethodByShipmentForwarderAgentMethodId(short shipmentforwarderagentmethodid)
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

        partial void OnShipmentForwarderAgentMethodCreated(Aldebaran.Web.Models.AldebaranDb.ShipmentForwarderAgentMethod item);
        partial void OnAfterShipmentForwarderAgentMethodCreated(Aldebaran.Web.Models.AldebaranDb.ShipmentForwarderAgentMethod item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.ShipmentForwarderAgentMethod> CreateShipmentForwarderAgentMethod(Aldebaran.Web.Models.AldebaranDb.ShipmentForwarderAgentMethod shipmentforwarderagentmethod)
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

        public async Task<Aldebaran.Web.Models.AldebaranDb.ShipmentForwarderAgentMethod> CancelShipmentForwarderAgentMethodChanges(Aldebaran.Web.Models.AldebaranDb.ShipmentForwarderAgentMethod item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnShipmentForwarderAgentMethodUpdated(Aldebaran.Web.Models.AldebaranDb.ShipmentForwarderAgentMethod item);
        partial void OnAfterShipmentForwarderAgentMethodUpdated(Aldebaran.Web.Models.AldebaranDb.ShipmentForwarderAgentMethod item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.ShipmentForwarderAgentMethod> UpdateShipmentForwarderAgentMethod(short shipmentforwarderagentmethodid, Aldebaran.Web.Models.AldebaranDb.ShipmentForwarderAgentMethod shipmentforwarderagentmethod)
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

        partial void OnShipmentForwarderAgentMethodDeleted(Aldebaran.Web.Models.AldebaranDb.ShipmentForwarderAgentMethod item);
        partial void OnAfterShipmentForwarderAgentMethodDeleted(Aldebaran.Web.Models.AldebaranDb.ShipmentForwarderAgentMethod item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.ShipmentForwarderAgentMethod> DeleteShipmentForwarderAgentMethod(short shipmentforwarderagentmethodid)
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

        partial void OnShipmentMethodsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.ShipmentMethod> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranDb.ShipmentMethod>> GetShipmentMethods(Query query = null)
        {
            var items = Context.ShipmentMethods.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnShipmentMethodsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnShipmentMethodGet(Aldebaran.Web.Models.AldebaranDb.ShipmentMethod item);
        partial void OnGetShipmentMethodByShipmentMethodId(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.ShipmentMethod> items);


        public async Task<Aldebaran.Web.Models.AldebaranDb.ShipmentMethod> GetShipmentMethodByShipmentMethodId(short shipmentmethodid)
        {
            var items = Context.ShipmentMethods
                              .AsNoTracking()
                              .Where(i => i.SHIPMENT_METHOD_ID == shipmentmethodid);

 
            OnGetShipmentMethodByShipmentMethodId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnShipmentMethodGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnShipmentMethodCreated(Aldebaran.Web.Models.AldebaranDb.ShipmentMethod item);
        partial void OnAfterShipmentMethodCreated(Aldebaran.Web.Models.AldebaranDb.ShipmentMethod item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.ShipmentMethod> CreateShipmentMethod(Aldebaran.Web.Models.AldebaranDb.ShipmentMethod shipmentmethod)
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

        public async Task<Aldebaran.Web.Models.AldebaranDb.ShipmentMethod> CancelShipmentMethodChanges(Aldebaran.Web.Models.AldebaranDb.ShipmentMethod item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnShipmentMethodUpdated(Aldebaran.Web.Models.AldebaranDb.ShipmentMethod item);
        partial void OnAfterShipmentMethodUpdated(Aldebaran.Web.Models.AldebaranDb.ShipmentMethod item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.ShipmentMethod> UpdateShipmentMethod(short shipmentmethodid, Aldebaran.Web.Models.AldebaranDb.ShipmentMethod shipmentmethod)
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

        partial void OnShipmentMethodDeleted(Aldebaran.Web.Models.AldebaranDb.ShipmentMethod item);
        partial void OnAfterShipmentMethodDeleted(Aldebaran.Web.Models.AldebaranDb.ShipmentMethod item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.ShipmentMethod> DeleteShipmentMethod(short shipmentmethodid)
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

        partial void OnShippingMethodsRead(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.ShippingMethod> items);

        public async Task<IQueryable<Aldebaran.Web.Models.AldebaranDb.ShippingMethod>> GetShippingMethods(Query query = null)
        {
            var items = Context.ShippingMethods.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnShippingMethodsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnShippingMethodGet(Aldebaran.Web.Models.AldebaranDb.ShippingMethod item);
        partial void OnGetShippingMethodByShippingMethodId(ref IQueryable<Aldebaran.Web.Models.AldebaranDb.ShippingMethod> items);


        public async Task<Aldebaran.Web.Models.AldebaranDb.ShippingMethod> GetShippingMethodByShippingMethodId(short shippingmethodid)
        {
            var items = Context.ShippingMethods
                              .AsNoTracking()
                              .Where(i => i.SHIPPING_METHOD_ID == shippingmethodid);

 
            OnGetShippingMethodByShippingMethodId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnShippingMethodGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnShippingMethodCreated(Aldebaran.Web.Models.AldebaranDb.ShippingMethod item);
        partial void OnAfterShippingMethodCreated(Aldebaran.Web.Models.AldebaranDb.ShippingMethod item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.ShippingMethod> CreateShippingMethod(Aldebaran.Web.Models.AldebaranDb.ShippingMethod shippingmethod)
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

        public async Task<Aldebaran.Web.Models.AldebaranDb.ShippingMethod> CancelShippingMethodChanges(Aldebaran.Web.Models.AldebaranDb.ShippingMethod item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnShippingMethodUpdated(Aldebaran.Web.Models.AldebaranDb.ShippingMethod item);
        partial void OnAfterShippingMethodUpdated(Aldebaran.Web.Models.AldebaranDb.ShippingMethod item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.ShippingMethod> UpdateShippingMethod(short shippingmethodid, Aldebaran.Web.Models.AldebaranDb.ShippingMethod shippingmethod)
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

        partial void OnShippingMethodDeleted(Aldebaran.Web.Models.AldebaranDb.ShippingMethod item);
        partial void OnAfterShippingMethodDeleted(Aldebaran.Web.Models.AldebaranDb.ShippingMethod item);

        public async Task<Aldebaran.Web.Models.AldebaranDb.ShippingMethod> DeleteShippingMethod(short shippingmethodid)
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
        }
}