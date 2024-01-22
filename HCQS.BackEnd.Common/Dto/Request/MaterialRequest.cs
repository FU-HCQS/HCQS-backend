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

        public MaterialConstructionType MaterialType { get; set; }

        public enum MaterialConstructionType
        {
            RawMaterials,
            Furniture
        }

        public int Quantity { get; set; }
    }
}
