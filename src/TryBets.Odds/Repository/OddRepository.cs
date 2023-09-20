using TryBets.Odds.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Globalization;

namespace TryBets.Odds.Repository;

public class OddRepository : IOddRepository
{
    protected readonly ITryBetsContext _context;
    public OddRepository(ITryBetsContext context)
    {
        _context = context;
    }

    public Match Patch(int MatchId, int TeamId, string BetValue)
    {
        var oddMatch = _context.Matches.Find(MatchId);

        if (oddMatch == null) return null;
        if (!decimal.TryParse(BetValue.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture,
               out var betValue)) return null;
        if (oddMatch.MatchTeamAId == TeamId)
            oddMatch.MatchTeamAValue += betValue;
        else if (oddMatch.MatchTeamBId == TeamId)
            oddMatch.MatchTeamBValue += betValue;
        else
            return null;
        _context.SaveChanges();
        return new Match
        {
            MatchId = oddMatch.MatchId,
            MatchDate = oddMatch.MatchDate,
            MatchTeamAId = oddMatch.MatchTeamAId,
            MatchTeamAValue = oddMatch.MatchTeamAValue,
            MatchTeamBId = oddMatch.MatchTeamBId,
            MatchTeamBValue = oddMatch.MatchTeamBValue,
            MatchFinished = oddMatch.MatchFinished,
            MatchWinnerId = oddMatch.MatchWinnerId,
            MatchTeamA = null,
            MatchTeamB = null,
            Bets = null
        };
    }
}