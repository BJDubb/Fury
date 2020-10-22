using Fury.Core;
using Fury.Events;

using System;

namespace Sandbox
{
    public class EditorLayer : Layer
    {
        #region Constructor

        public EditorLayer() : base("EditorLayer")
        {

        }

        #endregion

        public override void OnAttach()
        {
            base.OnAttach();
        }

        public override void OnDettach()
        {
            base.OnDettach();
        }

        public override void OnUpdate(double elapsed)
        {
            base.OnUpdate(elapsed);
        }

        public override void OnImGuiRender()
        {
            base.OnImGuiRender();
        }

        public override void OnEvent(Event e)
        {
            base.OnEvent(e);
        }

    }
}
