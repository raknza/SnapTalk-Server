using android_backend.Models;
using Microsoft.AspNetCore.Mvc;
using android_backend.Repositories;
using android_backend.Helper;
using Microsoft.AspNetCore.Authorization;

namespace android_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{

    private readonly UserRepository _repository;
    private readonly JwtHelper _jwtHelper;

    public UserController(UserRepository repository, JwtHelper jwtHelper)
    {
        _repository = repository;
        _jwtHelper = jwtHelper;
    }

    /// <summary>
    /// create new user
    /// </summary>
    /// <param name="parameter">User</param>
    /// <returns></returns>
    [HttpPost("registration"), AllowAnonymous]
    public async Task<IActionResult> CreateAsync([FromForm] User parameter)
    {
        if (_repository.FindByUsername(parameter.username) != null)
            return Conflict();
        User user = new User
        {
            id = 0,
            username = parameter.username,
            password = MD5Helper.hash(parameter.password),
            email = parameter.email,
            avatar = parameter.avatar
        };
        _repository.Create(user);
        try
        {
            return Ok(new { message = "Success", token = _jwtHelper.GenerateToken(parameter.username) });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error adding user: {ex.Message}");
        }

    }

    /// <summary>
    /// get a user
    /// </summary>
    /// <returns></returns>
    [HttpGet("{username}"), Authorize(Roles = "user")]
    public IActionResult GetUser(string username)
    {
        User user = _repository.FindByUsername(username);
        user.password = "";
        return Ok(user);
    }

    /// <summary>
    /// get all of users
    /// </summary>
    /// <returns></returns>
    [HttpGet, Authorize(Roles = "admin")]
    public List<User> GetList()
    {
        return _repository.FindAll();
    }

    /// <summary>
    /// update user information
    /// </summary>
    /// <returns></returns>
    [HttpPut("{username}"), Authorize(Roles = "user")]
    public IActionResult UpdateUser(string username, [FromForm] string? password, [FromForm] string? email, [FromForm] string? avatar, [FromForm] string? profile)
    {
        User user = _repository.FindByUsername(username);
        user.password = password == null ? user.password : MD5Helper.hash(password);
        user.email = email ?? user.email;
        user.avatar = avatar ?? user.avatar;
        user.profile = profile ?? user.profile;
        _repository.Update(user);
        return Ok();
    }

    /// <summary>
    /// Login
    /// </summary>
    /// <returns></returns>
    [HttpPost("login"), AllowAnonymous]
    public IActionResult Login([FromForm] string username, [FromForm] string password)
    {
        User user = _repository.FindByUsername(username);
        if (user != null && user.password.Equals(MD5Helper.hash(password)))
            return Ok(new { token = _jwtHelper.GenerateToken(username) });
        else
            return Unauthorized();

    }

}