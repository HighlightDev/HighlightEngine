#version 330

#define SIZE 1.4
layout (points ) in;
layout (triangle_strip, max_vertices = 4) out;

smooth out vec2 texCoords;

uniform mat4 projectionMatrix;

void pointLight()
{
	gl_Position = projectionMatrix * (vec4(-SIZE, - SIZE, 0.0, 0.0) + gl_in[0].gl_Position);
	texCoords = vec2(0.0, 1.0);
	EmitVertex();

	gl_Position = projectionMatrix * (vec4(SIZE, - SIZE, 0.0, 0.0) + gl_in[0].gl_Position);
	texCoords = vec2(1.0, 1.0);
	EmitVertex();

	gl_Position = projectionMatrix * (vec4(-SIZE,  SIZE, 0.0, 0.0) + gl_in[0].gl_Position);
	texCoords = vec2(0.0, 0.0);
	EmitVertex();

	gl_Position = projectionMatrix * (vec4(SIZE,  SIZE, 0.0, 0.0) + gl_in[0].gl_Position);
	texCoords = vec2(1.0, 0.0);
	EmitVertex();

	EndPrimitive();
}

void main(void)
{
	pointLight();
}
