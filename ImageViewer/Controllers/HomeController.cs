using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ImageViewer.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using ImageViewer.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace ImageViewer.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _env;

        public HomeController(IHostingEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            List<FileInfo> images = Directory.EnumerateFiles(_env.WebRootPath + "\\images").Select(x => new FileInfo(x)).ToList();
            images.Shuffle();

            return View(images);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}