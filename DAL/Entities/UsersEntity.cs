using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceAPI.DAL.Entities
{
    [Table("Users", Schema = "dbo")]
    public class UsersEntity
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public String name { get; set; }
        [Required]
        public String policyNumber { get; set; }
        [Required]
        public int age { get; set; }
        [Required]
        public int salary{ get; set; }
        [Required]
        public String occupation { get; set; }
        [Required]
        public DateTime policyExpiryDate{ get; set; }
        [Required]
        public String productCode { get; set;}
        [Required]
        public String email { get; set;}
    }
}
