using Database.Entities;

namespace Services.Interfaces
{
    public interface IDoctorService
    {
        Task<IEnumerable<Doctor>> GetDoctors();
    }

    //Generic Repo
}



