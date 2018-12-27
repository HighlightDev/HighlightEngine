#version 400

layout (points ) in; 
layout (triangle_strip, max_vertices = 4) out;

const float size = 20;

uniform mat4 projectionMatrix;

void main(void)
{
	gl_Position = projectionMatrix * (vec4(-size,-size,0.0,0.0) + 
						gl_in[0].gl_Position);
	EmitVertex();

	gl_Position = projectionMatrix * (vec4(size,-size,0.0,0.0) + 
						gl_in[0].gl_Position);
	EmitVertex();

	gl_Position = projectionMatrix * (vec4(-size,size,0.0,0.0) + 
						gl_in[0].gl_Position);
	EmitVertex();

	gl_Position = projectionMatrix * (vec4(size,size,0.0,0.0) + 
						gl_in[0].gl_Position);
	EmitVertex();

	EndPrimitive();
}
