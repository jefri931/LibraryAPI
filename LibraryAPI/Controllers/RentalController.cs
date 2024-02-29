using AutoMapper;
using LibraryAPI.Exceptions;
using LibraryAPI.DTOs;
using LibraryAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "DefaultScheme")]
    [ApiController]
    [Route("api/[controller]")]
    public class RentalController : BaseController
    {
        private readonly IBookRentalService _rentalService;
        private readonly IMapper _mapper;
        private readonly IOverdueRentalNotificationService _notificationService;
        public RentalController(IMapper mapper, IBookRentalService rentalService, IOverdueRentalNotificationService notificationService, ILogger<RentalController> logger) {
            _mapper = mapper;
            _rentalService = rentalService;
            _notificationService = notificationService;
        }

        [HttpPost("Rent")]
        public async Task<IActionResult> Rent([FromBody]RentRequestDTO request)
        {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {
                var rental = await _rentalService.Rent(UserID, request.BookId, request.DueDate);
                var rentalDto = _mapper.Map<RentalDTO>(rental);
                return Ok(rentalDto);
            }
            catch(ArgumentOutOfRangeException e) {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("Extend")]
        public async Task<IActionResult> Extend([FromBody]ExtendRequestDTO request)
        {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if(!await _rentalService.IsRenter(UserID, request.RentalId)) {
                return Unauthorized();
            }

            try {
                await _rentalService.Extend(request.RentalId, request.DueDate);
                return NoContent();
            }
            catch(NotFoundException e) {
                return NotFound(e.Message);
            }
            catch(ArgumentOutOfRangeException e) {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("Return/{rentalId}")]
        public async Task<IActionResult> Return(int rentalId)
        {
            if(!await _rentalService.IsRenter(UserID, rentalId)) {
                return Unauthorized();
            }

            try {
                await _rentalService.Return(rentalId);
                return NoContent();
            }
            catch(NotFoundException e) {
                return NotFound(e.Message);
            }
            catch(ArgumentOutOfRangeException e) {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("wsnotifications")]
        public async Task<IActionResult> GetWebSocket()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                return BadRequest("WebSocket request expected.");
            }
            
            var cts = new CancellationTokenSource();
            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await _notificationService.HandleNotification(UserID, webSocket, cts.Token);
            return new EmptyResult();
        }
    }
}
