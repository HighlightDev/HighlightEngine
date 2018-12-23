#version 400

#define rCoef 1.2 
#define gCoef 0.7692307 
#define bCoef 0.5555555
layout (location = 0) out vec4 FragColor;

in vec2 texCoord;

uniform sampler2D sunTexture1;
uniform sampler2D sunTexture2;
uniform vec3 sunDirection;

void main(void)
{
	vec4 tex1 = texture(sunTexture1, texCoord);
	vec4 tex2 = texture(sunTexture2, texCoord);

	//Sun zenith intense
	vec3 nSunDir = normalize(-sunDirection);
	nSunDir = vec3(0, nSunDir.y, 0);
	float sDotN = max( dot(nSunDir, vec3(0, 1, 0)), 0.0);
	float sunset = 1 - sDotN;

	//Measuring distance of radius
	float dx = texCoord.x - 0.5;
	float dy = texCoord.y - 0.5;
	float dist = sqrt(dx * dx + dy * dy);

	//Arc boundaries calculations
	float component2 = smoothstep(0.41, 0.2, dist);

	vec4 sunsetCoreColor = vec4(tex1.r * rCoef, tex1.g * gCoef, tex1.b * bCoef, 0.0);
	vec4 noonCoreColor = vec4(tex1.r + 0.3, tex1.g + 0.1, tex1.b + 0.1, 0.0);

	vec4 sunsetArcColor = vec4(tex2.r * rCoef , tex2.g * gCoef, tex2.b * bCoef, 0.0);
	vec4 noonArcColor = vec4(tex2.r + 0.3 , tex2.g / 1.05, tex2.b / 1.2, 0.0);

	vec4 coreColor = mix(noonCoreColor, sunsetCoreColor, sunset);
	vec4 arcColor = mix(noonArcColor, sunsetArcColor, sunset);

	if (dist >= 0.0 && dist <= 0.65)
	{
		FragColor = vec4(vec3(coreColor + arcColor) , component2);
	}
	//Outside
	if (dist > 0.65)
	{
		discard;
	}
}
