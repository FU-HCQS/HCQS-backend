using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Dto.Response
{
    public class StaticFileResponse
    {
        [Key]
        public Guid Id { get; set; }

        public string Url { get; set; }

        public Type StaticFileType { get; set; }

        public enum Type
        {
            Image,
            Video,
            Pdf
        }

        public Guid? SampleProjectId { get; set; }

    }
}
