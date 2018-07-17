#version 400

#define LIGHT_COUNT 5 
#define MAX_MIST_VISIBLE_AREA 1 
#define SHADOWMAP_BIAS 0.005 
#define PCF_SAMPLES 2
layout (location = 0) out vec4 FragColor;

uniform sampler2D backgroundTexture;
uniform sampler2D rTexture;
uniform sampler2D gTexture;
uniform sampler2D bTexture;
uniform sampler2D blendMap;
uniform sampler2D normalMapR;
uniform sampler2D normalMapG;
uniform sampler2D normalMapB;
uniform sampler2D normalMapBlack;
uniform sampler2D dirLightShadowMap;

uniform vec3 materialAmbient;
uniform vec3 materialDiffuse;

uniform bool sunEnable;
uniform vec3 sunAmbientColour;
uniform vec3 sunDiffuseColour;

uniform vec3 diffuseColour[LIGHT_COUNT];
uniform vec3 attenuation[LIGHT_COUNT];
uniform bool enableLight[LIGHT_COUNT];

uniform bool enableNMr;
uniform bool enableNMg;
uniform bool enableNMb;
uniform bool enableNMblack;

uniform bool mistEnable;
uniform vec3 mistColour;

in vec2 pass_textureCoordinates;
in vec3 surfaceNormal;
in vec3 toLightVecSimple[LIGHT_COUNT];
in vec3 toLightVecNormMap[LIGHT_COUNT];
in vec3 SunDirectionSimple;
in vec3 SunDirectionNormMap;
in float mistContribution;
in vec4 fragPosLightSpace;


vec3 phongModelDirectLight(vec3 normal, vec3 sunDirection)
{
	//Directional light calculations
	float nDotP = dot(normal, sunDirection);
	float sunBrightness = max(nDotP, 0.0);
	vec3 diffuseLight = sunBrightness * sunDiffuseColour * materialDiffuse;
	return diffuseLight;
}

vec3 phongModelPointLight(vec3 normal, vec3 toLightV, vec3 InAttenuation, vec3 diffuseColor)
{
		float distance = length(toLightV);
		float AttenuationLVL = 1.0 / (InAttenuation.x + (InAttenuation.y * distance) + (InAttenuation.z * (distance * distance)));
	//Diffuse light calculations
		vec3 normLightVec = normalize(toLightV);
		float nDotP = dot(normal, normLightVec);
		float brightness = max(nDotP, 0.0);
		vec3 diffuseLight = (brightness * diffuseColor * materialDiffuse) * AttenuationLVL;
		return diffuseLight;
}

