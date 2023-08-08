using InsuranceAPI.DAL.Repositories.Interfaces;
using InsuranceAPI.Models.ResponseViewModels;
using InsuranceAPI.Services.Interfaces;
using MimeKit;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Reflection;
using MailKit.Security;

namespace InsuranceAPI.Services.Implementations
{
    public class InsuranceServices : IInsuranceServices
    {
        private const string UserName = "jg986511@gmail.com";
        private const string Password = "hwzgnejrbmlwoupa";
        private readonly IInsuranceRepositories _repositories;

        public InsuranceServices(IServiceProvider serviceProvider)
        {
            _repositories = serviceProvider.GetRequiredService<IInsuranceRepositories>();
        }
        public string PopulateHtmlTemplateWithUserData(string htmlTemplate, UserResponseModels user)
        {
            PropertyInfo[] properties = typeof(UserResponseModels).GetProperties();

            foreach (var property in properties)
            {
                string placeholder = $"{{{{{property.Name}}}}}";
                object value = property.GetValue(user);
                string valueString = value != null ? value.ToString() : string.Empty;

                htmlTemplate = htmlTemplate.Replace(placeholder, valueString);
            }

            return htmlTemplate;
        }

        public async Task<byte[]> HtmlToPdf(string html)
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

            File.WriteAllBytes("pdf/Converted.pdf", pdfContent);

            return pdfContent;
        }

        public async Task<bool> SendEmail()
        {
            var emails = await _repositories.GetEmailDb().ConfigureAwait(false);
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
                            var str = smtpClient.Send(message);//not throw exception when email is not valid
                            //await Console.Out.WriteLineAsync(str);
                            await smtpClient.DisconnectAsync(true);
                            var check = await _repositories.UpdateEmailDB(Email, true);
                            if (check != true)
                            {
                                await Console.Out.WriteLineAsync(Email.ID + " was not updated in the database");
                            }
                        }
                        catch (Exception ex)
                        {
                            await smtpClient.DisconnectAsync(true);
                            bool check = await _repositories.UpdateEmailDB(Email, false);
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

        public async Task<string> populateDataAndCreatePdfSaveInDb(int id)
        {
            var template = await _repositories.GetTemplateDB();
            var userbody = await _repositories.GetUserDB(id).ConfigureAwait(false);

            var html = PopulateHtmlTemplateWithUserData(template.HTML, userbody);

            var pdf = await HtmlToPdf(html);

            var doc = await _repositories.InsertIntoDocumentDB(userbody, pdf);
            if (doc == "true")
            {
                var exits = await _repositories.IsUserExitsDB(userbody);
                if (exits == false)
                {
                    var email = await _repositories.InsertIntoEmailDB(userbody, pdf);
                    if (email == "true")
                    {
                        return email;
                    }
                    return email;
                }
                return "already exits in the table email";
            }
            return doc;
        }
    }
}
