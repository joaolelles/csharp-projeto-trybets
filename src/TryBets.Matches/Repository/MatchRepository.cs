using TryBets.Matches.DTO;

namespace TryBets.Matches.Repository;

public class MatchRepository : IMatchRepository
{
    protected readonly ITryBetsContext _context;
    public MatchRepository(ITryBetsContext context)
    {
        _context = context;
    }

    public IEnumerable<MatchDTOResponse> Get(bool matchFinished)
    {
        return _context.Matches.Where(match => match.MatchFinished == matchFinished)
                .OrderBy(match => match.MatchId)
                .Select(match => new MatchDTOResponse
                {
                    MatchId = match.MatchId,
                    MatchDate = match.MatchDate,
                    MatchTeamAId = match.MatchTeamAId,
                    MatchTeamBId = match.MatchTeamBId,
                    TeamAName = match.MatchTeamA!.TeamName,
                    TeamBName = match.MatchTeamB!.TeamName,
                    MatchTeamAOdds = Math.Round((match.MatchTeamAValue + match.MatchTeamBValue) / match.MatchTeamAValue, 2).ToString("###.##"),
                    MatchTeamBOdds = Math.Round((match.MatchTeamAValue + match.MatchTeamBValue) / match.MatchTeamBValue, 2).ToString("###.##"),
                    MatchFinished = match.MatchFinished,
                    MatchWinnerId = match.MatchWinnerId
                }).ToList();
    }
}