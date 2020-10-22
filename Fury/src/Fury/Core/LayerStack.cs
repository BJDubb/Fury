using System;
using System.Collections.Generic;
using System.Text;

namespace Fury.Core
{
    public class LayerStack
    {
        private List<Layer> layers;
        private List<Layer> overlays;

        public List<Layer> Layers
        {
            get
            {
                var list = new List<Layer>(layers);
                list.AddRange(overlays);
                return list;
            }
        }

        public LayerStack()
        {
            layers = new List<Layer>();
            overlays = new List<Layer>();
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

        public void PushOverlay(Layer layer)
        {
            overlays.Add(layer);
            layer.OnAttach();
        }

        public void PopOverlay(Layer layer)
        {
            overlays.Remove(layer);
            layer.OnDettach();
        }
    }
}
