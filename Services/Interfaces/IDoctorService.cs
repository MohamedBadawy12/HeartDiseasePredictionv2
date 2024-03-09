using Database.Entities;

namespace Services.Interfaces
{
    public interface IDoctorService
    {
        Task<IEnumerable<Doctor>> GetDoctors();
        Task<IEnumerable<Doctor>> GetAvailableDoctors();
        Task<Doctor> GetDoctor(int id);
        Doctor FindDoctor(int id);
        //Task<NewDoctorDropDownViewModel> GetNewDoctorDropDownsValues();
        Task<IEnumerable<Doctor>> FilterDoctors(string search);
        Doctor Get_Doctor(int id);
        Task<Doctor> GetProfile(string userId);
        Task Add(Doctor doctor);
        void Delete(Doctor doctor);
    }

    //Generic Repo
}



