#version 400

layout (location = 0) out vec4 FragColor;

uniform samplerCube cubemap;

in vec3 textureDirection;

void main(void)
{
    vec4 textureColor = texture(cubemap, textureDirection);
    FragColor = textureColor;
}
