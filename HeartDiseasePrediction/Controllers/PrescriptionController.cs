using Database.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace HeartDiseasePrediction.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly IToastNotification _toastNotification;
        private readonly IPrescriptionsService _prescriptionService;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PrescriptionController(IToastNotification toastNotification, IPrescriptionsService prescriptionService,
            AppDbContext context, IDoctorService doctorService, IPatientService patientService)
        {
            _toastNotification = toastNotification;
            _prescriptionService = prescriptionService;
            _doctorService = doctorService;
            _patientService = patientService;
            _context = context;
        }
        //Get All Prescriptions
        public async Task<IActionResult> Index()
        {
            var prescriptions = await _prescriptionService.GetPrescriptions();
            return View(prescriptions);
        }
        //Search for Prescriptions
        [HttpPost]
        public async Task<IActionResult> Index(long search)
        {
            var prescriptions = await _prescriptionService.FilterPrescriptions(search);
            return View(prescriptions);
        }

        //get Prescription details
        public async Task<IActionResult> PrescriptionDetails(int id)
        {
            try
            {
                var prescription = await _prescriptionService.GetPrescription(id);
                if (prescription == null)
                    return View("NotFound");

                var prescriptionVM = new Prescription
                {
                    MedicineName = prescription.MedicineName,
                    PatientSSN = prescription.PatientSSN,
                    date = prescription.date,
                    DoctorId = prescription.DoctorId,
                    //Doctor = prescription.Doctor,
                };
                return View(prescriptionVM);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        //Create Prescriptions
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Prescription model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var doctor = await _doctorService.GetDoctor(model.DoctorId);
            if (doctor == null)
                return View("NotFound");

            var patient = await _patientService.GetPatient(model.PatientSSN);
            if (patient == null)
                return View("NotFound");

            try
            {
                await _prescriptionService.AddAsync(model);
                doctor.prescriptions.Add(model);
                patient.Prescriptions.Add(model);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Prescription Created successfully");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                _toastNotification.AddErrorToastMessage("An error occurred while saving the prescription.");
                return View(model);
            }
        }

        //Edit details of Prescription
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var prescription = await _prescriptionService.GetPrescription(id);
                if (prescription == null)
                    return View("NotFound");

                var prescriptionVM = new Prescription
                {
                    MedicineName = prescription.MedicineName,
                    PatientSSN = prescription.PatientSSN,
                    date = prescription.date,
                    DoctorId = prescription.DoctorId,
                    //Doctor = prescription.Doctor,
                };
                return View(prescriptionVM);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Prescription model)
        {
            try
            {
                var prescription = await _prescriptionService.GetPrescription(id);
                if (prescription == null)
                    return View("NotFound");

                //string wwwRootPath = _webHostEnvironment.WebRootPath;
                //string fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                //string extension = Path.GetExtension(model.ImageFile.FileName);
                //model.ProfileImg = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                //string path = Path.Combine(wwwRootPath + "/Upload", fileName);

                //using (var fileStream = new FileStream(path, FileMode.Create))
                //{
                //    await model.ImageFile.CopyToAsync(fileStream);
                //}
                prescription.MedicineName = model.MedicineName;
                prescription.PatientSSN = model.PatientSSN;
                prescription.date = model.date;
                prescription.DoctorId = model.DoctorId;
                //prescription.Doctor = model.Doctor;

                _context.Prescriptions.Update(prescription);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Prescription Updated successfully");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                _toastNotification.AddErrorToastMessage("Prescription Updated Failed");
                return View();
            }
        }

        //Delete Patient 
        public async Task<IActionResult> DeletePrescription(int id)
        {
            var prescription = _prescriptionService.Get_Prescription(id);
            if (prescription != null)
            {
                _prescriptionService.Remove(prescription);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage($"Prescription with ID {id} removed successfully");
            }
            return RedirectToAction("Index");
            //return View("NotFound");
        }
    }
}
