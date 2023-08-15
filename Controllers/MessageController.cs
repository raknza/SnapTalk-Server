namespace android_backend.Controllers;

using android_backend.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using android_backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class MessageController: ControllerBase
{
    MessageService messageService;
    public MessageController(MessageService messageService)
    {
        this.messageService = messageService;
    }


    [HttpGet(), Authorize(Roles = "user")]
    public List<Message> GetUserMessage()
    {
        string username = User.Identity.Name;
        return messageService.GetMessage(username);
        
    }

    [HttpGet("{messageId}"), Authorize(Roles = "user")]
    public IActionResult ReceivedMessage(int messageId)
    {
        string username = User.Identity.Name;
        Boolean success = messageService.ReceivedMessage(messageId);
        if(success)
            return Ok( new {message = "success"});
        else
            return NotFound();
        
    }

    [HttpGet("receivedAll"), Authorize(Roles = "user")]
    public IActionResult ReceivedAllMessage()
    {
        string username = User.Identity.Name;
        messageService.ReceivedAllMessage(username);
        return Ok( new {message = "success"});
        
    }


        
}