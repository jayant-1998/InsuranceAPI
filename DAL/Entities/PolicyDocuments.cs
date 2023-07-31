using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceAPI.DAL.Entities
{
    [Table("PolicyDocuments", Schema="dbo")]
    public class PolicyDocuments
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string ObjectCode { get; set; }
        [Required]
        public string ReferenceNumber { get; set; }
        [Required]
        public string Context { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]   
        public string FileExtension { get; set; }
        [Required]
        public string LanguageCode { get; set; }
        [Required]
        public string CreatedUser { get; set; }
        [Required]
        public DateTime CreatedDateTime { get; set;}
        [Required]
        public bool IsDeleted { get; set;}

    }
}
