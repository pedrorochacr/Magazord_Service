using Newtonsoft.Json;
using Magazord_Service.configuracao;
using Magazord_Service.controle;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System;

public class EstoqueRobot
{
    private readonly HttpClient httpClient;

    public EstoqueRobot()
    {
        httpClient = new HttpClient();
    }

    public async Task<bool> MovimentarEstoque(EstoqueData data)
    {
        var apiUrl = Settings.LoadSettings().ApiUrl;
        var USERNAME = Settings.LoadSettings().ApiUserName;
        var PASSWORD = Settings.LoadSettings().ApiPassword;
        Console.WriteLine(apiUrl);
        var jsonData = JsonConvert.SerializeObject(data);
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        // Configurar autenticação básica na requisição
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{USERNAME}:{PASSWORD}")));

        var request = new HttpRequestMessage(HttpMethod.Post, apiUrl + "api/v1/estoque");
        request.Headers.Add("Accept", "application/json");


        request.Content = new StringContent(
            Newtonsoft.Json.JsonConvert.SerializeObject(data),
            System.Text.Encoding.UTF8,
            "application/json"
        );

        var response = await httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            // Processar a resposta aqui
            var responseContent = await response.Content.ReadAsStringAsync();
            GravarLog(data.produto, data.quantidade, responseContent, false, data.deposito);

            return true;

        }
        else
        {
            // Não foi bem-sucedido, trata o erro aqui
            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro na requisição: {response.StatusCode} - {response.ReasonPhrase}");
            Console.WriteLine($"Detalhes do erro: {errorMessage}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Gravar log de erro
            GravarLog(data.produto, data.quantidade, responseContent, true, data.deposito);

            return false;
        }

    }

    private void GravarLog(string produto, int quantidade, string responseContent, bool indicadorErro, int deposito)
    {
        var connectionString = $"Server={Settings.LoadSettings().Server};Database={Settings.LoadSettings().Database};User={Settings.LoadSettings().User};Password={Settings.LoadSettings().Password};";

        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                if (indicadorErro)
                {
                    // Executar o INSERT na tabela quando indicadorErro for verdadeiro
                    var comando = "INSERT INTO [dbo].SYNC_B2C_LOG (DATA_HORA, DESCRICAO, INDICA_ERRO, CLASSE) VALUES (@DataHora, @Descricao, @IndicadorErro, @Classe)";

                    using (var sqlCommand = new SqlCommand(comando, connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@DataHora", DateTime.Now);
                        sqlCommand.Parameters.AddWithValue("@Descricao", responseContent);
                        sqlCommand.Parameters.AddWithValue("@IndicadorErro", 1);
                        sqlCommand.Parameters.AddWithValue("@Classe", "ESTOQUE");

                        sqlCommand.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Executar a stored procedure quando indicadorErro for falso
                    var comando = "EXEC [dbo].[SP_SYNC_ESTOQUE] @Produto, @DEPOSITO, @estoque";

                    using (var sqlCommand = new SqlCommand(comando, connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@Produto", produto);
                        sqlCommand.Parameters.AddWithValue("@estoque", quantidade);
                        sqlCommand.Parameters.AddWithValue("@DEPOSITO", deposito);

                        sqlCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Trate o erro de acordo com a necessidade do seu projeto
            Console.WriteLine($"Erro ao gravar o log: {ex.Message}");
        }
    }
}