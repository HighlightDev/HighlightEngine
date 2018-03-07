#version 400

layout (location = 0) out vec4 FragColor;

const int MAX_LIGHT_COUNT = 5;
const float shininess = 150;

uniform bool pointEnable[MAX_LIGHT_COUNT];
uniform vec3 pointSpecular[MAX_LIGHT_COUNT];
uniform vec3 pointAttenuation[MAX_LIGHT_COUNT];

uniform bool sunEnable;
uniform vec3 sunPosition;
uniform vec3 sunSpecular;

uniform sampler2D frameTexture;

in vec2 texCoords;
in vec3 normalVec;
in vec3 lightDir[MAX_LIGHT_COUNT];
in vec3 toCameraVec;

vec3 directionalLight(vec3 normal)
{
	vec3 normLVec = normalize(sunPosition);
	normLVec = -normLVec;
	vec3 localView = normalize(toCameraVec);
	vec3 halfwayV = normalize(normLVec + localView);
	float reflFactor = max(dot(halfwayV, normal), 0.0);
	reflFactor = pow(reflFactor, shininess);
	vec3 specularL = sunSpecular * reflFactor;
	return specularL;
}

vec3 pointLight(vec3 lightDir, vec3 normal, vec3 lightSpecular, vec3 attenuation)
{
	vec3 normLVec = normalize(lightDir);
	float lightDistance = length(lightDir);
	float Attenuation = 1 / (attenuation.x + (attenuation.y * lightDistance) + (attenuation.z * (lightDistance * lightDistance)));
	vec3 localView = normalize(toCameraVec);
	vec3 halfwayV = normalize(normLVec + localView);
	float reflFactor = max(dot(halfwayV, normal), 0.0);
	reflFactor = pow(reflFactor, shininess);
	vec3 specularL = lightSpecular * reflFactor * Attenuation;
	return specularL;
}


void main(void)
{
	vec4 mirrorTex = texture(frameTexture,texCoords);

	vec3 normNVec = normalize(normalVec);

	vec3 totalLight = vec3(0);
	/*Calculation of point lights contribution*/
	for (int i = 0; i < MAX_LIGHT_COUNT; i++)
	{
		if (!pointEnable[i]) { continue; }
		totalLight = totalLight + pointLight(lightDir[i], normNVec, pointSpecular[i], pointAttenuation[i]);
	}
	if (sunEnable)
	{
		totalLight = totalLight + directionalLight(normNVec);
	}
	vec4 lightContribution = vec4(totalLight ,1.0);
	FragColor = lightContribution + mirrorTex;
}