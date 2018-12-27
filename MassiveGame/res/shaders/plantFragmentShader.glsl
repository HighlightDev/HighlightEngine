#version 400

#define MAX_MIST_VISIBLE_AREA 1
layout (location = 0) out vec4 FragColor;

uniform sampler2D plantTexture;

uniform vec3 materialAmbient;
uniform vec3 materialDiffuse;

uniform bool sunEnable;
uniform vec3 sunDirection;
uniform vec3 sunDiffuseColour;
uniform vec3 sunAmbientColour;

uniform bool mistEnable;
uniform vec3 mistColour;

in vec2 pass_textureCoordinates;
in vec3 surfaceNormal;
in float mistContribution;
in vec3 positionInEyeSpace;

vec3 lightCalculations(in vec3 normal, in vec3 sunDirection)
{
	float sDotN = max( dot(normal, sunDirection), 0.0);
	vec3 diffuseLight = sDotN * sunDiffuseColour * materialDiffuse;
	return diffuseLight;
}

void main(void)
{
    vec4 resultColour = vec4(mistColour, 1.0);

    if (mistContribution <= MAX_MIST_VISIBLE_AREA)
    {
	    vec3 normSunDirection = normalize(sunDirection);
	    normSunDirection = -normSunDirection;
	    vec3 normNormal = normalize(surfaceNormal);
	    vec3 totalAmbientColour = materialAmbient * sunAmbientColour;
	    vec3 totalDiffuseColour = vec3(0.0);
	    if (sunEnable) {
	    	if (gl_FrontFacing)	//Front face calculations
	    	{
	    		totalDiffuseColour = lightCalculations(normNormal, normSunDirection);
	    	}
	    //Back face calcualtions
	    	else {
	    		totalDiffuseColour = lightCalculations(-normNormal, normSunDirection);
	    	}
	    }
	    vec4 totalLightColour = (vec4(totalAmbientColour,1.0) + vec4(totalDiffuseColour,1.0));	//Calculated total light
	    vec4 textureColour = texture2D(plantTexture, pass_textureCoordinates);
	    if (textureColour.a < 0.5)
	    {
	    	discard;
	    }

	    resultColour = textureColour * totalLightColour;

	    /*If is mist enabled*/
	    if (mistEnable)
	    	resultColour = mix(vec4(mistColour, 1.0), resultColour, mistContribution);
    }
    else
        discard;

    FragColor = resultColour;
}
