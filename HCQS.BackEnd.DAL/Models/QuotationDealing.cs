using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.DAL.Models
{
    public class QuotationDealing
    {
        [Key]
        public Guid Id { get; set; }
        public double MaterialDiscount { get; set; }
        public double FurnitureDiscount { get; set; }
        public double LaborDiscount { get; set; }
        public Guid? QuotationId { get; set; }
        [ForeignKey(nameof(QuotationId))]
        public Quotation? Quotation { get; set; }
    }
}
