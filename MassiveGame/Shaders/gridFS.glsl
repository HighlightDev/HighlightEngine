#version 400

#define alphaRadiusSqr 100
layout (location = 0) out vec4 FragColor;

in vec3 vertexLocation;

void main()
{
	float dstFromCameraSqr = dot(vertexLocation, vertexLocation);
	float aspectRatioDstToRadius = dstFromCameraSqr / alphaRadiusSqr;

	float alpha = clamp(1 - smoothstep(aspectRatioDstToRadius, 0, 1), 0, 1);

	FragColor = vec4(vec3(0.8), alpha);
}
