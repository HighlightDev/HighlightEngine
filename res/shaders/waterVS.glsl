#version 400

#define MAX_LIGHT_COUNT 5 
#define tiling 15

layout (location = 0) in vec3 Vertex;
layout (location = 1) in vec3 Normal;
layout (location = 2) in vec2 TexCoords;
layout (location = 3) in vec3 Tangent;
layout (location = 4) in vec3 Bitangent;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
uniform vec3 cameraPosition;
uniform vec3 sunPos;
uniform vec3 lightPosition[MAX_LIGHT_COUNT];
uniform bool enableLight[MAX_LIGHT_COUNT];

uniform bool mistEnable;
uniform float mistDensity;
uniform float mistGradient;

out vec4 clipSpace;
out vec3 fresnelCamera;
out vec3 worldNormal;
out vec2 texCoords;
out vec3 eyeCameraVec;
out vec3 sunDir;
out vec3 toLightDiffuseVec[MAX_LIGHT_COUNT];
out float mistContribution;

float getMist(float density, float gradient, vec3 eyePos)
{
    float mistContribution = 0.0;
	/*If mist enabled*/
	if (mistEnable)
	{
	    float distance = length(eyePos.xyz);
	    mistContribution = exp(-pow((density * distance), gradient));
	    mistContribution = clamp(mistContribution, 0.0, 1.0);
    }
	return mistContribution;
}

void main(void)
{
	vec3 worldPos = (modelMatrix * vec4(Vertex, 1.0)).xyz;
	vec3 eyePos = (viewMatrix * modelMatrix * vec4(Vertex, 1.0)).xyz;

	vec3 WorldNormal = normalize((modelMatrix * vec4(Normal, 0.0)).xyz);
	vec3 WorldTangent = normalize((modelMatrix * vec4(Tangent, 0.0)).xyz);
	vec3 WorldBitangent = normalize((modelMatrix * vec4(Bitangent, 0.0)).xyz);
	mat3 WorldTangentSpace = mat3(
		WorldTangent.x, WorldBitangent.x, WorldNormal.x,
		WorldTangent.y, WorldBitangent.y, WorldNormal.y,
		WorldTangent.z, WorldBitangent.z, WorldNormal.z
	);

	vec3 toCameraVec = cameraPosition - worldPos;

	eyeCameraVec = WorldTangentSpace * toCameraVec;
	sunDir = WorldTangentSpace * sunPos;

	for (int i = 0; i < MAX_LIGHT_COUNT; i++)
	{
		if (!enableLight[i]) continue;
		toLightDiffuseVec[i] = WorldTangentSpace * (lightPosition[i] - worldPos);
	}

		/*Normal mapping*/

	texCoords = TexCoords * tiling;
	worldNormal = (modelMatrix * vec4(Normal, 0.0)).xyz;
	clipSpace = projectionMatrix * vec4(eyePos, 1.0);
	fresnelCamera = toCameraVec;

	/*If mist enabled*/
	if (mistEnable)
	{
		mistContribution = getMist(mistDensity, mistGradient, eyePos);
	}

	gl_Position = clipSpace;
}
