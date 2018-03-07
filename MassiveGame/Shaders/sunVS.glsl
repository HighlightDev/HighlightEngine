#version 400

layout (location = 0) in vec3 vertex;
layout (location = 2) in vec2 texCoords;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
uniform vec4 clipPlane;

out vec2 texCoord;

void main(void)
{
    vec4 worldSpaceVertex = modelMatrix * vec4(vertex, 1.0);
    gl_ClipDistance[0] = dot(worldSpaceVertex, clipPlane);
    
	//right vector and up vector3
	mat4 mvMatrix = viewMatrix * modelMatrix;
	vec3 X = vec3( mvMatrix[0][0], mvMatrix[1][0], mvMatrix[2][0] );
    vec3 Y = vec3( mvMatrix[0][1], mvMatrix[1][1], mvMatrix[2][1] );

    // billboard vertex position
    vec3 billboardV = vertex.x * X + vertex.y * Y;

	vec4 worldPos =  modelMatrix * vec4(billboardV, 1.0);
	
	texCoord = texCoords;

	gl_Position = projectionMatrix * viewMatrix * worldPos;
}