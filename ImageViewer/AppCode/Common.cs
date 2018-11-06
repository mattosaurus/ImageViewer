using Microsoft.WindowsAzure.Storage.Blob;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageViewer.AppCode
{
    public static class Common
    {
        public static async Task ResizeImage(Stream stream, CloudBlockBlob destinationBlob)
        {
            var image = SixLabors.ImageSharp.Image.Load(stream);
            var width = 2000;
            var height = 2000 * image.Height / image.Width;

            var output = new MemoryStream();
            image.Clone(i =>
                i.Resize(width, height, KnownResamplers.Bicubic))
                .SaveAsJpeg(output);

            output.Seek(0, SeekOrigin.Begin);

            // Save to blob
            await destinationBlob.UploadFromStreamAsync(output);
        }
    }
}
