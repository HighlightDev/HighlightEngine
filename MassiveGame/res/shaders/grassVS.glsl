#version 400

layout (location = 0) in vec3 vertex;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

void main()
{
	
	gl_Position = vec4(vertex, 1.0);
}