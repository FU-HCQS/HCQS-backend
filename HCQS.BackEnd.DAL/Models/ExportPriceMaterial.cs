using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class ExportPriceMaterial
    {
        [Key]
        public Guid Id { get; set; }

        public double Price { get; set; }
        public DateTime Date { get; set; }

        public Guid MaterialId { get; set; }

        [ForeignKey(nameof(MaterialId))]
        public Material Material { get; set; }
    }
}