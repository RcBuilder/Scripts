using System;
using System.Reflection;

namespace BrokersEngine
{
    // load a Broker by internal dedicated (constant) class instance 
    [Obsolete("Use ProxyBrokerFactory Instead")]
    public class BrokerFactory : IBrokerFactory
    {
        public Broker Produce(string BrokerName) {
            /* 
                // dynamic loading using reflection
                var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                var brokerType = Type.GetType($"{assemblyName}.Brokers.{BrokerName ?? "DefaultBroker"}");
                var aaa = Activator.CreateInstance(brokerType) as IBroker;            
            */

            switch (BrokerName) {
                case "Broker_208423368": return new Broker_208423368();
                case "Broker_208423368_QA": return new Broker_208423368_QA();
                case "Broker_031472517": return new Broker_031472517();
                case "Broker_031472517_QA": return new Broker_031472517_QA();
                case "Broker_514018878": return new Broker_514018878();
                case "Broker_514018878_QA": return new Broker_514018878_QA();
                case "Broker_514544147": return new Broker_514544147();
                case "Broker_514544147_QA": return new Broker_514544147_QA();
                case "Broker_032828162": return new Broker_032828162();  
                case "Broker_032828162_QA": return new Broker_032828162_QA();

                case "Broker_513933895": return new Broker_513933895();
                case "Broker_514559731": return new Broker_514559731();
                case "Broker_516111416": return new Broker_516111416();                
                case "Broker_557101045": return new Broker_557101045();   
                    
                case "Broker_999999999": return new Broker_999999999();
                case "Broker_123456789": return new Broker_123456789();
                default: return null;
            }                       
        }
    }
}