using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class SampleProject
    {
        [Key]
        public Guid Id { get; set; }

        public int NumOfFloor { get; set; }
        public double ConstructionArea { get; set; }
        public double TotalArea { get; set; }
        public Type ProjectType { get; set; }

        public enum Type
        {
            Level_4_House,
            House_With_Multiple_Floors
        }

        public string Function { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public double EstimatePrice { get; set; }
        public string Location { get; set; }

        public string? AccountId { get; set; }

        [ForeignKey(nameof(AccountId))]
        public Account? Account { get; set; }
    }
}