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

        [HttpGet("{id}/CreatePdf")]
        public async Task<ActionResult> GenerateDocument(int id)
        {
            try
            {
                var result = await _service.populateDataAndCreatePdfSaveInDbAsync(id);
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

        [HttpGet("SendMails")]
        [Obsolete]
        public string SendEmail()
        {
            try
            {
                _service.SendAllEmailAsync();
                var jobId = BackgroundJob.Enqueue(() => _service.SendAllEmailAsync());
                //RecurringJob.AddOrUpdate(() => _service.SendEmail(), "*/2 * * * *");
                //IRecurringJobManager.Equals(() => _service.SendEmail(), "*/2 * * * *");
                //var JobId = BackgroundJob.Schedule(() => _service.SendEmail(), TimeSpan.FromMinutes(10));
                RecurringJob.AddOrUpdate("Sending mails",() => _service.SendAllEmailAsync(),Cron.Hourly);
                return "sending all emails";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}