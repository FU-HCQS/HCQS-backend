using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class PaymentResponseRepository : Repository<PaymentResponse>, IPaymentResponseRepository
    {
        public PaymentResponseRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}