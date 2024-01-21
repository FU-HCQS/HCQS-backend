using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class MaterialRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public Unit UnitMaterial { get; set; }

        public enum Unit
        {
            KG,
            M3,
            BAR,
        }

        public Type MaterialType { get; set; }

        public enum Type
        {
            RawMaterials,
            Furniture
        }

        public int Quantity { get; set; }
    }
}
