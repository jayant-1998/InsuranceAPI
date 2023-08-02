using InsuranceAPI.DAL.Entities;
using InsuranceAPI.DAL.Repositories.Interfaces;
using InsuranceAPI.Models.RequestViewModels;
using InsuranceAPI.Models.ResponseViewModels;
using InsuranceAPI.Services.Interfaces;
using PuppeteerSharp;
using PuppeteerSharp.Media;
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

        

        public async Task<string> FinalApi(int id)
        {
            var template = await _repositories.GetTemplateDB();
            var userbody = await _repositories.GetUserDB(id).ConfigureAwait(false);

            var html = PopulateHtmlTemplateWithUserData(template.HTML, userbody);

            var pdf = await HtmlToPdf(html);

            var  temp = await _repositories.InsertIntoDocument(userbody, pdf);
            if (temp == "true") 
            {
                return temp;
            }
            return temp;

        }

    }
}
