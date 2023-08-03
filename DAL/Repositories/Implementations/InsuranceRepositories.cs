using InsuranceAPI.DAL.DBContexts;
using InsuranceAPI.DAL.Entities;
using InsuranceAPI.DAL.Repositories.Interfaces;
using InsuranceAPI.Models.ResponseViewModels;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.DAL.Repositories.Implementations
{
    public class InsuranceRepositories : IInsuranceRepositories
    {
        private readonly ApplicationDBContexts _dBContext;

        public InsuranceRepositories(ApplicationDBContexts dbContext)
        {
            _dBContext = dbContext;
        }

        public async Task<PolicyDocument> GetDocummentDb(int id, UserResponseModels user)
        {
            var body = await _dBContext.documents
                        .Where(doc => doc.CreatedUser == id.ToString() 
                        && doc.ObjectCode == $"{user.PolicyNumber}-{user.ProductCode}" 
                        && doc.IsDeleted == false)
                        .SingleOrDefaultAsync();
            if (body == null)
            {
                return null;
            }
            var res = new PolicyDocument()
            {
                ID = body.ID,
                CreatedUser = body.CreatedUser,
                ObjectCode = body.ObjectCode,
                ReferenceNumber = body.ReferenceNumber,
                ReferenceType = body.ReferenceType,
                Content = body.Content,
                FileExtension = body.FileExtension,
                FileName = body.FileName,
                CreatedDateTime = body.CreatedDateTime,
                IsDeleted = body.IsDeleted,
                LanguageCode = body.LanguageCode
            };
            return res;
        }

        public async Task<TemplateResponseModels> GetTemplateDB()
        {
            int id = 1;
            var body = _dBContext.templates.Where(i => i.ID == id);

            var res = await body.Select(i => new TemplateResponseModels
            {
                ID = i.ID,
                Name = i.Name,
                HTML = i.HTML,
            }).FirstOrDefaultAsync();

            return res;
        }

        public async Task<UserResponseModels> GetUserDB(int id)
        {
            var body = _dBContext.users.Where(i => i.ID == id);

            var res = await body.Select(i => new UserResponseModels
            {
                ID = i.ID,
                Name = i.Name,
                PolicyNumber = i.PolicyNumber,
                Age = i.Age,
                Salary = i.Salary,
                Occupation = i.Occupation,
                PolicyExpiryDate = i.PolicyExpiryDate,
                ProductCode = i.ProductCode,   
                EmailAddress = i.EmailAddress

            }).FirstOrDefaultAsync();

            return res;
        }
        public async Task<string> InsertIntoDocumentDB(UserResponseModels user, byte[] pdf)
        {
            try 
            {
                var body =  await _dBContext.documents
                           .Where(todo => todo.ObjectCode == $"{user.PolicyNumber}-{user.ProductCode}" && todo.IsDeleted == false)
                           .SingleOrDefaultAsync();

                if (body != null)
                {
                    body.IsDeleted = true;
                    await _dBContext.SaveChangesAsync();
                }




                PolicyDocument request = new PolicyDocument
                {
                    ObjectCode = $"{user.PolicyNumber}-{user.ProductCode}",
                    ReferenceType = "Policy",
                    ReferenceNumber = user.PolicyNumber,
                    Content = pdf,
                    FileName = $"{user.PolicyNumber}" + DateTime.Now.ToString(),
                    FileExtension = ".pdf",
                    LanguageCode = "km-KH",
                    CreatedUser = Convert.ToString(user.ID),
                    CreatedDateTime = DateTime.Now,
                    IsDeleted = false
                };

                _dBContext.documents.Add(request);
                await _dBContext.SaveChangesAsync();

                return "true";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
