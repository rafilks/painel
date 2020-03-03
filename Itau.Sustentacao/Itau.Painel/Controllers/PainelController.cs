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
            List<Configuracao> config = new List<Configuracao>();
            List<string> equipes = new List<string>();
            Base basedados = new Base();

            if (!string.IsNullOrEmpty(equipe))
            {
                ViewBag.equipe = equipe;
                ViewBag.logo_equipe = $"/Dados/{equipe}/arquivos/logo.png";
                ViewBag.bannerEquipe = $"/Imagens/banner.png";
                ViewBag.inicioSemana = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                ViewBag.fimSemana = DateTime.Now.StartOfWeek(DayOfWeek.Monday).AddDays(7);

                #region Lê os slides

                try
                {
                    using (StreamReader sr = new StreamReader(Server.MapPath($"~/Dados/{equipe}/slides-{equipe}.json"), Encoding.GetEncoding("ISO-8859-1")))
                    {
                        string conteudo = sr.ReadToEnd();

                        if (!string.IsNullOrEmpty(equipe))
                        {
                            var settings = new JsonSerializerSettings { DateFormatString = "dd-MM-yyyy" };
                            slides = JsonConvert.DeserializeObject<List<Slide>>(conteudo, settings);
                            basedados.Slides = slides;
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

                #region Lê a configuração

                try
                {
                    using (StreamReader sr = new StreamReader(Server.MapPath($"~/Dados/{equipe}/config-{equipe}.json"), Encoding.GetEncoding("ISO-8859-1")))
                    {
                        string conteudo = sr.ReadToEnd();

                        if (!string.IsNullOrEmpty(equipe))
                        {
                            var settings = new JsonSerializerSettings { DateFormatString = "dd-MM-yyyy" };
                            config = JsonConvert.DeserializeObject<List<Configuracao>>(conteudo, settings);
                            basedados.Configuracao = config;
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

                AjustaDadosCarregados(basedados);
                ViewBag.proposito = basedados.Configuracao[0].Proposito;
            }
            else
            {
                var listaDiretorio = Directory.GetDirectories(Server.MapPath($"~/Dados/")).ToList();
                foreach (var item in listaDiretorio)
                {
                    equipes.Add(Path.GetFileName(item));
                }
                ViewBag.equipes = equipes;                         
            }

            return View(basedados);
        }

        public void AjustaDadosCarregados(Base basedados)
        {
            //Exclui os plantões fora de data
            foreach (var p in basedados.Configuracao[0].DadosPlantao)
            {
                //Verifica se o plantão é válido
                p.Valido = (p.DataInicio <= DateTime.Now && p.DataFim >= DateTime.Now);
            }

            //Exclui as ausências passadas
            foreach (var a in basedados.Configuracao[0].AusenciaProgramada)
            {
                a.Valido = (a.Data >= DateTime.Now);
            }

            //Caso o colaborador não tenha imagem, mostra uma genérica
            foreach (var c in basedados.Configuracao[0].NossoTime)
            {
                if (!System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + c.UrlImagem))
                {
                    c.UrlImagem = "../../Imagens/genericUser.png";
                }
            }

            //trata o gráfico de próximas férias
            int iNumFerias = 0;

            if (basedados.Configuracao[0].Ferias != null)
            {
                basedados.Configuracao[0].Ferias = basedados.Configuracao[0].Ferias.OrderBy(x => x.DataInicio).ThenByDescending(x => x.DataFim).ThenBy(x => x.Nome).Where(x => x.DataFim >= DateTime.Now.Date).ToList();
                iNumFerias = basedados.Configuracao[0].Ferias.Count;
            }
            else
            {
                basedados.Configuracao[0].Ferias = new List<Ferias>();
            }

            //Acrescenta as linhas que falta para 10 registros
            for (int j = iNumFerias; j < 10; j++)
            {
                Ferias f = new Ferias();
                f.Nome = "";
                f.Tipo = "";
                //f.DataDisplay = "";
                basedados.Configuracao[0].Ferias.Add(f);
            }

            //Trata as férias para mostrar no gráfico
            foreach (Ferias f in basedados.Configuracao[0].Ferias)
            {
                f.Meses = new List<MesFerias>();

                if (!String.IsNullOrEmpty(f.Nome))
                {
                    //f.DataDisplay = String.Concat(f.DataInicio.ToString("dd/MM"), "-", f.DataFim.ToString("dd/MM"));

                    for (int m = 0; m <= 2; m++)
                    {
                        f.Meses.Add(new MesFerias());
                        f.Meses[m].espaco1 = DateTime.DaysInMonth(DateTime.Now.AddMonths(m).Year, DateTime.Now.AddMonths(m).Month);

                        if (f.DataInicio.Month < DateTime.Now.AddMonths(m).Month)
                        {
                            if (f.DataFim.Month == DateTime.Now.AddMonths(m).Month)
                            {
                                f.Meses[m].espaco1 = 0;
                                f.Meses[m].espaco2 = f.DataFim.Day;
                                f.Meses[m].espaco3 = DateTime.DaysInMonth(DateTime.Now.AddMonths(m).Year, DateTime.Now.AddMonths(m).Month) - (f.Meses[m].espaco1 + f.Meses[m].espaco2);
                            }

                            if (f.DataFim.Month > DateTime.Now.AddMonths(m).Month)
                            {
                                f.Meses[m].espaco1 = 0;
                                f.Meses[m].espaco2 = DateTime.DaysInMonth(DateTime.Now.AddMonths(m).Year, DateTime.Now.AddMonths(m).Month);
                            }
                        }
                        else
                        {
                            if (f.DataInicio.Month == DateTime.Now.AddMonths(m).Month)
                            {
                                f.Meses[m].espaco1 = f.DataInicio.Day - 1;

                                if (f.DataFim.Month == DateTime.Now.AddMonths(m).Month)
                                {
                                    f.Meses[m].espaco2 = f.DataFim.Day - f.DataInicio.Day + 1;
                                    f.Meses[m].espaco3 = DateTime.DaysInMonth(f.DataInicio.Year, f.DataInicio.Month) - (f.Meses[m].espaco1 + f.Meses[m].espaco2);
                                }
                                else
                                {
                                    f.Meses[m].espaco2 = DateTime.DaysInMonth(f.DataInicio.Year, f.DataInicio.Month) - f.DataInicio.Day + 1;
                                    f.Meses[m].espaco3 = 0;
                                }
                            }
                        }

                    }

                    //Ajusta data que será exibida
                    //f.DataDisplay = String.Concat(f.DataInicio.ToString("dd/MM"), "-", f.DataFim.ToString("dd/MM"));
                    //f.mes1e1 = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                    //f.mes1e2 = 0;
                    //f.mes1e3 = 0;
                    //f.mes2e1 = DateTime.DaysInMonth(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month);
                    //f.mes2e2 = 0;
                    //f.mes2e3 = 0;
                    //f.mes3e1 = DateTime.DaysInMonth(DateTime.Now.AddMonths(2).Year, DateTime.Now.AddMonths(2).Month);
                    //f.mes3e2 = 0;
                    //f.mes3e3 = 0;



                    ////Ajusta o primeiro mês
                    //if (f.DataInicio.Month < DateTime.Now.Month)
                    //{
                    //    f.mes1e1 = 0;

                    //    if (f.DataFim.Month == DateTime.Now.Month)
                    //    {
                    //        f.mes1e2 = f.DataFim.Day;
                    //        f.mes1e3 = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - (f.mes1e1 + f.mes1e2);
                    //    }
                    //    else
                    //    {
                    //        f.mes1e2 = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                    //    }
                    //}
                    //else
                    //{ 
                    //    if (f.DataInicio.Month == DateTime.Now.Month)
                    //    {
                    //        f.mes1e1 = f.DataInicio.Day - 1;

                    //        if (f.DataFim.Month == DateTime.Now.Month)
                    //        {
                    //            f.mes1e2 = f.DataFim.Day - f.DataInicio.Day + 1;
                    //            f.mes1e3 = DateTime.DaysInMonth(f.DataInicio.Year, f.DataInicio.Month) - (f.mes1e1 + f.mes1e2);
                    //        }
                    //        else
                    //        {
                    //            f.mes1e2 = DateTime.DaysInMonth(f.DataInicio.Year, f.DataInicio.Month) - f.DataInicio.Day + 1;
                    //            f.mes1e3 = 0;
                    //        }
                    //    }
                    //}

                    ////Ajusta o segundo mês
                    //if (f.DataInicio.Month < DateTime.Now.AddMonths(1).Month)
                    //{
                    //    if (f.DataFim.Month == DateTime.Now.AddMonths(1).Month)
                    //    {
                    //        f.mes2e1 = 0;
                    //        f.mes2e2 = f.DataFim.Day;
                    //        f.mes2e3 = DateTime.DaysInMonth(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month) - (f.mes2e1 + f.mes2e2);
                    //    }
                    //}
                    //else
                    //{
                    //    if (f.DataInicio.Month == DateTime.Now.AddMonths(1).Month)
                    //    {
                    //        f.mes2e1 = f.DataInicio.Day - 1;

                    //        if (f.DataFim.Month == DateTime.Now.AddMonths(1).Month)
                    //        {
                    //            f.mes2e2 = f.DataFim.Day - f.DataInicio.Day + 1;
                    //            f.mes2e3 = DateTime.DaysInMonth(f.DataInicio.Year, f.DataInicio.Month) - (f.mes2e1 + f.mes2e2);
                    //        }
                    //        else
                    //        {
                    //            f.mes2e2 = DateTime.DaysInMonth(f.DataInicio.Year, f.DataInicio.Month) - f.DataInicio.Day + 1;
                    //            f.mes2e3 = 0;
                    //        }
                    //    }
                    //}

                    ////Ajusta o terceiro mês
                    //if (f.DataInicio.Month < DateTime.Now.AddMonths(2).Month)
                    //{
                    //    if (f.DataFim.Month == DateTime.Now.AddMonths(2).Month)
                    //    {
                    //        f.mes3e1 = 0;
                    //        f.mes3e2 = f.DataFim.Day;
                    //        f.mes3e3 = DateTime.DaysInMonth(DateTime.Now.AddMonths(2).Year, DateTime.Now.AddMonths(2).Month) - (f.mes3e1 + f.mes3e2);
                    //    }
                    //}
                    //else
                    //{
                    //    if (f.DataInicio.Month == DateTime.Now.AddMonths(2).Month)
                    //    {
                    //        f.mes3e1 = f.DataInicio.Day - 1;

                    //        if (f.DataFim.Month == DateTime.Now.AddMonths(2).Month)
                    //        {
                    //            f.mes3e2 = f.DataFim.Day - f.DataInicio.Day + 1;
                    //            f.mes3e3 = DateTime.DaysInMonth(f.DataInicio.Year, f.DataInicio.Month) - (f.mes3e1 + f.mes3e2);
                    //        }
                    //        else
                    //        {
                    //            f.mes3e2 = DateTime.DaysInMonth(f.DataInicio.Year, f.DataInicio.Month) - f.DataInicio.Day + 1;
                    //            f.mes3e3 = 0;
                    //        }
                    //    }
                    //}
                }
            }
        }       
    }
}
