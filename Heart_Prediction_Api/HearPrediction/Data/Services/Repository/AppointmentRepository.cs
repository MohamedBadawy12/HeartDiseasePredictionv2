using Database.Entities;
using HearPrediction.Api.Data.Services.IRepository;
using HearPrediction.Api.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HearPrediction.Api.Data.Services.Repository
{
	public class AppointmentRepository : IAppointmentRepository
	{
		private readonly AppDbContext _context;
		public AppointmentRepository(AppDbContext context)
		{
			_context = context;
		}
		public async Task Add(Appointment appointment) =>
			await _context.Appointments.AddAsync(appointment);

		public async Task<int> CountAppointments(long ssn) =>
			await _context.Appointments.CountAsync(a => a.PatientSSN == ssn);

		public IQueryable<Appointment> FilterAppointmentsAsync(SearchDto searchDto)
		{
			var result = _context.Appointments
				//.Include(p => p.Patient)
				.Include(d => d.Doctor).AsQueryable();
			if (searchDto != null)
			{
				if (!string.IsNullOrWhiteSpace(searchDto.Name))
					result = result.Where(a => a.Doctor.User.FirstName == searchDto.Name.ToLower());
				if (!string.IsNullOrWhiteSpace(searchDto.Option))
				{
					if (searchDto.Option == "ThisMonth")
					{
						result = result.Where(x => x.date.Year == DateTime.Now.Year && x.date.Month == DateTime.Now.Month);
					}
					else if (searchDto.Option == "Pending")
					{
						result = result.Where(x => x.Status == false);
					}
					else if (searchDto.Option == "Approved")
					{
						result = result.Where(x => x.Status);
					}
				}
			}
			return result;
		}

		public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync() =>
			 await _context.Appointments.ToListAsync();


		public async Task<Appointment> GetAppointment(int id) =>
			 await _context.Appointments.FirstOrDefaultAsync(x => x.Id == id);


		public async Task<IEnumerable<Appointment>> GetAppointmentByDoctorAsync(int id) =>
			 await _context.Appointments.Where(d => d.DoctorId == id)
				.Include(p => p.Patient).ToListAsync();

		public async Task<AppointmentsDropDownDTO> GetAppointmentsDropDownValues()
		{
			var result = new AppointmentsDropDownDTO()
			{
				Appointments = await _context.Appointments.OrderBy(a => a.date).ToListAsync(),
			};
			return result;
		}

		public async Task<Appointment> GetAppointmentWithDoctor(int id) =>
			await _context.Appointments.Include(d => d.Doctor).FirstOrDefaultAsync(a => a.Id == id);

		public async Task<IEnumerable<Appointment>> GetAppointmentWithPatientAsync(long ssn) =>
			 await _context.Appointments.Where(p => p.PatientSSN == ssn)
				.Include(p => p.Patient).Include(d => d.Doctor)
				.ToListAsync();

		public async Task<IEnumerable<Appointment>> GetDailyAppointmentsAsync(DateTime getDate) =>
			throw new NotImplementedException();

		public async Task<Appointment> GetExistingAppointmentsAsync(int id)
		{
			return await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id && a.Status == false);
		}

		//await _context.Appointments.Where(a => DbFunctions.DiffDays(a.StartTime, getDate) == 0)
		// .Include(p => p.Patient)
		//	.Include(d => d.Doctor)
		//	.ToList();


		public async Task<IEnumerable<Appointment>> GetTodaysAppointmentsAsync(int id)
		{
			DateTime today = DateTime.Now.Date;
			return await _context.Appointments
				.Where(d => d.DoctorId == id && d.date >= today)
				.Include(p => p.Patient)
				.OrderBy(d => d.StartTime)
				.ToListAsync();
		}

		public async Task<IEnumerable<Appointment>> GetUpCommingAppointmentsAsync(string userId)
		{
			DateTime today = DateTime.Now.Date;
			return await _context.Appointments
				.Where(d => d.Doctor.UserId == userId && d.date >= today && d.Status == true)
				.Include(p => p.Patient)
				.OrderBy(d => d.StartTime)
				.ToListAsync();
		}

		public Appointment Get_Appointment(int id) => _context.Appointments.FirstOrDefault(a => a.Id == id);

		public void Remove(Appointment appointment) => _context.Appointments.Remove(appointment);

		public async Task<bool> ValidateAppointment(DateTime appntDate, int id) =>
			await _context.Appointments.AnyAsync(a => a.date == appntDate && a.DoctorId == id);

	}
}
