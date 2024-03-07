using Database.Entities;
using HearPrediction.Api.Data.Services.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HearPrediction.Api.Data.Services.Repository
{
	public class ReceptionistRepository : IReciptionistRepository
	{
		private readonly AppDbContext _context;
		public ReceptionistRepository(AppDbContext context)
		{
			_context = context;
		}
		public async Task Add(Reciptionist reciptionist) => await _context.Reciptionists.AddAsync(reciptionist);

		public async Task<IEnumerable<Reciptionist>> GetReciptionists()
		{
			return await _context.Reciptionists
				 .Include(m => m.User)
				 .ToListAsync();
		}

		public async Task<Reciptionist> GetReciptionist(int id)
		{
			return await _context.Reciptionists
				 .Include(m => m.User)
				 .FirstOrDefaultAsync(m => m.Id == id);
		}

		public async Task<Reciptionist> GetProfile(string userId)
		{
			return await _context.Reciptionists
				 .Include(m => m.User)
				 .FirstOrDefaultAsync(m => m.UserId == userId);
		}

		public void Remove(Reciptionist reciptionist) => _context.Reciptionists.Remove(reciptionist);

		public Reciptionist Get_Reciptionist(int id)
		{
			return _context.Reciptionists
				 .Include(m => m.User)
				 .FirstOrDefault(m => m.Id == id);
		}

		public async Task<IEnumerable<Reciptionist>> FilterReciptionist(string search)
		{
			var reciptionists = await GetReciptionists();
			if (!string.IsNullOrEmpty(search))
			{
				reciptionists = await _context.Reciptionists.
				Where(x => x.User.FirstName.Contains(search) || x.User.LastName.Contains(search)).ToListAsync();
			}
			return reciptionists;
		}

	}
}
