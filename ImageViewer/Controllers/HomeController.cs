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
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Configuration;

namespace ImageViewer.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _config;

        public HomeController(IHostingEnvironment env, IConfigurationRoot config)
        {
            _env = env;
            _config = config;
        }

        public IActionResult Index()
        {
            // Retrieve a reference to a container.
            CloudBlobContainer container = new CloudBlobContainer(new Uri(_config["Azure:ContainerConnectionString"]));

            List<FileInfo> images = Directory.EnumerateFiles(Path.Combine(_env.ContentRootPath, "StaticFiles", "images")).Select(x => new FileInfo(x)).ToList();
            images.Shuffle();

            return View(images);
        }

        [HttpGet]
        public IActionResult Image(string imageName)
        {
            string file = Path.Combine(_env.ContentRootPath, "StaticFiles", "images", imageName);

            return PhysicalFile(file, "image/jpeg");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}