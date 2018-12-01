#version 400

#define MAX_LIGHT_COUNT 5 
#define SHADOWMAP_BIAS 0.005 
#define PCF_SAMPLES 2
layout (location = 0) out vec4 FragColor;

uniform sampler2D entitieTexture;
uniform sampler2D normalMap;
uniform sampler2D dirLightShadowMap;

uniform vec3 materialAmbient;
uniform vec3 materialDiffuse;
uniform vec3 materialSpecular;
uniform float materialReflectivity;
uniform float meterialShineDamper;

uniform bool sunEnable;
uniform vec3 sunAmbientColour;
uniform vec3 sunDiffuseColour;
uniform vec3 sunSpecularColour;

uniform vec3 diffuseColour[MAX_LIGHT_COUNT];
uniform vec3 specularColour[MAX_LIGHT_COUNT];
uniform vec3 attenuation[MAX_LIGHT_COUNT];
uniform bool enableLight[MAX_LIGHT_COUNT];

uniform vec3 mistColour;

uniform bool normalMapEnableDisable;

in vec2 pass_textureCoordinates;
in vec3 surfaceDiffuseNormal;
in vec3 surfaceSpecularNormal;
in vec3 toLightDiffuseVec[MAX_LIGHT_COUNT];
in vec3 toLightSpecularVec[MAX_LIGHT_COUNT];
in vec3 toCameraVec;
in vec3 SunDirection;
in vec4 fragPosLightSpace;

in float mistContribution;

vec3 phongModelDirectLight(vec3 diffuseNormal, vec3 specularNormal, vec3 toLightVector)
{
	//Directional light calculations
	float sDotN = dot(diffuseNormal, toLightVector);
	float sunBrightness = max(sDotN, 0.0);
	vec3 totalDirectDiffuse = sunBrightness * sunDiffuseColour * materialDiffuse;
	vec3 totalDirectSpecular = vec3(0.0);
	if (sunBrightness > 0.0)
	{
		vec3 normLightVec = toLightVector;
		vec3 halfWayVector = normalize(normLightVec + toCameraVec);
		float specularFactor = dot(halfWayVector, specularNormal);
		specularFactor = max(specularFactor, 0.0);
		float dampedFactor = pow(specularFactor, meterialShineDamper);
		totalDirectSpecular = (dampedFactor * sunSpecularColour * materialSpecular * materialReflectivity);
	}
	vec3 totalDirectLight = totalDirectSpecular + totalDirectDiffuse;
	return totalDirectLight;
}

vec3 phongModelPointLight(vec3 diffuseNormal, vec3 specularNormal, vec3 ligthDiffuseV, vec3 lightSpecularV, vec3 InAttenuation,
		vec3 diffuseColor, vec3 specularColor)
{
		vec3 totalSpecularColour = vec3(0);
		vec3 totalDiffuseColour = vec3(0);
		float lightDistance = length(ligthDiffuseV);
		float attenuationLVL = 1.0 / (InAttenuation.x + (InAttenuation.y * lightDistance) + (InAttenuation.z * (lightDistance * lightDistance)));
	//Diffuse light calculations
		vec3 normLightVec = normalize(ligthDiffuseV);
		float sDotN = dot(diffuseNormal,  normLightVec);
		float brightness = max(sDotN, 0.0);
		totalDiffuseColour = (brightness * diffuseColor * materialDiffuse) * attenuationLVL;
	//Specular light calculations
		if (brightness > 0.0) {
			vec3 normCameraV = normalize(toCameraVec);
			normLightVec = normalize(lightSpecularV);
			vec3 halfWayVector = normalize(normLightVec + normCameraV);
			float specularFactor = dot(halfWayVector, specularNormal);
			specularFactor = max(specularFactor, 0.0);
			float dampedFactor = pow(specularFactor, meterialShineDamper);
			totalSpecularColour = (dampedFactor * specularColor * materialSpecular * materialReflectivity) * attenuationLVL;
		}
		vec3 totalColour = totalSpecularColour + totalDiffuseColour;
		return totalColour;
}

float GetSunShadowFactor()
{
    float resultShadow = 0.0;
	vec3 shadowTexCoord = fragPosLightSpace.xyz / fragPosLightSpace.w;
    float actualDepth = shadowTexCoord.z - SHADOWMAP_BIAS;

    float SumDepth = 0.0;
    vec2 texelSize = 1.0 / textureSize(dirLightShadowMap, 0);
    int countSamples = 0;
    for (int x = -PCF_SAMPLES; x <= PCF_SAMPLES; x++)
    {
       for (int y = -PCF_SAMPLES; y <= PCF_SAMPLES; y++)
       {
            float pcfDepth = texture(dirLightShadowMap, shadowTexCoord.xy + vec2(x, y) * texelSize).r;
            resultShadow += actualDepth > pcfDepth ? 1.0 : 0.0;
            countSamples++;
       }
    }
    resultShadow /= countSamples;
    resultShadow = 1 - resultShadow;
	return resultShadow;
}

void main()
{
    vec4 textureColour = texture2D(entitieTexture, pass_textureCoordinates);

    vec3 DiffuseNormal = vec3(0);
	vec3 SpecularNormal = vec3(0);
	vec3 normSunDirection = normalize(-SunDirection);

	if (normalMapEnableDisable)
	{
		vec4 normalMapUnit =  2.0 * texture2D(normalMap, pass_textureCoordinates) - 1.0;
		SpecularNormal = DiffuseNormal = normalize(normalMapUnit.rgb);
	} else
	{
		DiffuseNormal = normalize(surfaceDiffuseNormal);
		SpecularNormal = normalize(surfaceSpecularNormal);
	}

	vec3 totalPointLight;
	vec3 totalAmbientColour = sunAmbientColour * materialAmbient;
	for (int i = 0;i < MAX_LIGHT_COUNT;i ++)
	{
		if (!enableLight[i]) continue;
		totalPointLight =  totalPointLight + phongModelPointLight(DiffuseNormal, SpecularNormal,
			toLightDiffuseVec[i], toLightSpecularVec[i], attenuation[i], diffuseColour[i], specularColour[i]);
	}
	totalPointLight = totalPointLight + totalAmbientColour;

	vec3 totalDirectLight = vec3(0);
    //Directional light calculations
	if (sunEnable)
    {
        float SunShadowFactor = GetSunShadowFactor();
        if (SunShadowFactor > 0.0)
        {
            totalDirectLight = phongModelDirectLight(DiffuseNormal, SpecularNormal, normSunDirection) * SunShadowFactor;
        }
    }
	vec3 totalLight = totalDirectLight + totalPointLight;

	vec4 resultColour = textureColour * vec4(totalLight, 1.0);

	FragColor = mix(vec4(mistColour, 1.0), resultColour, mistContribution);
}
