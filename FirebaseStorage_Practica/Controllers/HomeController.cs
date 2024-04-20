using Firebase.Auth;
using Firebase.Storage;
using FirebaseStorage_Practica.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FirebaseStorage_Practica.Controllers
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
        public async Task<ActionResult> SubirArchivo(IFormFile archivo)
        {
            //Leemos el archivo subido
            Stream archivoASubir = archivo.OpenReadStream();

            //Configuramos la conexion hacia FireBase
            string email = "carlos.murga1@catolica.edu.sv";
            string clave = "h1n12002";
            string ruta = "dulcesabor-imagenes.appspot.com";
            string api_key = "AIzaSyCUmhGjhkkuvkE5S5bnPXjTHIYn9qW5pl4";

            var auth = new FirebaseAuthProvider(new FirebaseConfig(api_key));
            var autenticarFireBase = await auth.SignInWithEmailAndPasswordAsync(email, clave);

            var cancellation = new CancellationTokenSource();
            var tokenUser = autenticarFireBase.FirebaseToken;

            var tareaCargarArchivo = new FirebaseStorage(ruta,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(tokenUser),
                    ThrowOnCancel = true
                }).Child("Archivos").Child(archivo.FileName).PutAsync(archivoASubir);

            var urlArchivoCargado = await tareaCargarArchivo;

            return RedirectToAction("VerImagen");
        }
    }
}
