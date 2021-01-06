using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BipBip.Extensions.Abstractions;
using BipBipBot.DataEngine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BipBipBot
{
    public class ExtensionsManager
    {
        protected ServiceProvider ServiceProvider;
        protected ILogger<ExtensionsManager> Logger;
        protected List<IBipExtension> Extensions;
        public ExtensionsManager(ServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            this.Extensions = new List<IBipExtension>();
            using (var scope = serviceProvider.CreateScope())
            {
                Logger = scope.ServiceProvider.GetService<ILogger<ExtensionsManager>>();
            }
            
            LoadExtensions();
        }

        private  void LoadExtensions()
        {
            string extDirectory = Path.Combine(Directory.GetCurrentDirectory(), "extensions");
            List<IBipExtension> extensions = new List<IBipExtension>();
            var type = typeof(IBipExtension);
            if (Directory.Exists(extDirectory))
            {
                var dlls = Directory.GetFiles(extDirectory, "*.dll");
                foreach (string dll in dlls)
                {
                    Assembly assembly = Assembly.LoadFile(dll);
                    var types = assembly.GetTypes()
                        .Where(p => type.IsAssignableFrom(p));

                    foreach (Type extensionType in types)
                    {
                        object[] ctorParams = ResolveCtorParams(extensionType, ServiceProvider);
                        if (Activator.CreateInstance(extensionType, ctorParams) is IBipExtension instance)
                            extensions.Add(instance);
                    }
                }
            }

            string info = extensions.Count > 0 ? $"{extensions.Count} extensions loaded" : "no extensions found";
            Log(info, LogLevel.Information);

            this.Extensions = extensions;
        }

        private object[] ResolveCtorParams(Type extensionType, ServiceProvider serviceProvider)
        {
            List<object> parametersList = new List<object>();
            using (var scope = serviceProvider.CreateScope())
            {
                ConstructorInfo constructor = extensionType.GetConstructors().FirstOrDefault();
                var pList = constructor?.GetParameters();
                if (pList?.Length > 0)
                {
                    foreach (var info in pList)
                    {
                        var pType = info.ParameterType;
                        var resolved = scope.ServiceProvider.GetService(pType);
                        if (resolved != null)
                        {
                            parametersList.Add(resolved);
                        }
                    }
                }
            }

            return parametersList.ToArray();
        }

        private void Log(string message, LogLevel level)
        {
            Logger.Log(level, message);
        }

        public void ExecutePrivateMessageEvent(PrivateMessageEvent privateMessageEvent)
        {
            foreach (var extension in Extensions)
            {
                extension.OnMessageReceivedAsync(privateMessageEvent.Destination, privateMessageEvent.SenderName,
                    privateMessageEvent.ContentText);

            }
        }
    }
}