using System;
using Microsoft.AspNetCore.Mvc;

public class SessionController : Controller
{
    [HttpGet]
    public IActionResult KeepAlive() 
    {
        HttpContext.Session.Set("Heartbeat", BitConverter.GetBytes(DateTime.Now.Ticks));
        return Ok();
    }
}