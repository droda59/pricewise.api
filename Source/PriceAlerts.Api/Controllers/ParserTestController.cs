using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceAlerts.Common.CommandHandlers;
using PriceAlerts.Common.Factories;
using PriceAlerts.Common.Infrastructure;

namespace PriceAlerts.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "LocalOnly")]
    public class ParserTestController : Controller
    {
        private readonly IHandlerFactory _handlerFactory;

        public ParserTestController(IHandlerFactory handlerFactory)
        {
            this._handlerFactory = handlerFactory;
        }

        [HttpPost("parse")]
        [LoggingDescription("Parsing url")]
        public virtual async Task<IActionResult> ParseAsync([FromBody]Uri uri)
        {
            ICommandHandler commandHandler;

            try
            {
                commandHandler = this._handlerFactory.CreateHandler(uri);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }

            var parsedContent = await commandHandler.HandleGetInfo(uri);

            return new ObjectResult(parsedContent);
        }
    }
}
