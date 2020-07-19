using System;
using System.Threading.Tasks;
using Api.Models;
using Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(
            IUserService userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// Authenticate user and return JWT token
        /// </summary>
        /// <param name="dto">Username and password</param>
        /// <returns>JWT token in base64 format</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("user/authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserDto dto)
        {
            var token = await this.userService.AuthenticateAsync(dto.Username, dto.Password);

            if (token == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(token);
        }

        /// <summary>
        /// Return user items
        /// </summary>
        /// <returns>List of items</returns>
        [HttpGet]
        [Route("user/items")]
        public async Task<IActionResult> GetUserItems()
        {
            var items = await this.userService.GetUserItemsAsync(this.GetUserId());

            return Ok(items);
        }

        /// <summary>
        /// Return user auction items
        /// </summary>
        /// <returns>List of items on auction</returns>
        [HttpGet]
        [Route("user/auctionitems")]
        public async Task<IActionResult> GetUserAuctionItems()
        {
            var auctionItems = await this.userService.GetUserAuctionItemsAsync(this.GetUserId());

            return Ok(auctionItems);
        }

        /// <summary>
        /// Return user bids
        /// </summary>
        /// <returns>List of bids</returns>
        [HttpGet]
        [Route("user/bids")]
        public async Task<IActionResult> GetUserBids()
        {
            var bids = await this.userService.GetUserBidsAsync(this.GetUserId());

            return Ok(bids);
        }

        private Guid GetUserId() =>
            Guid.Parse(User.Identity.Name);
    }
}
