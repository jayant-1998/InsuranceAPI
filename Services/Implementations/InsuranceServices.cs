using InsuranceAPI.DAL.Repositories.Interfaces;
using InsuranceAPI.Models.ResponseViewModels;
using InsuranceAPI.Services.Interfaces;
using MimeKit;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using MailKit.Net.Smtp;
using System.Reflection;

namespace InsuranceAPI.Services.Implementations
{
    public class InsuranceServices : IInsuranceServices
    {
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

        public async Task<bool> SendEmail(int id)
        {
            var user = await _repositories.GetUserDB(id).ConfigureAwait(false);
            var doc = await _repositories.GetDocummentDb(id, user).ConfigureAwait(false);


            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("jayant", "jayant.grover@remotestate.com"));
            message.To.Add(new MailboxAddress(user.Name, user.EmailAddress));

            message.Subject = "Policy";

            string textBody = "Dear user,\n\nThis is the user policy.\n\nBest regards,\nxyz";

            BodyBuilder bodyBuilder = new BodyBuilder()
            {
                TextBody = textBody
            };

            bodyBuilder.Attachments.Add("Policy.pdf", doc.Content, new ContentType("application", "pdf"));

            message.Body = bodyBuilder.ToMessageBody();

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect("smtp.remotestate.com", 587, false);

                
                smtpClient.Authenticate("jayant.grover@remotestate.com", "grover@@@@");

                try
                {
                    await smtpClient.SendAsync(message);
                    smtpClient.Disconnect(true);
                    return true;
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync(ex.Message);
                    return false;
                }
            }
        }
        public async Task<string> FinalApi(int id)
        {
            var template = await _repositories.GetTemplateDB();
            var userbody = await _repositories.GetUserDB(id).ConfigureAwait(false);

            var html = PopulateHtmlTemplateWithUserData(template.HTML, userbody);

            var pdf = await HtmlToPdf(html);

            var  temp = await _repositories.InsertIntoDocumentDB(userbody, pdf);
            if (temp == "true") 
            {
                return temp;
            }
            return temp;

        }

    }
}
