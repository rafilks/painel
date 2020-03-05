using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Itau.Models
{
    public class Equipe
    {
        [Required(ErrorMessage = "Valor do Nome é requerido")]
        public string Nome { get; set; }
    }
}