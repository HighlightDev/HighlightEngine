#version 400

layout (location = 0) in vec3 vertex;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 texCoord;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
uniform vec3 cameraPosition;

out vec2 texCoords;
out vec3 outNormal;
out vec3 cameraDir;

void main(void)
{
	texCoords = texCoord;
	outNormal = (modelMatrix * vec4(normal, 0.0)).xyz;

	vec3 worldPos = (modelMatrix * vec4(vertex, 1.0)).xyz;
	cameraDir = worldPos - cameraPosition;

	gl_Position = projectionMatrix * viewMatrix * vec4(worldPos, 1.0);
}