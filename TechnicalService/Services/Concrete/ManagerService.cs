using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TechnicalService.Dto;
using TechnicalService.Enums;
using TechnicalService.Models;
using TechnicalService.Services.Abstract;

namespace TechnicalService.Services.Concrete
{
    public class ManagerService : IManagerService
    {
        TechnicalServiceContext _databaseContex;
        IDistributedCache _distributedCache;
        public ManagerService(TechnicalServiceContext databaseContex, IDistributedCache distributedCache)
        {
            _databaseContex = databaseContex;
            _distributedCache = distributedCache;
        }
        public async Task ChangeStatus(int id)
        {
            try
            {
                var entity = await _databaseContex.Works.FirstOrDefaultAsync(x => x.Id == id);
                entity.Status = (int)StatusType.Tamamlandı;
                await _databaseContex.SaveChangesAsync();
                await _distributedCache.RemoveAsync("servicerecords");
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        public async Task Delete(int id)
        {
            try
            {
                var entity = await _databaseContex.Works.FirstOrDefaultAsync(x => x.Id == id);
                _databaseContex.Works.Remove(entity);
                await _databaseContex.SaveChangesAsync();
                await _distributedCache.RemoveAsync("servicerecords");
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        public async Task<T> Get<T>(Expression<Func<T, bool>> expression) where T : class
        {
            var entity = await _databaseContex.Set<T>().FirstOrDefaultAsync(expression);
            return entity;
        }
        public async Task<List<WorksDto>> GetAll()
        {

            var cacheData = await _distributedCache.GetStringAsync("servicerecords");
            if (cacheData == null)
            {
                var data = await _databaseContex.Works.Include(x => x.Categories).Select(x => new WorksDto
                {

                    CategoryName = x.Categories.Name,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber,
                    CustomerNo = x.CustomerNo,
                    ProductName = x.ProductName,
                    InsurancePeriod = x.InsurancePeriod,
                    CreatedDate = x.CreatedDate,
                    Brand = x.Brand,
                    FirstName = x.FirstName,
                    Status = x.Status,
                    ProblemDescription = x.ProblemDescription,
                    Id = x.Id,
                    UserId = x.UserId
                }).OrderByDescending(z => z.Id).ToListAsync();

                if (data.Any())
                {
                    await _distributedCache.SetStringAsync("servicerecords", JsonConvert.SerializeObject(data));
                    return data;
                }
                return data;
            }
            else
            {
                var data = await _distributedCache.GetStringAsync("servicerecords");
                return JsonConvert.DeserializeObject<List<WorksDto>>(data);
            }
        }
    }
}
