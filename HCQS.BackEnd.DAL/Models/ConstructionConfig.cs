using System.ComponentModel.DataAnnotations;

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