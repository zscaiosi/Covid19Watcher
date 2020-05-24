using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Covid19Watcher.Application.Contracts;
using Covid19Watcher.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery]GetFiltersRequest filters)
        {
            try
            {
                var response = await _service.ListNotificationsAsync(filters);

                if (response.Success)
                {
                    return StatusCode(200, response);
                }
                else
                {
                    return StatusCode(404, response);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, nameof(Exception));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody]PostNotificationsRequest request)
        {
            try
            {
                var response = await _service.CreateNotificationAsync(request);

                if (response.Success)
                    return StatusCode(200, response);
                else
                    return StatusCode(400, response);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, nameof(Exception));
            }
        }
        [HttpGet("find/{id}")]
        public async Task<IActionResult> GetFind(string id)
        {
            try
            {
                var response = await _service.FindByIdAsync(id);

                if (response.Success)
                    return StatusCode(200, response);
                else
                    return StatusCode(400, response);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, nameof(Exception));
            }
        }
    }
}