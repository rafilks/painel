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
    }    

    public class Base
    {
        public List<Slide> Slides { get; set; }
        public List<Configuracao> Configuracao { get; set; }
    }
}