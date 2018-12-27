#version 400

layout (location = 0) out vec4 FragColor;

uniform sampler2D albedo;
uniform sampler2D normalMap;
uniform sampler2D specularMap;

uniform vec3 matAmbient;
uniform vec3 matDiffuse;
uniform vec3 matSpecular;
uniform float matReflectivity;
uniform float matShineDamper;

uniform bool bSunEnable;
uniform bool bNormalMapEnable = false;
uniform bool bSpecularMapEnable = false;
uniform vec3 sunAmbientColour;
uniform vec3 sunDiffuseColour;
uniform vec3 sunSpecularColour;

in vec2 pass_textureCoordinates;
in vec3 surfaceDiffuseNormal;
in vec3 surfaceSpecularNormal;
in vec3 toCameraVec;
in vec3 SunDirection;

vec3 phongModelDirectLight(vec3 diffuseNormal, vec3 specularNormal, vec3 toLightVector, vec3 specularMaterial)
{    
	//Directional light calculations
	float sDotN = dot(diffuseNormal, toLightVector);
	float sunBrightness = max(sDotN, 0.0);
	vec3 totalDirectDiffuse = sunBrightness * sunDiffuseColour * matDiffuse;
	vec3 totalDirectSpecular = vec3(0.0);
	if (sunBrightness > 0.0)	//≈сли источник света освещает поверхность
	{
		vec3 normLightVec = toLightVector;
		vec3 halfWayVector = normalize(normLightVec + toCameraVec);
		float specularFactor = dot(halfWayVector, specularNormal);
		specularFactor = max(specularFactor, 0.0);
		float dampedFactor = pow(specularFactor, matShineDamper);
		totalDirectSpecular = (dampedFactor * sunSpecularColour * specularMaterial * matReflectivity);
	}
	vec3 totalDirectLight = totalDirectSpecular + totalDirectDiffuse;
	return totalDirectLight;
}

void main()
{
    vec4 textureColour = texture(albedo, pass_textureCoordinates);    

    vec3 DiffuseNormal = vec3(0);
	vec3 SpecularNormal = vec3(0);
	vec3 normSunDirection = normalize(-SunDirection);

	if (bNormalMapEnable)
	{
		vec4 normalMapUnit =  2.0 * texture2D(normalMap, pass_textureCoordinates) - 1.0;
		SpecularNormal = DiffuseNormal = normalize(normalMapUnit.rgb);
	}
	else
	{
		DiffuseNormal = normalize(surfaceDiffuseNormal);
		SpecularNormal = normalize(surfaceSpecularNormal);
	}

	vec3 totalAmbientColour = sunAmbientColour * matAmbient;

    vec3 specularMaterialComponent = texture(specularMap, pass_textureCoordinates).rgb;
    if (!bSpecularMapEnable) // if specular map wasn't binded
    {
        specularMaterialComponent = matSpecular;
    }

	vec3 totalDirectLight = vec3(0);
    //Directional light calculations
	if (bSunEnable) 
    { 
        totalDirectLight = phongModelDirectLight(DiffuseNormal, SpecularNormal, normSunDirection, specularMaterialComponent); 
    }
	vec3 totalLight = totalDirectLight + totalAmbientColour;

	vec4 resultColour = textureColour * vec4(totalLight, 1.0);

	FragColor = resultColour;
}