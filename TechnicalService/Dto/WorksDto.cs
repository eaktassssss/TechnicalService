using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechnicalService.Dto
{
    public class WorksDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string CustomerNo { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string Brand { get; set; }
        public string ProblemDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public int InsurancePeriod { get; set; }
        public int Status { get; set; }
        public int UserId { get; set; }
    }
}
