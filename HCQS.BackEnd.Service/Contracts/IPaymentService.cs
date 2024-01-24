using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IPaymentService
    {
        public Task<AppActionResult> CreatePaymentUrlMomo(Guid paymentId);

        public Task<AppActionResult> CreatePaymentUrlVNPay(Guid paymentId, HttpContext context);
        public Task<AppActionResult> UpdatePaymentStatus(string paymentId, bool status, int type);

    }
}
