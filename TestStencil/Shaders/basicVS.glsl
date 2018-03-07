﻿#version 400

layout (location = 0) in vec3 vertex;
layout (location = 1) in vec2 texCoords;

uniform mat4 worldMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

out vec2 texCoord;

void main(void)
{
    texCoord = texCoords;

    gl_Position = projectionMatrix * viewMatrix * worldMatrix * vec4(vertex, 1.0);
}