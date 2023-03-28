﻿using System.Reflection;
using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher.ParameterExtractors;

public class MessageEnvelopeExtractor : IParameterExtractor
{
    public object ExtractParameter(ParameterInfo _, MessageContext messageContext)
    {
        return messageContext.Message;
    }
}