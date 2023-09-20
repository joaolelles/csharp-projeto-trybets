using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using TryBets.Bets.Repository;
using TryBets.Bets.DTO;
using TryBets.Bets.Services;


namespace TryBets.Bets.Controllers;

[Route("[controller]")]
public class BetController : Controller
{
    private readonly IBetRepository _repository;
    private readonly IOddService _oddService;
    public BetController(IBetRepository repository, IOddService oddService)
    {
        _repository = repository;
        _oddService = oddService;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "Client")]
    public async Task<IActionResult> Post([FromBody] BetDTORequest request)
    {
        try
        {
            var token = HttpContext.User.Identity as ClaimsIdentity;
            var email = token?.Claims.FirstOrDefault(e => e.Type == ClaimTypes.Email)?.Value;
            await _oddService.UpdateOdd(request.MatchId, request.TeamId, request.BetValue);

            return Created("", _repository.Post(request, email!));
        }
        catch (Exception err)
        {

            return BadRequest(new { message = err.Message });
        }
    }

    [HttpGet("{BetId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "Client")]
    public IActionResult Get(int BetId)
    {
        try
        {
            var token = HttpContext.User.Identity as ClaimsIdentity;
            var email = token?.Claims.FirstOrDefault(e => e.Type == ClaimTypes.Email)?.Value;
            return Ok(_repository.Get(BetId, email!));
        }
        catch (Exception err)
        {

            return BadRequest(new { message = err.Message });
        }
    }
}