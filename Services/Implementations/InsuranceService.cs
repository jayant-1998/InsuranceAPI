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
        private readonly IInsuranceRepositorie _repositories;

        public InsuranceService(IServiceProvider serviceProvider)
        {
            _repositories = serviceProvider.GetRequiredService<IInsuranceRepositorie>();
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

        public async Task<bool> SendAllEmailAsync()
        {
            var emails = await _repositories.GetAllEmailDBAsync().ConfigureAwait(false);
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
                            //await Console.Out.WriteLineAsync(str);
                            await smtpClient.DisconnectAsync(true);
                            var check = await _repositories.UpdateEmailDBAsync(Email, true);
                            if (check != true)
                            {
                                await Console.Out.WriteLineAsync(Email.ID + " was not updated in the database");
                            }
                        }
                        catch (Exception ex)
                        {
                            await smtpClient.DisconnectAsync(true);
                            bool check = await _repositories.UpdateEmailDBAsync(Email, false);
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

        public async Task<string> populateDataAndCreatePdfSaveInDbAsync(int id)
        {
            var htmlTemplate = await _repositories.GetTemplateDBAsync();
            var userbody = await _repositories.GetUserDBAsync(id).ConfigureAwait(false);

            //Populate data in the template 
            string html = htmlTemplate.HTML.PopulateHtmlTemplateWithUserData(userbody);

            //convert html to pdf 
            var pdf = await HtmlToPdfAsync(html,id);

            //make a new row in Policy Documents and delete previous one
            var doc = await _repositories.InsertIntoDocumentDBAsync(userbody, pdf);

            // regex to check email is valid or not
            if (!Regex.Match(userbody.EmailAddress,Pattern).Success)
            {
                return id + " this id email is not valid";  
            }
            if (doc == "true")
            {
                var exits = await _repositories.IsEmailExitsDBAsync(userbody);
                if (exits == false)
                {
                    var email = await _repositories.InsertIntoEmailDBAsync(userbody, pdf);
                    if (email == "true")
                    {
                        return email;
                    }
                    return email;
                }
                return "already exits in emails table";
            }
            return doc;
        }
    }
}
