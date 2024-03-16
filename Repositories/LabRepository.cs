﻿using Database.Entities;
using HeartDiseasePrediction.ViewModel;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories
{
    public class LabRepository : ILabRepository
    {
        private readonly AppDbContext _context;
        public LabRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Lab lab) => await _context.Labs.AddAsync(lab);

        public void Delete(Lab lab) => _context.Labs.Remove(lab);

        public async Task<Lab> GetLab(int id) => await _context.Labs.FirstOrDefaultAsync(l => l.Id == id);

        public async Task<IEnumerable<Lab>> GetLabs() => await _context.Labs.ToListAsync();

        public async Task<NewLabDropDownViewMode> GetLabDropDownsValues()
        {
            var data = new NewLabDropDownViewMode()
            {
                labs = await _context.Labs.OrderBy(a => a.Name).ToListAsync(),
            };
            return data;
        }

        public Lab Get_Lab(int id) => _context.Labs.FirstOrDefault(l => l.Id == id);
    }
}