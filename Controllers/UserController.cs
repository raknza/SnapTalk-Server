using android_backend.Models;
using Microsoft.AspNetCore.Mvc;
using android_backend.Repositories;
using android_backend.Helper;

namespace android_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{

    private readonly UserRepository _repository;

    public UserController(UserRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// create new user
    /// </summary>
    /// <param name="parameter">User</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Insert([FromBody] User parameter)
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
    [HttpGet]
    public List<User> GetList()
    {
        return _repository.FindAll();
    }

}