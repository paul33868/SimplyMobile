﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Controls;
using SimplyMobile.Text;

namespace SimplyMobile.Web
{
    public partial class WebHybrid
    {
        protected WebBrowser webView;

        public WebHybrid(WebBrowser webView, IJsonSerializer serializer)
        {
            this.webView = webView;
            this.Serializer = serializer;
            this.Initialize();
        }

        public void Dispose()
        {
            this.webView.Navigating -= webView_Navigating;
            this.webView.LoadCompleted -= webView_LoadCompleted;
            this.webView.ScriptNotify -= WebViewOnScriptNotify;
        }

        private void Initialize()
        {
            this.registeredActions = new Dictionary<string, Action<string>>();

            this.webView.IsScriptEnabled = true;
            this.webView.Navigating += webView_Navigating;
            this.webView.LoadCompleted += webView_LoadCompleted;
            this.webView.ScriptNotify += WebViewOnScriptNotify;
        }

        private void WebViewOnScriptNotify(object sender, NotifyEventArgs notifyEventArgs)
        {
            Action<string> action;
            var values = notifyEventArgs.Value.Split('/');
            var name = values.FirstOrDefault();

            if (name != null && this.registeredActions.TryGetValue(name, out action))
            {
                var data = Uri.UnescapeDataString(values.ElementAt(1));
                action.Invoke(data);
            }
        }

        void webView_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            this.InjectNativeFunctionScript();
        }

        void webView_Navigating(object sender, NavigatingEventArgs e)
        {

        }

        partial void Inject(string script)
        {
            //this.webView.InvokeScript(string.Format("javascript: {0}", script));
            this.webView.InvokeScript("eval", script);
        }

        partial void LoadFile(string fileName)
        {
            this.webView.Navigate(new Uri(fileName));
        }
    }
}
