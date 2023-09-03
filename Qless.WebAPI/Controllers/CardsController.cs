using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qless.Repositories.Exceptions;
using Qless.Services.Interfaces;
using Qless.WebAPI.RequestModels;

namespace Qless.WebAPI.Controllers
{
    [EnableCors("AnyOrigin")]
    [Route("api/cards")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly ILogger<CardsController> _logger;
        private readonly ICardService _cardService;

        public CardsController(ILogger<CardsController> logger, ICardService cardService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cardService = cardService ?? throw new ArgumentNullException(nameof(cardService));
        }

        [HttpGet]
        public async Task<IActionResult> GetCards()
        {
            return Ok(await _cardService.GetCards());
        }

        [HttpGet]
        [Route("{cardId}")]
        public async Task<IActionResult> GetCard(int cardId)
        {
            return Ok(await _cardService.GetCard(cardId));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard([FromBody] CreateCardRequest request)
        {
            var created = await _cardService.CreateCard((int)request.CardTypeId, request.IdNo);
            if (created == null)
                throw new QlessException("No card created.");
            return Created("", created);
        }

        [HttpPost]
        [Route("{cardId}/load-card")]
        public async Task<IActionResult> LoadCard(int cardId, [FromBody] LoadCardRequest request)
        {
            return Ok(await _cardService.LoadCard(cardId, request.NewBalance));
        }

        [HttpPost]
        [Route("{cardId}/exit-station")]
        public async Task<IActionResult> ExitStation(int cardId)
        {
            var card = await _cardService.GetCard(cardId);
            if (card == null)
                return NotFound("Invalid Card.");

            return Ok(await _cardService.ExitStation(card));
        }

        [HttpPost]
        [Route("{cardId}/enter-station")]
        public async Task<IActionResult> EnterStation(int cardId)
        {
            var card = await _cardService.GetCard(cardId);
            if (card == null)
                return NotFound("Invalid Card.");

            return Ok(await _cardService.EnterStation(card));
        }
    }
}
