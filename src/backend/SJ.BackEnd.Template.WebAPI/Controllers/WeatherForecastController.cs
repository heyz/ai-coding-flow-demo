using Microsoft.AspNetCore.Mvc;
using SJ.BackEnd.Template.IServices;
using SJ.BackEnd.Template.Model;

namespace SJ.BackEnd.Template.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly IBaseServices<LlmConfig> _configSrv;

        private readonly ITranService _tranSrv;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IBaseServices<LlmConfig> configSrv, ITranService tranSrv)
        {
            _logger = logger;
            _configSrv = configSrv;
            _tranSrv = tranSrv;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            throw new Exception("²âÊÔÒì³£");
            _logger.LogInformation("WeatherForecastController Get");
            //var configs = _configSrv.Query().Result;

            // await _tranSrv.TestTran();

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
