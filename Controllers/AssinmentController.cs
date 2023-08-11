using Hangfire;
using InsuranceAPI.Models.ResponseViewModels;
using InsuranceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceAPI.Controllers
{
    [ApiController]
    [Route("insurance")]
    public class AssignmentController : ControllerBase
    {
        private readonly IInsuranceService _service;

        public AssignmentController(IInsuranceService service)
        {
            _service = service;
        }

        [HttpGet("create-pdf/{id}")]
        public async Task<ActionResult> CreatePdf(int id)
        {
            try
            {
                var result = await _service.CreatePdfAsync(id);
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
        public string SendEmails()
        {
            try
            {
                var jobId = BackgroundJob.Enqueue(() => _service.SendEmailsAsync());
                //RecurringJob.AddOrUpdate(() => _service.SendEmail(), "*/2 * * * *");
                //IRecurringJobManager.Equals(() => _service.SendEmail(), "*/2 * * * *");
                //var JobId = BackgroundJob.Schedule(() => _service.SendEmail(), TimeSpan.FromMinutes(10));
                RecurringJob.AddOrUpdate("Sending mails",() => _service.SendEmailsAsync(),Cron.Hourly);
                return "sending all emails";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}