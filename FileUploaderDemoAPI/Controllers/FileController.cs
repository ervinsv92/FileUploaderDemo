using FileUploaderDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileUploaderDemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] FileUploadModel model)
        {
            if (model.File != null && model.File.Length > 0)
            {
                // Usa el nombre del archivo proporcionado o el nombre del archivo original
                var fileName = model.FileName ?? model.File.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", model.File.FileName);

                // Asegúrate de que el directorio exista
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                // Guarda el archivo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                return Ok(new { Message = "File uploaded successfully!" });
            }

            return BadRequest(new { Message = "No file selected." });
        }
    }
}
