using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TechnicalService.Dto;
using TechnicalService.Models;

namespace TechnicalService.Services.Abstract
{
    public interface IManagerService
    {
        Task<T> Get<T>(Expression<Func<T,bool>> expression) where T:class;
        Task ChangeStatus(int id);
        Task Delete(int id);
        Task<List<WorksDto>> GetAll();

    }
}
