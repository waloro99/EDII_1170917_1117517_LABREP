using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LABREPO_ED2.ClassLab1;
using LABREPO_ED2.Repository;

namespace LABREPO_ED2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        //------------------------------ ENDPOINTS LAB 1 -----------------------------------------------
        private static readonly ISodasDataBase SDatabase = new SodasDataBase();

        // localhost:51626/weatherforecast/GetWithParam/?SearchSoda=""
        [HttpGet("GetWithParam", Name = "GetSoda")]
        /*[Route("weatherforecast/Sodas/")]*/
        public IEnumerable<Soda> Get(string SearchSoda)
        {
            if (SearchSoda == null)
            {
                return SDatabase.GetSodas();
            }
            else
            {
                List<Soda> SodaOrden = new List<Soda>(); //new list
                SodaOrden.Add(SDatabase.ReturnMySoda(SearchSoda));
                return SodaOrden;
            }
        }

        [HttpPost]
        public void Post([FromBody]Soda newSoda)
        {
            SDatabase.AddNewSoda(newSoda.Name, newSoda); //method insert
        }
        //------------------------------ END ENDPOINTS LAB 1 -------------------------------------------

        //------------------------------ ENDPOINTS LAB 3 -----------------------------------------------
        //------------------------------ END ENDPOINTS LAB 3 -------------------------------------------

        //------------------------------ ENDPOINTS LAB 4 -----------------------------------------------
        //------------------------------ END ENDPOINTS LAB 4 -------------------------------------------

        //------------------------------ ENDPOINTS LAB 5 -----------------------------------------------
        //------------------------------ END ENDPOINTS LAB 5 -------------------------------------------

        //------------------------------ ENDPOINTS LAB 6 -----------------------------------------------
        //------------------------------ END ENDPOINTS LAB 6 -------------------------------------------



    }
}
