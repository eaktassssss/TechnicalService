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
using TechnicalService.Models;
using TechnicalService.Services.Abstract;

namespace TechnicalService.Services.Concrete
{
    public class CustomerManager : ICustomerService
    {
        TechnicalServiceContext _databaseContex;
        IMapper _mapper;
        IDistributedCache _distributedCache;
        public CustomerManager(TechnicalServiceContext databaseContex, IMapper mapper, IDistributedCache disributedCache)
        {
            _databaseContex = databaseContex;
            _mapper = mapper;
            _distributedCache = disributedCache;
        }
        public async Task Add(WorkDto worksDto)
        {
            try
            {
                var entity = _mapper.Map<Works>(worksDto);
                _databaseContex.Works.Add(entity);
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
        List<WorksDto> data;
        public async Task<List<WorksDto>> GetByUserId(int userId)
        {

            var cacheData = await _distributedCache.GetStringAsync("servicerecords");
            if (cacheData == null)
            {
                data = await _databaseContex.Works.Include(x => x.Categories).Select(x => new WorksDto
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
                    return data.Where(x => x.UserId == userId).ToList();
                }
                return data;
            }
            else
            {
                var data = JsonConvert.DeserializeObject<List<WorksDto>>(await _distributedCache.GetStringAsync("servicerecords")).Where(x => x.UserId == userId).ToList();
                return data;
            }
        }
        public async Task<List<CategoryDto>> GetCategories()
        {
            return await _databaseContex.Categories.Select(x => new CategoryDto { Id = x.Id, Name = x.Name }).ToListAsync();
        }
        public async Task Update(WorkDto worksDto)
        {
            try
            {
                var entity = _mapper.Map<Works>(worksDto);
                var entry = _databaseContex.Entry(entity);
                entry.State = EntityState.Modified;
                await _databaseContex.SaveChangesAsync();
                await _distributedCache.RemoveAsync("servicerecords");
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
