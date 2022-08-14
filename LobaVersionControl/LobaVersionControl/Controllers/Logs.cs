using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LobaVersionControl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Logs : ControllerBase
    {
        public Logs()
        {
        }


        // GET: api/Logs
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "1", "Player has Invited", "456", "65678687" };
        }

        // GET api/Logs/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "1";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
