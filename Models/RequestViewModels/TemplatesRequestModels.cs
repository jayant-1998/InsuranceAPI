using System.ComponentModel.DataAnnotations;

namespace InsuranceAPI.Models.RequestViewModels
{
    public class TemplatesRequestModels
    {
        
        //public int ID { get; set; }
        
        public string ObjectCode { get; set; }

        public string ReferenceType { get; set; }
       
        public string ReferenceNumber { get; set; }
        
        public byte[] Context { get; set; }
        
        public string FileName { get; set; }
        
        public string FileExtension { get; set; }
        
        public string LanguageCode { get; set; }
        
        public string CreatedUser { get; set; }
        
        public DateTime CreatedDateTime { get; set; }
        
        public bool IsDeleted { get; set; }
    }
}
