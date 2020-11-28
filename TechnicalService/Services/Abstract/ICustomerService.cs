using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TechnicalService.Dto;
using TechnicalService.Models;

namespace TechnicalService.Services.Abstract
{
    public interface ICustomerService
    {
        Task<T> Get<T>(Expression<Func<T, bool>> expression) where T : class;
        Task Add(WorkDto worksDto);
        Task Update(WorkDto worksDto);
        Task<List<CategoryDto>> GetCategories();
        Task<List<WorksDto>> GetAllByUserId();
        Task Delete(int id);
    }
}
