using Database.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly AppDbContext _appDbContext;

        public DoctorRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }




        public async Task<IEnumerable<Doctor>> GetDoctors()
        {
            return await _appDbContext.Doctors.Include(c => c.User).ToListAsync();
        }


    }
}