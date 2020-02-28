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
                    ViewBag.equipe = novaEquipe;

                    try
                    {
                        Directory.CreateDirectory(Server.MapPath($"~/Dados/{novaEquipe.Nome}"));
                        Directory.CreateDirectory(Server.MapPath($"~/Dados/{novaEquipe.Nome}/Arquivos"));
                        Directory.CreateDirectory(Server.MapPath($"~/Dados/{novaEquipe.Nome}/Arquivos/Time"));

                        string caminhoArquivoSlides = Path.Combine(Server.MapPath($"~/Dados/{novaEquipe.Nome}"), $"slides-{novaEquipe.Nome}.json");
                        string caminhoArquivoConfig = Path.Combine(Server.MapPath($"~/Dados/{novaEquipe.Nome}"), $"config-{novaEquipe.Nome}.json");

                        using (StreamWriter slidesWriter = new StreamWriter(caminhoArquivoSlides))
                        {
                            slidesWriter.WriteLine("[{}]");
                        }
                        using (StreamWriter configWriter = new StreamWriter(caminhoArquivoConfig))
                        {
                            List<Configuracao> configuracoes = new List<Configuracao>() { new Configuracao() };

                            var settings = new JsonSerializerSettings { DateFormatString = "dd-MM-yyyy" };
                            var result = JsonConvert.SerializeObject(configuracoes, settings);
                            configWriter.WriteLine(result);
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

        [HttpGet]
        public ActionResult EditarSquad(string equipe)
        {
            Configuracao config = new Configuracao();

            if (!string.IsNullOrEmpty(equipe))
            {
                ViewBag.equipe = equipe;
                ViewBag.logo_equipe = $"/Dados/{equipe}/arquivos/logo.png";
                ViewBag.bannerEquipe = $"/Imagens/banner.png";
                ViewBag.mensagemSucesso = TempData["mensagemSucesso"] != null ? TempData["mensagemSucesso"].ToString() : String.Empty;

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
        public ActionResult EditarSquad(string equipe, Configuracao configUpdate)
        {
            if (!string.IsNullOrEmpty(equipe))
            {
                string caminhoArquivoConfig = Path.Combine(Server.MapPath($"~/Dados/{equipe}"), $"config-{equipe}.json");
                var settings = new JsonSerializerSettings { DateFormatString = "dd-MM-yyyy" };
                Configuracao configToSave;

                try
                {
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];

                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var path = Path.Combine(Server.MapPath($"~/Dados/{equipe}/arquivos/"), "logo.png");
                            file.SaveAs(path);
                        }
                    }

                    using (StreamReader sr = new StreamReader(caminhoArquivoConfig, Encoding.GetEncoding("ISO-8859-1")))
                    {
                        string conteudo = sr.ReadToEnd();
                        configToSave = JsonConvert.DeserializeObject<List<Configuracao>>(conteudo, settings).FirstOrDefault();

                        //Atualização dos dados
                        configToSave.NomeSquad = configUpdate.NomeSquad;
                        configToSave.Proposito = configUpdate.Proposito;
                    }

                    using (StreamWriter configWriter = new StreamWriter(caminhoArquivoConfig))
                    {
                        List<Configuracao> configuracoes = new List<Configuracao>();
                        configuracoes.Add(configToSave);

                        var result = JsonConvert.SerializeObject(configuracoes, settings);
                        configWriter.WriteLine(result);

                        TempData["mensagemSucesso"] = "Configuração atualizada com sucesso";
                    }

                    return RedirectToAction("EditarSquad", "Config", new { equipe = equipe });

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
    }
}