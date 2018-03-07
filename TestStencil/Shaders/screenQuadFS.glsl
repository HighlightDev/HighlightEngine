#version 400

layout (location = 0) out vec4 FragColor;

in vec2 texCoord;

uniform sampler2D screenTexture;

void main(void)
{
    FragColor = texture(screenTexture, texCoord);
}