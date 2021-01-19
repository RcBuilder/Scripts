using Helpers;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BrokersEngine
{
    // use this attribute to Decorate ANY Service Controller to trigger the Broker Pipeline! 
    /*
        [BrokerProcessor(typeof(BrokerFactory))]        
        public abstract class BrokerApiController : ApiController {
            public Broker Broker { get; set; }
        } 
    */
    [AttributeUsage(AttributeTargets.Class)]
    public class BrokerProcessor : ActionFilterAttribute
    {
        public BrokerProcessorContext Context { get; protected set; } = new BrokerProcessorContext();
        public IBrokerProcessorExtractor Extractor { get; protected set; }
        public IBrokerFactory BrokerFactory { get; protected set; }
        public Broker Broker { get; protected set; }

        public BrokerProcessor() {
            this.Extractor = new BrokerProcessorExtractor(new RequestHelper());            
        }
        
        public BrokerProcessor(Type brokerFactoryType, params string[] args) {
            this.Extractor = new BrokerProcessorExtractor(new RequestHelper());
            this.BrokerFactory = Activator.CreateInstance(brokerFactoryType, args) as IBrokerFactory;                       
        }

        public BrokerProcessor(IBrokerProcessorExtractor Extractor, IBrokerFactory BrokerFactory) {
            this.Extractor = Extractor;
            this.BrokerFactory = BrokerFactory;            
        }

        private void InitRequestContext(HttpActionContext actionContext) {
            // load the current broker
            this.Broker = this.BrokerFactory.Produce(this.Extractor.ExtractBrokerName(actionContext));
            if (this.Broker == null) throw new Exception("NO SUCH BROKER!");

            this.Broker.Configure();

            // update the controller properties
            var controller = actionContext.ControllerContext.Controller as BrokerApiController;
            controller.Broker = this.Broker;

            this.Context.ActionContext = actionContext;
            this.Context.ControllerName = this.Extractor.ExtractControllerName(actionContext);
            this.Context.ActionName = this.Extractor.ExtractActionName(actionContext);
        }

        private void InitResponseContext(HttpActionContext actionContext) {
            this.Context.ActionContext = actionContext; // change the context to the outbound context            
        }

        // before the action executes
        public sealed override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            try
            {
                this.InitRequestContext(actionContext);  // init context
                this.Broker.PreProcessRequest(this.Context);   // before processing the request 
                this.Broker.ProcessRequest(this.Context);      // process the request       
                this.Broker.PostProcessRequest(this.Context);   // after processing the request 
            }
            catch (Exception ex) {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        // after the action executes
        public sealed override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            try
            {
                this.InitResponseContext(actionExecutedContext.ActionContext);  // init context
                this.Broker.PreProcessResponse(this.Context);  // before processing the response
                this.Broker.ProcessResponse(this.Context);     // process the response
                this.Broker.PostProcessResponse(this.Context);  // after processing the response
            }
            catch (Exception ex) {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }

        public sealed override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
        }

        public sealed override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
        }
    }
}