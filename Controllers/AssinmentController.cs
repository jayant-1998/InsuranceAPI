using Hangfire;
using InsuranceAPI.Models.ResponseViewModels;
using InsuranceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceAPI.Controllers
{
    [ApiController]
    [Route("insurance")]
    public class AssinmentController : ControllerBase
    {
        private readonly IInsuranceService _service;

        public AssinmentController(IInsuranceService service)
        {
            _service = service;
        }

        [HttpGet("create-pdf/{id}")]
        public async Task<ActionResult> GenerateDocument(int id)
        {
            try
            {
                var result = await _service.PopulateDataAndCreatePdfAsync(id);
                var response = new ApiResponseViewModel
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
                var response = new ApiResponseViewModel
                {
                    Timestamp = DateTime.Now,
                    Code = 500,
                    Message = ex.Message,
                    Body = null
                };
                return Ok(response);
            }
        }

        [HttpGet("send-mails")]
        [Obsolete]
        public string SendEmail()
        {
            try
            {
                _service.GetEmailsAsync();
                var jobId = BackgroundJob.Enqueue(() => _service.GetEmailsAsync());
                //RecurringJob.AddOrUpdate(() => _service.SendEmail(), "*/2 * * * *");
                //IRecurringJobManager.Equals(() => _service.SendEmail(), "*/2 * * * *");
                //var JobId = BackgroundJob.Schedule(() => _service.SendEmail(), TimeSpan.FromMinutes(10));
                RecurringJob.AddOrUpdate("Sending mails",() => _service.GetEmailsAsync(),Cron.Hourly);
                return "sending all emails";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}