using Licentra.API.Data;
using Licentra.API.Interfaces.Licenses;
using Licentra.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Licentra.API.Repositories.Licenses
{
    public class LicenseRepository : ILicenseRepository
    {
        private readonly LicentraDbContext _context;

        public LicenseRepository(LicentraDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<License>> GetAllAsync()
        {
            return await _context.Licenses
                .Include(l => l.Software)
                .OrderBy(l => l.LicenseId)
                .ToListAsync();
        }

        public async Task<License?> GetByIdAsync(int licenseId)
        {
            return await _context.Licenses
                .Include(l => l.Software)
                .FirstOrDefaultAsync(l => l.LicenseId == licenseId);
        }

        public async Task AddAsync(License license)
        {
            await _context.Licenses.AddAsync(license);
        }

        public Task UpdateAsync(License license)
        {
            _context.Licenses.Update(license);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(License license)
        {
            _context.Licenses.Remove(license);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(int licenseId)
        {
            return await _context.Licenses
                .AnyAsync(l => l.LicenseId == licenseId);
        }

        public async Task<bool> SoftwareExistsAsync(int softwareId)
        {
            return await _context.Softwares
                .AnyAsync(s => s.SoftwareId == softwareId);
        }

        public async Task<bool> LicenseKeyExistsAsync(string licenseKey)
        {
            return await _context.Licenses
                .AnyAsync(l => l.LicenseKey == licenseKey);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}