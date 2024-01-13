using System.ComponentModel.DataAnnotations;

namespace HCQS.BackEnd.DAL.Models
{
    public class Material
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public enum Unit
        {
            KG,
            M3,
            BAR,
        }

        public enum Type
        {
            RawMaterials,
            Furniture
        }

        public int Quantity { get; set; }
    }
}