#version 400

layout (location = 0) in vec3 Vertex;
layout (location = 1) in vec3 Normals;
layout (location = 2) in vec2 TexCoords;
layout (location = 4) in vec3 Tangent;
layout (location = 5) in vec3 Bitangent;

uniform vec3 sunDirection;

uniform mat4 ModelMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ProjectionMatrix;
uniform vec4 clipPlane;

out vec2 pass_textureCoordinates;
out vec3 surfaceNormal;
out vec3 SunDirectionSimple;

void main(void)
{
	SunDirectionSimple = sunDirection;

	vec3 WorldPosition = vec3(ModelMatrix * vec4(Vertex,1.0));

	gl_ClipDistance[0] = dot(vec4(WorldPosition, 1.0), clipPlane);	// Clipping beyond the plane

	pass_textureCoordinates = TexCoords.xy;

	// Calculate world normal
	surfaceNormal = (ModelMatrix * vec4(Normals, 0.0)).xyz;

	gl_Position = ProjectionMatrix * ViewMatrix * vec4(WorldPosition,1.0);
}