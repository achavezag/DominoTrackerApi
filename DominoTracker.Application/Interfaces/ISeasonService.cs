using DominoTracker.Application.DTOs;

namespace DominoTracker.Application.Interfaces;

public interface ISeasonService
{
    Task<List<SeasonDto>> GetAllAsync();
    Task<SeasonDto> GetByIdAsync(int id);
    Task<SeasonDto> GetByDateAsync(DateTime date);
    Task<SeasonDto> CreateAsync(CreateSeasonRequest request);
    Task<SeasonDto> UpdateAsync(int id, UpdateSeasonRequest request);
    Task DeleteAsync(int id);
}
