#version 400

#define LIGHT_COUNT 5
layout (location = 0) in vec3 Vertex;
layout (location = 1) in vec3 Normals;
layout (location = 2) in vec2 TexCoords;
layout (location = 4) in vec3 Tangent;
layout (location = 5) in vec3 Bitangent;

uniform vec3 lightPosition[LIGHT_COUNT];
uniform bool enableLight[LIGHT_COUNT];
uniform vec3 sunDirection;

uniform mat4 ModelMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ProjectionMatrix;
uniform mat4 dirLightShadowMatrix;
uniform vec4 clipPlane;

uniform bool enableNMr;
uniform bool enableNMg;
uniform bool enableNMb;
uniform bool enableNMblack;

uniform bool  mistEnable;
uniform float mistDensity;
uniform float mistGradient;

out vec2 pass_textureCoordinates;
out vec3 surfaceNormal;
out vec3 toLightVecSimple[LIGHT_COUNT];
out vec3 toLightVecNormMap[LIGHT_COUNT];
out vec3 SunDirectionSimple;
out vec3 SunDirectionNormMap;
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
	SunDirectionSimple = sunDirection;
    fragPosLightSpace = dirLightShadowMatrix * vec4(Vertex, 1.0);

	vec3 WorldPosition = vec3(ModelMatrix * vec4(Vertex,1.0));
	vec3 EyeSpecularPosition = (ViewMatrix * vec4(WorldPosition, 1.0)).xyz;

	gl_ClipDistance[0] = dot(vec4(WorldPosition, 1.0), clipPlane);	// Clipping beyond the plane

	pass_textureCoordinates = TexCoords.xy;

	// Calculate world normal
	surfaceNormal = (ModelMatrix * vec4(Normals, 0.0)).xyz;

	for (int i = 0;i < LIGHT_COUNT;i ++)
	{
		if (!enableLight[i]) continue;
		toLightVecSimple[i] = lightPosition[i] - WorldPosition.xyz;
	}

	// IF normal map enabled - create matrix to transoform object to tangent space
	if (enableNMr == true || enableNMg == true || enableNMb == true || enableNMblack == true)
	{
		vec3 norm = normalize(surfaceNormal);
		vec3 tang = normalize(vec3(ModelMatrix * vec4(Tangent, 0.0)));
		vec3 bitang = normalize(vec3(ModelMatrix * vec4(Bitangent, 0.0)));

		mat3 toTangentSpace = mat3(
		tang.x, bitang.x, norm.x,
		tang.y, bitang.y, norm.y,
		tang.z, bitang.z, norm.z);

		//Transform point light vectors from object space to tangent space
		for (int i = 0;i < LIGHT_COUNT;i ++)
		{
			if (!enableLight[i]) continue;
			toLightVecNormMap[i] = toTangentSpace * toLightVecSimple[i];
		}

		//Trasform sun light vector from object space to tangent space
		SunDirectionNormMap = toTangentSpace * SunDirectionSimple;
	}

	mistContribution = getMist(mistDensity, mistGradient, EyeSpecularPosition);

	gl_Position = ProjectionMatrix * ViewMatrix * vec4(WorldPosition,1.0);
}
