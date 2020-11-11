using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Fury.Events
{
    public abstract class MouseEvent : Event { }

    public class MouseMovedEvent : MouseEvent
    {
        protected float x;
        protected float y;

        public MouseMovedEvent(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public override EventType GetEventType()
        {
            return EventType.MouseMoved;
        }

        public override string ToString()
        {
            return $"MouseEvent {x}, {y}";
        }

        public float X => x;
        public float Y => y;
    }

    public class MouseScrolledEvent : MouseEvent
    {
        protected float xOffset, yOffset;

        public MouseScrolledEvent(float xOffset, float yOffset)
        {
            this.xOffset = xOffset;
            this.yOffset = yOffset;
        }
        public override EventType GetEventType()
        {
            return EventType.MouseScrolled;
        }

        public override string ToString()
        {
            return $"MouseScroll {xOffset}, {yOffset}";
        }

        public float XOffset => xOffset;
        public float YOffset => yOffset;
    }

    public abstract class MouseButtonEvent : MouseEvent
    {
        protected MouseButton button;

        protected MouseButtonEvent(MouseButton button)
        {
            this.button = button;
        }

        public MouseButton Button => button; //TODO: Change to Fury.Input.MouseButton
    }

    public class MouseButtonPressedEvent : MouseButtonEvent
    {
        public MouseButtonPressedEvent(MouseButton button) : base(button) { }

        public override EventType GetEventType()
        {
            return EventType.MouseButtonPressed;
        }

        public override string ToString()
        {
            return $"MouseButtonPressed {Enum.GetName(typeof(MouseButton), button)}";
        }
    }

    public class MouseButtonReleasedEvent : MouseButtonEvent
    {
        public MouseButtonReleasedEvent(MouseButton button) : base(button) { }
        public override EventType GetEventType()
        {
            return EventType.MouseButtonReleased;
        }

        public override string ToString()
        {
            return $"MouseButtonReleased {Enum.GetName(typeof(MouseButton), button)}";
        }
    }
}
