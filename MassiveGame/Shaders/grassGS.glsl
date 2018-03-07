#version 330

layout (points ) in;
layout (triangle_strip, max_vertices = 15) out;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

uniform float angle;

const float radius = 0.08;
const float WIDTH = 0.032;
const float HEIGHT_PARAMETER = 0.19;

/*
 old parameters

const float radius = 0.08;
const float WIDTH = 0.025;
const float HEIGHT_PARAMETER = 0.15;

*/

smooth out vec4 color;
smooth out vec2 texCoord;

void main()
{
	mat4 mvpMatrix = projectionMatrix * viewMatrix * modelMatrix;

	float wZ = cos(angle);

	float yWind[7];
	yWind[0] = -(abs((wZ * radius * 0.5) * (radius * 0.5)));
	yWind[1] = -(abs((wZ * radius * 1) *   (radius * 1)));
	yWind[2] = -(abs((wZ * radius * 1.5) * (radius * 1.5)));
	yWind[3] = -(abs((wZ * radius * 2) *   (radius * 2)));
	yWind[4] = -(abs((wZ * radius * 2.5) * (radius * 2.5)));
	yWind[5] = -(abs((wZ * radius * 4.5) * (radius * 4.5)));
	yWind[6] = -(abs((wZ * radius * 4.9) * (radius * 4.9)));

	float zWind[7];
	zWind[0] = ((wZ * radius * 1) * (radius * 1));
	zWind[1] = ((wZ * radius * 2) * (radius * 2));
	zWind[2] = ((wZ * radius * 3) * (radius * 3));
	zWind[3] = ((wZ * radius * 4) * (radius * 4));
	zWind[4] = ((wZ * radius * 5) * (radius * 5));
	zWind[5] = ((wZ * radius * 7) * (radius * 7));
	zWind[6] = ((wZ * radius * 7.5) * (radius * 7.5));

	// 1
	gl_Position = mvpMatrix * (vec4(-WIDTH, 0.0, 0.0, 0.0) + gl_in[0].gl_Position);
	color = vec4(0.305, 0.65, 0.345, 1);
		texCoord = vec2(0, -1);
	EmitVertex();

	// 2
	gl_Position = mvpMatrix * (vec4(WIDTH, 0.0, 0.0, 0.0) + gl_in[0].gl_Position);
	color = vec4(0.305, 0.65, 0.345, 1);
		texCoord = vec2(1, -1);
	EmitVertex();

	// 3 
	gl_Position = mvpMatrix * (vec4(-WIDTH * 0.9, (HEIGHT_PARAMETER * 2) + yWind[0], 0.0 + zWind[0], 0.0) + gl_in[0].gl_Position);
	color = vec4(0.305, 0.73, 0.345, 1);
		texCoord = vec2(0, -1 + (1 / 7));
	EmitVertex();

	// 4
	gl_Position = mvpMatrix * (vec4(WIDTH * 0.9, (HEIGHT_PARAMETER * 2) + yWind[0], 0.0 + zWind[0], 0.0) + gl_in[0].gl_Position);
	color = vec4(0.305, 0.73, 0.345, 1);
		texCoord = vec2(1, -1 + (1 / 7));
	EmitVertex();

	// 5
	gl_Position = mvpMatrix * (vec4(-WIDTH * 0.85, (HEIGHT_PARAMETER * 3) + yWind[1], 0.0 + zWind[1], 0.0) + gl_in[0].gl_Position);
	color = vec4(0.315, 0.75, 0.345, 1);
		texCoord = vec2(0, -1 + (2 * (1 / 7)));
	EmitVertex();

	// 6
	gl_Position = mvpMatrix * (vec4(WIDTH * 0.85, (HEIGHT_PARAMETER * 3) + yWind[1], 0.0 + zWind[1], 0.0) + gl_in[0].gl_Position);
	color = vec4(0.315, 0.75, 0.345, 1);
		texCoord = vec2(1, -1 + (2 * (1 / 7)));
	EmitVertex();

	// 7
	gl_Position = mvpMatrix * (vec4(-WIDTH * 0.75, (HEIGHT_PARAMETER * 4) + yWind[2], 0.0 + zWind[2], 0.0) + gl_in[0].gl_Position);
	color = vec4(0.315, 0.76, 0.346, 1);
		texCoord = vec2(0, -1 + (3 * (1 / 7)));
	EmitVertex();

	// 8
	gl_Position = mvpMatrix * (vec4(WIDTH * 0.75, (HEIGHT_PARAMETER * 4) + yWind[2], 0.0 + zWind[2], 0.0) + gl_in[0].gl_Position);
	color = vec4(0.315, 0.76, 0.346, 1);
		texCoord = vec2(1, -1 + (3 * (1 / 7)));
	EmitVertex();

	// 9
	gl_Position = mvpMatrix * (vec4(-WIDTH * 0.65, (HEIGHT_PARAMETER * 5) + yWind[3], 0.0 + zWind[3], 0.0) + gl_in[0].gl_Position);
	color = vec4(0.318, 0.77, 0.347, 1);
		texCoord = vec2(0, -1 + (4 * (1 / 7)));
	EmitVertex();

	// 10
	gl_Position = mvpMatrix * (vec4(WIDTH * 0.65, (HEIGHT_PARAMETER * 5) + yWind[3], 0.0 + zWind[3], 0.0) + gl_in[0].gl_Position);
	color = vec4(0.318, 0.77, 0.347, 1);
		texCoord = vec2(1, -1 + (4 * (1 / 7)));
	EmitVertex();

	// 11
	gl_Position = mvpMatrix * (vec4(-WIDTH * 0.55, (HEIGHT_PARAMETER * 6) + yWind[4], 0.0 + zWind[4], 0.0) + gl_in[0].gl_Position);
	color = vec4(0.34, 0.79, 0.357, 1);
		texCoord = vec2(0, -1 + (5 * (1 / 7)));
	EmitVertex();

	// 12
	gl_Position = mvpMatrix * (vec4(WIDTH * 0.55, (HEIGHT_PARAMETER * 6) + yWind[4], 0.0 + zWind[4], 0.0) + gl_in[0].gl_Position);
	color = vec4(0.34, 0.79, 0.357, 1);
		texCoord = vec2(1, -1 + (5 * (1 / 7)));
	EmitVertex();

	// 13
	gl_Position = mvpMatrix * (vec4(-WIDTH * 0.45, (HEIGHT_PARAMETER * 7.5) + yWind[5], 0.0 + zWind[5], 0.0) + gl_in[0].gl_Position);
	color = vec4(0.345, 0.79, 0.36, 1);
		texCoord = vec2(0, -1 + (6 * (1 / 7)));
	EmitVertex();

	// 14
	gl_Position = mvpMatrix * (vec4(WIDTH * 0.45, (HEIGHT_PARAMETER * 7.5) + yWind[5], 0.0 + zWind[5], 0.0) + gl_in[0].gl_Position);
	color = vec4(0.345, 0.79, 0.36, 1);
		texCoord = vec2(1, -1 + (6 * (1 / 7)));
	EmitVertex();

	// 15
	gl_Position = mvpMatrix * (vec4(0.0, (HEIGHT_PARAMETER * 7.9) + yWind[6], 0.0 + zWind[6], 0.0) + gl_in[0].gl_Position);
	color = vec4(0.347, 0.795, 0.365, 1);
		texCoord = vec2(0.5, 0);
	EmitVertex();

	EndPrimitive();
}