using Licentra.API.Data;
using Licentra.API.Interfaces.Vendors;
using Licentra.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Licentra.API.Repositories.Vendors
{
    public class VendorRepository : IVendorRepository
    {
        private readonly LicentraDbContext _context;

        public VendorRepository(LicentraDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vendor>> GetAllAsync()
        {
            return await _context.Vendors
                .OrderBy(v => v.VendorName)
                .ToListAsync();
        }

        public async Task<Vendor?> GetByIdAsync(int vendorId)
        {
            return await _context.Vendors
                .FirstOrDefaultAsync(v => v.VendorId == vendorId);
        }

        public async Task<Vendor?> GetByNameAsync(string vendorName)
        {
            return await _context.Vendors
                .FirstOrDefaultAsync(v => v.VendorName == vendorName);
        }

        public async Task AddAsync(Vendor vendor)
        {
            await _context.Vendors.AddAsync(vendor);
        }

        public Task UpdateAsync(Vendor vendor)
        {
            _context.Vendors.Update(vendor);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Vendor vendor)
        {
            _context.Vendors.Remove(vendor);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(int vendorId)
        {
            return await _context.Vendors
                .AnyAsync(v => v.VendorId == vendorId);
        }

        public async Task<bool> VendorNameExistsAsync(string vendorName)
        {
            return await _context.Vendors
                .AnyAsync(v => v.VendorName == vendorName);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}