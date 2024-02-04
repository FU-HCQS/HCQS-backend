using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class PaymentResponse
    {
        [Key]
        public Guid Id { get; set; }

        public double Amount { get; set; }
        public PaymentType PaymentTypeResponse { get; set; }

        public enum PaymentType
        {
            VNPAY,
            MOMO
        }

        public string? OrderInfo { get; set; }
        public bool IsSuccess { get; set; }
        public Guid? PaymentId { get; set; }

        [ForeignKey(nameof(PaymentId))]
        public Payment? Payment { get; set; }
    }
}