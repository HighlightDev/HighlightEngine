#version 400

#define alphaRadiusSqr 100
layout (location = 0) out vec4 FragColor;

in vec3 vertexLocation;
in vec2 texCoordinates;

uniform sampler2D gridSampler;

void main()
{
	vec2 scaledTexCoords = texCoordinates * 150;
	float alpha = texture(gridSampler, scaledTexCoords).a;

	if (alpha < 0.1)
		discard;

	float dstFromCameraSqr = dot(vertexLocation, vertexLocation);
	float aspectRatioDstToRadius = dstFromCameraSqr / alphaRadiusSqr;

	alpha = clamp(0.8 - smoothstep(aspectRatioDstToRadius, 0.0, 0.5), 0.0, 1.0);

	FragColor = vec4(vec3(0.8), alpha);
}
