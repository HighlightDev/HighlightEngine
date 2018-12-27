#version 400

layout(location = 0) out vec4 FragColor;

uniform sampler2D frameSampler;
uniform sampler2D postProcessResultSampler;

in vec2 texCoord;

void main(void)
{
	FragColor = texture(frameSampler, texCoord) + texture(postProcessResultSampler, texCoord);
}