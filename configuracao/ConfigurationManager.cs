
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazord_Service.configuracao
{
    public class Settings
    {
        public string Server { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public bool IntegratedSecurity { get; set; }
        public bool ModuloEstoque { get; set; }
        public string DelayEstoque { get; set; }
        public string ApiUrl { get; set; }
        public string ApiUserName { get; set; }
        public string ApiPassword { get; set; }

        public static Settings LoadSettings()
        {
            string diretorioExecucao = AppDomain.CurrentDomain.BaseDirectory;
            string settingsFile = Path.Combine(diretorioExecucao, "settings.json");
            

            if (File.Exists(settingsFile))
            {
                var json = File.ReadAllText(settingsFile);
                return JsonConvert.DeserializeObject<Settings>(json);
            }
            else
            {
                throw new FileNotFoundException("O arquivo de configuração settings.json não foi encontrado.");
            }
        }
    }
}
