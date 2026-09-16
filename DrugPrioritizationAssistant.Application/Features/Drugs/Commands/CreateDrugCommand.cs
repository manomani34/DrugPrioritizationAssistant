using MediatR;

namespace DrugPrioritizationAssistant.Application.Features.Drugs.Commands;

public record CreateDrugCommand(
    string Name,
    string? GenericName,
    string? ActiveIngredient,
    string? DosageForm,
    string? TherapeuticCategory
) : IRequest<int>;