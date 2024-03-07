using Database.Entities;
using HearPrediction.Api.Data.Services.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HearPrediction.Api.Data.Services.Repository
{
	public class LabRepository : ILabRepository
	{
		public readonly AppDbContext _context;

		public LabRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Lab>> GetLabs()
		{
			return await _context.Labs.ToListAsync();
		}
	}
}
