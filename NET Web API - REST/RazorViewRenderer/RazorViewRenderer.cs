using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RestAPITests
{
    /*
        INFO
        ----
        use razor engine to render an html template

        Using in WebAPI
        ---------------
        GET /api/render/razor

        -

        public class DefaultController : ApiController{
            [HttpGet]
            [Route("api/render/razor")]
            public HttpResponseMessage RenderHTMLByRazor() {
                var model = new SampleModel {
                    FirstName = "John",
                    LastName = "Doe"                
                };
           
                var sDocument = RazorViewRenderer.RenderView("~/views/Sample.cshtml", model);

                var response = new HttpResponseMessage();
                response.Content = new StringContent(sDocument);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return response;
            }
        }

        -

        public class SampleModel {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        -

        // Sample.cshtml
        @inherits System.Web.Mvc.WebViewPage
        @using System.Web.Mvc.Html;
        @{
            Layout = null;
            var MyModel = (Model as RestAPITests.SampleModel);
        }
        <!DOCTYPE html>

        <html>
        <head>
            <meta name="viewport" content="width=device-width" />
            <title></title>
        </head>
        <body>
            @Html.Partial("~/Partial/Header.cshtml")
            <h2>Hello @MyModel.FirstName @MyModel.LastName</h2>
            <div>
                bla bla bla ...
            </div>            
        </body>
        </html>

        -

        // Header.cshtml
        @inherits System.Web.Mvc.WebViewPage
        <header>
            ...
        </header>
    */


    // Inside of MVC Controller
    // var sHTML = RenderViewToString(ControllerContext, "~/views/Sample.cshtml", model, true);
    /*
    static string RenderViewToString(ControllerContext context, string viewPath, object model = null, bool partial = false)
    {
        // first find the ViewEngine for this view
        ViewEngineResult viewEngineResult = null;
        if (partial)
            viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
        else
            viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, null);

        if (viewEngineResult == null)
            throw new FileNotFoundException("View cannot be found.");

        // get the view and attach the model to view data
        var view = viewEngineResult.View;
        context.Controller.ViewData.Model = model;

        string result = null;

        using (var sw = new StringWriter())
        {
            var ctx = new ViewContext(context, view, context.Controller.ViewData, context.Controller.TempData, sw);
            view.Render(ctx, sw);
            result = sw.ToString();
        }

        return result;
    }
    */

    // Outside/Inside of MVC Controller
    // Generic Views Renderer    
    // var sHTML = RazorViewRenderer.RenderView("~/views/Sample.cshtml", model);
    public class RazorViewRenderer
    {
        public class EmptyController : Controller { }

        protected ControllerContext Context { get; set; }

        public RazorViewRenderer(ControllerContext controllerContext = null)
        {
            // Create a known controller from HttpContext if no context is passed
            if (controllerContext == null)
            {
                if (HttpContext.Current != null)
                    controllerContext = CreateController<EmptyController>().ControllerContext;
                else
                    throw new InvalidOperationException("ViewRenderer must run in the context of an ASP.NET " + "Application and requires HttpContext.Current to be present.");
            }
            Context = controllerContext;
        }

        public string RenderViewToString(string viewPath, object model = null)
        {
            return RenderViewToStringInternal(viewPath, model, false);
        }

        public void RenderView(string viewPath, object model, TextWriter writer)
        {
            RenderViewToWriterInternal(viewPath, writer, model, false);
        }

        public string RenderPartialViewToString(string viewPath, object model = null)
        {
            return RenderViewToStringInternal(viewPath, model, true);
        }

        public void RenderPartialView(string viewPath, object model, TextWriter writer)
        {
            RenderViewToWriterInternal(viewPath, writer, model, true);
        }

        public static string RenderView(string viewPath, object model = null, ControllerContext controllerContext = null)
        {
            var renderer = new RazorViewRenderer(controllerContext);
            return renderer.RenderViewToString(viewPath, model);
        }

        public static void RenderView(string viewPath, TextWriter writer, object model, ControllerContext controllerContext)
        {
            var renderer = new RazorViewRenderer(controllerContext);
            renderer.RenderView(viewPath, model, writer);
        }

        public static string RenderView(string viewPath, object model, ControllerContext controllerContext, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                var renderer = new RazorViewRenderer(controllerContext);
                return renderer.RenderViewToString(viewPath, model);
            }
            catch (Exception ex)
            {
                errorMessage = ex.GetBaseException().Message;
            }
            return null;
        }

        public static void RenderView(string viewPath, object model, TextWriter writer, ControllerContext controllerContext, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                var renderer = new RazorViewRenderer(controllerContext);
                renderer.RenderView(viewPath, model, writer);
            }
            catch (Exception ex)
            {
                errorMessage = ex.GetBaseException().Message;
            }
        }

        public static string RenderPartialView(string viewPath, object model = null, ControllerContext controllerContext = null)
        {
            var renderer = new RazorViewRenderer(controllerContext);
            return renderer.RenderPartialViewToString(viewPath, model);
        }

        public static void RenderPartialView(string viewPath, TextWriter writer, object model = null, ControllerContext controllerContext = null)
        {
            var renderer = new RazorViewRenderer(controllerContext);
            renderer.RenderPartialView(viewPath, model, writer);
        }

        protected void RenderViewToWriterInternal(string viewPath, TextWriter writer, object model = null, bool partial = false)
        {
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = null;
            if (partial)
                viewEngineResult = ViewEngines.Engines.FindPartialView(Context, viewPath);
            else
                viewEngineResult = ViewEngines.Engines.FindView(Context, viewPath, null);

            if (viewEngineResult == null)
                throw new FileNotFoundException();

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            Context.Controller.ViewData.Model = model;

            var ctx = new ViewContext(Context, view, Context.Controller.ViewData, Context.Controller.TempData, writer);
            view.Render(ctx, writer);
        }

        private string RenderViewToStringInternal(string viewPath, object model, bool partial = false)
        {
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = null;
            if (partial)
                viewEngineResult = ViewEngines.Engines.FindPartialView(Context, viewPath);
            else
                viewEngineResult = ViewEngines.Engines.FindView(Context, viewPath, null);

            if (viewEngineResult == null || viewEngineResult.View == null)
                throw new FileNotFoundException("View Not Found");

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            Context.Controller.ViewData.Model = model;

            string result = null;

            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(Context, view, Context.Controller.ViewData, Context.Controller.TempData, sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }

        public static T CreateController<T>(RouteData routeData = null, params object[] parameters)
                    where T : Controller, new()
        {
            // create a disconnected controller instance
            T controller = (T)Activator.CreateInstance(typeof(T), parameters);

            // get context wrapper from HttpContext if available
            HttpContextBase wrapper = null;
            if (HttpContext.Current != null)
                wrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
            else
                throw new InvalidOperationException("Can't create Controller Context if no active HttpContext instance is available.");

            if (routeData == null)
                routeData = new RouteData();

            // add the controller routing if not existing
            if (!routeData.Values.ContainsKey("controller") && !routeData.Values.ContainsKey("Controller"))
                routeData.Values.Add("controller", controller.GetType().Name.ToLower().Replace("controller", ""));

            controller.ControllerContext = new ControllerContext(wrapper, routeData, controller);
            return controller;
        }
    }
}