#version 400

layout (location = 0) out vec4 FragColor;

uniform sampler2D albedoTexture;

in vec2 texCoords;

void main()
{
    FragColor = texture(albedoTexture, texCoords);
}
