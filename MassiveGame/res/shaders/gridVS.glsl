#version 400

layout (location = 0) in vec3 vertex;
layout (location = 1) in vec2 texCoord;

uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

out vec3 vertexLocation;
out vec2 texCoordinates;

void main()
{
	texCoordinates = texCoord;
	vertexLocation = vertex;	

	gl_Position = projectionMatrix * viewMatrix * vec4(vertex, 1.0);
}