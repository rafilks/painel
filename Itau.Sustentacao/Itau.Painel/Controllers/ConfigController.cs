using Itau.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Itau.Controllers
{
    public class ConfigController : Controller
    {
        // GET: Config
        public ActionResult Index(string equipe)
        {
            string mensagemErro = string.Empty;
            List<Slide> slides = new List<Slide>();

            if (!string.IsNullOrEmpty(equipe))
            {
                ViewBag.equipe = equipe;   
                try
                {
                    using (StreamReader sr = new StreamReader(Server.MapPath($"~/Dados/{equipe}/slides-{equipe}.json"), Encoding.GetEncoding("ISO-8859-1")))
                    {
                        string conteudo = sr.ReadToEnd();

                        var settings = new JsonSerializerSettings { DateFormatString = "dd-MM-yyyy" };
                        slides = JsonConvert.DeserializeObject<List<Slide>>(conteudo, settings);

                        return View();
                    }
                }
                catch (Exception)
                {

                }

            }
            return View();
        }
    }
}