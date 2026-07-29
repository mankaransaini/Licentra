using Licentra.API.Data;
using Licentra.API.Interfaces.Software;
using Microsoft.EntityFrameworkCore;

namespace Licentra.API.Repositories.Software
{
    public class SoftwareRepository : ISoftwareRepository
    {
        private readonly LicentraDbContext _context;

        public SoftwareRepository(LicentraDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Models.Software>> GetAllAsync()
        {
            return await _context.Softwares
                .Include(s => s.Vendor)
                .OrderBy(s => s.SoftwareName)
                .ThenBy(s => s.Version)
                .ToListAsync();
        }

        public async Task<Models.Software?> GetByIdAsync(int softwareId)
        {
            return await _context.Softwares
                .Include(s => s.Vendor)
                .FirstOrDefaultAsync(s => s.SoftwareId == softwareId);
        }

        public async Task AddAsync(Models.Software software)
        {
            await _context.Softwares.AddAsync(software);
        }

        public Task UpdateAsync(Models.Software software)
        {
            _context.Softwares.Update(software);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Models.Software software)
        {
            _context.Softwares.Remove(software);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(int softwareId)
        {
            return await _context.Softwares
                .AnyAsync(s => s.SoftwareId == softwareId);
        }

        public async Task<bool> VendorExistsAsync(int vendorId)
        {
            return await _context.Vendors
                .AnyAsync(v => v.VendorId == vendorId);
        }

        public async Task<bool> SoftwareExistsAsync(string softwareName, string version)
        {
            return await _context.Softwares
                .AnyAsync(s =>
                    s.SoftwareName == softwareName &&
                    s.Version == version);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}