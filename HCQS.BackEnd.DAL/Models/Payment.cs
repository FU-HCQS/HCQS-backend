using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HCQS.BackEnd.DAL.Models
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; }

        public double Price { get; set; }
        public string? Content { get; set; }
        public Status PaymentStatus { get; set; }

        public enum Status
        {
            Pending,
            Success
        }

        [JsonIgnore]
        public ContractProgressPayment? ContractProgressPayment { get; set; } = null!;
    }
}