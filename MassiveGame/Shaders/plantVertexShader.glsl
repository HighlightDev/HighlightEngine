#version 400

layout (location = 0) in vec3 Position;
layout (location = 1) in vec3 Normals;
layout (location = 2) in vec2 TexCoords;
layout (location = 6) in float wind;
layout (location = 7) in float textureUnit;
layout (location = 8) in vec4 mCol0;
layout (location = 9) in vec4 mCol1;
layout (location = 10) in vec4 mCol2;
layout (location = 11) in vec4 mCol3;

uniform mat4 ViewMatrix;
uniform mat4 ProjectionMatrix;
uniform vec3 windDirection;
uniform float windPower;
uniform float time;
uniform vec4 clipPlane;

uniform bool mistEnable;
uniform float mistDensity;
uniform float mistGradient;

out vec2 pass_textureCoordinates;
out vec3 surfaceNormal;
out float mistContribution;
out vec3 positionInEyeSpace;

float getMist(in float density, in float gradient, in vec4 eyePos)
{
	float mistContribution = 0.0;
	mistContribution = length(eyePos.xyz);
	mistContribution = exp(-pow((density * mistContribution), gradient));
	mistContribution = clamp(mistContribution, 0.0, 1.0);
	return mistContribution;
}

vec4 windCalculations(in vec2 texCoords)
{
	vec4 InComponent = vec4(0);
	if (texCoords.y < 0.7)
	{
		float currentLoop = wind + time;
		// Range is [0 - 360]
		currentLoop = currentLoop > 360.0 ? currentLoop - 360.0 : currentLoop;
		currentLoop = radians(currentLoop);
		InComponent = vec4(windDirection * (windPower * sin(currentLoop)), 0.0);
		InComponent.y = 0.0;
	}
	return InComponent;
}

void main(void)
{	
	mat4 ModelMatrix = mat4(
	mCol0, mCol1,
    mCol2, mCol3
	);
	
	vec4 worldPos = ModelMatrix * vec4(Position,1.0);
   
	gl_ClipDistance[0] = dot(worldPos, clipPlane);	// Clipping beyond the plane


	vec4 eyePos = ViewMatrix * worldPos;
    positionInEyeSpace = eyePos.xyz;
	pass_textureCoordinates = TexCoords;
	surfaceNormal = (ModelMatrix * vec4(Normals, 0.0)).xyz;
	
	// Mist calculations
	if (mistEnable)
	{
		mistContribution = getMist(mistDensity, mistGradient, eyePos);
	}

	eyePos += windCalculations(TexCoords);
	gl_Position = ProjectionMatrix * eyePos;
}