﻿using InsuranceAPI.DAL.Entities;
using InsuranceAPI.Models.ResponseViewModels;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceAPI.DAL.Repositories.Interfaces
{
    public interface IInsuranceRepositories
    {
        public Task<TemplateResponseModels> GetTemplateDB();
        public Task<UserResponseModels> GetUserDB(int id);
        public Task<string> InsertIntoDocumentDB(UserResponseModels user, byte[] pdf);
        public Task<PolicyDocument> GetDocummentDb(int id, UserResponseModels user);
    }
}
