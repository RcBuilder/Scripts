using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.V8;
using React;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(TestReactNET.ReactConfig), "Configure")]

namespace TestReactNET
{
	public static class ReactConfig
	{
		public static void Configure()
		{
            ReactSiteConfiguration.Configuration                
                .AddScript("~/Scripts/React/testReact.jsx")
                .AddScript("~/Scripts/React/testReact2.jsx")                
                .AddScript("~/Scripts/React/component1.jsx")
                .AddScript("~/Scripts/React/testCss.jsx")
                .AddScript("~/Scripts/React/testLifecycle.jsx")
                ;

            JsEngineSwitcher.Current.DefaultEngineName = V8JsEngine.EngineName;
            JsEngineSwitcher.Current.EngineFactories.AddV8();
        }
    }
}