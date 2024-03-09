using Database.Entities;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task Add(Doctor doctor) => await _doctorRepository.Add(doctor);

        public void Delete(Doctor doctor) => _doctorRepository.Delete(doctor);

        public async Task<IEnumerable<Doctor>> FilterDoctors(string search) =>
            await _doctorRepository.FilterDoctors(search);

        public Doctor FindDoctor(int id) => _doctorRepository.FindDoctor(id);

        public async Task<IEnumerable<Doctor>> GetAvailableDoctors() => await _doctorRepository.GetAvailableDoctors();

        public async Task<Doctor> GetDoctor(int id) => await _doctorRepository.GetDoctor(id);

        public async Task<IEnumerable<Doctor>> GetDoctors() => await _doctorRepository.GetDoctors();

        public async Task<Doctor> GetProfile(string userId) => await _doctorRepository.GetProfile(userId);

        public Doctor Get_Doctor(int id) => _doctorRepository.Get_Doctor(id);
    }
}