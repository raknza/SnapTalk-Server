using android_backend.Models;
using Microsoft.AspNetCore.Mvc;
using android_backend.Service;
using android_backend.Helper;
using Microsoft.AspNetCore.Authorization;
using android_backend.Filter;

namespace android_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{

    private readonly UserService userService;

    /// <summary>
    /// Initializes a new instance of the UserController class.
    /// </summary>
    /// <param name="userService">The user service.</param>
    public UserController(UserService userService)
    {
        this.userService = userService;
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="parameter">User details.</param>
    /// <returns>Returns HTTP OK if the user was created successfully, or Conflict if the username already exists.</returns>
    [HttpPost("registration"), AllowAnonymous]
    public IActionResult Create([FromForm] User parameter)
    {
        if (userService.Create(parameter))
            return Ok(new { message = "Success" });
        else
            return Conflict();

    }

    /// <summary>
    /// Retrieves a user's information.
    /// </summary>
    /// <returns>Returns the user information if authorized, or NotFound if the user doesn't exist.</returns>
    [HttpGet, Authorize(Roles = "user")]
    public IActionResult GetUser()
    {
        string username = User.Identity.Name;
        User user = userService.GetUser(username);
        if (user != null)
            return Ok(user);
        return NotFound();
    }

    /// <summary>
    /// Retrieves a list of all users.
    /// </summary>
    /// <returns>Returns a list of users if authorized as an admin.</returns>
    [HttpGet("getAllUser"), Authorize(Roles = "admin")]
    public List<User> GetList()
    {
        return userService.GetList();
    }

    /// <summary>
    /// Updates user information.
    /// </summary>
    /// <returns>Returns HTTP OK if the user information was updated successfully.</returns>
    [HttpPut, Authorize(Roles = "user")]
    public IActionResult UpdateUser([FromForm] string? password, [FromForm] string? name, [FromForm] string? email, [FromForm] string? avatar, [FromForm] string? profile)
    {
        string username = User.Identity.Name;
        userService.UpdateUser(username, password, name, email, avatar, profile);
        return Ok();
    }

    /// <summary>
    /// User Login API
    /// </summary>
    /// <param name="username">The username for login.</param>
    /// <param name="password">The password for login.</param>
    /// <returns>Returns an authentication token upon successful login, or a 401 Unauthorized status on failure.</returns>
    [HttpPost("login"), AllowAnonymous]
    public IActionResult Login([FromForm] string username, [FromForm] string password)
    {
        LoginResult result = userService.Login(username, password);
        if (result.success){
            return Ok(new { token = result.token });
        }
        return Unauthorized(); ;

    }

    /// <summary>
    /// Check if the provided JWT token is valid for the authenticated user.
    /// </summary>
    /// <remarks>
    /// Validates the JWT token associated with the authenticated user against the Redis cache.
    /// </remarks>
    /// <returns>Returns an OK response with a boolean indicating token validity.</returns>
    [HttpGet("auth"), AllowAnonymous]
    [ServiceFilter(typeof(JwtAuthenticationFilter))]
    public IActionResult CheckAuth()
    {
        return Ok( new { isTokenValid = (Boolean)HttpContext.Items["isTokenValid"] });
    }

}