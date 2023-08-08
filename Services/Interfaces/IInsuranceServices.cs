﻿using InsuranceAPI.DAL.Entities;
using InsuranceAPI.Models.ResponseViewModels;

namespace InsuranceAPI.Services.Interfaces
{
    public interface IInsuranceServices
    {
        public string PopulateHtmlTemplateWithUserData(string htmlTemplate, UserResponseModels user);
        public Task<string> populateDataAndCreatePdfSaveInDb(int id);
        public Task<byte[]> HtmlToPdf(string html);
        public Task<bool> SendEmail();
    }
}
