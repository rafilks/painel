using Itau.Common;
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
    public class PainelController : Controller
    {
        public ActionResult Index(string equipe)
        {
            string mensagemErro = string.Empty;
            List<Slide> slides = new List<Slide>();

            if (!string.IsNullOrEmpty(equipe))
            {
                ViewBag.equipe = equipe;
                ViewBag.bannerEquipe = $"/Dados/{equipe}/Imagens/banner.png";
                ViewBag.inicioSemana = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                ViewBag.fimSemana = DateTime.Now.StartOfWeek(DayOfWeek.Monday).AddDays(7);

                try
                {
                    using (StreamReader sr = new StreamReader(Server.MapPath($"~/Dados/{equipe}/slides-{equipe}.json"), Encoding.GetEncoding("ISO-8859-1")))
                    {
                        string conteudo = sr.ReadToEnd();

                        if (!string.IsNullOrEmpty(equipe))
                        {
                            var settings = new JsonSerializerSettings { DateFormatString = "dd-MM-yyyy" };
                            slides = JsonConvert.DeserializeObject<List<Slide>>(conteudo, settings);

                            //não necessário visto que agora os arquivos foram separados
                            ////Filtra os slides do json baseados na query string apresentada
                            //slides = slides.Where(x => x.Equipe.Equals(equipe)).ToList();

                            //Percorre todos os slides retornados
                            for (int i = 0; i < slides.Count; i++)
                            {
                                //Verifica se existe slide do tipo plantão
                                if (slides[i].Tipo == TipoSlide.GestaoVisual)
                                {
                                    //Percorre o conjunto de dados do plantão
                                    for (int ii = 0; ii < slides[i].DadosPlantao.Count; ii++)
                                    {
                                        //Verifica se o funcionário está em plantão
                                        if (slides[i].DadosPlantao[ii].DataInicio <= DateTime.Now &&
                                            slides[i].DadosPlantao[ii].DataFim >= DateTime.Now)
                                        {
                                            slides[i].DadosPlantao[ii].PlantaoHoje = true;
                                        }
                                        else
                                        {
                                            slides[i].DadosPlantao[ii].PlantaoHoje = false;
                                        }
                                    }
                                }
                            }
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
            return View(slides);
        }
    }
}