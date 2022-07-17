using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LobaVersionControl.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LobaVersionController : ControllerBase
    {
        // This below two variable we have change give the same number we have given in unity
        public static float androidVersion = 16.0f;
        public static float iOSVersion = 16.0f;

        private readonly ILogger<LobaVersionController> _logger;

        public LobaVersionController(ILogger<LobaVersionController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public float Get(string platform)
        {
            if (platform == null) return -1;

            if (platform.Equals("Android", StringComparison.OrdinalIgnoreCase))
                return androidVersion;
            else if (platform.Equals("iOS", StringComparison.OrdinalIgnoreCase))
                return iOSVersion;

            return -1;
        }
    }
}
