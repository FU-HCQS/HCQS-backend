using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class ImportExportInventoryHistory
    {
        [Key]
        public Guid Id { get; set; }

        public int Quantity { get; set; }
        public Guid MaterialHistoryId { get; set; }

        [ForeignKey(nameof(MaterialHistoryId))]
        public MaterialHistory MaterialHistory { get; set; }
    }
}