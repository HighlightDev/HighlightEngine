#version 400

#define HAS_PREVIOUS_STAGE 1 
#define lum vec3(0.2126, 0.7152, 0.0722) 
#define MAX_BLUR_WIDTH 10
layout (location = 0) out vec4 FragColor;

uniform sampler2D frameTexture;
uniform sampler2D bluredTexture;

#if HAS_PREVIOUS_STAGE
uniform sampler2D previousPostProcessResultSampler;
#endif

uniform int Ghosts;
uniform float GhostDispersal;
uniform float HaloWidth;
uniform float Distortion;
uniform float threshold;
uniform float Weight[MAX_BLUR_WIDTH];
uniform int PixOffset[MAX_BLUR_WIDTH];
uniform int screenHeight;
uniform int screenWidth;
uniform int blurWidth;

noperspective in vec2 texCoord;

subroutine vec4 LensFlare(vec2 TC);
subroutine uniform LensFlare lens;

float luma(vec4 color)
{
	return dot((color.rgb), lum);
}

vec4 textureDistorted(in sampler2D tex, in vec2 texcoord, in vec2 direction, in vec3 distortion)
{
	return vec4(
		texture(tex, texcoord + direction * distortion.r).r,
		texture(tex, texcoord + direction * distortion.g).g,
		texture(tex, texcoord + direction * distortion.b).b,
		1.0
	);
}

subroutine(LensFlare)
vec4 lensSimple(vec2 TC)
{
	vec4 texColor = texture(frameTexture, TC);
	return texColor;
}

subroutine(LensFlare)
vec4 lensThreshold(vec2 TC)
{
	vec4 texColor = texture(frameTexture, TC);
	if ( luma(texColor) > threshold )
	{
		return texColor;
	}
	else { return vec4(0, 0, 0, 1); }
}

subroutine(LensFlare)
vec4 lensEffect(vec2 TC)
{
    float precalculatedLength = 0.707106;
    vec2 deltaTexCoord = vec2(0.5) - TC;

	//Get size of one texel
	vec2 texelSize = 1.0 / vec2(textureSize(frameTexture, 0));

	//Ghost vector to image centre:
    vec2 ghostVec = deltaTexCoord * GhostDispersal;
    vec2 ghostVecNormalized = normalize(ghostVec);
	vec2 haloVec = ghostVecNormalized * HaloWidth;

	//Get distortion
	vec3 distortion = vec3(-texelSize.x * Distortion, 0.0, texelSize.x * Distortion);

	  //Sample ghosts:
	  vec4 result = vec4(0.0);
      for (int i = 0; i < Ghosts; ++i)
	  {
        vec2 offset = fract(TC + ghostVec * float(i));

		//Allowing only bright spots from the centre of the source image to generate ghosts
		float weight = length(vec2(0.5) - offset) / precalculatedLength;

		weight = pow(1.0 - weight, 20.0);

		result += textureDistorted(frameTexture, offset, ghostVecNormalized, distortion) * weight;
      }

	  //Add to lenses some color
	  //result *= lensColour;

	  //Sample halo
      vec2 fractTCnHaloVec = fract(TC + haloVec);
	  float weight = length(precalculatedLength - fractTCnHaloVec) / precalculatedLength;
	  weight = pow(1.0 - weight, 20.0);

	  result += textureDistorted(frameTexture, fractTCnHaloVec, ghostVecNormalized, distortion) * weight;

	return result;
}

subroutine(LensFlare)
vec4 vertBlur(vec2 TC)
{
	float dy = 1.0 / float(screenHeight);
	vec4 sum = texture(frameTexture, TC) * Weight[0];
	for( int i = 1; i < blurWidth; i++ )
	{
		sum += texture( frameTexture, TC + vec2(0.0, PixOffset[i]) * dy ) * Weight[i];
		sum += texture( frameTexture, TC - vec2(0.0, PixOffset[i]) * dy ) * Weight[i];
	}
	return sum;
}

subroutine(LensFlare)
vec4 horizBlur(vec2 TC)
{
	float dx = 1.0 / float(screenWidth);
	vec4 sum = texture(frameTexture, TC) * Weight[0];
	for( int i = 1; i < blurWidth; i++ )
	{
		sum += texture( frameTexture, TC + vec2(PixOffset[i], 0.0) * dx ) * Weight[i];
		sum += texture( frameTexture, TC - vec2(PixOffset[i], 0.0) * dx ) * Weight[i];
	}
	return sum;
}

subroutine(LensFlare)
vec4 lensModifer(vec2 TC)
{
    vec4 lensFlare = texture(bluredTexture, TC);
	vec4 resultColor = lensFlare;

#if HAS_PREVIOUS_STAGE
    resultColor += texture(previousPostProcessResultSampler, TC);
#endif

    return resultColor;
}

void main(void)
{
	//Invert y-axis texture coordinate
	vec2 texcoord = vec2(texCoord.x, 1 - texCoord.y);

	FragColor = lens(texcoord);
}
