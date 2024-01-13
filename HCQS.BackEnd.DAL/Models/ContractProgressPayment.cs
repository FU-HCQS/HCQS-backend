using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class ContractProgressPayment
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime Date { get; set; }
        public string Name { get; set; }
        public Guid ContractId { get; set; }

        [ForeignKey(nameof(ContractId))]
        public Contract Contract { get; set; }

        public Guid PaymentId { get; set; }

        [ForeignKey(nameof(PaymentId))]
        public Payment Payment { get; set; }
    }
}