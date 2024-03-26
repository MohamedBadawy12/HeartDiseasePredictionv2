﻿using Database.Entities;
using Repositories.DropDownViewModel;
using Repositories.ViewModel;

namespace Repositories.Interfaces
{
    public interface IPrescriptionRepository
    {
        Task AddPrescriptionAsync(PrescriptionViewModel model);
        Task<List<Prescription>> GetPrescriptionByUserId(string userId, string userRole);
        Task<List<Prescription>> GetPrescriptionByEmail(string Email, string userRole);
        Task<IEnumerable<Prescription>> GetPrescriptions();
        Task<List<Prescription>> GetPrescriptionsByUserSSN(long ssn);
        Task<IEnumerable<Prescription>> FilterPrescriptions(long search);
        Task<Prescription> GetPrescription(int id);
        Prescription Get_Prescription(int id);
        Task<DoctorDropDownViewMode> GetDoctorDropDownsValues();
        Task AddAsync(Prescription prescription);
        void Remove(Prescription prescription);
        bool Delete(int id);
    }
}
