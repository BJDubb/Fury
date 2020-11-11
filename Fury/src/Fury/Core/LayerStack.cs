using System;
using System.Collections.Generic;
using System.Text;

namespace Fury
{
    public class LayerStack
    {
        private List<Layer> layers;

        public List<Layer> Layers => layers;

        public LayerStack()
        {
            layers = new List<Layer>();
        }

        public void PushLayer(Layer layer)
        {
            layers.Add(layer);
            layer.OnAttach();
        }

        public void PopLayer(Layer layer)
        {
            layers.Remove(layer);
            layer.OnDettach();
        }
    }
}
