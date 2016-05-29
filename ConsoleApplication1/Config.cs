/**
  * Autor: Lucas 
  *  
  * A classe Config foi criada apenas para carregar as configurações do app.config; é uma classe de apoio.
  *
*/
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Config
    {
        public static string Diretorio_Arquivos()
        {
            return ConfigurationSettings.AppSettings["Diretorio_Arquivos"].ToString();
        }
        public static int Porta_Servidor_Master()
        {
            return int.Parse(ConfigurationSettings.AppSettings["Porta_Servidor_Master"]);
        }
        public static List<int> Portas_Replica_Set()
        {
            List<int> retorno = new List<int>();
            foreach (var item in ConfigurationSettings.AppSettings["Portas_Serdores_Replica_Set"].ToString().Split(';'))
            {
                retorno.Add(int.Parse(item));
            }
            return retorno;
        }
    }
}
