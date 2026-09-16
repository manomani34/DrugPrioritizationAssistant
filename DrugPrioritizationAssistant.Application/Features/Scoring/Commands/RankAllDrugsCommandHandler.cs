using DrugPrioritizationAssistant.Application.Services;
using MediatR;

namespace DrugPrioritizationAssistant.Application.Features.Scoring.Commands;

public class RankAllDrugsCommandHandler
    : IRequestHandler<RankAllDrugsCommand>
{
    private readonly IRankingService _rankingService;

    public RankAllDrugsCommandHandler(
        IRankingService rankingService)
    {
        _rankingService = rankingService;
    }

    public async Task Handle(
        RankAllDrugsCommand request,
        CancellationToken cancellationToken)
    {
        await _rankingService.RankAllAsync(
            cancellationToken);
    }
}