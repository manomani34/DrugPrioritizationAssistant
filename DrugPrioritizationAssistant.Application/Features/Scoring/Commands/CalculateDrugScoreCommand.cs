using MediatR;

namespace DrugPrioritizationAssistant.Application.Features.Scoring.Commands;

public record CalculateDrugScoreCommand(
    int DrugId
) : IRequest<int>;