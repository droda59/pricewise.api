using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PriceAlerts.Common.Parsers;

namespace PriceAlerts.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "LocalOnly")]
    public class ParserTestController : Controller
    {
        private readonly IParserFactory _parserFactory;

        public ParserTestController(IParserFactory parserFactory)
        {
            this._parserFactory = parserFactory;
        }

        [HttpPost("parse")]
        public async Task<IActionResult> ParseAsync([FromBody]Uri uri)
        {
            IParser parser = null;

            try
            {
                parser = this._parserFactory.CreateParser(uri);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }

            var parsedContent = await parser.GetSiteInfo(uri);

            return new ObjectResult(parsedContent);
        }
    }
}
