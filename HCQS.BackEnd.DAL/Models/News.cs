using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class News
    {
        [Key]
        public Guid Id { get; set; }

        public string Header { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public string AccountId { get; set; }

        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; }
    }
}