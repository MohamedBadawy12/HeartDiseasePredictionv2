using Database.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.DropDownViewModel;
using Repositories.Interfaces;
using Repositories.ViewModel;

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
                doctors = await _context.Doctors.OrderBy(a => a.Name).ToListAsync(),
            };
            return data;
        }


        public async Task<Prescription> GetPrescription(int id) =>
             await _context.Prescriptions.Include(d => d.Doctor).FirstOrDefaultAsync(p => p.Id == id);

        public async Task<List<Prescription>> GetPrescriptionByUserId(string userId, string userRole)
        {
            var Prescriptions = await _context.Prescriptions.Include(n => n.Doctorr)
                //.Include(p => p.Patientt)
                .Include(p => p.Doctor)
                .Include(p => p.Patient)
                .ToListAsync();
            if (userRole == "Doctor")
            {
                Prescriptions = Prescriptions.Where(n => n.ApDoctorId == userId).ToList();
            }
            if (userRole == "User")
            {
                Prescriptions = Prescriptions.Where(n => n.ApDoctorId == userId).ToList();
            }
            return Prescriptions;
        }

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
            .Include(p => p.Patient)
            .Include(D => D.Doctorr)
            //.Include(P => P.Patientt)
            .FirstOrDefault(i => i.Id == id);

        public void Remove(Prescription prescription) =>
            _context.Prescriptions.Remove(prescription);

        public async Task AddPrescriptionAsync(PrescriptionViewModel model)
        {
            var prescription = new Prescription()
            {
                ApDoctorId = model.ApDoctorId,
                DoctorEmail = model.DoctorEmail,
                PatientEmail = model.PatientEmail,
                PatientSSN = model.PatientSSN,
                DoctorId = model.DoctorId,
                date = model.date,
                MedicineName = model.MedicineName,
            };
            await _context.Prescriptions.AddAsync(prescription);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Prescription>> GetPrescriptionByEmail(string Email, string userRole)
        {
            var Prescriptions = await _context.Prescriptions.Include(n => n.Doctorr)
                //.Include(p => p.Patientt)
                .Include(p => p.Doctor)
                .Include(p => p.Patient)
                .ToListAsync();
            if (userRole == "Doctor")
            {
                Prescriptions = Prescriptions.Where(n => n.DoctorEmail == Email).ToList();
            }
            if (userRole == "User")
            {
                Prescriptions = Prescriptions.Where(n => n.PatientEmail == Email).ToList();
            }
            return Prescriptions;
        }
    }
}
