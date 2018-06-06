#version 400

#define zenith vec3(0, 1, 0) 
#define UPPER_LIMIT 30.0 
#define LOWER_LIMIT 0.0
layout (location = 0) out vec4 FragColor;

uniform samplerCube skyboxSampler;
uniform samplerCube skyboxSampler2;
uniform vec3 sunPosition;
uniform bool sunEnable;

uniform bool mistEnable;
uniform vec3 mistColour;

in vec3 passTextureCoord;

float getMist(vec3 texCoord)
{
	float factor = (texCoord.y -  LOWER_LIMIT) / (UPPER_LIMIT - LOWER_LIMIT);
	factor = clamp(factor, 0.0, 1.0);
	return factor;
}

void main(void)
{
	vec4 texDayColour = texture(skyboxSampler, passTextureCoord);
	vec4 texNightColour = texture(skyboxSampler2, passTextureCoord);

	vec4 resultColour = vec4(0);

	//Day cycle calculations
	if (sunEnable) {
		vec3 nSunPos = normalize(sunPosition);
		nSunPos.x = 0.0; nSunPos.z = 0.0;
		vec3 nZenith = normalize(zenith);
		float zDotS = dot(nZenith, nSunPos);
		float nightComponent = zDotS + 0.4;
		nightComponent = max(nightComponent, 0.0);

		//Suset color calculations
		zDotS = -zDotS;
		zDotS = smoothstep(/*suset start phase*/ 0.6 , /*suset end phase*/ 1 - (zDotS + 1.8), zDotS );
		vec4 sunsetColour = vec4(0.8, 0.45, 0.25, 1.0) * zDotS;
		sunsetColour *= smoothstep(1 - (zDotS - 0.8),1 - (zDotS - 0.4), zDotS);

		resultColour = mix(texDayColour + sunsetColour, texNightColour, nightComponent);
	}
	else { resultColour = texDayColour; }

	/*If mist enabled*/
	if (mistEnable)
	{
		FragColor = mix(vec4(mistColour, 1.0), resultColour, getMist(passTextureCoord));
	}
	else { FragColor = resultColour; }
}
