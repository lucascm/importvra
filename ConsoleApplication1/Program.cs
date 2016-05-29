/**
  * Autor: Lucas
  * 
  * Esta aplicação será responsável para importação dos dados VRA para o mongoDB
  * 
  * O sistema faz a leitura dos arquivos .csv colhidos no site da ANAC (http://www2.anac.gov.br/vra/basehistorica.asp)
  * e carrega um objeto do tipo Voo para cada linha dos arquivos.
  * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ConsoleApplication1
{
    class Person
    {
        public ObjectId id { get; set; }
        public string nome { get; set; }

    }


    class Program
    {
        private static MongoClient Get_MongoClinte(bool withReplicaSet)
        {
            //Console.WriteLine(withReplicaSet.ToString());
            if (withReplicaSet)
            {
                /**
                  * Se for replica set a aplicação deverá conhecer os caminhos de todos os servidores
                  * visto que se o servidor principal sair do ar, será necessário conhecer seu
                  * substituto
                  */
                Console.WriteLine("Iniciando conexao para Replicaset");
                MongoClientSettings mc = new MongoClientSettings();
                List<MongoServerAddress> _servers = new List<MongoServerAddress>();
                
                // Ver no app.config os endereços configurados. Se necessário, estes podem ser modificdos.
                _servers.Add(new MongoServerAddress("localhost", Config.Portas_Replica_Set()[0]));
                _servers.Add(new MongoServerAddress("localhost", Config.Portas_Replica_Set()[0]));
                _servers.Add(new MongoServerAddress("localhost", Config.Portas_Replica_Set()[0]));

                mc.ConnectionMode = ConnectionMode.ReplicaSet;
                mc.ReplicaSetName = "TgFatec";
                mc.Servers = _servers;
                return new MongoClient(mc);
            }
            else
            {
                // No formato master/slave só será necessário conhecer o servidor principal.
                Console.WriteLine("Iniciando conexao para Master/Slave");
                return new MongoClient(string.Concat("mongodb://localhost:", Config.Porta_Servidor_Master().ToString())); 
            }
        }
        private static void Carregar_Arquivos (MongoClient  p_m_cliente )
        {

            string dir_arquivos = Config.Diretorio_Arquivos(); // o caminho do diretório está configurado no app.config

            int pula_linha = 1;     // variável criada para evitar incluir o cabeçalho como registro
            int contador = 0;       // Controla o tamanho da lista. Quando chegar em 100.000 o sistema vai incluir a lista no banco
            int qtde_registros = 0; // guarda a qtde de registros incluídos no banco de dados.

            Console.WriteLine("Iniciando conexão com o banco");
            /**
             *   Conexão com o mongoDB    
             */

            var m_cliente = p_m_cliente; //new MongoDB.Driver.MongoClient("mongodb://localhost:27017");
            var banco = m_cliente.GetDatabase("anac_1");
            Console.WriteLine("Eliminando collection anterior...");
            banco.DropCollection("voos");
            var table = banco.GetCollection<Voo>("voos");
            /**
             * Lista de voos
             * O programa vai carregar os dados na lista e depois descarregar no banco
             * Como a importação será realizada no notebook, então temos memória limitada
             * por isso a cada 100000 registros, o aplicativo vai descarregar a memória
             */
            List<Voo> Voos = new List<Voo>();

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("procurando arquivos...");
            Console.WriteLine("");
            /**
             * Laço para ler os arquivos
             *   A cada iteração o nome do arquivo será colocado na variável s
             */
            foreach (var s in System.IO.Directory.GetFiles(dir_arquivos , "*.csv")) {
            
                // Console.WriteLine(string.Concat("Lendo ", s));
                /**
                 * executar a leitura linha a linha
                 */
                foreach (var l in System.IO.File.ReadLines(s)    )
                {
                    if (contador == 100000)                    
                        {
                        /**
                         * a cada 100.000 registros o sistema vai incluir os registros e descarregar a memória
                         */
                        Console.Write(".");
                        // Console.WriteLine("Inserindo dados a cada 100000 registros");

                        table.InsertMany(Voos);
                        contador = 0;
                        Voos.Clear();
                    }
                    if (pula_linha == 1) 
                    {
                        /**
                          * Esta checagem foi adicionada para saltar o cabeçalho.
                         */
                        pula_linha = 0;
                    }
                    else
                    {
                        string[] campos = l.Replace("\";\"",";").Replace("; ", ";").Replace("\"", "").Split(';');
                        if (campos.Length == 12)
                        {

                            Int32 n_voo, b_i;
                            DateTime partida_prevista, partida_real, chegada_prevista, chegada_real;

                            Int32.TryParse(campos[1], out n_voo);
                            Int32.TryParse(campos[2], out b_i);
                            DateTime.TryParse(campos[6], out partida_prevista);
                            DateTime.TryParse(campos[7], out partida_real);
                            DateTime.TryParse(campos[8], out chegada_prevista);
                            DateTime.TryParse(campos[9], out chegada_real);

                            Voos.Add (Voo.Pegar_Nova_Instancia(campos[0], n_voo,
                                b_i, campos[3], campos[4], campos[5],
                                partida_prevista, partida_real, chegada_prevista, chegada_real,
                                campos[10], campos[11]));

                            contador++;
                            qtde_registros++;

                        }
                    }

                }
                pula_linha = 1; // indica que vai saltar o cabeçalho do próximo arquivo
            }
            if (Voos.Count > 0) // se o sistema carregou menos de 100.000 registros, então vai passar por aqui
            {
                Console.Write(".");
                // Console.WriteLine(string.Concat ("Incluíndo ", Voos.Count , " registros restantes..."));
                table.InsertMany(Voos);
            }
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(string.Concat("Foram inseridos ", qtde_registros , " registro" ));
        }

        static void Main(string[] args)
        {
            bool with_Replica_Set = false;
            if (args.Length > 0) 
            {
                /**
                  * se for passado algum parâmetro e este for "replicacao" então o sistema vai assumir o replica set
                */
                with_Replica_Set = (args[0].ToLower() == "replicacao");
                //Console.WriteLine(args[0].ToLower());
            }


            DateTime inicio = DateTime.Now;

            Console.WriteLine(string.Concat("Iniciando trabalho... [", inicio.ToString(),"]"));

            Carregar_Arquivos(Get_MongoClinte(with_Replica_Set ));

            DateTime fim = DateTime.Now;

            Console.WriteLine("");
            Console.WriteLine(string.Concat("Processo Finalizado! [",DateTime.Now.ToString (),"]"));
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(string.Concat("Iniciou em: ", inicio.ToString()));
            Console.WriteLine(string.Concat("Finalizou em: ", fim.ToString ()));
            Console.WriteLine("");

            

            // **** Os comandos abaixo foram usados para testes. Deixei aqui para ver alguns detalhes no futuro.

            //AZU,6963,0,N,SBFZ,SBKP,16/05/2015 02:23,16/05/2015 02:07,16/05/2015 05:48,16/05/2015 05:43,Realizado,HD
            /*Voo v = Voo.Pegar_Nova_Instancia("AZU", 6963, 0, "N", "SBFZ", "SBKP",
                Convert.ToDateTime ("16/05/2015 02:23"), Convert.ToDateTime("16/05/2015 02:07"),
                Convert.ToDateTime("16/05/2015 05:48"), Convert.ToDateTime("16/05/2015 05:43"),
                "Realizado", "HD");
            var m_cliente = new MongoDB.Driver.MongoClient("mongodb://192.168.1.104:27017");
            var banco = m_cliente.GetDatabase("anac");
            var table = banco.GetCollection<Voo>("voos");
            table.InsertOne(v);*/

            /*  var cliente = new MongoDB.Driver.MongoClient("mongodb://192.168.1.9:27017");
              var banco = cliente.GetDatabase("cadastro");

              var client = new MongoClient("mongodb://192.168.1.9:27017");
              var database = client.GetDatabase("foo");
              var collection = database.GetCollection<BsonDocument>("bar");

              collection.InsertOne(new BsonDocument("Name", "Black Jack"));

              var list = collection.Find(new BsonDocument("Name", "Black Jack")).ToList();
              Console.WriteLine("Entrando no laço");
              Console.Read();

              foreach (var document in list)
              {
                  Console.WriteLine(document["Name"]);
              }

              var pessoas = database.GetCollection<Person>("pessoas");
              pessoas.InsertOne(new Person { nome = "Lucas" });

              var lista = pessoas.Find(x => x.nome == "Lucas").ToList();
              foreach (var item in lista)
              {
                  Console.WriteLine("Nome: " + item.nome);

              }
              Console.WriteLine(pessoas.ToString());*/
        }
    }
}
