using DrugPrioritizationAssistant.Application.Interfaces.Repositories;
using DrugPrioritizationAssistant.Domain.Entities;

namespace DrugPrioritizationAssistant.Application.Services;

public class ScoringSettingsService
    : IScoringSettingsService
{
    private readonly IScoringSettingsRepository _repository;

    public ScoringSettingsService(
        IScoringSettingsRepository repository)
    {
        _repository = repository;
    }

    public async Task<ScoringSettings> GetOrCreateAsync(
        CancellationToken cancellationToken = default)
    {
        var settings =
            await _repository.GetAsync(
                cancellationToken);

        if (settings != null)
        {
            return settings;
        }

        settings = new ScoringSettings
        {
            NeedWeight = 60m,
            FeasibilityWeight = 40m,
            UpdatedAt = DateTime.UtcNow
        };

        return await _repository.AddAsync(
            settings,
            cancellationToken);
    }

    public async Task<ScoringSettings> UpdateAsync(
        decimal needWeight,
        decimal feasibilityWeight,
        CancellationToken cancellationToken = default)
    {
        if (needWeight < 0 ||
            feasibilityWeight < 0)
        {
            throw new InvalidOperationException(
                "وزن‌ها نمی‌توانند منفی باشند.");
        }

        if (needWeight + feasibilityWeight != 100m)
        {
            throw new InvalidOperationException(
                "مجموع وزن‌ها باید برابر 100 باشد.");
        }

        var settings =
            await _repository.GetAsync(
                cancellationToken);

        if (settings == null)
        {
            settings = new ScoringSettings
            {
                NeedWeight = needWeight,
                FeasibilityWeight = feasibilityWeight,
                UpdatedAt = DateTime.UtcNow
            };

            return await _repository.AddAsync(
                settings,
                cancellationToken);
        }

        settings.NeedWeight = needWeight;

        settings.FeasibilityWeight =
            feasibilityWeight;

        settings.UpdatedAt =
            DateTime.UtcNow;

        await _repository.UpdateAsync(
            settings,
            cancellationToken);

        return settings;
    }
}