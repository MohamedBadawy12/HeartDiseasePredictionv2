using HearPrediction.Api.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Repositories.ViewModel;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HearPrediction.Api.Controllers
{
    [Authorize(Roles = "Doctor")]
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public PrescriptionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        //Get All Prescriptions By UserID
        [HttpGet()]
        public async Task<IActionResult> GetAllPrescriptionsByUserId()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userRole = User.FindFirstValue(ClaimTypes.Role);
            var prescriptions = await _unitOfWork.prescriptions.GetPrescriptionByUserId(userId, userRole);
            return Ok(prescriptions);
        }
        //Get All Prescriptions By Email
        [Authorize(Roles = "User")]
        [HttpGet("GetAllPrescriptionsByEmail")]
        public async Task<IActionResult> GetAllPrescriptionsByEmail()
        {
            string PatientEmail = User.FindFirstValue(ClaimTypes.Email);
            string userRole = User.FindFirstValue(ClaimTypes.Role);
            var prescriptions = await _unitOfWork.prescriptions.GetPrescriptionByEmail(PatientEmail, userRole);
            return Ok(prescriptions);
        }
        //Get Prescription from db
        [HttpGet("id")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPrescription(int id)
        {
            var result = await _unitOfWork.prescriptions.GetPrescription(id);
            return Ok(result);
        }

        //Get Details of Prescription by id from db
        [HttpGet("GetPrescriptionById")]
        public async Task<IActionResult> GetPrescriptionDetails(int id)
        {
            var prescription = await _unitOfWork.prescriptions.GetPrescription(id);
            if (prescription == null)
                return NotFound($"No prescription was found with Id: {id}");

            var PrescriptionrDetail = new PrescriptionFormDTO
            {
                DoctorId = prescription.DoctorId,
                PatientSSN = prescription.PatientSSN,
                MedicineName = prescription.MedicineName,
                Date = prescription.date
            };
            return Ok(PrescriptionrDetail);
        }

        //Get Prescription of Patient by ssn from db
        [HttpGet("GetPrescriptionByPatientSSN")]
        public async Task<IActionResult> GetPrescriptionofPatient(long ssn)
        {
            var prescription = await _unitOfWork.prescriptions.GetPrescriptionsByUserSSN(ssn);
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
                var result = await _unitOfWork.prescriptions.FilterPrescriptions(search);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in search for data");
            }
        }

        //Create a Prescription and save it in db
        [HttpPost("Create")]
        public async Task<IActionResult> CreatePrescription([FromBody] PrescriptionViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var doctor = await _unitOfWork.Doctors.GetDoctor(model.DoctorId);
            if (doctor == null)
                return BadRequest("Doctor not Found");

            var patient = await _unitOfWork.Patients.GetPatient(model.PatientSSN);
            if (patient == null)
                return BadRequest("Patient not Found");

            //var prescription = new Prescription()
            //{
            //	DoctorId = model.DoctorId,
            //	PatientSSN = model.PatientSSN,
            //	date = model.Date,
            //	MedicineName = model.MedicineName,
            //	DoctorEmail = model.DoctorEmail,
            //	PatientEmail = model.PatientEmail,
            //	ApDoctorId = model.ApDoctorId,
            //	PatientID = model.PatientID,
            //};
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string doctorEmail = User.FindFirstValue(ClaimTypes.Email);
            model.ApDoctorId = userId;
            model.DoctorEmail = doctorEmail;
            await _unitOfWork.prescriptions.AddPrescriptionAsync(model);
            //await _unitOfWork.Complete();

            //doctor.prescriptions.Add(prescription);
            //patient.Prescriptions.Add(prescription);
            //await _unitOfWork.Complete();
            return Ok(model);
        }


        //Edit the Prescription
        [HttpPut("EditPrescription")]
        public async Task<IActionResult> EditPrescription(int id, [FromBody] PrescriptionFormDTO model)
        {
            try
            {
                var prescription = await _unitOfWork.prescriptions.GetPrescription(id);
                if (prescription == null)
                    return NotFound($"No prescription was found with Id: {id}");

                prescription.DoctorId = model.DoctorId;
                prescription.PatientSSN = model.PatientSSN;
                prescription.date = model.Date;
                prescription.MedicineName = model.MedicineName;
                prescription.ApDoctorId = model.ApDoctorId;
                //prescription.PatientID = model.PatientID;
                prescription.DoctorEmail = model.DoctorEmail;
                prescription.PatientEmail = model.PatientEmail;
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
            var prescription = _unitOfWork.prescriptions.Get_Prescription(id);
            if (prescription == null)
                return NotFound($"No prescription was found with Id: {id}");
            try
            {
                _unitOfWork.prescriptions.Remove(prescription);
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
