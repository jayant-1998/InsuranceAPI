using InsuranceAPI.DAL.DBContext;
using InsuranceAPI.DAL.Entities;
using InsuranceAPI.DAL.Repositories.Interfaces;
using InsuranceAPI.Extensions;
using InsuranceAPI.Models.ResponseViewModels;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.DAL.Repositories.Implementations
{
    public class InsuranceRepository : IInsuranceRepository
    {
        private readonly ApplicationDBContext _dBContext;

        public InsuranceRepository(IServiceProvider serviceProvider)
        {
            _dBContext = serviceProvider.GetRequiredService<ApplicationDBContext>();
        }

        public async Task<IEnumerable<EmailResponseViewModel>> GetEmailsAsync()
        {
            List<EmailEntity> emails = await _dBContext.email
                        .Where(doc => doc.isSend == false
                        && doc.attempts < 3).ToListAsync();

            if (emails == null)
            {
                return null;
            }
            
            List<EmailResponseViewModel> respond = new List<EmailResponseViewModel>();
            foreach (var email in emails)
            {
                var temp = email.ToViewModel<EmailEntity,EmailResponseViewModel>();
                respond.Add(temp);
            }


            return respond;
        }


        public async Task<bool> UpdateEmailAsync(EmailResponseViewModel email ,bool isSend)
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

        public async Task<TemplateResponseViewModel> GetHtmlTemplateAsync()
        {
            int id = 1;
            HtmlTemplateEntity template = await _dBContext.templates
                    .Where(i => i.ID == id)
                    .FirstOrDefaultAsync();


            var respond = template.ToViewModel<HtmlTemplateEntity, TemplateResponseViewModel>();
            return respond;
        }

        public async Task<UserResponseViewModel> GetUserAsync(int id)
        {
            var user = await _dBContext.users
                    .Where(i => i.ID == id)
                    .FirstOrDefaultAsync();

            var respond = user.ToViewModel<UsersEntity, UserResponseViewModel>();

            return respond;
        }
        public async Task<string> InsertDocumentAsync(UserResponseViewModel user, byte[] pdf)
        {
            try 
            {
                var body =  await _dBContext.documents
                           .Where(todo => todo.ObjectCode == $"{user.policyNumber}-{user.productCode}" && todo.IsDeleted == false)
                           .SingleOrDefaultAsync();

                if (body != null)
                {
                    body.IsDeleted = true;
                    await _dBContext.SaveChangesAsync();
                }
                PolicyDocumentEntity request = new PolicyDocumentEntity
                {
                    ObjectCode = $"{user.policyNumber}-{user.productCode}",
                    ReferenceType = "Policy",
                    ReferenceNumber = user.policyNumber,
                    Content = pdf,
                    FileName = $"{user.policyNumber}" + DateTime.Now.ToString(),
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

        public async Task<string> InsertEmailAsync(UserResponseViewModel user, byte[] pdf)
        {
            try
            {
                var email  = user.ToViewModel<UserResponseViewModel, EmailEntity>();
                email.ID = 0;
                email.userId = user.ID;
                email.subject = "Policy";
                email.body = "Dear  " + user.name + ",\n\nThis is the user policy.\n\nBest regards,\nxyz".ToString();
                email.attachment = pdf;
                email.maxAttempts = 3;

                _dBContext.email.Add(email);
                await _dBContext.SaveChangesAsync();

                return "true";
            }
            catch (Exception ex) 
            {
                return ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
        }

        public async Task<bool> IsEmailExitsAsync(UserResponseViewModel user)
        {
            return await _dBContext.email.AnyAsync(e => e.userId == user.ID);
        }
    }
}
