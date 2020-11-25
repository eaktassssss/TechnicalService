using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    public class TechnicalService : ITechnicalService
    {
        TechnicalServiceContext _databaseContex;
        IMapper _mapper;
        public TechnicalService(TechnicalServiceContext databaseContex, IMapper mapper)
        {
            _databaseContex = databaseContex;
            _mapper = mapper;
        }
        public async Task Add(WorkDto worksDto)
        {
            try
            {
                var entity = _mapper.Map<Works>(worksDto);
                _databaseContex.Works.Add(entity);
                await _databaseContex.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task ChangeStatus(int id)
        {
            try
            {
                var entity = await _databaseContex.Works.FirstOrDefaultAsync(x => x.Id == id);
                entity.Status = (int)StatusType.Tamamlandı;
                await _databaseContex.SaveChangesAsync();
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
            var response = await (from work in _databaseContex.Works
                                  join
                                  category in _databaseContex.Categories on
                                  work.CategoryId equals category.Id
                                  select new WorksDto
                                  {
                                      CategoryName = category.Name,
                                      LastName = work.LastName,
                                      PhoneNumber = work.PhoneNumber,
                                      CustomerNo = work.CustomerNo,
                                      ProductName = work.ProductName,
                                      InsurancePeriod = work.InsurancePeriod,
                                      CreatedDate = work.CreatedDate,
                                      Brand = work.Brand,
                                      FirstName = work.FirstName,
                                      Status = work.Status,
                                      ProblemDescription = work.ProblemDescription,
                                      Id = work.Id
                                  }).ToListAsync();
            return response;
        }

        public async Task<List<Categories>> GetCategories()
        {
            return await _databaseContex.Categories.ToListAsync();
        }

        public async Task Update(WorkDto worksDto)
        {
            try
            {
                var entity = _mapper.Map<Works>(worksDto);
                var entry = _databaseContex.Entry(entity);
                entry.State = EntityState.Modified;
                await _databaseContex.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
