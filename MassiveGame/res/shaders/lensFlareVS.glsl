﻿#version 400

layout (location = 0) in vec3 vertex;
layout (location = 1) in vec2 texCoords;

noperspective out vec2 texCoord;

void main(void)
{
	texCoord = texCoords;
	gl_Position = vec4(vertex, 1.0);
}