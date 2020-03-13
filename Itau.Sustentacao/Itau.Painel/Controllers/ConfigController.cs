using Itau.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Itau.Controllers
{
    public class ConfigController : Controller
    {
        Encoding encoding = Encoding.GetEncoding("ISO-8859-1");
        JsonSerializerSettings settings = new JsonSerializerSettings { DateFormatString = "dd-MM-yyyy" };

        #region Criar Equipe

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

                        SalvarConfiguracaoSlides(novaEquipe.Nome, new List<Slide>(), true);
                        SalvarConfiguracao(novaEquipe.Nome, new Configuracao(), true);

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

        #endregion

        #region EditarSquad

        [HttpGet]
        public ActionResult EditarSquad(string equipe)
        {
            Configuracao config = new Configuracao();

            if (!string.IsNullOrEmpty(equipe))
            {
                config = LeConfiguracao(equipe);
            }
            return View(config);
        }

        [HttpPost]
        public ActionResult EditarSquad(string equipe, Configuracao configUpdate)
        {
            if (!string.IsNullOrEmpty(equipe))
            {

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

                    var configToSave = LeConfiguracao(equipe);

                    configToSave.NomeSquad = configUpdate.NomeSquad;
                    configToSave.Proposito = configUpdate.Proposito;

                    SalvarConfiguracao(equipe, configToSave);

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

        #endregion

        #region Siglas

        [HttpGet]
        public ActionResult EditarSiglas(string equipe)
        {
            Configuracao config = new Configuracao();

            if (!string.IsNullOrEmpty(equipe))
            {
                config = LeConfiguracao(equipe);
            }
            return View(config.Siglas);
        }

        [HttpPost]
        public ActionResult CriarEditarSigla(string equipe, Sigla sigla)
        {
            if (!string.IsNullOrEmpty(equipe) && !string.IsNullOrEmpty(sigla.CodSigla))
            {
                var configToSave = LeConfiguracao(equipe);
                var objUpdate = configToSave.Siglas.FirstOrDefault(x => x.Id == sigla.Id);
                if (objUpdate == null)
                {
                    var ultimoItem = configToSave.Siglas.OrderByDescending(s => s.Id).FirstOrDefault();
                    sigla.Id = ultimoItem != null ? ultimoItem.Id + 1 : 1;

                    configToSave.Siglas.Add(sigla);
                }
                else
                {
                    objUpdate.CodSigla = sigla.CodSigla;
                    objUpdate.Tipo = sigla.Tipo;
                }

                SalvarConfiguracao(equipe, configToSave);

                return RedirectToAction("EditarSiglas", "Config", new { equipe = equipe });

            }

            return RedirectToAction("EditarSiglas", "Config", new { equipe = equipe });
        }

        [HttpGet]
        public ActionResult ExcluirSigla(string equipe, int Id)
        {
            if (!string.IsNullOrEmpty(equipe))
            {
                var configToSave = LeConfiguracao(equipe);

                var objUpdate = configToSave.Siglas.FirstOrDefault(x => x.Id == Id);
                if (objUpdate != null)
                {
                    configToSave.Siglas.Remove(objUpdate);
                }

                SalvarConfiguracao(equipe, configToSave);

                return RedirectToAction("EditarSiglas", "Config", new { equipe = equipe });
            }

            return RedirectToAction("EditarSiglas", "Config", new { equipe = equipe });
        }

        #endregion

        #region Nosso Time

        [HttpGet]
        public ActionResult EditarNossoTime(string equipe)
        {
            Configuracao config = new Configuracao();
            if (!string.IsNullOrEmpty(equipe))
            {
                config = LeConfiguracao(equipe);
            }
            return View(config.NossoTime);
        }

        [HttpPost]
        public ActionResult CriarEditarNossoTime(string equipe, NossoTime nossoTime)
        {
            if (!string.IsNullOrEmpty(equipe) && !string.IsNullOrEmpty(nossoTime.Nome))
            {
                var configToSave = LeConfiguracao(equipe);

                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath($"~/Dados/{equipe}/arquivos/Time/"), $"{nossoTime.Principal}.png");
                        var caminhoFoto = $"/Dados/{equipe}/arquivos/Time/{nossoTime.Principal}.png";

                        file.SaveAs(path);
                        nossoTime.UrlImagem = caminhoFoto;
                    }
                }

                var objUpdate = configToSave.NossoTime.FirstOrDefault(x => x.Id == nossoTime.Id);
                if (objUpdate == null)
                {
                    var ultimoItem = configToSave.NossoTime.OrderByDescending(x => x.Id).FirstOrDefault();
                    nossoTime.Id = ultimoItem != null ? ultimoItem.Id + 1 : 1;

                    configToSave.NossoTime.Add(nossoTime);
                }
                else
                {
                    objUpdate.Nome = nossoTime.Nome;
                    objUpdate.Papel = nossoTime.Papel;
                    objUpdate.Principal = nossoTime.Principal;
                    objUpdate.TipoPapel = nossoTime.TipoPapel;
                    objUpdate.Ausencia = nossoTime.Ausencia;
                    objUpdate.UrlImagem = string.IsNullOrEmpty(nossoTime.UrlImagem) ? objUpdate.UrlImagem : nossoTime.UrlImagem;
                }

                SalvarConfiguracao(equipe, configToSave);

                return RedirectToAction("EditarNossoTime", "Config", new { equipe = equipe });
            }

            return RedirectToAction("EditarNossoTime", "Config", new { equipe = equipe });
        }

        [HttpGet]
        public ActionResult ExcluirNossoTime(string equipe, int id)
        {
            if (!string.IsNullOrEmpty(equipe))
            {
                var configToSave = LeConfiguracao(equipe);

                var objUpdate = configToSave.NossoTime.FirstOrDefault(x => x.Id == id);
                if (objUpdate != null)
                {
                    configToSave.NossoTime.Remove(objUpdate);
                }

                SalvarConfiguracao(equipe, configToSave);

                return RedirectToAction("EditarNossoTime", "Config", new { equipe = equipe });

            }

            return RedirectToAction("EditarNossoTime", "Config", new { equipe = equipe });
        }

        #endregion

        #region Ausência Programada

        [HttpGet]
        public ActionResult EditarAusencias(string equipe)
        {
            Configuracao config = new Configuracao();
            if (!string.IsNullOrEmpty(equipe))
            {
                config = LeConfiguracao(equipe);
            }
            return View(config.AusenciaProgramada);
        }

        [HttpPost]
        public ActionResult CriarEditarAusencias(string equipe, AusenciaProgramada ausencia)
        {
            if (!string.IsNullOrEmpty(equipe) && !string.IsNullOrEmpty(ausencia.Nome))
            {
                var configToSave = LeConfiguracao(equipe);

                var objUpdate = configToSave.AusenciaProgramada.FirstOrDefault(n => n.Id == ausencia.Id);
                if (objUpdate == null)
                {
                    var ultimoItem = configToSave.AusenciaProgramada.OrderByDescending(x => x.Id).FirstOrDefault();
                    ausencia.Id = ultimoItem != null ? ultimoItem.Id + 1 : 1;

                    configToSave.AusenciaProgramada.Add(ausencia);
                }
                else
                {
                    objUpdate.Nome = ausencia.Nome;
                    objUpdate.Data = ausencia.Data;
                    objUpdate.Motivo = ausencia.Motivo;
                    //objUpdate.Valido = ausencia.Valido;
                }

                SalvarConfiguracao(equipe, configToSave);

                return RedirectToAction("EditarAusencias", "Config", new { equipe = equipe });

            }
            return RedirectToAction("EditarAusencias", "Config", new { equipe = equipe });
        }

        [HttpGet]
        public ActionResult ExcluirAusencias(string equipe, int id)
        {
            if (!string.IsNullOrEmpty(equipe))
            {

                var configToSave = LeConfiguracao(equipe);

                var objUpdate = configToSave.AusenciaProgramada.FirstOrDefault(x => x.Id == id);
                if (objUpdate != null)
                {
                    configToSave.AusenciaProgramada.Remove(objUpdate);
                }

                SalvarConfiguracao(equipe, configToSave);

                return RedirectToAction("EditarAusencias", "Config", new { equipe = equipe });
            }
            return RedirectToAction("EditarAusencias", "Config", new { equipe = equipe });
        }


        #endregion

        #region Férias

        [HttpGet]
        public ActionResult EditarFerias(string equipe)
        {
            Configuracao config = new Configuracao();
            if (!string.IsNullOrEmpty(equipe))
            {
                config = LeConfiguracao(equipe);
            }
            return View(config.Ferias);
        }

        [HttpPost]
        public ActionResult CriarEditarFerias(string equipe, Ferias ferias)
        {
            if (!string.IsNullOrEmpty(equipe) && !string.IsNullOrEmpty(ferias.Nome))
            {
                var configToSave = LeConfiguracao(equipe);

                var objUpdate = configToSave.Ferias.FirstOrDefault(x => x.Id == ferias.Id);
                if (objUpdate == null)
                {
                    var ultimoItem = configToSave.Ferias.OrderByDescending(x => x.Id).FirstOrDefault();
                    ferias.Id = ultimoItem != null ? ultimoItem.Id + 1 : 1;

                    configToSave.Ferias.Add(ferias);
                }
                else
                {
                    objUpdate.Nome = ferias.Nome;
                    objUpdate.Tipo = ferias.Tipo;
                    objUpdate.DataInicio = ferias.DataInicio;
                    objUpdate.DataFim = ferias.DataFim;
                }

                SalvarConfiguracao(equipe, configToSave);
                return RedirectToAction("EditarFerias", "Config", new { equipe = equipe });
            }
            return RedirectToAction("EditarFerias", "Config", new { equipe = equipe });
        }

        [HttpGet]
        public ActionResult ExcluirFerias(string equipe, int id)
        {
            if (!string.IsNullOrEmpty(equipe))
            {
                var configToSave = LeConfiguracao(equipe);

                var objUpdate = configToSave.Ferias.FirstOrDefault(x => x.Id == id);
                if (objUpdate != null)
                {
                    configToSave.Ferias.Remove(objUpdate);
                }

                SalvarConfiguracao(equipe, configToSave);

                return RedirectToAction("EditarFerias", "Config", new { equipe = equipe });
            }
            return RedirectToAction("EditarFerias", "Config", new { equipe = equipe });
        }

        #endregion

        #region Plantão

        [HttpGet]
        public ActionResult EditarPlantao(string equipe)
        {
            Configuracao config = new Configuracao();
            if (!string.IsNullOrEmpty(equipe))
            {
                config = LeConfiguracao(equipe);
            }
            return View(config.DadosPlantao);
        }

        [HttpPost]
        public ActionResult CriarEditarPlantao(string equipe, DadosPlantao plantao)
        {
            if (!string.IsNullOrEmpty(equipe) && !string.IsNullOrEmpty(plantao.Plataforma))
            {
                var configToSave = LeConfiguracao(equipe);

                var objUpdate = configToSave.DadosPlantao.FirstOrDefault(x => x.Id == plantao.Id);
                if (objUpdate == null)
                {
                    var ultimoItem = configToSave.DadosPlantao.OrderByDescending(x => x.Id).FirstOrDefault();
                    plantao.Id = ultimoItem != null ? ultimoItem.Id + 1 : 1;

                    configToSave.DadosPlantao.Add(plantao);
                }
                else
                {
                    objUpdate.Plataforma = plantao.Plataforma;
                    objUpdate.Funcionario = plantao.Funcionario;
                    objUpdate.DataInicio = plantao.DataInicio;
                    objUpdate.DataFim = plantao.DataFim;                    
                }

                SalvarConfiguracao(equipe, configToSave);
                return RedirectToAction("EditarPlantao", "Config", new { equipe = equipe });
            }
            return RedirectToAction("EditarPlantao", "Config", new { equipe = equipe });
        }

        [HttpGet]
        public ActionResult ExcluirPlantao(string equipe, int id)
        {
            if (!string.IsNullOrEmpty(equipe))
            {
                var configToSave = LeConfiguracao(equipe);

                var objUpdate = configToSave.DadosPlantao.FirstOrDefault(x => x.Id == id);
                if (objUpdate != null)
                {
                    configToSave.DadosPlantao.Remove(objUpdate);
                }

                SalvarConfiguracao(equipe, configToSave);

                return RedirectToAction("EditarPlantao", "Config", new { equipe = equipe });
            }
            return RedirectToAction("EditarPlantao", "Config", new { equipe = equipe });
        }

        #endregion

        #region Cerimonia

        [HttpGet]
        public ActionResult EditarCerimonias(string equipe)
        {
            Configuracao config = new Configuracao();
            if (!string.IsNullOrEmpty(equipe))
            {
                config = LeConfiguracao(equipe);
            }
            return View(config.InfoCerimonias);
        }

        [HttpPost]
        public ActionResult CriarEditarCerimonia(string equipe, Cerimonia cerimonia)
        {
            if (!string.IsNullOrEmpty(equipe) && !string.IsNullOrEmpty(cerimonia.Descricao))
            {
                var configToSave = LeConfiguracao(equipe);

                var objUpdate = configToSave.InfoCerimonias.FirstOrDefault(x => x.Id == cerimonia.Id);
                if (objUpdate == null)
                {
                    var ultimoItem = configToSave.InfoCerimonias.OrderByDescending(x => x.Id).FirstOrDefault();
                    cerimonia.Id = ultimoItem != null ? ultimoItem.Id + 1 : 1;

                    configToSave.InfoCerimonias.Add(cerimonia);
                }
                else
                {
                    objUpdate.Descricao = cerimonia.Descricao;
                    objUpdate.Data = cerimonia.Data;
                    objUpdate.Hora = cerimonia.Hora;
                }

                SalvarConfiguracao(equipe, configToSave);
                return RedirectToAction("EditarCerimonias", "Config", new { equipe = equipe });
            }
            return RedirectToAction("EditarCerimonias", "Config", new { equipe = equipe });
        }

        [HttpGet]
        public ActionResult ExcluirCerimonia(string equipe, int id)
        {
            if (!string.IsNullOrEmpty(equipe))
            {
                var configToSave = LeConfiguracao(equipe);

                var objUpdate = configToSave.InfoCerimonias.FirstOrDefault(x => x.Id == id);
                if (objUpdate != null)
                {
                    configToSave.InfoCerimonias.Remove(objUpdate);
                }

                SalvarConfiguracao(equipe, configToSave);

                return RedirectToAction("EditarCerimonia", "Config", new { equipe = equipe });
            }
            return RedirectToAction("EditarCerimonia", "Config", new { equipe = equipe });
        }
        #endregion

        #region Slides

        [HttpGet]
        public ActionResult EditarSlides(string equipe)
        {
            List<Slide> slides = new List<Slide>();

            if (!string.IsNullOrEmpty(equipe))
            {
                slides = LeConfiguracaoSlides(equipe);
            }
            return View(slides);
        }

        [HttpPost]
        public ActionResult CriarEditarSlide(string equipe, Slide slide)
        {
            if (!string.IsNullOrEmpty(equipe) && !string.IsNullOrEmpty(slide.Nome))
            {
                var configToSave = LeConfiguracaoSlides(equipe);

                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath($"~/Dados/{equipe}/arquivos/"), fileName);
                        var caminhoArquivo = $"/Dados/{equipe}/arquivos/{fileName}";

                        file.SaveAs(path);
                        slide.Conteudo = caminhoArquivo;
                    }
                }

                var objUpdate = configToSave.FirstOrDefault(x => x.Id == slide.Id);
                if (objUpdate == null)
                {
                    var ultimoItem = configToSave.OrderByDescending(x => x.Id).FirstOrDefault();
                    slide.Id = ultimoItem != null ? ultimoItem.Id + 1 : 1;
                    configToSave.Add(slide);
                }
                else
                {
                    objUpdate.Nome = slide.Nome;
                    objUpdate.Tipo = slide.Tipo;
                    objUpdate.Tempo = slide.Tempo;
                    objUpdate.Conteudo = string.IsNullOrEmpty(slide.Conteudo) ? objUpdate.Conteudo : slide.Conteudo;
                }

                SalvarConfiguracaoSlides(equipe, configToSave);
                return RedirectToAction("EditarSlides", "Config", new { equipe = equipe });
            }
            return RedirectToAction("EditarSlides", "Config", new { equipe = equipe });
        }

        [HttpGet]
        public ActionResult ExcluirSlide(string equipe, int idSlide)
        {
            if (!string.IsNullOrEmpty(equipe) && idSlide > 0)
            {
                var configToSave = LeConfiguracaoSlides(equipe);

                var objUpdate = configToSave.FirstOrDefault(x => x.Id == idSlide);
                if (objUpdate != null)
                {
                    configToSave.Remove(objUpdate);
                }
                SalvarConfiguracaoSlides(equipe, configToSave);

                return RedirectToAction("EditarSlides", "Config", new { equipe = equipe });
            }
            return RedirectToAction("EditarSlides", "Config", new { equipe = equipe });
        }

        #endregion

        #region Manipula Arquivo Configuração

        private Configuracao LeConfiguracao(string equipe)
        {


            Configuracao config = new Configuracao();

            ViewBag.equipe = equipe;
            ViewBag.logo_equipe = $"/Dados/{equipe}/arquivos/logo.png";
            ViewBag.bannerEquipe = $"/Imagens/banner.png";
            ViewBag.mensagemSucesso = TempData["mensagemSucesso"] != null ? TempData["mensagemSucesso"].ToString() : String.Empty;

            if (!string.IsNullOrEmpty(equipe))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(Server.MapPath($"~/Dados/{equipe}/config-{equipe}.json"), encoding))
                    {
                        string conteudo = sr.ReadToEnd();

                        if (!string.IsNullOrEmpty(equipe))
                        {
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
            }


            return config;
        }

        private void SalvarConfiguracao(string equipe, Configuracao configToSave, bool criacao = false)
        {
            string caminhoArquivoConfig = Path.Combine(Server.MapPath($"~/Dados/{equipe}"), $"config-{equipe}.json");

            using (StreamWriter configWriter = new StreamWriter(System.IO.File.Open(caminhoArquivoConfig, FileMode.Create), encoding))
            {
                List<Configuracao> configuracoes = new List<Configuracao>();
                configuracoes.Add(configToSave);

                var result = JsonConvert.SerializeObject(configuracoes, Formatting.Indented, settings);

                configWriter.WriteLine(result);
                if (!criacao)
                {
                    TempData["mensagemSucesso"] = "Configuração atualizada com sucesso";
                }
            }

        }

        #endregion

        #region Manipula Arquivo Slides

        private List<Slide> LeConfiguracaoSlides(string equipe)
        {
            List<Slide> slides = new List<Slide>();

            ViewBag.equipe = equipe;
            ViewBag.logo_equipe = $"/Dados/{equipe}/arquivos/logo.png";
            ViewBag.bannerEquipe = $"/Imagens/banner.png";
            ViewBag.mensagemSucesso = TempData["mensagemSucesso"] != null ? TempData["mensagemSucesso"].ToString() : String.Empty;

            if (!string.IsNullOrEmpty(equipe))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(Server.MapPath($"~/Dados/{equipe}/slides-{equipe}.json"), encoding))
                    {
                        string conteudo = sr.ReadToEnd();

                        if (!string.IsNullOrEmpty(equipe))
                        {
                            slides = JsonConvert.DeserializeObject<List<Slide>>(conteudo, settings);
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
            }
            return slides;
        }

        private void SalvarConfiguracaoSlides(string equipe, List<Slide> slidesToSave, bool criacao = false)
        {
            string caminhoArquivoConfig = Path.Combine(Server.MapPath($"~/Dados/{equipe}"), $"slides-{equipe}.json");

            using (StreamWriter configWriter = new StreamWriter(System.IO.File.Open(caminhoArquivoConfig, FileMode.Create), encoding))
            {
                var result = JsonConvert.SerializeObject(slidesToSave, Formatting.Indented, settings);

                configWriter.WriteLine(result);
                if (!criacao)
                {
                    TempData["mensagemSucesso"] = "Configuração atualizada com sucesso";
                }
            }
        }


        #endregion
    }
}