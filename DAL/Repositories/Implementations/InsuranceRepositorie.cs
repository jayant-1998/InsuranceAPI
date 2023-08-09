using InsuranceAPI.DAL.DBContexts;
using InsuranceAPI.DAL.Entities;
using InsuranceAPI.DAL.Repositories.Interfaces;
using InsuranceAPI.Models.ResponseViewModels;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.DAL.Repositories.Implementations
{
    public class InsuranceRepositorie : IInsuranceRepositorie
    {
        private readonly ApplicationDBContexts _dBContext;

        public InsuranceRepositorie(IServiceProvider serviceProvider)
        {
            _dBContext = serviceProvider.GetRequiredService<ApplicationDBContexts>();
        }

        public async Task<IEnumerable<EmailResponseModel>> GetAllEmailDBAsync()
        {
            var body = await _dBContext.email
                        .Where(doc => doc.isSend == false
                        && doc.attempts < 3).ToListAsync();

            if (body == null)
            {
                return null;
            }
            var responseEmail = body.Select(doc => new EmailResponseModel
            {
                ID = doc.ID,
                userId = doc.userId,
                name = doc.name,
                email = doc.email,
                subject = doc.subject,
                body = doc.body,
                attachment = doc.attachment,
                attempts = doc.attempts,
                maxAttempts = doc.maxAttempts,
                isSend = doc.isSend,
                createdAt = doc.createdAt,
                modifiedAt = doc.modifiedAt
            });

            return responseEmail;
        }


        public async Task<bool> UpdateEmailDBAsync(EmailResponseModel email ,bool isSend)
        {
            var body = await _dBContext.email
                .Where (doc => doc.isSend == false
                && doc.ID == email.ID
                && doc.userId == email.userId).SingleOrDefaultAsync();

            body.isSend = isSend;
            body.attempts = body.attempts + 1;
            body.modifiedAt = DateTime.Now;
            await _dBContext.SaveChangesAsync();
            return true;

        }

        public async Task<TemplateResponseModel> GetTemplateDBAsync()
        {
            int id = 1;
            var res = await _dBContext.templates
                    .Where(i => i.ID == id)
                    .Select(i => new TemplateResponseModel
                    {
                        ID = i.ID,
                        Name = i.Name,
                        HTML = i.HTML,
                    })
                    .FirstOrDefaultAsync();

            return res;
        }

        public async Task<UserResponseModel> GetUserDBAsync(int id)
        {
            var res = await _dBContext.users
                    .Where(i => i.ID == id)
                    .Select(i => new UserResponseModel
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
                    })
                    .FirstOrDefaultAsync();

            return res;
        }
        public async Task<string> InsertIntoDocumentDBAsync(UserResponseModel user, byte[] pdf)
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

        public async Task<string> InsertIntoEmailDBAsync(UserResponseModel user, byte[] pdf)
        {
            try
            {
                Email email = new Email
                { 
                    userId = user.ID,
                    name = user.Name,
                    email = user.EmailAddress,
                    subject = "Policy",
                    body = "Dear  " + user.Name + ",\n\nThis is the user policy.\n\nBest regards,\nxyz".ToString(),
                    attachment = pdf,
                    maxAttempts = 3,
                    createdAt = DateTime.Now,
                };
                _dBContext.email.Add(email);
                await _dBContext.SaveChangesAsync();

                return "true";
            }
            catch (Exception ex) 
            {
                return ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
        }

        public async Task<bool> IsEmailExitsDBAsync(UserResponseModel user)
        {
            return await _dBContext.email.AnyAsync(e => e.userId == user.ID);
        }
    }
}
