using Database.Entities;
using HearPrediction.Api.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HearPrediction.Api.Data.Services.IRepository
{
	public interface IAppointmentRepository
	{
		Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
		Task<IEnumerable<Appointment>> GetAppointmentWithPatientAsync(long ssn);
		Task<IEnumerable<Appointment>> GetAppointmentByDoctorAsync(int id);
		Task<IEnumerable<Appointment>> GetTodaysAppointmentsAsync(int id);
		Task<Appointment> GetExistingAppointmentsAsync(int id);
		Task<IEnumerable<Appointment>> GetUpCommingAppointmentsAsync(string userId);
		Task<IEnumerable<Appointment>> GetDailyAppointmentsAsync(DateTime getDate);
		IQueryable<Appointment> FilterAppointmentsAsync(SearchDto searchDto);
		Task<AppointmentsDropDownDTO> GetAppointmentsDropDownValues();
		Task<bool> ValidateAppointment(DateTime appntDate, int id);
		Task<int> CountAppointments(long ssn);
		Task<Appointment> GetAppointment(int id);
		Task<Appointment> GetAppointmentWithDoctor(int id);
		Appointment Get_Appointment(int id);
		Task Add(Appointment appointment);
		void Remove(Appointment appointment);
	}
}
