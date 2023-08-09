namespace android_backend.Controllers;

using android_backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using android_backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly ContactRepository contactRepository;
    private readonly UserRepository userRepository;

    public ContactController(ContactRepository contactRepository, UserRepository userRepository)
    {
        this.contactRepository = contactRepository;
        this.userRepository = userRepository;
    }


    /// <summary>
    /// create contact
    /// </summary>
    /// <param name="parameter">Contact</param>
    /// <returns></returns>
    [HttpPost("create"), Authorize(Roles = "user")]
    public IActionResult Create([FromForm] String usernameA, [FromForm] String usernameB)
    {
        User userA = userRepository.FindByUsername(usernameA);
        User userB = userRepository.FindByUsername(usernameB);
        Contact contact = new Contact
        {
            id = 0,
            userId = userA.id,
            contactUserId = userB.id
        };
        contactRepository.Create(contact);
        return Ok(new { message = "Success" });

    }

    /// <summary>
    /// create contact
    /// </summary>
    /// <param name="parameter">Contact</param>
    /// <returns></returns>
    [HttpGet("contactList"), Authorize(Roles = "user")]
    public IActionResult contactList()
    {
        string username = User.Identity.Name;

        User user = userRepository.FindByUsername(username);
        if (user != null)
        {
            return Ok(userRepository.FindContactUsers(username).ToList());
        }
        else
            return NotFound();



    }

}