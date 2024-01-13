using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class Worker
    {
        [Key]
        public Guid Id { get; set; }

        public string PositionName { get; set; }
        public int Quantity { get; set; }
        public double LaborCost { get; set; }

        public Guid SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Supplier Supplier { get; set; }
    }
}