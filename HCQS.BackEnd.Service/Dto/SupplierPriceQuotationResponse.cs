using HCQS.BackEnd.Common.Dto.Response;
using HCQS.BackEnd.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Service.Dto
{
    public class SupplierPriceQuotationResponse
    {
        public SupplierPriceQuotation SupplierPriceQuotations { get; set; }
        public List<SupplierPriceDetail> SupplierPriceDetails { get; set; }
    }
}
