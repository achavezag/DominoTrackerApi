using DominoTracker.Application.DTOs;

namespace DominoTracker.Application.Interfaces;

public interface ITeamStatsService
{
    Task<List<TeamStatsResponse>> GetAllStatsAsync(List<int>? seasonIds = null);
}