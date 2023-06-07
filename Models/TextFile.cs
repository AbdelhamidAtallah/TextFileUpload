using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TextFileUpload.Models
{
    public class TextFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? FilePath { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }

        public bool IsError { get; set; }
        public DateTime UploadDate { get; set; }
      
    }
}

