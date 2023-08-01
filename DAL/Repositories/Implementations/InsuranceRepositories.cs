using InsuranceAPI.DAL.DBContexts;
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
                Name = i.Name,
                PolicyNumber = i.PolicyNumber,
                Age = i.Age,
                Salary = i.Salary,
                Occupation = i.Occupation,
                PolicyExpiryDate = i.PolicyExpiryDate,
                ProductCode = i.ProductCode,    

            }).FirstOrDefaultAsync();

            return res;
        }
    }
}
