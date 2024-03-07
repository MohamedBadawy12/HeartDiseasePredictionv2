using Database.Entities;
using HearPrediction.Api.Data.Services;
using HearPrediction.Api.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HearPrediction.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PrescriptionController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		public PrescriptionController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		//Get All Prescriptions from db
		[HttpGet]
		public async Task<IActionResult> GetAllPrescriptions()
		{
			var result = await _unitOfWork.prescription.GetPrescriptions();
			return Ok(result);
		}

		//Get Prescription from db
		[HttpGet("id")]
		public async Task<IActionResult> GetPrescription(int id)
		{
			var result = await _unitOfWork.prescription.GetPrescription(id);
			return Ok(result);
		}

		//Get Details of Prescription by id from db
		[HttpGet("GetPrescriptionById")]
		public async Task<IActionResult> GetPrescriptionDetails(int id)
		{
			var prescription = await _unitOfWork.prescription.GetPrescription(id);
			if (prescription == null)
				return NotFound($"No prescription was found with Id: {id}");

			var PrescriptionrDetail = new PrescriptionFormDTO
			{
				DoctorId = prescription.DoctorId,
				PatientSSN = prescription.PatientSSN,
				MedicineName = prescription.MedicineName,
				date = prescription.date
			};
			return Ok(PrescriptionrDetail);
		}

		//Get Prescription of Patient by ssn from db
		[HttpGet("GetPrescriptionByPatientSSN")]
		public async Task<IActionResult> GetPrescriptionofPatient(long ssn)
		{
			var prescription = await _unitOfWork.prescription.GetPrescriptionsByUserSSN(ssn);
			if (prescription == null)
				return NotFound($"No prescription with SSN was found with Id: {ssn}");

			return Ok(prescription);
		}

		//Search for Prescription
		[HttpGet("Search")]
		public async Task<IActionResult> SearchForPrescription([FromQuery] long search)
		{
			try
			{
				var result = await _unitOfWork.prescription.FilterPrescriptions(search);
				return Ok(result);
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Error in search for data");
			}
		}

		//Create a Prescription and save it in db
		[HttpPost("Create")]
		public async Task<IActionResult> CreatePrescription([FromBody] PrescriptionFormDTO model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var doctor = await _unitOfWork.Doctors.GetDoctor(model.DoctorId);
			if (doctor == null)
				return BadRequest("Doctor not Found");

			var patient = await _unitOfWork.Patients.GetPatient(model.PatientSSN);
			if (patient == null)
				return BadRequest("Patient not Found");

			var prescription = new Prescription()
			{
				DoctorId = model.DoctorId,
				PatientSSN = model.PatientSSN,
				date = model.date,
				MedicineName = model.MedicineName,
			};

			await _unitOfWork.prescription.Add(prescription);
			await _unitOfWork.Complete();

			doctor.prescriptions.Add(prescription);
			patient.Prescriptions.Add(prescription);
			await _unitOfWork.Complete();
			return Ok(prescription);
		}


		//Edit the Prescription
		[HttpPut("EditPrescription")]
		public async Task<IActionResult> EditPrescription(int id, [FromBody] PrescriptionFormDTO model)
		{
			try
			{
				var prescription = await _unitOfWork.prescription.GetPrescription(id);
				if (prescription == null)
					return NotFound($"No prescription was found with Id: {id}");

				prescription.DoctorId = model.DoctorId;
				prescription.PatientSSN = model.PatientSSN;
				prescription.date = model.date;
				prescription.MedicineName = model.MedicineName;
				await _unitOfWork.Complete();
				return Ok(prescription);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message + "  " + ex.StackTrace);
			}
		}

		//Delete the prescription
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var prescription = _unitOfWork.prescription.Get_Prescription(id);
			if (prescription == null)
				return NotFound($"No prescription was found with Id: {id}");
			try
			{
				_unitOfWork.prescription.Remove(prescription);
				await _unitOfWork.Complete();
				return Ok(prescription);
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data");
			}
		}
	}
}
