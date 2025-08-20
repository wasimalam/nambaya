using Common.BusinessObjects;
using Common.BusinessObjects.Interfaces;
using Common.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Common.Services.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LookupsController : ControllerBase
    {
        private readonly ILogger<LookupsController> _logger;
        private ILookupService _lookupsService;
        private ILanguageService _languageService;
        public LookupsController(ILogger<LookupsController> logger, ILookupService lookupsService, ILanguageService languageService)
        {
            _logger = logger;
            _lookupsService = lookupsService;
            _languageService = languageService;
        }
        [HttpGet("genders")]
        public ActionResult<IEnumerable<LookupsBO>> GetGenders()
        {
            var ph = _lookupsService.GetItems("Gender");
            if (ph == null)
            {
                return NotFound();
            }
            return Ok(ph);
        }
        [HttpGet("cities")]
        public ActionResult<IEnumerable<LookupsBO>> GetCities()
        {
            var ph = _lookupsService.GetItems("City");
            if (ph == null)
            {
                return NotFound();
            }
            return Ok(ph);
        }
        [HttpGet("items")]
        public ActionResult<IEnumerable<LookupsBO>> GetItems(string code)
        {
            var ph = _lookupsService.GetItems(code);
            if (ph == null)
            {
                return NotFound();
            }
            return Ok(ph);
        }
        [HttpGet("languages")]
        public ActionResult<IEnumerable<LanguageBO>> GetLanguages()
        {
            var ph = _languageService.GetLanguages();
            if (ph == null)
            {
                return NotFound();
            }
            return Ok(ph);
        }
        [HttpPost("cities")]
        public ActionResult<LookupsBO> InsertCity(LookupsBO lookupsBO)
        {
            lookupsBO.LookupCatID = LookUpCategoryCodes.LookUpCategories["CITY"].CategoryId;
            var id = _lookupsService.Insert(lookupsBO);
            return Ok(_lookupsService.Get(id));
        }
    }
}
