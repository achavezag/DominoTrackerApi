namespace DominoTracker.WebAPI.Controllers;

internal static class SeasonQuery
{
    public static List<int>? Parse(string? seasonIds)
    {
        if (string.IsNullOrWhiteSpace(seasonIds))
            return null;

        return seasonIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(int.Parse)
            .ToList();
    }
}
