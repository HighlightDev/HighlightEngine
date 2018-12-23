#version 400

layout(location = 0) out vec4 FragColor;

uniform sampler2D SrcColor;

in vec2 texCoord;

void main(void)
{
	FragColor = texture(SrcColor, texCoord);
}