#version 400

layout (location = 0) out vec4 FragColor;

in vec2 texCoord;

uniform vec3 color;

void main(void)
{
    FragColor = vec4(color, 1.0);
}