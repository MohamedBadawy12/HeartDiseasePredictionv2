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
		/// Get upcomming appointments for doctor - Admin section
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<IEnumerable<Appointment>> GetTodaysAppointments(int id)
		{
			DateTime today = DateTime.Now.Date;
			return await _context.Appointments
				.Where(d => d.DoctorId == id && d.date >= today)
				.Include(p => p.Patient)
				.OrderBy(d => d.date)
				.ToListAsync();
		}
		/// <summary>
		/// Get upcomming appointments for specific doctor
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public async Task<IEnumerable<Appointment>> GetUpcommingAppointments(string userId)
		{
			DateTime today = DateTime.Now.Date;
			return await _context.Appointments
				.Where(d => d.Doctor.UserId == userId && d.date >= today && d.Status == true)
				.Include(p => p.Patient)
				.OrderBy(d => d.date)
				.ToListAsync();
		}

		public IQueryable<Appointment> FilterAppointments(AppointmentSearchVM searchModel)
		{
			var result = _context.Appointments.Include(p => p.Patient).Include(d => d.Doctor).AsQueryable();
			if (searchModel != null)
			{
				if (!string.IsNullOrWhiteSpace(searchModel.Name))
					result = result.Where(a => a.Doctor.Name == searchModel.Name);
				if (!string.IsNullOrWhiteSpace(searchModel.Option))
				{
					if (searchModel.Option == "ThisMonth")
					{
						result = result.Where(x => Convert.ToDateTime(x.date).Year == DateTime.Now.Year && Convert.ToDateTime(x.date).Month == DateTime.Now.Month);
					}
					else if (searchModel.Option == "Pending")
					{
						result = result.Where(x => x.Status == false);
					}
					else if (searchModel.Option == "Approved")
					{
						result = result.Where(x => x.Status);
					}
				}
			}

			return result;

		}
		/// <summary>
		/// Get Daily appointments
		/// </summary>
		/// <param name="getDate"></param>
		/// <returns></returns>
		public async Task<IEnumerable<Appointment>> GetDaillyAppointments(DateTime getDate)
		{
			return await _context.Appointments
				.Where(a => a.date != null && a.date.Date == getDate.Date)
				.Include(p => p.Patient)
				.Include(d => d.Doctor)
				.ToListAsync();
		}

		/// <summary>
		/// Validate appointment date and time
		/// </summary>
		/// <param name="appntDate"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<bool> ValidateAppointment(DateTime appntDate, int id)
		{
			return await _context.Appointments.AnyAsync(a => a.date == appntDate && a.DoctorId == id);
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
		public async Task<Appointment> GetAppointment(int id)
		{
			return await _context.Appointments.FindAsync(id);
		}

		public async Task AddAsync(Appointment appointment)
		{
			await _context.Appointments.AddAsync(appointment);
		}

	}
}
