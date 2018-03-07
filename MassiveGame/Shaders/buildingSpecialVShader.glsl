#version 400

layout (location = 0) in vec3 Position;
layout (location = 1) in vec3 Normals;

uniform mat4 ModelMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ProjectionMatrix;
uniform mat3 NormalMatrix;

out vec3 normalV;

void main(void)
{ 
	normalV = vec3(NormalMatrix * Normals);
	normalV = normalize(vec3(ProjectionMatrix * vec4(normalV, 1.0)));
	gl_Position = ProjectionMatrix * ViewMatrix * ModelMatrix * vec4(Position, 1.0);
}


