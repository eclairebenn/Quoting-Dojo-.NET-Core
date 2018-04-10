using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using quoting_dojo.Models;
using DbConnection;

namespace quoting_dojo.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpGet]
        [Route("quotes")]
        public IActionResult DisplayQuotes()
        {
            var allQuotes = DbConnector.Query("SELECT quotes.*, users.name as username FROM quotes JOIN users on users.id = quotes.user_id;");

            ViewBag.allQuotes = allQuotes;
            return View("quotes");
        }

        [HttpPost]
        [Route("submit")]
        public IActionResult Submit(string name, string quote)
        {
            List<string> errors = new List<string>();
            if(name == null || name.Length < 3)
            {
                errors.Add("Name must be at least 3 characters");
            }
            if(quote == null || quote.Length < 3)
            {
                errors.Add("Quote must be at least 3 characters");
            }
            if(errors.Count == 0)
            {
                DbConnector.Execute($"INSERT INTO users (name, created_at) VALUES ('{name}', NOW())");
                var users = DbConnector.Query($"SELECT * FROM users ORDER BY created_at DESC");
                int user_id = (int)users[0]["id"];

                DbConnector.Execute($"INSERT INTO quotes (content, created_at, user_id) VALUES ('{quote}', NOW(), {user_id})");     
                return RedirectToAction("DisplayQuotes");            
            }
            else
            {
                ViewBag.error = errors;
                return View("Index");
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
