using Database.Entities;
using HearPrediction.Api.Data.Services;
using HearPrediction.Api.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace HearPrediction.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AppointmentController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly IUnitOfWork _unitOfWork;
		public AppointmentController(IUnitOfWork unitOfWork, AppDbContext context)
		{
			_context = context;
			_unitOfWork = unitOfWork;
		}
		//Get All Appointments from db
		[HttpGet]
		public async Task<IActionResult> GetAppointments()
		{
			var appointments = await _unitOfWork.appointment.GetAllAppointmentsAsync();
			return Ok(appointments);
		}

		//Get Appointment details from db
		[HttpGet("GetDetailSSN")]
		public async Task<IActionResult> GetDetails(long ssn)
		{
			var appointment = await _unitOfWork.appointment.GetAppointmentWithPatientAsync(ssn);
			return Ok(appointment);
		}

		//Create an Appointment and save in db
		[HttpPost("Create")]
		public async Task<IActionResult> CreateApp([FromBody] AppointmentFormDto model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var doctor = await _unitOfWork.Doctors.GetDoctor(model.doctorId);
			if (doctor == null)
				return BadRequest("Doctor not Found");

			var appointment = new Appointment()
			{
				date = model.Date,
				StartTime = model.StartTime,
				EndTime = model.EndTime,
				Detail = model.Detail,
				Status = false,
				//PatientSSN = model.patientSSN,
				DoctorId = model.doctorId,
				//Doctor = await _unitOfWork.Doctors.GetDoctor(model.doctorId),
			};

			//if (await _unitOfWork.appointment.ValidateAppointment(appointment.StartTime, model.doctorId))
			//	return Ok("InvalidAppointment");
			await _unitOfWork.appointment.Add(appointment);
			await _unitOfWork.Complete();

			//doctor.Appointments.Add(appointment);
			//await _unitOfWork.Complete();

			return Ok(appointment);
		}

		[HttpPost("book-appointment")]
		public async Task<ActionResult> BookAppointment([FromBody] AppointmentFormDto appointment)
		{
			if (appointment == null)
			{
				return BadRequest(new { Error = "Invalid appointment data." });
			}
			var existingAppointment = _unitOfWork.appointment.GetExistingAppointmentsAsync(appointment.id);
			if (existingAppointment == null)
			{
				return NotFound(new { Error = "Appointment not found or already booked." });
			}
			existingAppointment.Status.Equals(true);
			_context.Entry(existingAppointment).State = EntityState.Modified;
			await _unitOfWork.Complete().ConfigureAwait(false);
			return Ok(existingAppointment);

		}

		//[HttpPost("book/{appointmentId}")]
		//public async Task<ActionResult> BookAppointment(int appointmentId, [FromBody] Patient patient)
		//{
		//	var appointment = await _unitOfWork.appointment.GetAppointmentWithDoctor(appointmentId);
		//	if (appointment == null)
		//	{
		//		return NotFound("Appointment not found.");
		//	}

		//	// Check if the appointment is available (not already booked)
		//	if (appointment.PatientSSN.HasValue)
		//	{
		//		return BadRequest("Appointment is already booked.");
		//	}

		//	// Proceed with booking the appointment
		//	appointment.PatientSSN = patient.SSN;
		//	await _unitOfWork.Complete();

		//	return Ok("Appointment booked successfully.");
		//}

		//Edit an Appointment
		[HttpPut("edit/{appointmentId}")]
		public async Task<IActionResult> EditApp(int appointmentId, [FromBody] AppointmentFormDto model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var appointment = await _unitOfWork.appointment.GetAppointment(appointmentId);
			if (appointment == null)
				return BadRequest("Appointment not Found");

			//model.Heading = "New Appointment";
			//model.id = appointment.Id;
			appointment.date = model.Date;
			appointment.StartTime = model.StartTime;
			appointment.EndTime = model.EndTime;
			appointment.Detail = model.Detail;
			appointment.Status = model.Status;
			//model.patientSSN = appointment.PatientSSN;
			appointment.DoctorId = model.doctorId;
			//Patients = _unitOfWork.Patients.GetPatients(),
			//model.Doctors =await _unitOfWork.Doctors.GetDectors();
			//appointment.PatientSSN = model.patientSSN;

			await _unitOfWork.Complete();
			return Ok(appointment);
		}

		//Delete Appointment
		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			try
			{
				var appointments = _unitOfWork.appointment.Get_Appointment(id);
				if (appointments == null)
					return NotFound($"No doctor was found with Id: {id}");
				_unitOfWork.appointment.Remove(appointments);
				_unitOfWork.Complete();
				return Ok(appointments);
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data");
			}
		}
	}
}
