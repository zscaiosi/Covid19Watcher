using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Covid19Watcher.API.Public.Contracts;
using Covid19Watcher.API.Public.Interfaces;

namespace Covid19Watcher.API.Public.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationsService _service;
        public NotificationsController(INotificationsService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]GetFiltersRequest filters)
        {
            try
            {
                return StatusCode(200, await _service.ListNotificationsAsync(filters));
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, nameof(Exception));
            }
        }
    }
}