vec4 retrieveNormal(float rComponent, float gComponent, float bComponent, float blackComponent,
		vec2 texCoords )
		{
			vec4 normalUnit = vec4(-100);

		   /* TO DO :
			* true - retrieve normal from normal map R
			* false - check other */
			if (rComponent >= gComponent && rComponent >= bComponent && rComponent >= blackComponent)
			{
				// IF red component normal map is enabled - retrieve normal , else - do nothing
				if (enableNMr == true) { normalUnit = texture(normalMapR, texCoords); }
			}

		   /* TO DO :
			* true - retrieve normal from normal map G
			* false - check other */
			else if (gComponent >= rComponent && gComponent >= bComponent && gComponent >= blackComponent)
			{
				// IF green component normal map is enabled - retrieve normal , else - do nothing
				if (enableNMg == true) { normalUnit = texture(normalMapG, texCoords); }
			}

		   /* TO DO :
			* true - retrieve normal from normal map B
			* false - check other */
			else if (bComponent >= rComponent && bComponent >= gComponent && bComponent >= blackComponent)
			{
				// IF blue component normal map is enabled - retrieve normal , else - do nothing
				if (enableNMb == true) { normalUnit = texture(normalMapB, texCoords); }
			}

		   /* TO DO :
			* true - retrieve normal from normal map Black
			* false - check other */
			else if (blackComponent >= rComponent && blackComponent >= gComponent && blackComponent >= bComponent)
			{
				// IF black component normal map is enabled - retrieve normal , else - do nothing
				if (enableNMblack == true) { normalUnit = texture(normalMapBlack, texCoords); }
			}
			return normalUnit;
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

void main(void)
{
    vec4 resultColour = vec4(mistColour, 1.0);

    if (mistContribution <= MAX_MIST_VISIBLE_AREA)
    {
	    /* * *	Generic definitions * * */

	    vec3 normSunDirection = vec3(0);
	    vec2 tiledCoords = pass_textureCoordinates * 40.0;
	    vec3 normNormal = vec3(0);
	    bool enableNM = false;

	    /* * *	Get total texture colour * * */

	    vec4 blendMapColour = texture2D(blendMap, pass_textureCoordinates);
	    float backTextureAmount = 1.0 - (blendMapColour.r + blendMapColour.g + blendMapColour.b);

	    vec4 backgroundColour = texture2D(backgroundTexture , tiledCoords) * backTextureAmount;
	    vec4 rTextureColour = texture2D(rTexture , tiledCoords) * blendMapColour.r;
	    vec4 gTextureColour = texture2D(gTexture , tiledCoords) * blendMapColour.g;
	    vec4 bTextureColour = texture2D(bTexture , tiledCoords) * blendMapColour.b;
	    vec4 totalColour = backgroundColour + rTextureColour + gTextureColour + bTextureColour;

	    /* * * IF normal map enabled - retrieve normal from normalMap * * */

	    if (enableNMr == true || enableNMg == true || enableNMb == true || enableNMblack == true)
	    {
	    	vec4 normalMapUnit = retrieveNormal(blendMapColour.r, blendMapColour.g, blendMapColour.b, backTextureAmount, tiledCoords);

	    	// If normal map isn't enabled for this component - disable normal mapping
	    	if (normalMapUnit.r == -100) {
	    		normNormal = normalize(surfaceNormal);
	    		normSunDirection = normalize(SunDirectionSimple);
	    		normSunDirection = -normSunDirection;
	    	}
	    	else {
	    		normSunDirection = normalize(SunDirectionNormMap);
	    		normSunDirection = -normSunDirection;
	    		normalMapUnit =  2.0 * normalMapUnit - 1.0;
	    		normNormal = normalize(normalMapUnit.rgb);
	    		enableNM = true;
	    	}
	    }
	    else {
	    	normSunDirection = normalize(SunDirectionSimple);
	    	normSunDirection = -normSunDirection;
	    	normNormal = normalize(surfaceNormal);
	    }

	    /* * *	Get total light contribution * * */

	    vec3 totalAmbientLight = sunAmbientColour * materialAmbient;
	    vec3 totalPointLight = vec3(0);

	    if (enableNM) {

	    	for (int i = 0;i < LIGHT_COUNT;i ++)
	    	{
	    		if (!enableLight[i]) continue;
	    		 totalPointLight = totalPointLight + phongModelPointLight(normNormal,
	    			 toLightVecNormMap[i], attenuation[i], diffuseColour[i]);
	    	}
	    }
	    else {

	    	for (int i = 0;i < LIGHT_COUNT;i ++)
	    	{
	    		if (!enableLight[i]) continue;
	    		 totalPointLight = totalPointLight + phongModelPointLight(normNormal,
	    			 toLightVecSimple[i], attenuation[i], diffuseColour[i]);
	    	}
	    }

	    //Directional light calculations
        vec3 totalDirectLight = vec3(0);
	    if (sunEnable)
        {
            float SunShadowFactor = GetSunShadowFactor();
            if (SunShadowFactor > 0.0)
            {
                totalDirectLight = phongModelDirectLight(normNormal, normSunDirection) * SunShadowFactor;
            }
        }

	    vec3 totalLight = totalDirectLight + totalPointLight + totalAmbientLight;

	    /* * *	Result * * */
	   resultColour = totalColour * vec4(totalLight, 1.0);
       if (mistEnable)
        resultColour = mix(vec4(mistColour, 1.0), resultColour, mistContribution);
    }

	FragColor = resultColour;
}
