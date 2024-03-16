using Database.Entities;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Repositories;
using Repositories.ViewModel;
using System;
using System.Threading.Tasks;

namespace HeartDiseasePrediction.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IToastNotification _toastNotification;
        public AttendanceController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
        {
            _unitOfWork = unitOfWork;
            _toastNotification = toastNotification;
        }
        public async Task<ActionResult> Index(int id)
        {
            return View();
        }
        public async Task<ActionResult> Details(int id)
        {
            var attendance = await _unitOfWork.attendances.GetAttendance(id);
            return View("_attendancePartial", attendance);
        }

        public async Task<ActionResult> Create(int id)
        {
            var viewModel = new AttendanceFormViewModel
            {
                Patient = id,
                Heading = "Add Attendance"
            };
            return View("AttendanceForm", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Create(AttendanceFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View("AttendanceForm", viewModel);

            var attendance = new Attendance
            {
                Id = viewModel.Id,
                ClinicRemarks = viewModel.HospitalRemarks,
                Diagnosis = viewModel.Diagnosis,
                SecondDiagnosis = viewModel.SecondDiagnosis,
                ThirdDiagnosis = viewModel.ThirdDiagnosis,
                Therapy = viewModel.Therapy,
                Date = DateTime.Now,
                Patient = await _unitOfWork.Patients.GetPatient(viewModel.Patient)
            };
            await _unitOfWork.attendances.Add(attendance);
            await _unitOfWork.Complete();
            _toastNotification.AddErrorToastMessage("Successfully Saved");
            //ViewBag.Confirm = "Successfully Saved";
            //return PartialView("_Confirmation");
            return RedirectToAction("Details", "Patient", new { id = viewModel.Patient });
        }
    }
}
