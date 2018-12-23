#version 400

#define MAX_LIGHT_COUNT 5
layout (location = 0) in vec3 Position;
layout (location = 1) in vec3 Normals;
layout (location = 2) in vec2 TexCoords;
layout (location = 4) in vec3 Tangent;
layout (location = 5) in vec3 Bitangent;

uniform vec3 lightPosition[MAX_LIGHT_COUNT];
uniform bool sunEnable;
uniform vec3 sunDirection;
uniform bool enableLight[MAX_LIGHT_COUNT];

uniform mat4 ModelMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ProjectionMatrix;
uniform mat4 dirLightShadowMatrix;

uniform vec4 clipPlane;

uniform bool mistEnable;
uniform float mistDensity;
uniform float mistGradient;

uniform bool normalMapEnableDisable;

out vec2 pass_textureCoordinates;
out vec3 surfaceDiffuseNormal;
out vec3 surfaceSpecularNormal;
out vec3 toLightDiffuseVec[MAX_LIGHT_COUNT];
out vec3 toLightSpecularVec[MAX_LIGHT_COUNT];
out vec3 toCameraVec;
out vec3 SunDirection;
out float mistContribution;
out vec4 fragPosLightSpace;

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
	SunDirection = sunDirection;
	vec3 EyeSpecularPosition = vec3(ViewMatrix * ModelMatrix * vec4(Position,1.0));
	vec3 WorldDiffusePosition = vec3(ModelMatrix * vec4(Position,1.0));
	pass_textureCoordinates = TexCoords.xy;

	fragPosLightSpace = dirLightShadowMatrix * vec4(Position, 1.0);

	gl_ClipDistance[0] = dot(vec4(WorldDiffusePosition,1.0), clipPlane);// Clipping everything above the plane

	mat4 modelViewMatrix = ViewMatrix * ModelMatrix;
	surfaceDiffuseNormal = (ModelMatrix * vec4(Normals, 0.0)).xyz;
	surfaceSpecularNormal = (modelViewMatrix * vec4(Normals, 0.0)).xyz;

	toCameraVec = vec3(0,0,1);	//��������� ������ ����������� � local Viewer

	for (int i = 0; i < MAX_LIGHT_COUNT; i++)
	{
		if (!enableLight[i]) continue; //���� ������� ���� �������� - ���������� �������
		toLightDiffuseVec[i] = lightPosition[i] - WorldDiffusePosition;
		toLightSpecularVec[i] = lightPosition[i] - EyeSpecularPosition;
	}

	if (normalMapEnableDisable)	//NormalMapping enabled
	{
		vec3 normDiffuse = normalize(surfaceDiffuseNormal);
		vec3 tangDiffuse = normalize((ModelMatrix * vec4(Tangent, 0.0)).xyz);
		vec3 bitangDiffuse = normalize((ModelMatrix * vec4(Bitangent, 0.0)).xyz);
		mat3 toTangentSpaceDiffuse = mat3(
			tangDiffuse.x, bitangDiffuse.x, normDiffuse.x,
			tangDiffuse.y, bitangDiffuse.y, normDiffuse.y,
			tangDiffuse.z, bitangDiffuse.z, normDiffuse.z
		);

		vec3 normSpecular = normalize(surfaceSpecularNormal);
		vec3 tangSpecular = normalize((modelViewMatrix * vec4(Tangent, 0.0)).xyz);
		vec3 bitangSpecular = normalize((modelViewMatrix * vec4(Bitangent, 0.0)).xyz);
		mat3 toTangentSpaceSpecular = mat3(
			tangSpecular.x, bitangSpecular.x, normSpecular.x,
			tangSpecular.y, bitangSpecular.y, normSpecular.y,
			tangSpecular.z, bitangSpecular.z, normSpecular.z
		);
		for (int i = 0; i < MAX_LIGHT_COUNT; i++)
		{
			if (!enableLight[i]) continue; //���� ������� ���� �������� - ���������� �������
			toLightDiffuseVec[i] = toTangentSpaceDiffuse * toLightDiffuseVec[i];
			toLightSpecularVec[i] = toTangentSpaceSpecular * toLightSpecularVec[i];
		}
		toCameraVec = toTangentSpaceSpecular * toCameraVec;
		if (sunEnable) { SunDirection = toTangentSpaceDiffuse * SunDirection; }
	}


	mistContribution = getMist(mistDensity, mistGradient, EyeSpecularPosition);

	gl_Position = (ProjectionMatrix * vec4(EyeSpecularPosition, 1.0));
}
