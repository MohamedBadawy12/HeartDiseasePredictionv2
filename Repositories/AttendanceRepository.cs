using Database.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly AppDbContext _context;
        public AttendanceRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task Add(Attendance attendance)
        {
            await _context.Attendances.AddAsync(attendance);
        }

        public async Task<int> CountAttendances(long ssn)
        {
            return await _context.Attendances.CountAsync(a => a.PatientId == ssn);
        }

        public async Task<IEnumerable<Attendance>> GetAttandences()
        {
            return await _context.Attendances.ToListAsync();
        }
        /// <summary>
        /// Get attandences for single patient
        /// </summary>
        /// <param name="ssn"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Attendance>> GetAttendance(long ssn)
        {
            return await _context.Attendances.Where(p => p.PatientId == ssn).ToListAsync();
        }
        /// <summary>
        /// search  attandences for patient by token 
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Attendance>> GetPatientAttandences(string searchTerm = null)
        {
            var attandences = _context.Attendances.Include(p => p.Patient);
            //if (!string.IsNullOrWhiteSpace(searchTerm))
            //{
            //    attandences = attandences.Where(p => p.Patient.Token.Contains(searchTerm));
            //}
            return await attandences.ToListAsync();
        }
    }
}
