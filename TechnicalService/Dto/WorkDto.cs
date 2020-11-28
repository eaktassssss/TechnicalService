using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TechnicalService.Models;

namespace TechnicalService.Dto
{
    public class WorkDto
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        [Required(ErrorMessage = "Zorunlu alan")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Zorunlu alan")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Zorunlu alan")]
        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage ="Format dışı giriş")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Zorunlu alan")]
        public string CustomerNo { get; set; }
        [Required(ErrorMessage = "Zorunlu alan")]
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Zorunlu alan")]
        public string Brand { get; set; }
        [Required(ErrorMessage = "Zorunlu alan")]
        public string ProblemDescription { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int InsurancePeriod { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int Status { get; set; } = (int)StatusType.Beklemede;
    }
}
