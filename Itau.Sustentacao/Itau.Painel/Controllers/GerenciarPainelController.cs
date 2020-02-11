using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Itau.Controllers
{
    public class GerenciarPainelController : Controller
    {
        // GET: GerenciarPainel
        public ActionResult Index(string equipe)
        {
            if (!string.IsNullOrEmpty(equipe))
            {
                ViewBag.equipe = equipe;
                ViewBag.conteudoJson = string.Empty;
                try
                {
                    using (StreamReader sr = new StreamReader(Server.MapPath($"~/Dados/{equipe}/slides-{equipe}.json"), Encoding.GetEncoding("ISO-8859-1")))
                    {
                        string conteudo = sr.ReadToEnd();
                        ViewBag.conteudoJson = conteudo;
                    }
                }
                catch (FileNotFoundException ex)
                {
                    ViewBag.mensagemErro = ex.Message;
                }
                catch (Exception ex)
                {
                    ViewBag.mensagemErro = ex.Message;
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult SalvarJson(string equipe)
        {

            if (!string.IsNullOrEmpty(equipe))
            {
                try
                {
                    var jsonString = String.Empty;

                    Request.InputStream.Position = 0;
                    using (Stream receiveStream = Request.InputStream)
                    {
                        using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                        {
                            jsonString = readStream.ReadToEnd();

                            var uploadPath = Server.MapPath($"~/Dados/{equipe}/");
                            string caminhoArquivo = Path.Combine(@uploadPath, Path.GetFileName($"slides-{equipe}.json"));
                                                       
                            using (StreamWriter file = new StreamWriter(caminhoArquivo))
                            {
                                file.WriteLine(jsonString);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    return Json(new
                    {
                        success = false,
                        responseText = "Atualizado com sucesso"
                    }); ;                   
                }
            }


            return Json(new
            {
                success = true,
                responseText = "Atualizado com sucesso"
            }); ;
        }

    }
}