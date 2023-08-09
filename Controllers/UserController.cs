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

    private readonly UserRepository userRepository;
    private readonly JwtHelper jwtHelper;

    public UserController(UserRepository userRepository, JwtHelper jwtHelper)
    {
        this.userRepository = userRepository;
        this.jwtHelper = jwtHelper;
    }

    /// <summary>
    /// create new user
    /// </summary>
    /// <param name="parameter">User</param>
    /// <returns></returns>
    [HttpPost("registration"), AllowAnonymous]
    public IActionResult Create([FromForm] User parameter)
    {
        if (userRepository.FindByUsername(parameter.username) != null)
            return Conflict();
        User user = new User
        {
            id = 0,
            username = parameter.username,
            name = parameter.username,
            password = MD5Helper.hash(parameter.password),
            email = parameter.email,
            avatar = parameter.avatar

        };
        userRepository.Create(user);
        return Ok(new { message = "Success", token = jwtHelper.GenerateToken(parameter.username) });

    }

    /// <summary>
    /// get a user
    /// </summary>
    /// <returns></returns>
    [HttpGet("{username}"), Authorize(Roles = "user")]
    public IActionResult GetUser(string username)
    {
        User user = userRepository.FindByUsername(username);
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
        return userRepository.FindAll();
    }

    /// <summary>
    /// update user information
    /// </summary>
    /// <returns></returns>
    [HttpPut("{username}"), Authorize(Roles = "user")]
    public IActionResult UpdateUser(string username, [FromForm] string? password, [FromForm] string? name, [FromForm] string? email, [FromForm] string? avatar, [FromForm] string? profile)
    {
        User user = userRepository.FindByUsername(username);
        user.password = password == null ? user.password : MD5Helper.hash(password);
        user.name = name ?? user.name;
        user.email = email ?? user.email;
        user.avatar = avatar ?? user.avatar;
        user.profile = profile ?? user.profile;
        userRepository.Update(user);
        return Ok();
    }

    /// <summary>
    /// Login
    /// </summary>
    /// <returns></returns>
    [HttpPost("login"), AllowAnonymous]
    public IActionResult Login([FromForm] string username, [FromForm] string password)
    {
        User user = userRepository.FindByUsername(username);
        if (user != null && user.password.Equals(MD5Helper.hash(password)))
            return Ok(new { token = jwtHelper.GenerateToken(username) });
        else
            return Unauthorized();

    }

}