using Database.Entities;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }
        public async Task Add(Patient patient) => await _patientRepository.Add(patient);

        public async Task<IEnumerable<Patient>> FilterPatients(string search) => await _patientRepository.FilterPatients(search);

        public async Task<Patient> GetPatient(long ssn) => await _patientRepository.GetPatient(ssn);

        public async Task<IEnumerable<Patient>> GetPatients() => await _patientRepository.GetPatients();

        public async Task<Patient> GetProfile(string userId) => await _patientRepository.GetProfile(userId);

        public async Task<IEnumerable<Patient>> GetRecentPatients() => await _patientRepository.GetRecentPatients();

        public Patient Get_Patient(long ssn) => _patientRepository.Get_Patient(ssn);

        public void Remove(Patient patient) => _patientRepository.Remove(patient);
    }
}
