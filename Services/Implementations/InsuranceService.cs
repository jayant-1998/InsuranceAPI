using InsuranceAPI.DAL.Repositories.Interfaces;
using InsuranceAPI.Services.Interfaces;
using MimeKit;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using MailKit.Security;
using System.Text.RegularExpressions;
using InsuranceAPI.Extensions;

namespace InsuranceAPI.Services.Implementations
{
    public class InsuranceService : IInsuranceService
    {
        private const string UserName = "jg986511@gmail.com";
        private const string Password = "hwzgnejrbmlwoupa";
        private const string Pattern = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";
        private readonly IInsuranceRepository _repositories;

        public InsuranceService(IServiceProvider service)
        {
            _repositories = service.GetRequiredService<IInsuranceRepository>();
        }

        public async Task<byte[]> HtmlToPdfAsync(string html,int id)
        {
            await new BrowserFetcher().DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
            await using var page = await browser.NewPageAsync();
            await page.EmulateMediaTypeAsync(MediaType.Screen);
            await page.SetContentAsync(html);
            var pdfContent = await page.PdfDataAsync(new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true
            });
           
            File.WriteAllBytes("pdf/Created.pdf", pdfContent);

            return pdfContent;
        }

        public async Task<bool> SendEmailsAsync()
        {
            var emails = await _repositories.GetEmailsAsync().ConfigureAwait(false);
            if (emails.Count() >= 1)
            {
                foreach (var Email in emails)
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("jayant", "jg986511@gmail.com"));
                    message.To.Add(new MailboxAddress(Email.name, MailboxAddress.Parse(Email.email).ToString()));
                    message.Subject = Email.subject;
                    string textBody = Email.body;
                    BodyBuilder bodyBuilder = new BodyBuilder
                    {
                        TextBody = textBody
                    };
                    bodyBuilder.Attachments.Add("Policy.pdf", Email.attachment, new ContentType("application", "pdf"));
                    message.Body = bodyBuilder.ToMessageBody();
                    using (var smtpClient = new MailKit.Net.Smtp.SmtpClient())
                    {
                        smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        await smtpClient.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                        await smtpClient.AuthenticateAsync(UserName, Password);
                        try
                        {
                            await smtpClient.SendAsync(message);//not throw exception when email is not valid
                            await smtpClient.DisconnectAsync(true);
                            var check = await _repositories.UpdateEmailAsync(Email, true);
                            if (check != true)
                            {
                                await Console.Out.WriteLineAsync(Email.ID + " was not updated in the database");
                            }
                        }
                        catch (Exception ex)
                        {
                            await smtpClient.DisconnectAsync(true);
                            bool check = await _repositories.UpdateEmailAsync(Email, false);
                            if (check != true)
                            {
                                await Console.Out.WriteLineAsync(ex.Message);
                                await Console.Out.WriteLineAsync(Email.ID + " was not updated in the database");
                            }
                            await Console.Out.WriteLineAsync(ex.Message);
                            await Console.Out.WriteLineAsync(Email.ID + " was updated in the database");
                            throw;
                        }
                    }
                }
                return true;
            }
            await Console.Out.WriteLineAsync("No Emails found");
            return true;
        }

        public async Task<string> CreatePdfAsync(int id)
        {
            var htmlTemplate = await _repositories.GetHtmlTemplateAsync();
            var userDetails = await _repositories.GetUserAsync(id).ConfigureAwait(false);
            if (!Regex.Match(userDetails.email, Pattern).Success)
            {
                return "This id email is not valid";
            }
            string html = htmlTemplate.HTML.PopulateDataInHtmlTemplate(userDetails);
            var pdf = await HtmlToPdfAsync(html,id);
            var document = await _repositories.InsertDocumentAsync(userDetails, pdf);
            if (document == "true")
            {
                var exits = await _repositories.IsEmailExitsAsync(userDetails);
                if (exits == false)
                {
                    var email = await _repositories.InsertEmailAsync(userDetails, pdf);
                    if (email == "true")
                    {
                        return email;
                    }
                    return email;
                }
                return "already exits in emails table";
            }
            return document;
        }
    }
}
