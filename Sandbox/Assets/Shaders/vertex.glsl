#version 330 core


layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aColor;

uniform mat4 uViewProjection;
uniform mat4 uTransform;

out vec3 oColor;

void main()
{
    gl_Position = vec4(aPosition, 1.0) * uViewProjection * uTransform;
    oColor = aColor;
}