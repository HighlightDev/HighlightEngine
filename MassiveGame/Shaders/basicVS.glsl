#version 400

layout (location = 0) in vec3 Vertex;
layout (location = 2) in vec2 TexCoords;

uniform mat4 worldMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

out vec2 texCoord;

void main()
{
    vec4 worldSpaceVertex = worldMatrix * vec4(Vertex, 1.0);
    texCoord = TexCoords;

    gl_Position = projectionMatrix * viewMatrix * worldSpaceVertex;
}