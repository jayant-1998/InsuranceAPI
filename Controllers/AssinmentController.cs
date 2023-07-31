using Microsoft.AspNetCore.Mvc;
using InsuranceAPI.Entity;

namespace InsuranceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AssinmentController : ControllerBase
    {
        private readonly InsuranceContext _context;

        public AssinmentController(InsuranceContext context)
        {
            _context = context;
        }


        [HttpGet("{PolicyNumber}")]
        public IActionResult PopulateDataFromHtml(int policynumber)
        {
            try
            {
                var Insur = _context.Insurances
                    .Where(i => i.PolicyNumber == policynumber).ToList();

            }
            catch (Exception ex) 
            { 
                
            }
        }
    }
}