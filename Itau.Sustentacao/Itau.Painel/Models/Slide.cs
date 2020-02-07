using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Itau.Models
{
    public class Slide
    {
        public int Id { get; set; }
        public string Equipe { get; set; }
        public string Nome { get; set; }
        public TipoSlide Tipo { get; set; }      
        public int Tempo { get; set; }
        public string Conteudo { get; set; }
        public List<DadosPlantao> DadosPlantao { get; set; }
        public List<NossoTime> NossoTime { get; set; }
        public List<AusenciaProgramada> AusenciaProgramada { get; set; }
    }

    public class DadosPlantao
    {
        public string Funcionario { get; set; }
        public bool PlantaoHoje { get; set; }
        public string Plataforma { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
    public class NossoTime
    {
        public string UrlImagem { get; set; }        
        public string Ausencia { get; set; }
    }
    public class AusenciaProgramada
    {
        public string Nome { get; set; }
        public DateTime Data { get; set; }
        public string Motivo { get; set; }
    }
}