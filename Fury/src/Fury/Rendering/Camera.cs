using OpenTK.Mathematics;

using System;

namespace Fury.Rendering
{
    public abstract class Camera
    {
        public Matrix4 ProjectionMatrix = Matrix4.Identity;

        public Matrix4 ViewMatrix = Matrix4.Identity;

        protected Vector3 position;
        public Vector3 Position { get => position; set { position = value; RecalculateViewMatrix(); } }

        protected Vector3 rotation;
        public Vector3 Rotation { get => rotation; set { rotation = value; RecalculateViewMatrix(); } }

        public float Speed = 5f;

        private void RecalculateViewMatrix()
        {
            Matrix4 transform = Matrix4.CreateTranslation(-position) *
                                Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-rotation.X)) *
                                Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-rotation.Y)) *
                                Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(-rotation.Z));

            ViewMatrix = transform.Inverted();
        }
    }

    public class OrthographicCamera : Camera
    { 
        public OrthographicCamera(float left, float right, float bottom, float top)
        {
            ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, -1, 1);
        }

        public void SetProjection(float left, float right, float bottom, float top)
        {
            ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, -1, 1);
        }
    }

    public class PerspectiveCamera : Camera
    {
        public PerspectiveCamera(float fov, float aspect, float near, float far)
        {
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), aspect, near, far);
        }

        public void SetProjection(float fov, float aspect, float near, float far)
        {
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), aspect, near, far);
        }
    }
}
