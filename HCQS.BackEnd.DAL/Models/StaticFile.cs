using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class StaticFile
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

        [ForeignKey(nameof(SampleProjectId))]
        public SampleProject? SampleProject { get; set; }
    }
}