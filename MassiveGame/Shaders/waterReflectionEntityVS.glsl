#version 400

layout (location = 0) in vec3 Position;
layout (location = 1) in vec3 Normals;
layout (location = 2) in vec2 TexCoords;
layout (location = 4) in vec3 Tangent;
layout (location = 5) in vec3 Bitangent;

uniform bool sunEnable;
uniform vec3 sunDirection;

uniform mat4 worldMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
uniform mat4 lightWorldMatrix;

uniform vec4 clipPlane;

out vec2 pass_textureCoordinates;
out vec3 surfaceDiffuseNormal;
out vec3 surfaceSpecularNormal;
out vec3 toCameraVec;
out vec3 SunDirection;

void main(void)
{ 
	SunDirection = sunDirection;
	vec3 EyeSpecularPosition = vec3(viewMatrix * worldMatrix * vec4(Position,1.0));
	vec3 WorldDiffusePosition = vec3(worldMatrix * vec4(Position,1.0));
	pass_textureCoordinates = TexCoords.xy;

	gl_ClipDistance[0] = dot(vec4(WorldDiffusePosition,1.0), clipPlane);// Clipping everything above the plane

	mat4 lightModelViewMatrix = viewMatrix * lightWorldMatrix;
	surfaceDiffuseNormal = (lightWorldMatrix * vec4(Normals, 0.0)).xyz;
	surfaceSpecularNormal = (lightModelViewMatrix * vec4(Normals, 0.0)).xyz;

	toCameraVec = vec3(0,0,1);	//Установим вектор наблюдателя в local Viewer

	vec3 normDiffuse = normalize(surfaceDiffuseNormal);
	vec3 tangDiffuse = normalize((lightWorldMatrix * vec4(Tangent, 0.0)).xyz); 
	vec3 bitangDiffuse = normalize((lightWorldMatrix * vec4(Bitangent, 0.0)).xyz);
	mat3 toTangentSpaceDiffuse = mat3(
		tangDiffuse.x, bitangDiffuse.x, normDiffuse.x,
		tangDiffuse.y, bitangDiffuse.y, normDiffuse.y,
		tangDiffuse.z, bitangDiffuse.z, normDiffuse.z
	);

	vec3 normSpecular = normalize(surfaceSpecularNormal);
	vec3 tangSpecular = normalize((lightModelViewMatrix * vec4(Tangent, 0.0)).xyz); 
	vec3 bitangSpecular = normalize((lightModelViewMatrix * vec4(Bitangent, 0.0)).xyz);
	mat3 toTangentSpaceSpecular = mat3(
		tangSpecular.x, bitangSpecular.x, normSpecular.x,
		tangSpecular.y, bitangSpecular.y, normSpecular.y,
		tangSpecular.z, bitangSpecular.z, normSpecular.z
	);

	toCameraVec = toTangentSpaceSpecular * toCameraVec;
	if (sunEnable) { SunDirection = toTangentSpaceDiffuse * SunDirection; }
	
	gl_Position = (projectionMatrix * vec4(EyeSpecularPosition, 1.0));
}


