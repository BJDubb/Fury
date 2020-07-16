using Fury.Events;

namespace Fury
{
    public class Layer
    {
        private string name;

        public Layer(string name = "New Layer")
        {
            this.name = name;
        }

        public virtual void OnAttach() { }
        public virtual void OnDettach() { }
        public virtual void OnUpdate(double elapsed) { }
        public virtual void OnImGuiRender() { }
        public virtual void OnEvent(Event e) { }
    }
}
