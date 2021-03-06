﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Itau.Models
{

    public class Configuracao
    {
        public int Id { get; set; }
        public string NomeSquad { get; set; }
        public string Proposito { get; set; }        
        public List<Sigla> Siglas { get; set; }
        public List<NossoTime> NossoTime { get; set; }
        public List<AusenciaProgramada> AusenciaProgramada { get; set; }
        public List<DadosPlantao> DadosPlantao { get; set; }
        public List<Ferias> Ferias { get; set; }
        public List<Cerimonia> InfoCerimonias { get; set; }

        public Configuracao()
        {
            NomeSquad = string.Empty;
            Proposito = string.Empty;
            Siglas = new List<Sigla>();
            NossoTime = new List<NossoTime>();
            AusenciaProgramada = new List<AusenciaProgramada>();
            DadosPlantao = new List<DadosPlantao>();
            Ferias = new List<Ferias>();
            InfoCerimonias = new List<Cerimonia>();
        }
    }
    public class DadosPlantao
    {
        public int Id { get; set; }
        public string Funcionario { get; set; }
        public bool Valido { get; set; }
        public string Plataforma { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
    public class NossoTime
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Principal { get; set; }
        public string Papel { get; set; }
        public TipoPapel TipoPapel { get; set; }
        public string UrlImagem { get; set; }
        public string Ausencia { get; set; }
    }
    public class AusenciaProgramada
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime Data { get; set; }
        public string Motivo { get; set; }
        public bool Valido { get; set; }
    }
    public class MesFerias
    {
        public int espaco1 { get; set; }
        public int espaco2 { get; set; }
        public int espaco3 { get; set; }
    }
    public class Ferias
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string DataDisplay
        {
            get
            {

                if (DataInicio == DateTime.MinValue || DataFim == DateTime.MinValue)
                {
                    return "";
                }
                else
                {
                    return DataInicio.ToString("dd/MM") + "-" + DataFim.ToString("dd/MM");
                }
            }
        }
        public List<MesFerias> Meses { get; set; }
    }
    public class Cerimonia
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string Data { get; set; }
        public string Hora { get; set; }
    }
    public class Sigla
    {
        public int Id { get; set; }
        public string CodSigla { get; set; }
        public TipoSigla Tipo { get; set; }
    }
}