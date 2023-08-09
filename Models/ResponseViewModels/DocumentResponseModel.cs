

namespace InsuranceAPI.Models.ResponseViewModels
{
    public class DocumentResponseModel
    {
   
        public int ID { get; set; }
       
        public string ObjectCode { get; set; }
        
        public string ReferenceNumber { get; set; }
       
        public string Context { get; set; }
      
        public string FileName { get; set; }
       
        public string FileExtension { get; set; }
        
        public string LanguageCode { get; set; }
    
        public string CreatedUser { get; set; }
        
        public DateTime CreatedDateTime { get; set; }
        
        public bool IsDeleted { get; set; }
    }
}
