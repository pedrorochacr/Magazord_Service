using Magazord_Service.configuracao;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazord_Service.controle
{
    public class EstoqueData
    {
        public string produto { get; set; }
        public int deposito { get; set; }
        public int quantidade { get; set; }
        public int tipo { get; set; }
        public int tipoOperacao { get; set; }
        public string observacao { get; set; }
        public string dataHoraPrevisto { get; set; }
        public string dataHoraMovimento { get; set; }

        public static List<EstoqueData> PopularEstoque()
        {
            string query = "SELECT produto, deposito, quantidade, tipo, tipoOperacao, observacao, dataHoraPrevisto, dataHoraMovimento FROM dbo.v_magazord_estoque";
            var connectionString = $"Server={Settings.LoadSettings().Server};Database={Settings.LoadSettings().Database};User={Settings.LoadSettings().User};Password={Settings.LoadSettings().Password};";
            List<EstoqueData> estoqueDataList = new List<EstoqueData>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Ler os valores de cada coluna da view
                    string produto = reader["produto"].ToString();
                    int deposito = (int)reader["deposito"];
                    int quantidade = (int)reader["quantidade"];
                    int tipo = (int)reader["tipo"];
                    int tipoOperacao = (int)reader["tipoOperacao"];
                    string observacao = reader["observacao"].ToString();
                    DateTime dataHoraPrevisto = (DateTime)reader["dataHoraPrevisto"];
                    DateTime dataHoraMovimento = (DateTime)reader["dataHoraMovimento"];

                    // Criar o objeto EstoqueData populado
                    EstoqueData estoqueData = new EstoqueData
                    {
                        produto = produto,
                        deposito = deposito,
                        quantidade = quantidade,
                        tipo = tipo,
                        tipoOperacao = tipoOperacao,
                        observacao = observacao,
                        dataHoraPrevisto = dataHoraPrevisto.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        dataHoraMovimento = dataHoraMovimento.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    };

                    estoqueDataList.Add(estoqueData);
                }

                reader.Close();
            }

            return estoqueDataList;
        }


    }
}
