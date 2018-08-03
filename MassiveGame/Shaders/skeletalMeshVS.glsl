#version 400

#define MaxWeigths 3
layout(location = 0) in vec3 vertex;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 uv;
layout(location = 3) in vec3 tangent;
layout(location = 4) in vec3 bitangent;
layout(location = 5) in vec3 blendWeights;
layout(location = 6) in vec3 blendIndices;

uniform mat4 worldMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
uniform mat4 bonesMatrices[MaxWeigths];

out vec2 texCoords;

void main()
{
    texCoords = uv;
    gl_Position = projectionMatrix * viewMatrix * worldMatrix * vec4(vertex, 1.0);
}
