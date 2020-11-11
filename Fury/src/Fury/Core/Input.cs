using System;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Fury
{
    public class Input
    {
        public static bool IsKeyPressed(Keys key)
        {
            unsafe
            {
                return GLFW.GetKey(((WindowsWindow)Application.GetApplication().GetWindow().GetNativeWindow()).WindowPtr, key) == InputAction.Press ? true : false;
            }
        }

        public static bool IsMouseButtonPressed(MouseButton button)
        {
            unsafe
            {
                return GLFW.GetMouseButton(((WindowsWindow)Application.GetApplication().GetWindow().GetNativeWindow()).WindowPtr, button) == InputAction.Press ? true : false;
            }
        }
    }
}
