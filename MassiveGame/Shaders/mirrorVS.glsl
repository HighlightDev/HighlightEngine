#version 400

layout (location = 0) in vec3 Position;
layout (location = 1) in vec3 Normals;
layout (location = 2) in vec2 TexCoords;

const int MAX_LIGHT_COUNT = 5;

uniform bool pointEnable[MAX_LIGHT_COUNT];
uniform vec3 pointPosition[MAX_LIGHT_COUNT];

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
uniform mat3 normalMatrix;

out vec2 texCoords;
out vec3 normalVec;
out vec3 lightDir[MAX_LIGHT_COUNT];
out vec3 toCameraVec;

void main(void)
{
	texCoords = TexCoords;

	normalVec = normalMatrix * Normals;
	vec3 eyePos = (viewMatrix * modelMatrix * vec4(Position,1.0)).xyz;
	for (int i = 0; i < MAX_LIGHT_COUNT; i++)
	{
		if (!pointEnable[i]) { continue; }
		lightDir[i] = pointPosition[i] - eyePos;
	}
	toCameraVec = vec3(0,0,1);

	gl_Position = projectionMatrix * vec4(eyePos,1.0);
}

