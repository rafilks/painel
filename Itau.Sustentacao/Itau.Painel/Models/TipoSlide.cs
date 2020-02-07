using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Itau.Models
{
    public enum TipoSlide
    {
        [Description("Imagem")]
        Imagem = 1,

        [Description("Vídeo")]
        Video = 2,

        [Description("Literal")]
        Literal = 3,

        [Description("Gestão Visual")]
        GestaoVisual = 4,

        [Description("Embed")]
        Embed = 5
    }
}