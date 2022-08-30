using Microsoft.AspNetCore.Mvc;
using SOSync.Domain.Interfaces;

namespace SOSync.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatusController : ControllerBase
{
    private readonly IAPIService service;
    public StatusController(IAPIService service)
    {
        this.service = service;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetStatusList()
    {
        try
        {
            var sync = await service.GetSyncs();
            if (sync is not null)
                return Ok(sync);

            return BadRequest();
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
        
    }
}
