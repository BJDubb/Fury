#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;

uniform mat4 uView;
uniform mat4 uProjection;
uniform mat4 uTransform;

out vec3 oColor;
out vec2 oTexCoord;

void main()
{
    gl_Position = vec4(aPosition, 1.0) * uTransform * uView * uProjection * vec4(1, 1, -1, 1);
    oColor = aPosition;
    oTexCoord = aTexCoord;
}