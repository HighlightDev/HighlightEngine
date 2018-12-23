#version 400

layout (location = 0) out vec4 FragColor;

uniform sampler2D diffuseTexture;
uniform float opacity;

in vec2 texCoord;

void main(void)
{
    vec4 textureColor = texture(diffuseTexture, texCoord);
    textureColor.a *= opacity;
    FragColor = textureColor;
}
