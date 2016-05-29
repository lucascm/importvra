/**
  * Autor: Lucas
  * 
  * A classe Voo contém os campos do documento que será incluído no mongoDB.
  * Este objeto será utilizado para que o formato dos dados seja mantido conforme a documentação
  */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Voo
    {
        // Sigla  da Empresa,Numero do Voo,D I,Tipo de Linha,Aeroporto Origem,Aeroporto Destino,
        // Partida Prevista,Partida Real,Chegada Prevista,Chegada Real,Situacao,Justificativa

        public string Sigla_Empresa { get; set; }
        public int Numero_Voo { get; set; }
        public int D_I { get; set; }
        public string Tipo_Linha { get; set; }
        public string Aeroporto_Origem { get; set; }
        public string Aeroporto_Destino { get; set; }
        public DateTime  Partida_Prevista { get; set; }
        public DateTime Partida_Real { get; set; }
        public DateTime Chegada_Prevista { get; set; }
        public DateTime Chegada_Real { get; set; }
        public string Situacao { get; set; }
        public string Justificativa { get; set; }

        public static Voo Pegar_Nova_Instancia(
            string p_sigla_empresa, int p_numero_voo, int p_d_i, 
            string p_tipo_linha, string p_aeroporto_origem, string p_aeroporto_destino, 
            DateTime p_partida_prevista, DateTime p_partida_real,
            DateTime p_chegada_prevista, DateTime p_chegada_real,
            string p_situacao, string p_justificativa)
        {
            Voo ret = new Voo();
            ret.Sigla_Empresa = p_sigla_empresa;
            ret.Numero_Voo = p_numero_voo;
            ret.D_I = p_d_i;
            ret.Tipo_Linha = p_tipo_linha;
            ret.Aeroporto_Origem = p_aeroporto_origem;
            ret.Aeroporto_Destino = p_aeroporto_destino;
            ret.Partida_Prevista = p_partida_prevista;
            ret.Partida_Real = p_partida_real;
            ret.Chegada_Prevista = p_chegada_prevista;
            ret.Chegada_Real = p_chegada_real;
            ret.Situacao = p_situacao;
            ret.Justificativa = p_justificativa;
            return ret;
        }


    }
}
