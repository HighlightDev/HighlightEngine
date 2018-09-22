#version 400

#define UPPER_LIMIT 30 
#define LOWER_LIMIT 0 
#define COMPLEX_SKYBOX 1
layout (location = 0) out vec4 FragColor;

uniform samplerCube daySampler;
#if COMPLEX_SKYBOX
	uniform samplerCube nightSampler;
	uniform float dayCycleValue;
#endif
uniform bool mistEnable;
uniform vec3 mistColour;

in vec3 passTextureCoord;

float getMist(vec3 texCoord)
{
	float factor = (texCoord.y -  LOWER_LIMIT) / (UPPER_LIMIT - LOWER_LIMIT);
	factor = clamp(factor, 0.0, 1.0);
	return factor;
}

#if COMPLEX_SKYBOX
	vec4 GetSkyboxColour(in vec4 dayColour)
	{
		vec4 texNightColour = texture(nightSampler, passTextureCoord);

		//Day cycle calculations
		float sunAltitude = dayCycleValue;
		float nightComponent = sunAltitude + 0.4;
		nightComponent = max(nightComponent, 0.0);

		//Suset color calculations
		sunAltitude = -sunAltitude;
		sunAltitude = smoothstep(/*suset start phase*/ 0.6 , /*suset end phase*/ 1 - (sunAltitude + 1.8), sunAltitude);
		vec4 sunsetColour = vec4(0.8, 0.45, 0.25, 1.0) * sunAltitude;
		sunsetColour *= smoothstep(1 - (sunAltitude - 0.8), 1 - (sunAltitude - 0.4), sunAltitude);

		return mix(dayColour + sunsetColour, texNightColour, nightComponent);
	}
#endif

void main(void)
{
	vec4 texDayColour = texture(daySampler, passTextureCoord);

	vec4 resultColour = vec4(0);
	#if COMPLEX_SKYBOX
		resultColour = GetSkyboxColour(texDayColour);
	#else
		resultColour = texDayColour;
	#endif

	/*If mist enabled*/
	if (mistEnable)
	{
		FragColor = mix(vec4(mistColour, 1.0), resultColour, getMist(passTextureCoord));
	}
	else
	{
		FragColor = resultColour;
	}
}
