using System.Net;
using System.Text.Json;
using DocumentFormat.OpenXml.InkML;
using GestionFacturas.AccesoDatosSql;
using GestionFacturas.Aplicacion;
using GestionFacturas.Dominio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using X.PagedList;

namespace GestionFacturas.Web.Pages.Clientes
{
    //[Authorize]
    public class AutocompletarClientesController : Controller
    {
        private readonly SqlDb _contexto;

        public AutocompletarClientesController(SqlDb contexto)
        {
            _contexto = contexto;
        }

        #region Acciones de autocompletar

        public async Task<ActionResult> AutocompletarPorNombre(string term)
        {
            var consulta = 
                _contexto
                .Clientes.Where(m => m.NombreOEmpresa.Contains(term))
                .OrderBy(m => m.NombreOEmpresa)
                .Take(10)
                .Select(m => new
                {
                    m.Id,
                    m.NombreOEmpresa,
                    m.NumeroIdentificacionFiscal,
                    Direccion1 = m.Direccion1(),
                    Direccion2 = m.Direccion2(),
                    m.Localidad,
                    m.Provincia,
                    m.CodigoPostal,
                    m.CodigoPaisIso2,
                    m.Email
                });


            var clientes = await consulta.ToListAsync();
            return Json(clientes);
        }
        public async Task<ActionResult> AutocompletarPorId(int term)
        {
            var consulta =
                _contexto
                    .Clientes
                    .Where(m => m.Id == term)
                    .OrderBy(m => m.NombreOEmpresa)
                    .Take(10)
                    .Select(m => new
                    {
                        m.Id,
                        m.NombreOEmpresa,
                        m.NumeroIdentificacionFiscal,
                        Direccion1 = m.Direccion1(),
                        Direccion2 = m.Direccion2(),
                        m.Localidad,
                        m.Provincia,
                        m.CodigoPostal,
                        m.CodigoPaisIso2,
                        m.Email
                    });


            var clientes = await consulta.ToListAsync();
            return Json(clientes, new JsonSerializerOptions());
        }
        public async Task<ActionResult> AutocompletarPorIdentificacionFiscal(string term)
        {
            var consulta =
                _contexto
                    .Clientes
                    .Where(m => m.NumeroIdentificacionFiscal.Contains(term))
                    .OrderBy(m => m.NombreOEmpresa)
                    .Take(10)
                    .Select(m => new
                    {
                        m.Id,
                        m.NombreOEmpresa,
                        m.NumeroIdentificacionFiscal,
                        Direccion1 = m.Direccion1(),
                        Direccion2 = m.Direccion2(),
                        m.Localidad,
                        m.Provincia,
                        m.CodigoPostal,
                        m.CodigoPaisIso2,
                        m.Email
                    });


            var clientes = await consulta.ToListAsync();
            return Json(clientes, new JsonSerializerOptions());
        }

        #endregion

        
        
    }
}
