using TryBets.Bets.DTO;
using TryBets.Bets.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TryBets.Bets.Repository;

public class BetRepository : IBetRepository
{
    protected readonly ITryBetsContext _context;
    public BetRepository(ITryBetsContext context)
    {
        _context = context;
    }

    public BetDTOResponse Post(BetDTORequest betRequest, string email)
    {
        User betUser = _context.Users.FirstOrDefault(u => u.Email == email)!;
        if (betUser == null) throw new Exception("User not founded");

        Match betMatch = _context.Matches.FirstOrDefault(m => m.MatchId == betRequest.MatchId)!;
        if (betMatch == null) throw new Exception("Match not founded");

        Team betTeam = _context.Teams.FirstOrDefault(t => t.TeamId == betRequest.TeamId)!;
        if (betTeam == null) throw new Exception("Team not founded");

        if (betMatch.MatchFinished) throw new Exception("Match finished");

        if (betMatch.MatchTeamAId != betRequest.TeamId && betMatch.MatchTeamBId != betRequest.TeamId)
        {
            throw new Exception("Team is not in this match");
        }

        Bet newBet = new Bet
        {
            UserId = betUser.UserId,
            MatchId = betRequest.MatchId,
            TeamId = betRequest.TeamId,
            BetValue = betRequest.BetValue
        };
        _context.Bets.Add(newBet);
        _context.SaveChanges();

        Bet bet = _context.Bets
                        .Include(b => b.Team)
                        .Include(b => b.Match)
                        .Where(b => b.BetId == newBet.BetId)
                        .FirstOrDefault()!;

        return new BetDTOResponse
        {
            BetId = bet.BetId,
            MatchId = bet.MatchId,
            TeamId = bet.TeamId,
            BetValue = bet.BetValue,
            MatchDate = bet.Match!.MatchDate,
            TeamName = bet.Team!.TeamName,
            Email = bet.User!.Email
        };
    }
    public BetDTOResponse Get(int BetId, string email)
    {
        User betUser = _context.Users.FirstOrDefault(u => u.Email == email)!;
        if (betUser == null) throw new Exception("User not founded");

        Bet bet = _context.Bets
                        .Include(b => b.Team)
                        .Include(b => b.Match)
                        .Where(b => b.BetId == BetId)
                        .FirstOrDefault()!;
        if (bet == null) throw new Exception("Bet not founded");

        if (bet.User!.Email != email) throw new Exception("Bet view not allowed");

        return new BetDTOResponse
        {
            BetId = bet.BetId,
            MatchId = bet.MatchId,
            TeamId = bet.TeamId,
            BetValue = bet.BetValue,
            MatchDate = bet.Match!.MatchDate,
            TeamName = bet.Team!.TeamName,
            Email = bet.User!.Email
        };
    }
}