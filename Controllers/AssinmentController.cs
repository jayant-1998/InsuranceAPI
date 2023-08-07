using Hangfire;
using InsuranceAPI.Models.ResponseViewModels;
using InsuranceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace InsuranceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AssinmentController : ControllerBase
    {
        private readonly IInsuranceServices _service;

        public AssinmentController(IInsuranceServices service)
        {
            _service = service;
        }

        [HttpGet("{id}/generatepdf")]
        public async Task<ActionResult> GenerateDocument(int id)
        {
            try
            {
                await _service.SendEmail();
                var result = await _service.FinalApi(id);
                var response = new ApiResponseModel
                {
                    Timestamp = DateTime.Now,
                    Code = 200,
                    Message = "success",
                    Body = result
                };
                return Ok(response);
            }
            catch (Exception ex) 
            {
                var response = new ApiResponseModel
                {
                    Timestamp = DateTime.Now,
                    Code = 500,
                    Message = ex.Message,
                    Body = null
                };
                return Ok(response);
            }
        }

        //[HttpGet("sendemails")]
        //[Obsolete]
        //public string SendEmail()
        //{
        //    try
        //    {
        //        RecurringJob.AddOrUpdate(() => _service.SendEmail(), "*/2 * * * *");

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }

        //    return "sending all emails";
        //}





    }
}