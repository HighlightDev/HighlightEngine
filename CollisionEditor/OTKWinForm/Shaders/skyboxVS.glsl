#version 400

layout (location = 0) in vec3 Vertex;

uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

out vec3 textureDirection;

void main()
{
    vec4 worldSpaceVertex = vec4(Vertex, 1.0);
    textureDirection = worldSpaceVertex.xyz;

    gl_Position = projectionMatrix * viewMatrix * worldSpaceVertex;
}