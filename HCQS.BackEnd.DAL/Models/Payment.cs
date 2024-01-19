using System.ComponentModel.DataAnnotations;

namespace HCQS.BackEnd.DAL.Models
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; }

        public double Price { get; set; }
        public string Content { get; set; }
        public Status PaymentStatus { get; set; }

        public enum Status
        {
            Pending,
            Success
        }

        [Required]
        public ContractProgressPayment ContractProgressPayment { get; set; }
    }
}