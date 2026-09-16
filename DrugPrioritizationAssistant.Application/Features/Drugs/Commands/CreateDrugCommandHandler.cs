using DrugPrioritizationAssistant.Application.Interfaces.Repositories;
using DrugPrioritizationAssistant.Domain.Entities;
using MediatR;

namespace DrugPrioritizationAssistant.Application.Features.Drugs.Commands;

public class CreateDrugCommandHandler
    : IRequestHandler<CreateDrugCommand, int>
{
    private readonly IDrugRepository _drugRepository;

    public CreateDrugCommandHandler(
        IDrugRepository drugRepository)
    {
        _drugRepository = drugRepository;
    }

    public async Task<int> Handle(
        CreateDrugCommand request,
        CancellationToken cancellationToken)
    {
        var drug = new Drug
        {
            Name = request.Name,
            GenericName = request.GenericName,
            ActiveIngredient = request.ActiveIngredient,
            DosageForm = request.DosageForm,
            TherapeuticCategory = request.TherapeuticCategory,
            Status = Domain.Enums.DrugStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _drugRepository.AddAsync(
            drug,
            cancellationToken);

        return result.Id;
    }
}