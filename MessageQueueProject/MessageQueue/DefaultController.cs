using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MessageQueue
{
    [ActionLog]
    public class DefaultController : ApiController 
    {
        private static string QUEUE_ROOT_PATH = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "Queues\\");
        private const string MESSAGE_EXT = ".mq";

        [HttpGet]
        [Route("")]
        public List<string> Index()
        {
            return new List<string> { 
                "POST SendMessage/{queueKey}",
                "GET RecieveMessage/{queueKey}/{messageKey}",
                "GET RecieveMessages/{queueKey}/{rowCount}",
                "DELETE DeleteMessage/{queueKey}/{messageKey}"
            };
        }

        [HttpPost]
        [Route("SendMessage/{queueKey}")]
        public string SendMessage(string queueKey, [FromBody]JToken message) // JToken = raw json content
        {
            var messageKey = Guid.NewGuid().ToString();
            var queuePath = string.Concat(QUEUE_ROOT_PATH, queueKey);
            if (!Directory.Exists(queuePath))
                Directory.CreateDirectory(queuePath);

            var messagePath = string.Concat(queuePath, "\\", messageKey, MESSAGE_EXT);
            if (message != null)
                File.WriteAllText(messagePath, message.ToString());
            return messageKey;            
        }

        [HttpGet]
        [Route("RecieveMessage/{queueKey}/{messageKey}")]
        public Message RecieveMessage(string queueKey, string messageKey)
        {
            var queuePath = string.Concat(QUEUE_ROOT_PATH, queueKey);
            var messagePath = string.Concat(queuePath, "\\", messageKey, MESSAGE_EXT);
            if (!File.Exists(messagePath)) return null;

            return new Message {
                MessageId = messageKey,
                Body = File.ReadAllText(messagePath)
            };         
        }

        [HttpGet]
        [Route("RecieveMessages/{queueKey}/{rowCount}")]
        public IEnumerable<Message> RecieveMessages(string queueKey, int rowCount)
        {
            var queuePath = string.Concat(QUEUE_ROOT_PATH, queueKey);
            if (!Directory.Exists(queuePath)) return null;

            var queueDirectory = new DirectoryInfo(queuePath);
            var queueFiles = queueDirectory.GetFiles("*.mq").OrderBy(x => x.CreationTime).Take(rowCount);
            return queueFiles.Select(x => new Message
            {
                MessageId = Path.GetFileNameWithoutExtension(x.Name),
                Body = File.ReadAllText(x.FullName)
            });
        }

        [HttpDelete]
        [Route("DeleteMessage/{queueKey}/{messageKey}")]
        public bool DeleteMessage(string queueKey, string messageKey)
        {
            var queuePath = string.Concat(QUEUE_ROOT_PATH, queueKey);
            var messagePath = string.Concat(queuePath, "\\", messageKey, MESSAGE_EXT);
            if (!File.Exists(messagePath)) return false;

            File.Delete(messagePath);
            return true;
        } 
    }
}
