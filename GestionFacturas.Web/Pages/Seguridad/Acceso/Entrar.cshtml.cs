using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using GestionFacturas.AccesoDatosSql;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionFacturas.Web.Pages.Seguridad.Acceso
{
    public class EntrarViewModel
    {
        [Required(ErrorMessage = "Indica tu email")]
        [Display(Name = "Email")]
        [StringLength(100, ErrorMessage = "La longitud máxima permitida es de 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Indica tu contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;
    }



    [BindProperties]
    public class EntrarModel : PageModel
    {
        private readonly SqlDb _db;
        public EntrarModel(SqlDb db)
        {
            _db = db;
            
        }

        public EntrarViewModel ViewModel { get; set; } = new ();

        
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; } = string.Empty;

        public void OnGet()
        {
            // SIn implementación necesaria por ahora

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            var usuario = _db.Usuarios
                .FirstOrDefault(m => m.Email == ViewModel.Email);

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Email incorrecto.");
                return Page();
            }

            var autenticacion = usuario.EsPasswordCorrecto(ViewModel.Password);

            if (!autenticacion)
            {
                ModelState.AddModelError(string.Empty,"Contraseña incorrecta");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
            };

            //foreach (var permisoDeUsuario in usuario.Permisos)
            //{
            //    claims.Add(new Claim(ClaimCoffeeTrace.Permiso, permisoDeUsuario.Permiso.Nombre));
            //}

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme/*, "user", "role"*/);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60)                
            };

            await HttpContext.SignInAsync(claimsPrincipal, authProperties);

            return RedirectToLocal(ReturnUrl);

        }
        
        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return Redirect("/");
        }

    }
}