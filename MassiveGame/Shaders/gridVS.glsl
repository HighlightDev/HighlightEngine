#version 400

layout (location = 0) in vec3 vertex;

uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

out vec3 vertexLocation;

void main()
{
	vertexLocation = vertex;	
	gl_Position = projectionMatrix * viewMatrix * vec4(vertex, 1.0);
}