using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PriceAlerts.Api.Factories;
using PriceAlerts.Api.SourceHandlers;

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
        public async Task<IActionResult> ParseAsync([FromBody]Uri uri)
        {
            IHandler handler = null;

            try
            {
                handler = this._handlerFactory.CreateHandler(uri);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }

            var parsedContent = await handler.HandleParse(uri);

            return new ObjectResult(parsedContent);
        }
    }
}
