﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace OrderingFood.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Offer()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Feedback()
        {
            return View();
        }

        
    }
}