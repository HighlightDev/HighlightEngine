#version 400

layout (location = 0) out vec4 FragColor;

in vec2 texCoords;
in vec3 outNormal;
in vec3 cameraDir;

uniform sampler2D modelTexture;
uniform samplerCube environmentMap;

uniform vec3 IOR = vec3(1.45);

void main(void)
{
	// normalize vectors
	vec3 nNormal = normalize(outNormal);
	vec3 nCameraDir = normalize(cameraDir);

	// get all pixel results
	vec4 modelPixel = texture(modelTexture, texCoords);
	vec4 reflectionPixel = texture(environmentMap, reflect(nCameraDir, nNormal));
	vec4 refractionPixel = vec4(texture(environmentMap, refract(nCameraDir, nNormal, IOR.r)).r,
		texture(environmentMap, refract(nCameraDir, nNormal, IOR.g)).g,
		texture(environmentMap, refract(nCameraDir, nNormal, IOR.b)).b,
		1.0);

	// count Fresnel Effect
	float reflectionFactor = max(dot(nNormal, -nCameraDir), 0.0);

	// get result Colour :

	// without texture colour
	//FragColor = mix(reflectionPixel, refractionPixel, reflectionFactor);

	// with texture colour
	FragColor = mix(mix(reflectionPixel, refractionPixel, reflectionFactor), modelPixel, 0.36);
}