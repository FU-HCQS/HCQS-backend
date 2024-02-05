using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class Project
    {
        [Key]
        public Guid Id { get; set; }

        public int NumOfFloor { get; set; }
        public int SandMixingRatio { get; set; }
        public int CementMixingRatio { get; set; }
        public int StoneMixingRatio { get; set; }
        public double Area { get; set; }
        public double TiledArea { get; set; }
        public double WallLength { get; set; }
        public double WallHeight { get; set; }

        public string? LandDrawingFileUrl { get; set; }
        public Status ProjectStatus { get; set; }

        public ProjectConstructionType ConstructionType { get; set; }

        public enum Status
        {
            Pending,
            Processing,
            UnderConstruction,
            Closed
        }

        public enum ProjectConstructionType
        {
            RoughConstruction,
            CompleteConstruction
        }

        public int EstimatedTimeOfCompletion { get; set; }
        public int NumberOfLabor { get; set; }
        public DateTime CreateDate { get; set; }

        public string? AccountId { get; set; }

        [ForeignKey("AccountId")]
        public Account? Account { get; set; }

        public Contract? Contract { get; set; }
    }
}