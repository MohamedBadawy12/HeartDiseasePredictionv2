using Database.Entities;
using HeartDiseasePrediction.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using Repositories;
using Repositories.ViewModel;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HeartDiseasePrediction.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IToastNotification _toastNotification;

        public AppointmentController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
        {
            _unitOfWork = unitOfWork;
            _toastNotification = toastNotification;
        }

        public async Task<ActionResult> Index()
        {
            var appointments = await _unitOfWork.appointments.GetAppointments();
            return View(appointments);
        }

        public async Task<ActionResult> Details(int id)
        {
            var appointment = await _unitOfWork.appointments.GetAppointmentWithPatient(id);
            return View("_AppointmentPartial", appointment);
        }
        //public ActionResult Patients(int id)
        //{
        //    var viewModel = new DoctorDetailViewModel()
        //    {
        //        Appointments = _unitOfWork.appointments.GetAppointmentByDoctor(id),
        //    };
        //    //var upcomingAppnts = _unitOfWork.appointments.GetAppointmentByDoctor(id);
        //    return View(viewModel);
        //}

        public async Task<ActionResult> Create(int id)
        {
            var viewModel = new AppointmentFormViewModel
            {
                Patient = id,
                Doctors = await _unitOfWork.Doctors.GetAvailableDoctors(),

                Heading = "New Appointment"
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AppointmentFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Doctors = await _unitOfWork.Doctors.GetAvailableDoctors();
                return View(viewModel);

            }
            var appointment = new Appointment()
            {
                StartTime = viewModel.GetStartDateTime(),
                Detail = viewModel.Detail,
                Status = false,
                PatientSSN = viewModel.Patient,
                Doctor = await _unitOfWork.Doctors.GetDoctor(viewModel.Doctor)

            };
            //Check if the slot is available
            if (await _unitOfWork.appointments.ValidateAppointment(appointment.StartTime.Value, viewModel.Doctor))
                return View("InvalidAppointment");

            await _unitOfWork.appointments.AddAsync(appointment);
            await _unitOfWork.Complete();
            _toastNotification.AddErrorToastMessage("Appointment Created Successfully");
            return RedirectToAction("Index", "Appointment");
        }

        public async Task<ActionResult> Edit(int id)
        {
            var appointment = await _unitOfWork.appointments.GetAppointment(id);
            var viewModel = new AppointmentFormViewModel()
            {
                Heading = "New Appointment",
                Id = appointment.Id,
                Date = appointment.StartTime?.ToString("dd/MM/yyyy"),
                Time = appointment.StartTime?.ToString("HH:mm"),
                Detail = appointment.Detail,
                Status = appointment.Status,
                Patient = appointment.Patient.SSN,
                Doctor = appointment.Doctor.Id,
                //Patients = _unitOfWork.Patients.GetPatients(),
                Doctors = await _unitOfWork.Doctors.GetDoctors()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AppointmentFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Doctors = await _unitOfWork.Doctors.GetDoctors();
                viewModel.Patients = await _unitOfWork.Patients.GetPatients();
                return View(viewModel);
            }

            var appointmentInDb = await _unitOfWork.appointments.GetAppointment(viewModel.Id);
            appointmentInDb.Id = viewModel.Id;
            appointmentInDb.StartTime = viewModel.GetStartDateTime();
            appointmentInDb.Detail = viewModel.Detail;
            appointmentInDb.Status = viewModel.Status;
            appointmentInDb.PatientSSN = viewModel.Patient;
            appointmentInDb.DoctorId = viewModel.Doctor;

            await _unitOfWork.Complete();
            _toastNotification.AddErrorToastMessage("Appointment Updated Successfully");
            return RedirectToAction("Index");

        }

        public async Task<ActionResult> DoctorsList()
        {
            var doctors = await _unitOfWork.Doctors.GetAvailableDoctors();
            //if (HttpContext.Request.IsAjaxRequest())
            //    return Json(new SelectList(doctors.ToArray(), "Id", "Name"), JsonRequestBehavior.AllowGet);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new SelectList(doctors.ToArray(), "Id", "Name"));
            }
            return RedirectToAction("Create");
        }

        public async Task<ActionResult> GetUpcommingAppointments(int id)
        {
            var appointments = await _unitOfWork.appointments.GetTodaysAppointments(id);
            return View(appointments);
        }

        public async Task<IActionResult> DoctorDetailsWithAppointment(int id)
        {
            try
            {
                var doctor = await _unitOfWork.Doctors.GetDoctor(id);
                if (doctor == null)
                    return View("NotFound");

                var DoctorDetail = new DoctorVM
                {
                    FirstName = doctor.User.FirstName,
                    LastName = doctor.User.LastName,
                    BirthDate = doctor.User.BirthDate,
                    Email = doctor.User.Email,
                    Gender = doctor.User.Gender,
                    PhoneNumber = doctor.User.PhoneNumber,
                    Name = doctor.Name,
                    Location = doctor.Location,
                    Price = doctor.Price,
                    //ProfileImg = doctor.User.ProfileImg,
                };
                return View(DoctorDetail);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

    }
}
