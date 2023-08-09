namespace android_backend.Controllers;

using android_backend.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using android_backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly ContactService contactService;

    /// <summary>
    /// Initializes a new instance of the ContactController class.
    /// </summary>
    /// <param name="contactService">The contact service.</param>
    public ContactController(ContactService contactService)
    {
        this.contactService = contactService;
    }


    /// <summary>
    /// Creates a contact between the authenticated user and another user.
    /// </summary>
    /// <param name="username">The username of the user to create a contact with.</param>
    /// <returns>Returns HTTP OK if the contact was created successfully, or Conflict if the contact already exists or users do not exist.</returns>
    [HttpPost("create"), Authorize(Roles = "user")]
    public IActionResult Create([FromForm] String username)
    {

        if (contactService.CreateContact(User.Identity.Name, username) == true)
            return Ok(new { message = "Success" });
        return Conflict();

    }

    /// <summary>
    /// Retrieves a list of contact users for the authenticated user.
    /// </summary>
    /// <returns>Returns the list of contact users associated with the authenticated user, or NotFound if no contacts are found.</returns>
    [HttpGet("contactList"), Authorize(Roles = "user")]
    public IActionResult contactList()
    {
        string username = User.Identity.Name;
        List<ContactUsers> contactUserList = contactService.GetContactList(username);
        if (contactUserList != null)
        {
            return Ok(contactUserList);
        }
        else
            return NotFound();

    }

}