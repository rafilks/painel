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

    public enum TipoPapel
    {
        [Description("Product Owner")]
        PO = 1,
        [Description("Team Lead")]
        TL = 2,
        [Description("Tech Lead")]
        Tcl = 3,
        [Description("Team Member")]
        Tm = 4
    }
    
    public enum TipoSigla
    {
        [Description("Ambos")]
        ambos = 1,
        [Description("Mainframe")]
        mainframe = 2,
        [Description("Distribuída")]        
        distribuida = 3,
        [Description("Outros")]
        outros = 4
    }
}