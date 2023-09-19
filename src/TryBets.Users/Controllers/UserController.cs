using System;
using Microsoft.AspNetCore.Mvc;
using TryBets.Users.Repository;
using TryBets.Users.Services;
using TryBets.Users.Models;
using TryBets.Users.DTO;

namespace TryBets.Users.Controllers;

[Route("[controller]")]
public class UserController : Controller
{
    private readonly IUserRepository _repository;
    public UserController(IUserRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("signup")]
    public IActionResult Post([FromBody] User user)
    {
        try
        {
            var newUser = _repository.Post(user);
            var token = new TokenManager().Generate(newUser);
            AuthDTOResponse response = new AuthDTOResponse { Token = token };
            return Created("", response);
        }
        catch (Exception error)
        {

            return BadRequest(new { message = error.Message });
        }
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] AuthDTORequest login)
    {
        try
        {
            var userLogged = _repository.Login(login);
            var token = new TokenManager().Generate(userLogged);
            AuthDTOResponse response = new AuthDTOResponse { Token = token };
            return Ok(response);
        }
        catch (Exception error)
        {

            return BadRequest(new { message = error.Message });
        }
    }
}