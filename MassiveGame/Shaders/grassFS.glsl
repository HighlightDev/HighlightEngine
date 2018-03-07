#version 400

layout (location = 0) out vec4 FragColor;

uniform sampler2D grassTexture;
uniform vec3 sunDirection;

uniform vec3 sunAmbientColour;
uniform vec3 sunDiffuseColour;

smooth in vec4 color;
smooth in vec2 texCoord;

const vec3 normal = vec3(0, 1, 0);

vec3 directLight(in vec3 normal, in vec3 sunDir, in vec3 sunDiffuseColour)
{
	float albedo = max(dot(normal, sunDir), 0.0);
	return (albedo * sunDiffuseColour);
}

void main()
{
	// light preparation
	vec3 normSunDir = normalize(-sunDirection);
	
	vec4 resultColor = texture(grassTexture, vec2(texCoord.x, 1 - texCoord.y));
	vec4 resultLight = vec4(directLight(normal, normSunDir, sunDiffuseColour) + sunAmbientColour, 1.0);
	FragColor = resultColor * resultLight;
}