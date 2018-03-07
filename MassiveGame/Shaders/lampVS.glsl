#version 330

layout (location = 0) in vec3 vertex;

uniform mat4 viewMatrix;
uniform mat4 modelMatrix;

void main(void)
{
	gl_Position = viewMatrix * modelMatrix * vec4(vertex, 1.0);
}