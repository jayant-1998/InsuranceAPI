using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceAPI.DAL.Entities
{
    [Table("Email", Schema = "dbo")]
    public class EmailEntity
    {
        [Key]
        public int ID { get; set; }
        public int userId { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public byte[] attachment { get; set; }
        public int attempts { get; set; } // Renamed "count" to "attempts"
        public int maxAttempts { get; set; } // Default value set to 3 for "maxAttempts"
        public bool isSend { get; set; } // Default value set to false (0) for "isSend"
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime? modifiedAt { get; set; }
    }
}
