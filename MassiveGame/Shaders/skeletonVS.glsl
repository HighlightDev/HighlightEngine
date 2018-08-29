#version 400

layout (location = 0) in vec3 vertex;
layout (location = 1) in int index;

uniform mat4 worldMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
uniform mat4 skeletonMatrices[MAX_BONE_COUNT];

void main()
{
    gl_Position = projectionMatrix * viewMatrix * worldMatrix * skeletonMatrices[index] * vec4(vertex, 1.0);    
}

