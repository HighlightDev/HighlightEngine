#version 400

layout (location = 0) out vec4 FragColor;

uniform sampler2D backgroundTexture;
uniform sampler2D rTexture;
uniform sampler2D gTexture;
uniform sampler2D bTexture;
uniform sampler2D blendMap;

uniform vec3 materialAmbient;
uniform vec3 materialDiffuse;

uniform bool sunEnable;
uniform vec3 sunAmbientColour;
uniform vec3 sunDiffuseColour;

in vec2 pass_textureCoordinates;
in vec3 surfaceNormal;
in vec3 SunDirectionSimple;

vec3 phongModelDirectLight(vec3 normal, vec3 sunDirection)
{
	//Directional light calculations
	float nDotP = dot(normal, sunDirection);
	float sunBrightness = max(nDotP, 0.0);
	vec3 diffuseLight = sunBrightness * sunDiffuseColour * materialDiffuse;
	return diffuseLight;
}

void main(void)
{
	/* * *	Generic definitions * * */
	
	vec3 normSunDirection = normalize(-SunDirectionSimple);
	vec2 tiledCoords = pass_textureCoordinates * 40.0;
	vec3 normNormal = normalize(surfaceNormal);

	/* * *	Get total texture colour * * */

	vec4 blendMapColour = texture2D(blendMap, pass_textureCoordinates);
	float backTextureAmount = 1.0 - (blendMapColour.r + blendMapColour.g + blendMapColour.b);
	
	vec4 backgroundColour = texture2D(backgroundTexture , tiledCoords) * backTextureAmount;
	vec4 rTextureColour = texture2D(rTexture , tiledCoords) * blendMapColour.r;
	vec4 gTextureColour = texture2D(gTexture , tiledCoords) * blendMapColour.g;
	vec4 bTextureColour = texture2D(bTexture , tiledCoords) * blendMapColour.b;
	vec4 totalColour = backgroundColour + rTextureColour + gTextureColour + bTextureColour;
	
	/* * *	Get total light contribution * * */
	
	vec3 totalAmbientLight = sunAmbientColour * materialAmbient;

	//Directional light calculations
    vec3 totalDirectLight = vec3(0);
	if (sunEnable) 
    { 
        totalDirectLight = phongModelDirectLight(normNormal, normSunDirection); 
    }
    
	vec3 totalLight = totalDirectLight + totalAmbientLight;

	/* * *	Result * * */
	vec4 resultColour = totalColour * vec4(totalLight, 1.0);

	FragColor = resultColour;
}