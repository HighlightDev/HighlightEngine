#version 400

#define HAS_PREVIOUS_STAGE 0 
#define zNearPlane 0.1 
#define zFarPlane 900 
#define MAX_BLUR_WIDTH 10
layout (location = 0) out vec4 FragColor;

subroutine vec4 postProc();
subroutine uniform postProc process;

in vec2 texCoord;

#if HAS_PREVIOUS_STAGE
    uniform sampler2D previousPostProcessResultSampler;
#else
    uniform sampler2D frameTexture;
#endif

uniform sampler2D blurTexture;
uniform sampler2D depthTexture;

uniform float Weight[MAX_BLUR_WIDTH];
uniform int PixOffset[MAX_BLUR_WIDTH];
uniform int screenHeight;
uniform int screenWidth;
uniform int blurWidth;

uniform float blurStartEdge;
uniform float blurEndEdge;

float ToLinearDepth(float nonLinearDepth)
{
    float linearDepth = (2.0 * zNearPlane) / (zFarPlane + zNearPlane - nonLinearDepth * (zFarPlane - zNearPlane));
    return linearDepth;
}

//Downsample frame texture
subroutine(postProc)
vec4 downsampling()
{
    return texture(blurTexture, texCoord);
}

//Blur vertical Pass
subroutine(postProc)
vec4 verticalBlur()
{
	float dy = 1.0 / float(screenHeight);
	vec4 sum = texture(blurTexture, texCoord) * Weight[0];
	for( int i = 1; i < blurWidth; i++ )
	{
		sum += texture( blurTexture, texCoord + vec2(0.0, PixOffset[i]) * dy ) * Weight[i];
		sum += texture( blurTexture, texCoord - vec2(0.0, PixOffset[i]) * dy ) * Weight[i];
	}
	return sum;
}

//Gauss horizontal Pass
subroutine(postProc)
vec4 horizontalBlur()
{
	float dx = 1.0 / float(screenWidth);
	vec4 sum = texture(blurTexture, texCoord) * Weight[0];
	for( int i = 1; i < blurWidth; i++ )
	{
		sum += texture( blurTexture, texCoord + vec2(PixOffset[i], 0.0) * dx ) * Weight[i];
		sum += texture( blurTexture, texCoord - vec2(PixOffset[i], 0.0) * dx ) * Weight[i];
	}
	return sum;
}

//Depth of Field blur
subroutine (postProc)
vec4 depthOfField()
{
    vec4 dstColorSample = vec4(0);

#if HAS_PREVIOUS_STAGE
    dstColorSample = texture(previousPostProcessResultSampler, texCoord);
#else
    dstColorSample = texture(frameTexture, texCoord);
#endif

	vec4 bluredColorSample = texture(blurTexture, texCoord);
    vec4 depthSample = texture(depthTexture, texCoord);

    float nonLinearDepth = 2.0 * (depthSample.r) - 1.0;
    float linearDepth = ToLinearDepth(nonLinearDepth);

	float stepValue = smoothstep(blurStartEdge, blurEndEdge, linearDepth);

	return mix(dstColorSample, bluredColorSample, stepValue);
}

void main(void)
{
	FragColor = process();
}
