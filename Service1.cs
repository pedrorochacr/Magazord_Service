using Magazord_Service.configuracao;
using Magazord_Service.controle;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Magazord_Service
{
    public partial class Service1 : ServiceBase
    {
        public static Timer _timer { get; private set; }
        public Service1()
        {
            InitializeComponent();
        }

        protected override  void OnStart(string[] args)
        {
            
             _timer = new Timer();
            // In miliseconds 60000 = 1 minute
            // This timer will tick every 1 minute
            _timer.Interval += int.Parse(Settings.LoadSettings().DelayEstoque);
            // Activate the timer
            _timer.Enabled = true;
            // When timer "tick"
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
        

          

            EventLog.WriteEntry("Serviço iniciado!");
           

        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("Serviço Finalizado!");
            _timer.Enabled = false;
        }

        

        private async void _timer_Elapsed(object sender, EventArgs e)
        {

            EventLog.WriteEntry("Serviço executado!");
            List<EstoqueData> estoqueList = EstoqueData.PopularEstoque();
            var robot = new EstoqueRobot();

            foreach (EstoqueData estoque in estoqueList)
            {
                var result = await robot.MovimentarEstoque(estoque);

                if (result)
                {
                    Console.WriteLine("Estoque atualizado com sucesso!");
                }
                else
                {
                    Console.WriteLine("Falha ao atualizar o estoque.");
                }
            }


        }
    }
}
