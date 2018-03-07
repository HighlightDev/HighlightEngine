#version 400

layout (location = 0) out vec4 FragColor;

const vec4 normalColor = vec4(0.0, 0.8, 1.0, 1.0);

void main()
{
	FragColor = normalColor;
}