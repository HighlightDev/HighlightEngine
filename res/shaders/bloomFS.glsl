#version 400

#define HAS_PREVIOUS_STAGE 1 
#define lum vec3(0.2126, 0.7152, 0.0722) 
#define MAX_BLUR_WIDTH 10
layout (location = 0) out vec4 FragColor;

in vec2 texCoord;

subroutine vec4 postProc();
subroutine uniform postProc process;

uniform sampler2D frameTexture;
uniform sampler2D blurTexture;

#if HAS_PREVIOUS_STAGE
uniform sampler2D previousPostProcessResultSampler;
#endif

uniform float Weight[MAX_BLUR_WIDTH];
uniform int PixOffset[MAX_BLUR_WIDTH];
uniform int screenHeight;
uniform int screenWidth;
uniform int blurWidth;
uniform float bloomThreshold;

float luma(vec3 color)
{
	return dot(lum, color);
}

//Blur vertical Pass
subroutine(postProc)
vec4 verticalBlur()
{
	float dy = 1.0 / float(screenHeight);
	vec4 sum = texture(frameTexture, texCoord) * Weight[0];
	for( int i = 1; i < blurWidth; i++ )
	{
		sum += texture( frameTexture, texCoord + vec2(0.0, PixOffset[i]) * dy ) * Weight[i];
		sum += texture( frameTexture, texCoord - vec2(0.0, PixOffset[i]) * dy ) * Weight[i];
	}
	return sum;
}

//Gauss horizontal Pass
subroutine(postProc)
vec4 horizontalBlur()
{
	float dx = 1.0 / float(screenWidth);
	vec4 sum = texture(frameTexture, texCoord) * Weight[0];
	for( int i = 1; i < blurWidth; i++ )
	{
		sum += texture( frameTexture,texCoord + vec2(PixOffset[i], 0.0) * dx ) * Weight[i];
		sum += texture( frameTexture,texCoord - vec2(PixOffset[i], 0.0) * dx ) * Weight[i];
	}
	return sum;
}

//Bloom (get bright parts of image)
subroutine (postProc)
vec4 extractBrightParts()
{
	vec4 imgColor = texture(frameTexture, texCoord);
	if (luma(imgColor.rgb) <= bloomThreshold)
    {
        imgColor = vec4(0);
    }

    return imgColor;
}

//Blend bloom postprocess with previous post process result if it exists
subroutine(postProc)
vec4 endBloom()
{
	vec4 resultColor = texture(blurTexture, texCoord);

    #if HAS_PREVIOUS_STAGE
        resultColor += texture(previousPostProcessResultSampler, texCoord);
    #endif

	return resultColor;
}

void main(void)
{
	FragColor = process();
}
