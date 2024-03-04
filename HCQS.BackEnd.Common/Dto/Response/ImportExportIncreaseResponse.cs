using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Dto.Response
{
    public class ImportExportIncreaseResponse
    {
        public string Date {  get; set; }
        public double ImportIncrease  {  get; set; }
        public double ExportIncrease {  get; set; }
    }
}
