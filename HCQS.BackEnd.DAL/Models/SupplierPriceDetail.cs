using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.DAL.Models
{
    public class SupplierPriceDetail
    {
        public Guid Id { get; set; }
        public int MQO { get; set; }
        public double Price { get; set; }

        public Guid? MaterialId { get; set; }
        [ForeignKey(nameof(MaterialId))]
        public Material? Material { get; set; }
        public Guid? SupplierPriceQuotationId { get; set; }
        [ForeignKey(nameof (SupplierPriceQuotationId))]
        public SupplierPriceQuotation? SupplierPriceQuotation { get; set;}
    }
}
