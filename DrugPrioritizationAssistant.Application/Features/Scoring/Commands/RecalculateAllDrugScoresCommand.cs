using MediatR;

namespace DrugPrioritizationAssistant.Application.Features.Scoring.Commands;

public record RecalculateAllDrugScoresCommand : IRequest;
