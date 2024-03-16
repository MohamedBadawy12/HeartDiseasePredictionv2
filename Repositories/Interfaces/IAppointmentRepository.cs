using Database.Entities;
using Repositories.ViewModel;

namespace Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAppointments();
        Task<IEnumerable<Appointment>> GetAppointmentWithPatient(long ssn);
        Task<IEnumerable<Appointment>> GetAppointmentByDoctor(int id);
        Task<IEnumerable<Appointment>> GetTodaysAppointments(int id);
        Task<IEnumerable<Appointment>> GetUpcommingAppointments(string userId);
        Task<IEnumerable<Appointment>> GetDaillyAppointments(DateTime getDate);
        IQueryable<Appointment> FilterAppointments(AppointmentSearchVM searchModel);
        Task<bool> ValidateAppointment(DateTime appntDate, int id);
        Task<int> CountAppointments(long ssn);
        Task<Appointment> GetAppointment(int id);
        Task AddAsync(Appointment appointment);
    }
}
