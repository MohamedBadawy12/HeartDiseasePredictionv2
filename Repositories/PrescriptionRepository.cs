using Database.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.DropDownViewModel;
using Repositories.Interfaces;

namespace Repositories
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly AppDbContext _context;
        public PrescriptionRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Prescription prescription) =>
            await _context.Prescriptions.AddAsync(prescription);

        public async Task<IEnumerable<Prescription>> FilterPrescriptions(long search)
        {
            var prescriptions = await GetPrescriptions();
            if (search != 0)
            {
                prescriptions = await _context.Prescriptions.
                Where(x => x.PatientSSN.Equals(search)).ToListAsync();
            }
            return prescriptions;
        }

        public async Task<DoctorDropDownViewMode> GetDoctorDropDownsValues()
        {
            var data = new DoctorDropDownViewMode()
            {
                doctors = await _context.Doctors.OrderBy(a => a.User.Email)
                /*.OrderBy(x => x.User.LastName)*/.ToListAsync(),
            };
            return data;
        }

        public async Task<Prescription> GetPrescription(int id) =>
             await _context.Prescriptions.Include(d => d.Doctor).FirstOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<Prescription>> GetPrescriptions() =>
             await _context.Prescriptions.Include(d => d.Doctor).ToListAsync();

        public async Task<List<Prescription>> GetPrescriptionsByUserSSN(long ssn)
        {
            var prescriptions = await _context.Prescriptions.Include(d => d.Doctor).Where(n => n.PatientSSN == ssn).ToListAsync();
            //if (userRole == "User")
            //{
            //	prescriptions = prescriptions.Where(n => n.PatientSSN == ssn).ToList();
            //}
            return prescriptions;
        }

        public Prescription Get_Prescription(int id) =>
             _context.Prescriptions
            .Include(d => d.Doctor)
            .Include(d => d.Patient)
            .FirstOrDefault(p => p.Id == id);

        public void Remove(Prescription prescription) =>
            _context.Prescriptions.Remove(prescription);
    }
}
