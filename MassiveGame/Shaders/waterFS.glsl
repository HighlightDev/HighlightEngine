#version 400

#define MAX_LIGHT_COUNT 5 
#define materialReflectivity 0.7 
#define materialShineDamper 100

layout (location = 0) out vec4 FragColor;

uniform sampler2D reflectionTexture;
uniform sampler2D refractionTexture;
uniform sampler2D dudvTexture;
uniform sampler2D normalMap;
uniform sampler2D depthTexture;
uniform float moveFactor;
uniform float waveStrength;
uniform vec3 sunSpecularColour;
uniform bool bEnableSun;
uniform vec3 specularColour[MAX_LIGHT_COUNT];
uniform vec3 attenuation[MAX_LIGHT_COUNT];
uniform bool enableLight[MAX_LIGHT_COUNT];
uniform float nearClipPlane;
uniform float farClipPlane;
uniform float transparencyDepth;

uniform bool mistEnable;
uniform vec3 mistColour;

in vec4 clipSpace;
in vec3 fresnelCamera;
in vec3 worldNormal;
in vec2 texCoords;
in vec3 eyeCameraVec;
in vec3 sunDir;
in vec3 toLightDiffuseVec[MAX_LIGHT_COUNT];
in float mistContribution;

vec4 specularDirectLight(vec3 specularNormal, vec3 fromLightVector, float softEdge)
{
	vec3 totalDirectSpecular = vec3(0.0);

	/*TO DO : if light is facing water plane
	  - add specular contribution*/

	vec3 unitCameraV = normalize(eyeCameraVec);
	vec3 normLightVec = fromLightVector;
	vec3 halfWayVector = normalize(normLightVec + unitCameraV);
	float specularFactor = dot(halfWayVector, specularNormal);
	specularFactor = max(specularFactor, 0.0);
	float dampedFactor = pow(specularFactor, materialShineDamper);
	totalDirectSpecular = (dampedFactor * sunSpecularColour * materialReflectivity) * softEdge;

	return vec4(totalDirectSpecular, 1.0);
}

vec4 specularPointLight(vec3 specularNormal, vec3 ligthDiffuseV, vec3 InAttenuation, vec3 specularColor, float softEdge)
{
		vec3 totalSpecularColour = vec3(0);
		float lightDistance = length(ligthDiffuseV);
		float attenuationLVL = 1.0 / (InAttenuation.x + (InAttenuation.y * lightDistance) + (InAttenuation.z * (lightDistance * lightDistance)));
		vec3 normDiffVec = normalize(ligthDiffuseV);
	//Specular light calculations

		if (max(dot(specularNormal,  normDiffVec), 0.0) > 0.0)
		{
			vec3 unitCameraV = normalize(eyeCameraVec);
			vec3 halfWayVector = normalize(normDiffVec + unitCameraV);
			float specularFactor = dot(halfWayVector, specularNormal);
			specularFactor = max(specularFactor, 0.0);
			float dampedFactor = pow(specularFactor, materialShineDamper);
			totalSpecularColour = (dampedFactor * specularColor * materialReflectivity) * attenuationLVL * softEdge;
		}
		return vec4(totalSpecularColour, 1.0);
}

void main(void)
{
	//Texture projection
	vec2 normDeviceCoord = ((clipSpace.xy / clipSpace.w) * 0.5) + 0.5;
	vec2 reflectionTexCoords = vec2(normDeviceCoord.x, normDeviceCoord.y);
	vec2 refractionTexCoords = normDeviceCoord;

	//Soft edges modifer

    // depth to ground
	float depth = texture(depthTexture, refractionTexCoords).r;
	float floorDistance = (2.0 * nearClipPlane * farClipPlane) / (farClipPlane + nearClipPlane - (2.0 * depth - 1.0) * (farClipPlane - nearClipPlane));

    // depth to water plane
	depth = gl_FragCoord.z;
	float waterDistance = (2.0 * nearClipPlane * farClipPlane) / (farClipPlane + nearClipPlane - (2.0 * depth - 1.0) * (farClipPlane - nearClipPlane));
	float waterDepth = floorDistance - waterDistance;

	//Distortion
	vec2 distortedTexCoords = texture(dudvTexture, vec2(texCoords.x + moveFactor, texCoords.y)).rg * 0.1;
	distortedTexCoords = texCoords + vec2(distortedTexCoords.x, distortedTexCoords.y + moveFactor);
	vec2 totalDistortion = (texture(dudvTexture, distortedTexCoords).rg * 2.0 - 1.0) * waveStrength * clamp(waterDepth / 20.0, 0.0, 1.0);

	refractionTexCoords += totalDistortion;
	refractionTexCoords = clamp(refractionTexCoords, 0.001, 0.999);

	reflectionTexCoords += totalDistortion;
    reflectionTexCoords = clamp(reflectionTexCoords, 0.001, 0.999);

	vec4 reflectionColor = texture(reflectionTexture, reflectionTexCoords);
	vec4 refractionColor = texture(refractionTexture, refractionTexCoords);

	//Fresnel effect
	vec3 nNormal = normalize(worldNormal);
	vec3 nCamera = normalize(fresnelCamera);
	float reflFactor = dot(nNormal, nCamera);

	//Normal mapping
	vec3 normSunDirection = normalize(sunDir);
	normSunDirection = -normSunDirection;
	vec3 SpecularNormal = vec3(0);
	vec4 normalMapUnit =  2.0 * texture2D(normalMap, totalDistortion ) - 1.0;
	SpecularNormal = normalize(normalMapUnit.rgb);

	vec4 waterColor = mix(reflectionColor, refractionColor, clamp(reflFactor - (waterDepth / transparencyDepth), 0.0, 1.0));
	vec4 lightColor = vec4(0);
    if (bEnableSun)
    {
        lightColor = specularDirectLight(SpecularNormal, normSunDirection, clamp(waterDepth / 5.0, 0.0, 1.0));
    }

	vec4 pointLightColor = vec4(0);

	float value = 0;
	for (int i = 0;i < MAX_LIGHT_COUNT;i ++)
	{
		if (!enableLight[i]) continue;
		pointLightColor =  pointLightColor + specularPointLight(SpecularNormal, toLightDiffuseVec[i], attenuation[i],
		 specularColour[i], clamp(waterDepth / 5.0, 0.0, 1.0));
	}

	lightColor += pointLightColor;

	vec4 resultColour = mix(waterColor, vec4(0.0, 0.3, 0.5, 1.0), 0.2) + lightColor;
	resultColour.a = clamp(waterDepth / 5.0, 0.0, 1.0); // Soft edges

	/*If mist enabled*/
	if (mistEnable)
	{
		FragColor = vec4(mix(mistColour, resultColour.xyz, mistContribution), resultColour.a);
	}
	/*If mist disabled*/
	else {
		FragColor = resultColour;
	}
}
