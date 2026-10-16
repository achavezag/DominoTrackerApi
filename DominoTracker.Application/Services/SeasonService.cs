using DominoTracker.Application.DTOs;
using DominoTracker.Application.Exceptions;
using DominoTracker.Application.Interfaces;
using DominoTracker.Domain.Entities;

namespace DominoTracker.Application.Services;

public class SeasonService : ISeasonService
{
    private readonly ISeasonRepository _repository;

    public SeasonService(ISeasonRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<SeasonDto>> GetAllAsync()
    {
        var seasons = await _repository.GetAllAsync();

        return seasons.Select(ToDto).ToList();
    }

    public async Task<SeasonDto> GetByIdAsync(int id)
    {
        var season = await _repository.GetByIdAsync(id)
            ?? throw new SeasonNotFoundException(id);

        return ToDto(season);
    }

    public async Task<SeasonDto> GetByDateAsync(DateTime date)
    {
        var season = await _repository.GetByDateAsync(date)
            ?? throw new SeasonNotFoundException(date);

        return ToDto(season);
    }

    public async Task<SeasonDto> CreateAsync(CreateSeasonRequest request)
    {
        if (request.EndDate.Date <= request.StartDate.Date)
            throw new Exception("La fecha de fin debe ser posterior a la fecha de inicio");

        var seasons = await _repository.GetAllAsync();

        bool overlaps = seasons.Any(s =>
            request.StartDate.Date <= s.EndDate.Date &&
            request.EndDate.Date >= s.StartDate.Date);

        if (overlaps)
            throw new Exception("El rango de fechas se superpone con una temporada existente");

        var season = new Season
        {
            StartDate = request.StartDate.Date,
            EndDate = request.EndDate.Date
        };

        await _repository.AddAsync(season);
        await _repository.SaveChangesAsync();

        return ToDto(season);
    }

    public async Task<SeasonDto> UpdateAsync(int id, UpdateSeasonRequest request)
    {
        var season = await _repository.GetByIdAsync(id)
            ?? throw new SeasonNotFoundException(id);

        if (request.EndDate.Date <= request.StartDate.Date)
            throw new Exception("La fecha de fin debe ser posterior a la fecha de inicio");

        var seasons = await _repository.GetAllAsync();

        bool overlaps = seasons
            .Where(s => s.Id != id)
            .Any(s =>
                request.StartDate.Date <= s.EndDate.Date &&
                request.EndDate.Date >= s.StartDate.Date);

        if (overlaps)
            throw new Exception("El rango de fechas se superpone con una temporada existente");

        if (request.StartDate.Date != season.StartDate.Date && await _repository.HasMatchesAsync(id))
            throw new Exception("No se puede cambiar la fecha de inicio de una temporada que ya tiene partidos registrados");

        if (request.EndDate.Date < season.EndDate.Date && await _repository.HasMatchesAfterDateAsync(id, request.EndDate.Date))
            throw new Exception("No se puede reducir la fecha de fin: existen partidos registrados después de la nueva fecha");

        season.StartDate = request.StartDate.Date;
        season.EndDate = request.EndDate.Date;

        await _repository.SaveChangesAsync();

        return ToDto(season);
    }

    public async Task DeleteAsync(int id)
    {
        var season = await _repository.GetByIdAsync(id)
            ?? throw new SeasonNotFoundException(id);

        if (await _repository.HasDependenciesAsync(id))
            throw new SeasonInUseException();

        await _repository.RemoveAsync(season);
        await _repository.SaveChangesAsync();
        await _repository.ReseedIdentityAsync();
    }

    private static SeasonDto ToDto(Season season) => new()
    {
        Id = season.Id,
        StartDate = season.StartDate,
        EndDate = season.EndDate
    };
}
