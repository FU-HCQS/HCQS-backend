using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class ConstructionConfigRepository : Repository<ConstructionConfig>, IConstructionConfigRepository
    {
        public ConstructionConfigRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}
