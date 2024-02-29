using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.DAL.Models
{
    public class ContractVerificationCode
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ContractId { get; set; }
        [ForeignKey(nameof(ContractId))]
        public Contract? Contract { get; set; }
        public string? VerficationCode { get; set; }

    }
}
