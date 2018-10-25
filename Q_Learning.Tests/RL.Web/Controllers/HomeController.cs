using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RL.Web.Hubs;
using RL.Web.Models;

namespace RL.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHubContext<TimerHub> _timerHubContext;
        public HomeController(IHubContext<TimerHub> timerHubContext)
        {
            _timerHubContext = timerHubContext;
        }

        public IActionResult Index()
        {
            _timerHubContext.Clients.All.SendAsync("ReceiveMessage", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Tests()
        {
            return View();
        }
    }
}
