using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class Contract
    {
        [Key]
        public Guid Id { get; set; }

        public double Total { get; set; }
        public double TotalCostsIncurred { get; set; }
        public double Deposit { get; set; }
        public string Content { get; set; }
        public DateTime DateOfContract { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double ExpectedPrice { get; set; }
        public Guid ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }
    }
}