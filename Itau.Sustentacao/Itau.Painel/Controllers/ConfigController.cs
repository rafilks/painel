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
        [HttpGet]
        public ActionResult CriarEquipe()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CriarEquipe(Equipe novaEquipe)
        {
            if (!string.IsNullOrEmpty(novaEquipe.Nome))
            {

                if (!novaEquipe.Nome.Any(Char.IsWhiteSpace))
                {

                    try
                    {
                        Directory.CreateDirectory(Server.MapPath($"~/Dados/{novaEquipe.Nome}"));
                        Directory.CreateDirectory(Server.MapPath($"~/Dados/{novaEquipe.Nome}/Arquivos"));
                        Directory.CreateDirectory(Server.MapPath($"~/Dados/{novaEquipe.Nome}/Arquivos/Time"));

                        string caminhoArquivoSlides = Path.Combine(Server.MapPath($"~/Dados/{novaEquipe.Nome}"), $"slides-{novaEquipe.Nome}.json");
                        string caminhoArquivoConfig = Path.Combine(Server.MapPath($"~/Dados/{novaEquipe.Nome}"), $"config-{novaEquipe.Nome}.json");

                        using (StreamWriter slides = new StreamWriter(caminhoArquivoSlides))
                        {
                            slides.WriteLine("[{}]");
                        }
                        using (StreamWriter config = new StreamWriter(caminhoArquivoConfig))
                        {
                            List<Configuracao> configuracoes = new List<Configuracao>() { new Configuracao() };

                            var settings = new JsonSerializerSettings { DateFormatString = "dd-MM-yyyy" };
                            var result = JsonConvert.SerializeObject(configuracoes, settings);
                            config.WriteLine(result);
                        }

                        return RedirectToAction("EditarSquad", "Config", new { equipe = novaEquipe.Nome });
                    }

                    catch (Exception ex)
                    {
                        ModelState.Clear();
                        ModelState.AddModelError("Error", ex.Message);

                        return View("Criar", novaEquipe);
                    }

                }
                else
                {
                    ModelState.Clear();
                    ModelState.AddModelError("Error", "Não pode conter espaço");

                    return View(novaEquipe);
                }
            }
            else
            {
                ModelState.Clear();
                ModelState.AddModelError("Error", "O campo não pode estar vazio");

                return View(novaEquipe);
            }
        }

        public ActionResult EditarSquad(string equipe)
        {
            Configuracao config = new Configuracao();     

            if (!string.IsNullOrEmpty(equipe))
            {
                ViewBag.equipe = equipe;
                ViewBag.logo_equipe = $"/Dados/{equipe}/arquivos/logo.png";
                ViewBag.bannerEquipe = $"/Imagens/banner.png";               

                #region Lê a configuração

                try
                {
                    using (StreamReader sr = new StreamReader(Server.MapPath($"~/Dados/{equipe}/config-{equipe}.json"), Encoding.GetEncoding("ISO-8859-1")))
                    {
                        string conteudo = sr.ReadToEnd();

                        if (!string.IsNullOrEmpty(equipe))
                        {
                            var settings = new JsonSerializerSettings { DateFormatString = "dd-MM-yyyy" };
                            config = JsonConvert.DeserializeObject<List<Configuracao>>(conteudo, settings).FirstOrDefault();                            
                        }
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
                #endregion    
            }
            return View(config);
        }

        [HttpPost]
        public ActionResult EditarSquad(Configuracao config)
        {

            return View();
        }
    }
}