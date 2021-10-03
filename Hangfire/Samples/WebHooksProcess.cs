using Authorization;
using DAL;
using Entities;
using Helpers;
using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{    
    public class WebHooksProcess : IProcessAsync
    {
        protected ConfigSingleton Config { get; set; }
        protected ILogger Logger { get; set; }
        protected IBrokerServicesBLL BLL { get; set; }

        public bool IsRunning { protected set; get; }
        
        public WebHooksProcess() : this(
            new BrokerServicesBLL(),
            ConfigSingleton.Instance,
            LoggerSingleton.Instance
        )
        { }

        public WebHooksProcess(IBrokerServicesBLL bll, ConfigSingleton config, ILogger logger)
        {
            this.BLL = bll;
            this.Config = config;            
            this.Logger = logger;
        }

        public void Run()
        {            
            this.RunAsync().Wait(/*1000*60*2*/);
        }

        public async Task RunAsync()
        {
            if (this.Config.BGServiceOnOff != "ON") return;
            if (this.IsRunning) return;

            try {
                this.IsRunning = true;
                
                var brokers = this.BLL.GetBrokers();
                if (brokers == null || brokers.Count() == 0) return;
                Parallel.ForEach(brokers, HandleBroker);
            }
            catch (Exception ex) {
                ex.Data.Add("Method", "WebHooksProcess.RunAsync");
                this.Logger.Error("WebHooksProcess", ex);
            }
            finally {
                this.IsRunning = false;
            }
        }

        public void Run(BrokerData brokerData)
        {
            this.HandleBroker(brokerData);
        }

        private void HandleBroker(BrokerData brokerData)
        {
            var brokerHooks = this.BLL.GetBrokerWebHooks(brokerData.Name);
            if (brokerHooks == null || brokerHooks.Count() == 0) return;

            brokerHooks.ToList().ForEach(hookData => {
                HandleBrokerHook(brokerData, hookData);
            });

            /*
            var tasks = brokerHooks.Select(async hookData => 
            {
                await HandleBrokerHook(brokerData, hookData);
            });
            await Task.WhenAll(tasks);
            */
        }
        
        private void HandleBrokerHook(BrokerData brokerData, WebHookData hookData) {
            var step = 0;
            try
            {
                step = 1;

                var httpService = new HttpServiceHelper();
                var config = new DataUpdatesConfig {
                    FromTime = hookData.LastExecutionTime
                };

                var serviceData = this.BLL.GetServiceData(hookData.ServiceName, brokerData);
                if(serviceData == null)
                    throw new Exception($"No Service Data: {hookData.ServiceName}");

                step = 2;

                var proxyURL = this.Config.ProxyServerURI; // "http://proxy.crtv.co.il";
                var proxyPort = serviceData.Port;

                var generator = new JWTGenerator(this.Config.JWTSecretKey);
                var token = generator.GenerateToken(new {
                    brokerName = brokerData.Name,
                    role = "Broker"
                });
                
                step = 3;
                                
                var updatesURI = $"{proxyURL}:{proxyPort}/{hookData.ServiceName}/updates";                
                var serviceResult = httpService.POST(updatesURI, config, headers: new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Bearer {token}"
                });
                
                step = 4;

                // TODO ->> StatusCode
                if (!serviceResult.Success) {
                    // remove html content
                    if (serviceResult.Content.Contains("<!DOCTYPE"))
                        serviceResult.Content = serviceResult.Content.Split('<').FirstOrDefault();

                    throw new Exception($"Get Updates from {updatesURI} -> {serviceResult.Content}");
                }

                var updates = serviceResult.Content;
                var hasUpdates = !string.IsNullOrEmpty(updates) && updates != "[]";
                
                step = 5;

                // push updates
                if (hasUpdates) {
                    this.Logger.Info("WebHooksProcess", $"#{hookData.Id}, POST {hookData.ServiceName} Updates to {hookData.HookURL}");

                    var pushResult = httpService.POST($"{hookData.HookURL}", updates, headers: new Dictionary<string, string>
                    {
                        ["Content-Type"] = "application/json"
                    });

                    step = 6;

                    if (!pushResult.Success)
                        throw new Exception($"Push Updates to {hookData.HookURL} -> {serviceResult.Content}");
                }

                step = 7;

                hookData.LastExecutionTime = DateTime.Now; // update the last execution time
                this.BLL.UpdateWebHookLastExecutionTime(hookData);                
            }
            catch (Exception ex)
            {
                ex.Data.Add("Method", "WebHooksProcess.HandleBrokerHook");
                ex.Data.Add("HookId", hookData.Id);
                ex.Data.Add("BrokerName", hookData.BrokerName);
                ex.Data.Add("ServiceName", hookData.ServiceName);
                ex.Data.Add("Step", step);
                this.Logger.Error("WebHooksProcess", ex);
            }
        }
    }
}
