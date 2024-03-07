using Database.Entities;
using HearPrediction.Api.Data.Services.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HearPrediction.Api.Data.Services.Repository
{
	public class PatientRepository : IPatientRepository
	{
		private readonly AppDbContext _context;
		public PatientRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task Add(Patient patient) => await _context.Patients.AddAsync(patient);

		public async Task<IEnumerable<Patient>> FilterPatients(string search)
		{
			var patients = await GetPatients();
			if (!string.IsNullOrEmpty(search))
			{
				patients = await _context.Patients.
				Where(x => x.User.FirstName.Contains(search) || x.User.LastName.Contains(search)).ToListAsync();
			}
			return patients;
		}


		public async Task<Patient> GetPatient(long ssn)
		{
			return await _context.Patients
			   .Include(u => u.User)
			   .FirstOrDefaultAsync(d => d.SSN == ssn);
		}

		public async Task<IEnumerable<Patient>> GetPatients()
		{
			return await _context.Patients
				.Include(p => p.User)
				.ToListAsync();
		}
		public async Task<Patient> GetProfile(string userId)
		{
			return await _context.Patients
			.Include(u => u.User)
			.Include(u => u.MedicalTests)
			.FirstOrDefaultAsync(d => d.UserId == userId);
		}

		public Task<IEnumerable<Patient>> GetRecentPatients()
		{
			throw new System.NotImplementedException();
		}

		public Patient Get_Patient(long ssn)
		{
			return _context.Patients
			   .Include(u => u.User)
			   .Include(u => u.MedicalTests)
			   .FirstOrDefault(d => d.SSN == ssn);
		}

		public void Remove(Patient patient) => _context.Patients.Remove(patient);
	}
}
