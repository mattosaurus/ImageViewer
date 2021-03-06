﻿using System;
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
using ImageViewer.AppCode;

namespace ImageViewer.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;

        public HomeController(IHostingEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }

        public async Task<IActionResult> Index()
        {
            // Retrieve a reference to a container.
            CloudBlobContainer container = new CloudBlobContainer(new Uri(_config["AzureStorage:ContainerConnectionString"]));
            BlobContinuationToken blobContinuationToken = null;

            List<string> blobNames = new List<string>();

            do
            {
                BlobResultSegment results = await container.ListBlobsSegmentedAsync("raw/", blobContinuationToken);
                // Get the value of the continuation token returned by the listing call.
                blobContinuationToken = results.ContinuationToken;
                // Cast results to CloudBlockBlob
                blobNames.AddRange(results.Results.OfType<CloudBlockBlob>().Select(b => b.Name.Replace("raw/", "")).ToList());
            } while (blobContinuationToken != null); // Loop while the continuation token is not null.

            //foreach (string blobName in blobNames)
            //{
            //    CloudBlockBlob rawBlob = container.GetBlockBlobReference("raw/" + blobName);
            //    CloudBlockBlob webBlob = container.GetBlockBlobReference("web/" + blobName);
            //    webBlob.Properties.ContentType = "image/jpeg";

            //    MemoryStream memoryStream = new MemoryStream();
            //    await rawBlob.DownloadToStreamAsync(memoryStream);
            //    memoryStream.Seek(0, SeekOrigin.Begin);

            //    await Common.ResizeImage(memoryStream, webBlob);
            //}

            blobNames.Shuffle();

            return View(blobNames);
        }

        [HttpGet]
        public async Task<IActionResult> Image(string id)
        {
            // Retrieve a reference to a container.
            CloudBlobContainer container = new CloudBlobContainer(new Uri(_config["AzureStorage:ContainerConnectionString"]));
            CloudBlockBlob blob = container.GetBlockBlobReference("web/" + id);

            MemoryStream memoryStream = new MemoryStream();
            await blob.DownloadToStreamAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new FileStreamResult(memoryStream, "image/jpeg");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}