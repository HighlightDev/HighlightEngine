#version 400

layout (location = 0) in vec3 vertex;
layout (location = 1) in vec2 texCoord;

out vec2 texCoords;

uniform mat4 screenSpaceMatrix;

void main(void){

	texCoords = vec2(texCoord.x, 1 - texCoord.y);
	gl_Position = screenSpaceMatrix * vec4(vertex, 1.0);
}