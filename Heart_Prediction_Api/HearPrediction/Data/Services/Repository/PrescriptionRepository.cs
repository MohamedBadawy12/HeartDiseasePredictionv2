using Database.Entities;
using HearPrediction.Api.Data.Services.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HearPrediction.Api.Data.Services.Repository
{
	public class PrescriptionRepository : IPrescriptionRepository
	{
		private readonly AppDbContext _context;
		public PrescriptionRepository(AppDbContext context)
		{
			_context = context;
		}
		public async Task Add(Prescription prescription) =>
			await _context.Prescriptions.AddAsync(prescription);

		public async Task<IEnumerable<Prescription>> FilterPrescriptions(long search)
		{
			var prescriptions = await GetPrescriptions();
			if (search != 0)
			{
				prescriptions = await _context.Prescriptions.
				Where(x => x.PatientSSN.Equals(search)).ToListAsync();
			}
			return prescriptions;
		}



		//public async IEnumerable<Prescription> FilterPrescriptionsAsync(PrescriptionSearchDTO searchDto)
		//{
		//	//var patient = await _context.Patients
		//	//	.Include(p => p.prescriptions)
		//	//	.AsQueryable()
		//	//	.FirstOrDefaultAsync(p => p.SSN == patientSSN);

		//	//if (patient == null)
		//	//	return Enumerable.Empty<Prescription>();

		//	//if (!string.IsNullOrWhiteSpace(searchDto.patientSSN.ToString())
		//	//{
		//	//	var filterPrescriptions = patient.prescriptions
		//	//	.Where(p =>
		//	//		(searchDto.Date == null || p.date >= searchDto.Date) &&
		//	//		(string.IsNullOrEmpty(searchDto.MedicienName) || p.MedicineName.Contains(searchDto.MedicienName))
		//	//	);
		//	//	return filterPrescriptions.ToList();
		//	//}
		//	//return Enumerable.Empty<Prescription>();
		//	var prescription = from p in _context.Prescriptions
		//					   select p;

		//	if (!string.IsNullOrEmpty(searchDto.Date.ToString()))
		//	{
		//		prescription = prescription.Where(p=>p.date==searchDto.Date);
		//	}
		//	if (!string.IsNullOrEmpty(searchDto.patientSSN.ToString()))
		//	{
		//		prescription = prescription.Where(p=>p.PatientSSN== searchDto.patientSSN);
		//	}
		//	return prescription.ToListAsync();
		//}

		public async Task<Prescription> GetPrescription(int id) =>
			 await _context.Prescriptions.Include(d => d.Doctor).FirstOrDefaultAsync(p => p.Id == id);

		public async Task<IEnumerable<Prescription>> GetPrescriptions() =>
			 await _context.Prescriptions.Include(d => d.Doctor).ToListAsync();

		public async Task<List<Prescription>> GetPrescriptionsByUserSSN(long ssn)
		{
			var prescriptions = await _context.Prescriptions.Include(d => d.Doctor).Where(n => n.PatientSSN == ssn).ToListAsync();
			//if (userRole == "User")
			//{
			//	prescriptions = prescriptions.Where(n => n.PatientSSN == ssn).ToList();
			//}
			return prescriptions;
		}

		public Prescription Get_Prescription(int id) =>
			 _context.Prescriptions.Include(d => d.Doctor).FirstOrDefault(p => p.Id == id);

		public void Remove(Prescription prescription) =>
			_context.Prescriptions.Remove(prescription);

	}
}
