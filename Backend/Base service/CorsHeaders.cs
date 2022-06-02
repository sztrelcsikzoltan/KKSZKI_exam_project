﻿using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Base_service
{
    public class CustomHeaderMessageInspector : IDispatchMessageInspector
    {
        readonly Dictionary<string, string> requiredHeaders;
        public CustomHeaderMessageInspector(Dictionary<string, string> headers)
        {
            requiredHeaders = headers ?? new Dictionary<string, string>();
        }
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            string displayText = $"Server has received the following message:\n{request}\n";
            Console.WriteLine(displayText);
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (!reply.Properties.ContainsKey("httpResponse"))
                reply.Properties.Add("httpResponse", new HttpResponseMessageProperty());

            var httpHeader = reply.Properties["httpResponse"] as HttpResponseMessageProperty;
            foreach (var item in requiredHeaders)
            {
                httpHeader.Headers.Add(item.Key, item.Value);
            }

            string displayText = $"Server has replied the following message:\n{reply}\n";
            Console.WriteLine(displayText);

        }
    }

    public class MyBehaviorAttribute : Attribute, IContractBehavior, IContractBehaviorAttribute
    {
        public Type TargetContract => typeof(MyBehaviorAttribute);

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime) { }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
            var requiredHeaders = new Dictionary<string, string>
            {
                { "Access-Control-Allow-Origin", "*" },
                { "Access-Control-Request-Method", "POST,GET,PUT,DELETE,OPTIONS" },
                { "Access-Control-Allow-Headers", "X-Requested-With,Content-Type" }
            };

            dispatchRuntime.MessageInspectors.Add(new CustomHeaderMessageInspector(requiredHeaders));
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint) { }
    }
}