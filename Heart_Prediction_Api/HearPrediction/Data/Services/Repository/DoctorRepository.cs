using Database.Entities;
using HearPrediction.Api.Data.Services.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HearPrediction.Api.Data.Services.Repository
{
	public class DoctorRepository : IDoctorRepository
	{
		private readonly AppDbContext _context;
		public DoctorRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task Add(Doctor doctor) => await _context.Doctors.AddAsync(doctor);

		public async Task<IEnumerable<Doctor>> FilterDoctors(string search)
		{
			var doctors = await GetDectors();
			if (!string.IsNullOrEmpty(search))
			{
				doctors = await _context.Doctors.
				Where(x => x.User.FirstName.Contains(search) || x.User.LastName.Contains(search)).ToListAsync();
			}
			return doctors;
		}



		public Task<IEnumerable<Doctor>> GetAvailableDoctors()
		{
			throw new System.NotImplementedException();
		}

		public async Task<IEnumerable<Doctor>> GetDectors()
		{
			return await _context.Doctors
				.Include(d => d.User)
				.ToListAsync();
		}

		public async Task<Doctor> GetDoctor(int id)
		{
			return await _context.Doctors
			   .Include(d => d.User)
			   .FirstOrDefaultAsync(d => d.Id == id);
		}

		public async Task<Doctor> GetProfile(string userId)
		{
			return await _context.Doctors
			   .Include(u => u.User)
			   //.Include(d => d.medicalTests)
			   .FirstOrDefaultAsync(d => d.UserId == userId);
		}

		public Doctor Get_Doctor(int id)
		{
			return _context.Doctors
			   .Include(d => d.User)
			   //.Include(d => d.medicalTests)
			   .FirstOrDefault(d => d.Id == id);
		}

		public void Delete(Doctor doctor) => _context.Doctors.Remove(doctor);

		public Doctor FindDoctor(int id) =>
			 _context.Doctors.Find(id);

	}
}
