using System.Threading.Tasks;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService adminService;

        public AdminController(IAdminService adminService)
        {
            this.adminService = adminService;
        }

        /// <summary>
        /// Initializes users, items and the auction databases with some data
        /// </summary>
        /// <returns>NoContent</returns>
        [HttpPost]
        [Route("admin/initialize")]
        public async Task<IActionResult> Initialize()
        {
            await this.adminService.InitializeAsync();
            return NoContent();
        }
    }
}
