using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.DAL.DBContexts
{
    public class ApplicationDBContexts : DbContext

    {
        public ApplicationDBContexts(DbContextOptions options) : base(options)
        {
        }

    }
}
