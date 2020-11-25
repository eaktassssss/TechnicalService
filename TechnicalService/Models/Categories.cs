﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TechnicalService.Models
{

    [Table("Categories")]
    public class Categories
    {
        public Categories()
        {
            Works = new List<Works>();
        }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Works> Works { get; set; }
    }
}
