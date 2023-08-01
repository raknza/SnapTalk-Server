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
    [HttpPost, AllowAnonymous]
    public IActionResult Create([FromBody] User parameter)
    {
        if (_repository.FindByUsername(parameter.username) != null)
            return Ok("Username has been used.");
        _repository.Create(new User
        {
            id = 0,
            username = parameter.username,
            password = MD5Helper.hash(parameter.password),
            email = parameter.email,
            avatar = parameter.avatar
        });
        return Ok();
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
    /// Login
    /// </summary>
    /// <returns></returns>
    [HttpPost("Login"), AllowAnonymous]
    public ActionResult<string> Login([FromForm] string username, [FromForm] string password)
    {
        User user = _repository.FindByUsername(username);
        if (user != null && user.password.Equals(MD5Helper.hash(password)))
            return Ok(_jwtHelper.GenerateToken(username));
        else
            return BadRequest();

    }

}