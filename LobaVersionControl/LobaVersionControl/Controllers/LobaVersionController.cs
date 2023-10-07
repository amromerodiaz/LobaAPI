using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Net.Mail;
using Newtonsoft.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace LobaVersionControl.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LobaVersionController : ControllerBase
    {
        // This below two variable we have change give the same number we have given in unity
        public static float androidVersion = 27.0f;
        public static float iOSVersion = 27.0f;

        public static Dictionary<string, string> verfication_code = new Dictionary<string, string>();
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

        //[HttpGet("Test")]
        //public string Sendcode(string Email)
        //{
        //    Random random = new Random();
        //    int code = random.Next(100000, 999999);

        //    string frommail = "@gmail.com";
        //    string fromPassword = "rmy";

        //    MailMessage mailMessage = new MailMessage();
        //    mailMessage.From = new MailAddress(frommail);
        //    mailMessage.Subject = "Test Subject";
        //    mailMessage.To.Add(new MailAddress(Email));
        //    mailMessage.Body = "<html><body> Test body "+ code + "<body><html>";
        //    mailMessage.IsBodyHtml = true;

        //    var smtpClient = new SmtpClient("smtp.gmail.com")
        //    {
        //        Port = 587,
        //        Credentials = new NetworkCredential(frommail, fromPassword),
        //        EnableSsl = true,
        //    };

        //   // smtpClient.Send(mailMessage);

        //    if (verfication_code.ContainsKey(Email))
        //    {
        //        verfication_code[Email] = code.ToString();
        //    }
        //    else
        //    {
        //        verfication_code.Add(Email, code.ToString());
        //    }
        //    return JsonConvert.SerializeObject(verfication_code);
        //}

        //[HttpGet("Verfication")]
        //public string Verfication(string Email ,string code)
        //{
        //    Console.WriteLine("Email " + Email + " code " + code);
        //    Console.WriteLine(JsonConvert.SerializeObject(verfication_code));
        //    if(verfication_code.ContainsKey(Email))
        //    {
        //        if(verfication_code[Email] == code)
        //        {
        //            verfication_code.Remove(Email);
        //            return "true";
        //        }
        //        else
        //        {
        //            return "false";
        //        }
        //    }
        //    else
        //    {
        //        return "user is not there in list";
        //    }
        //}
    }
}
