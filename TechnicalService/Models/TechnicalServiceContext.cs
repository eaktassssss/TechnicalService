using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechnicalService.Models
{
    public class TechnicalServiceContext : DbContext
    {

     
        public TechnicalServiceContext(DbContextOptions<TechnicalServiceContext> contextOptions) : base(contextOptions)
        {

        }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Works> Works { get; set; }
        public DbSet<Users> Users { get; set; }
    }
}
