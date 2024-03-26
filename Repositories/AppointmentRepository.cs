using Database.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.ViewModel;

namespace Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;
        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Get all appointments
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Appointment>> GetAppointments()
        {
            return await _context.Appointments
                .Include(p => p.Patient)
                .Include(d => d.Doctor)
                .ToListAsync();
        }
        /// <summary>
        /// Get appointments for single patient
        /// </summary>
        /// <param name="ssn"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Appointment>> GetAppointmentWithPatient(long ssn)
        {
            return await _context.Appointments
                .Where(p => p.PatientSSN == ssn)
                .Include(p => p.Patient)
                .Include(d => d.Doctor)
                .ToListAsync();
        }
        /// <summary>
        /// Get appointments for single doctor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Appointment>> GetAppointmentByDoctor(int id)
        {
            //return (from a in _context.Appointments where a.DoctorId == id select a).AsEnumerable();

            return await _context.Appointments
                .Where(d => d.DoctorId == id)
                .Include(p => p.Patient)
                .ToListAsync();
        }

        /// <summary>
        /// Get number of appointments for defined patient
        /// </summary>
        /// <param name="ssn"></param>
        /// <returns></returns>
        public async Task<int> CountAppointments(long ssn)
        {
            return await _context.Appointments.CountAsync(a => a.PatientSSN == ssn);
        }


        /// <summary>
        /// Get single appointment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Appointment> GetAppointment(int id) =>
            await _context.Appointments
            .Include(d => d.Doctor)
            .Include(p => p.Patientt)
            .Include(P => P.Patient)
            .FirstOrDefaultAsync(i => i.Id == id);


        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
        }

        public async Task AddAppointmentAsync(AppointmentViewModel model)
        {
            var appointment = new Appointment()
            {
                PatientID = model.PatientID,
                DoctorEmail = model.DoctorEmail,
                PatientEmail = model.PatientEmail,
                PatientSSN = model.PatientSSN,
                DoctorId = model.DoctorId,
                date = model.date,
                Time = model.Time,
            };
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Appointment>> GetAppointmenByUserId(string userId, string userRole)
        {
            var appointments = await _context.Appointments/*.Include(n => n.Doctorr)*/
                 .Include(p => p.Patientt)
                 .Include(p => p.Doctor)
                 .Include(p => p.Patient)
                 .ToListAsync();
            if (userRole == "User")
            {
                appointments = appointments.Where(n => n.PatientID == userId).ToList();
            }
            return appointments;
        }

        public async Task<List<Appointment>> GetAppointmenByEmail(string Email, string userRole)
        {
            var appointments = await _context.Appointments
                .Include(p => p.Patientt)
                .Include(p => p.Doctor)
                .Include(p => p.Patient)
                .ToListAsync();
            if (userRole == "Doctor")
            {
                appointments = appointments.Where(n => n.DoctorEmail == Email).ToList();
            }
            if (userRole == "User")
            {
                appointments = appointments.Where(n => n.PatientEmail == Email).ToList();
            }
            return appointments;
        }

        public void Cancel(Appointment appointment) =>
            _context.Appointments.Remove(appointment);

        public Appointment Get_Appointment(int id) =>
             _context.Appointments
            .Include(d => d.Doctor)
            .Include(p => p.Patient)
            .Include(P => P.Patientt)
            .FirstOrDefault(i => i.Id == id);

        public async Task AddMessageAsync(Message message) =>
            await _context.Messages.AddAsync(message);

        public async Task<List<Message>> GetMessageByUserId(string userId, string userRole)
        {
            var messages = await _context.Messages
                 .Include(p => p.Doctor)
                 .ToListAsync();
            if (userRole == "Doctor")
            {
                messages = messages.Where(n => n.DoctorId == userId).ToList();
            }
            return messages;
        }
        public async Task<List<Message>> GetMessageByEmail(string Email, string userRole)
        {
            var messages = await _context.Messages
                 .Include(p => p.Doctor)
                 .ToListAsync();
            if (userRole == "User")
            {
                messages = messages.Where(n => n.PatientEmail == Email).ToList();
            }
            return messages;
        }

        public bool Canceled(int id)
        {
            var isDeleted = false;

            var appointment = _context.Appointments
            .Include(d => d.Doctor)
            .Include(p => p.Patient)
            .Include(P => P.Patientt)
            .FirstOrDefault(i => i.Id == id);

            if (appointment is null)
                return isDeleted;

            _context.Remove(appointment);
            var effectedRows = _context.SaveChanges();

            if (effectedRows > 0)
            {
                isDeleted = true;
            }

            return isDeleted;
        }
    }
}
