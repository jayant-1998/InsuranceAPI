using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceAPI.DAL.Entities
{
    [Table("HtmlTemplate" , Schema ="dbo")]
    public class HtmlTemplateEntity
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string HTML { get; set; }
    }
}
