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
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionService auctionService;

        public AuctionController(
            IAuctionService auctionService)
        {
            this.auctionService = auctionService;
        }

        /// <summary>
        /// Return items on the auction.
        /// If item auction is finished, transfer the ownership to the top bidder
        /// </summary>
        /// <returns>Auction items</returns>
        [HttpGet]
        [Route("auction/items")]
        public async Task<IActionResult> GetItems()
        {
            var items = await this.auctionService.GetItemsAsync();
            return Ok(items);
        }

        /// <summary>
        /// Put item on the auction
        /// </summary>
        /// <param name="itemId">Item id</param>
        /// <param name="dto">Item info</param>
        /// <returns>NoContent</returns>
        [HttpPost]
        [Route("auction/items/{itemId}")]
        public async Task<IActionResult> PutItemOnAuction(Guid itemId, [FromBody] AuctionItemDto dto)
        {
            await this.auctionService.PutItemOnActionAsync(itemId, dto.Value, dto.ExpiryDate, this.GetUserId());
            return NoContent();
        }

        /// <summary>
        /// Remove item from the auction
        /// </summary>
        /// <param name="itemId">Item id</param>
        /// <returns>NoContent</returns>
        [HttpDelete]
        [Route("auction/items/{itemId}")]
        public async Task<IActionResult> RemoveItemFromAuction(Guid itemId)
        {
            await this.auctionService.RemoveItemFromAuctionAsync(itemId, this.GetUserId());
            return NoContent();
        }

        /// <summary>
        /// Bid on item
        /// </summary>
        /// <param name="itemId">Item id</param>
        /// <param name="dto">Bid info</param>
        /// <returns>NoContent</returns>
        [HttpPost]
        [Route("auction/items/{itemId}/bids")]
        public async Task<IActionResult> BidOnItem(Guid itemId, [FromBody] BidDto dto)
        {
            await this.auctionService.BidOnItemAsync(itemId, dto.Value, this.GetUserId());
            return NoContent();
        }

        /// <summary>
        /// Remove bid on item
        /// </summary>
        /// <param name="itemId">Item id</param>
        /// <returns>NoContent</returns>
        [HttpDelete]
        [Route("auction/items/{itemId}/bids")]
        public async Task<IActionResult> RemoveBidOnItem(Guid itemId)
        {
            await this.auctionService.RemoveBidOnItemAsync(itemId, this.GetUserId());
            return NoContent();
        }

        private Guid GetUserId() =>
            Guid.Parse(User.Identity.Name);
    }
}
