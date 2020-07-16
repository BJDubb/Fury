using System;
using Fury.Utils;

namespace Fury.Events
{
    public abstract class EngineEvent : Event { }

    public class ConsoleLoggedEvent : EngineEvent
    {
        protected Log log;

        public ConsoleLoggedEvent(Log log)
        {
            this.log = log;
        }
        public override EventType GetEventType()
        {
            return EventType.ConsoleLogged;
        }

        public override string ToString()
        {
            return $"ConsoleLog Event [{Enum.GetName(typeof(Severity), log.Severity)}], {log.Message}";
        }

        public Log Log => log;
    }
}
