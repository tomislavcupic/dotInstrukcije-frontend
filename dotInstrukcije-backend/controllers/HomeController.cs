using Microsoft.AspNetCore.Mvc;

public class HomeController : ControllerBase
{
    [HttpGet("/")] // Route to handle requests to the root URL
    public IActionResult Index()
    {
        return Ok("Hello, world!"); // Example response
    }
}
