using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.DAL.Models
{
    public class ConstructionConfig
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Value { get; set; }
    }
}
