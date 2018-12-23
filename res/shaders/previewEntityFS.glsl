#version 400

layout (location = 0) out vec4 FragColor;

uniform sampler2D diffuseTexture;

in vec2 texCoords;

void main(void)
{
	vec3 fragmentColor = texture(diffuseTexture, texCoords).rgb;
	FragColor = vec4(fragmentColor, 1);
}