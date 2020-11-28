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
    public class CustomerManager : ICustomerService
    {
        TechnicalServiceContext _databaseContex;
        IMapper _mapper;
        public CustomerManager(TechnicalServiceContext databaseContex, IMapper mapper)
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
        public async Task<List<WorksDto>> GetAllByUserId()
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
            return data;
        }
        public async Task<List<CategoryDto>> GetCategories()
        {
            return await _databaseContex.Categories.Select(x=> new CategoryDto {Id=x.Id,Name=x.Name}).ToListAsync();
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
