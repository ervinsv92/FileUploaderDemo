using FileUploaderDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace FileUploaderDemo.Controllers
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Upload(FileUploadModel model)
        {
            if (model.File != null && model.File.Length > 0)
            {
                // Usa el nombre del archivo proporcionado
                //var fileName = model.FileName ?? model.File.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", model.File.FileName);

                // Asegúrate de que el directorio exista
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                // Guarda el archivo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }


                await UploadFileAsync(filePath, model.File.FileName, model.FileName);
                ViewBag.Message = "File uploaded successfully!";
            }
            else
            {
                ViewBag.Message = "No file selected.";
            }

            return RedirectToAction("Index");
        }

        public async Task UploadFileAsync(string filePath, string OriginalName, string fileName)
        {
            using (var client = new HttpClient())
            {
                using (var form = new MultipartFormDataContent())
                {
                    // Agregar el archivo
                    var fileContent = new StreamContent(System.IO.File.OpenRead(filePath));
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                    form.Add(fileContent, "file", OriginalName);

                    // Agregar el nombre del archivo
                    form.Add(new StringContent(fileName), "fileName");

                    // Enviar la solicitud
                    var response = await client.PostAsync("http://localhost:5277/api/file/upload", form);
                    response.EnsureSuccessStatusCode();

                    // Leer la respuesta
                    var responseString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseString);
                }
            }
        }
    }
}
