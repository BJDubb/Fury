#version 330 core

in vec3 oColor;
in vec2 oTexCoord;

out vec4 FragColor;

uniform sampler2D uTexture;

void main()
{
    FragColor = texture(uTexture, oTexCoord);
    //FragColor = vec4(oTexCoord, 0, 1);
    //FragColor = vec4(oColor, 1);
}