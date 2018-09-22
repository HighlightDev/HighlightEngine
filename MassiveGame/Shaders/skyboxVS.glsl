#version 400

layout (location = 0) in vec3 Position;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

uniform vec4 clipPlane;

out vec3 passTextureCoord;

void main(void)
{
    vec4 worldSpaceVertex = modelMatrix * vec4(Position, 1.0);
    passTextureCoord = Position;

    gl_ClipDistance[0] = dot(worldSpaceVertex, clipPlane);

	gl_Position = projectionMatrix * viewMatrix * worldSpaceVertex;
}