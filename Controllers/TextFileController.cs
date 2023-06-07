using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using TextFileUpload.Data;

using TextFileUpload.Models;

namespace TextFileUpload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TextFileController : ControllerBase
    {
        private readonly TextFileDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public TextFileController(TextFileDbContext context, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
        }

        [HttpPost("upload-to-server-add-to-database")]
        public async Task<IActionResult> UploadFilee(IFormFile file)
        {
            var client = _httpClientFactory.CreateClient();
            var formDataContent = new MultipartFormDataContent
    {
        { new StreamContent(file.OpenReadStream()), "file", file.FileName }
    };
            var response = await client.PostAsync(@"http://192.168.40.1:8001/Upload", formDataContent);
            response.EnsureSuccessStatusCode();
            var filePath = await response.Content.ReadAsStringAsync();

            // Add the uploaded file to the database
            
                var textFile = new TextFile()

                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Content = await ReadFileAsync(file),
                    FilePath = filePath,
                    UploadDate = DateTime.Now
                };
                try
                {
                    _context.TextFiles.Add(textFile);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    throw;
                }


                return Ok();
            
        }

        private async Task<byte[]> ReadFileAsync(IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }



        [HttpPost("upload")]
        public async Task<IActionResult> UploadFiles(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please select a file to upload.");

            var tempFilePath = Path.GetTempFileName();

            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            using (var client = new HttpClient())
            {
                var serverUrl = "http://192.168.40.1:8001/UploadFolder";
                var fileContent = new StreamContent(System.IO.File.OpenRead(tempFilePath));
                var formData = new MultipartFormDataContent
        {
            { fileContent, "file", Path.GetFileName(tempFilePath) } // Utilisez le nom du fichier temporaire comme nom de fichier dans la demande
        };

                var response = await client.PostAsync(serverUrl, formData);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Supprimez éventuellement le fichier temporaire
                System.IO.File.Delete(tempFilePath);

                return Ok(responseContent);
            }
        }
    }
}




