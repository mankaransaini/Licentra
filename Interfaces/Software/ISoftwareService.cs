using Licentra.API.DTOs.Software;

namespace Licentra.API.Interfaces.Software
{
    public interface ISoftwareService
    {
        Task<IEnumerable<SoftwareDto>> GetAllAsync();

        Task<SoftwareDto?> GetByIdAsync(int softwareId);

        Task<SoftwareDto> AddAsync(CreateSoftwareDto dto);

        Task<bool> UpdateAsync(int softwareId, UpdateSoftwareDto dto);

        Task<bool> DeleteAsync(int softwareId);
    }
}