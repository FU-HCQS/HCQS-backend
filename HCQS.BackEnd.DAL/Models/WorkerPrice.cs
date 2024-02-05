using System.ComponentModel.DataAnnotations;

namespace HCQS.BackEnd.DAL.Models
{
    public class WorkerPrice
    {
        [Key]
        public Guid Id { get; set; }

        public string? PositionName { get; set; }
        public double LaborCost { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}