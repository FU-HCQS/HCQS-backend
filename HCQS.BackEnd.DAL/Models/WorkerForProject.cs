using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class WorkerForProject
    {
        [Key]
        public Guid Id { get; set; }

        public double ExportLaborCost { get; set; }
        public Guid WorkerId { get; set; }

        [ForeignKey(nameof(WorkerId))]
        public Worker Worker { get; set; }

        public Guid ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }
    }
}