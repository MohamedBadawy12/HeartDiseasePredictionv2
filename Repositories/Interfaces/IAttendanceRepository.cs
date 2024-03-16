using Database.Entities;

namespace Repositories.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<IEnumerable<Attendance>> GetAttandences();
        Task<IEnumerable<Attendance>> GetAttendance(long ssn);
        Task<IEnumerable<Attendance>> GetPatientAttandences(string searchTerm = null);
        Task<int> CountAttendances(long ssn);
        Task Add(Attendance attendance);
    }
}
