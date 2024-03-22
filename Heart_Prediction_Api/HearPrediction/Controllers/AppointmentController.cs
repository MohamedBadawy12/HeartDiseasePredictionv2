using Database.Entities;
using HearPrediction.Api.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HearPrediction.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AppointmentController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<ApplicationUser> _userManager;
		public AppointmentController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
		}

		//Get All Appointments by User ID
		[Authorize(Roles = "User")]
		[HttpGet()]
		public async Task<IActionResult> Index()
		{
			string userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userName == null)
				return BadRequest("Register Or Login Please");
			var user = await _userManager.FindByNameAsync(userName);
			string userId = user.Id;
			string userRole = User.FindFirstValue(ClaimTypes.Role);
			var appointments = await _unitOfWork.appointments.GetAppointmenByUserId(userId, userRole);
			return Ok(appointments);
		}

		//Get All Appointments by Email
		[Authorize(Roles = "Doctor")]
		[HttpGet("GetAppointmentByEmail")]
		public async Task<IActionResult> GetAppointmentByEmail()
		{
			string doctorEmail = User.FindFirstValue(ClaimTypes.Email);
			string userRole = User.FindFirstValue(ClaimTypes.Role);
			var appointments = await _unitOfWork.appointments.GetAppointmenByEmail(doctorEmail, userRole);
			return Ok(appointments);
		}

		//Get All Appointments by User ID
		[Authorize(Roles = "Doctor")]
		[HttpGet("GetMessageByUserId")]
		public async Task<IActionResult> GetMessageById()
		{
			string userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userName == null)
				return BadRequest("Register Or Login Please");
			var user = await _userManager.FindByNameAsync(userName);
			string userId = user.Id;
			string userRole = User.FindFirstValue(ClaimTypes.Role);
			var messages = await _unitOfWork.appointments.GetMessageByUserId(userId, userRole);
			return Ok(messages);
		}

		//Get All Appointments by Email
		[Authorize(Roles = "User")]
		[HttpGet("GetMessagetByEmail")]
		public async Task<IActionResult> GetMessagetByEmail()
		{
			string patientEmail = User.FindFirstValue(ClaimTypes.Email);
			string userRole = User.FindFirstValue(ClaimTypes.Role);
			var messages = await _unitOfWork.appointments.GetMessageByEmail(patientEmail, userRole);
			return Ok(messages);
		}

		//Create Appointment
		[Authorize(Roles = "User")]
		[HttpPost("BookAppointment")]
		public async Task<IActionResult> Create(int id, BookAppointmentDto model)
		{
			var doctor = await _unitOfWork.Doctors.GetDoctor(id);
			if (doctor == null)
				return BadRequest("NotFound");

			string userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userName == null)
				return BadRequest("Register Or Login Please");
			var user = await _userManager.FindByNameAsync(userName);
			string userId = user.Id;
			string patientEmail = User.FindFirstValue(ClaimTypes.Email);
			var appointment = new Appointment()
			{
				PatientID = userId,
				PateintName = model.PateintName,
				PatientEmail = patientEmail,
				DoctorEmail = doctor.User.Email,
				date = model.Date,
				Time = model.Time,
				PhoneNumber = model.PatientPhone,
				PatientSSN = model.PatientSSN,
				DoctorId = doctor.Id,
			};

			await _unitOfWork.appointments.AddAsync(appointment);
			await _unitOfWork.Complete();
			return Ok("Appointment Created Successfully");
		}

		//Accept Appointment
		[Authorize(Roles = "Doctor")]
		[HttpPost("Accept")]
		public async Task<IActionResult> AcceptsAppointment(int id)
		{
			var appointment = await _unitOfWork.appointments.GetAppointment(id);
			if (appointment == null)
				return BadRequest("NotFound");
			string userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userName == null)
				return BadRequest("Register Or Login Please");
			var user = await _userManager.FindByNameAsync(userName);
			string userId = user.Id;
			string doctorEmail = User.FindFirstValue(ClaimTypes.Email);
			var message = new Message
			{
				Messages = "Your Appointment is Accepted",
				Date = DateTime.Now,
				PatientEmail = appointment.PatientEmail,
				DoctorEmail = doctorEmail,
				DoctorId = userId,
			};
			await _unitOfWork.appointments.AddMessageAsync(message);
			await _unitOfWork.Complete();
			return Ok(message);
		}

		//Cancel Appointment by Doctor
		[Authorize(Roles = "Doctor")]
		[HttpPost("CancelByDoctor")]
		public async Task<IActionResult> CancelAppointment(int id)
		{
			var appointment = _unitOfWork.appointments.Get_Appointment(id);
			if (appointment != null)
			{
				string userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (userName == null)
					return BadRequest("Register Or Login Please");
				var user = await _userManager.FindByNameAsync(userName);
				string userId = user.Id;
				string doctorEmail = User.FindFirstValue(ClaimTypes.Email);
				var message = new Message
				{
					Messages = "Sorry,Your Appointment is Canceled because Doctor is busy in this time",
					Date = DateTime.Now,
					PatientEmail = appointment.PatientEmail,
					DoctorEmail = doctorEmail,
					DoctorId = userId,
				};
				await _unitOfWork.appointments.AddMessageAsync(message);
				await _unitOfWork.Complete();
				_unitOfWork.appointments.Cancel(appointment);
				await _unitOfWork.Complete();
				return Ok(message);
			}
			return BadRequest($"Appointment with {id} is Not Found");
		}

		//Cancel Appointment by Patient
		[Authorize(Roles = "User")]
		[HttpDelete("CancelByPatient")]
		public async Task<IActionResult> CancelAppointmentByPatient(int id)
		{
			var appointment = _unitOfWork.appointments.Get_Appointment(id);
			if (appointment == null)
				return BadRequest($"Appointment with {id} is Not Found");

			_unitOfWork.appointments.Cancel(appointment);
			await _unitOfWork.Complete();
			return Ok(appointment);
		}
	}
}
