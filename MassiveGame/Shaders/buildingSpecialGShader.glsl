#version 400

layout (triangles ) in;
layout (line_strip, max_vertices = 6) out;

in vec3 normalV[];

const float MAGNITUDE = 1.6f;

void main(void)
{
	gl_Position = gl_in[0].gl_Position;
	EmitVertex();

	gl_Position = gl_in[0].gl_Position + vec4(normalV[0] * MAGNITUDE, 0.0);
	EmitVertex();

	EndPrimitive();

	gl_Position = gl_in[1].gl_Position;
	EmitVertex();

	gl_Position = gl_in[1].gl_Position + vec4(normalV[1] * MAGNITUDE, 0.0);
	EmitVertex();

	EndPrimitive();

	gl_Position = gl_in[2].gl_Position;
	EmitVertex();

	gl_Position = gl_in[2].gl_Position + vec4(normalV[2] * MAGNITUDE, 0.0);
	EmitVertex();

	EndPrimitive();
}

