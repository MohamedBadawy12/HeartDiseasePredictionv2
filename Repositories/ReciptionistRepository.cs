using Database.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories
{
    public class ReciptionistRepository : IReciptionistRepository
    {
        private readonly AppDbContext _context;
        public ReciptionistRepository(AppDbContext context)
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
