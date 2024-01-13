using System.ComponentModel.DataAnnotations;

namespace HCQS.BackEnd.DAL.Models
{
    public class StaticFile
    {
        [Key]
        public Guid Id { get; set; }

        public string Url { get; set; }

        public enum Type
        {
            Image,
            Video,
            Pdf
        }
    }
}