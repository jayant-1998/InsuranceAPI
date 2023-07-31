using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceAPI.DAL.Entities
{
    [Table("Users", Schema = "dbo")]
    public class Users
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public String PolicyNumber { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public int Salary{ get; set; }
        [Required]
        public String Occupation { get; set; }
        [Required]
        public DateTime PolicyExpiryDate{ get; set; }
        [Required]
        public String ProductCode { get; set;}
        [Required]
        public String EmailAddress { get; set;}



    }
}
