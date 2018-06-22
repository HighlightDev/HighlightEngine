#version 400

#define Exposure 0.35 
#define MAX_BLUR_WIDTH 10 
#define lum vec3(0.2126, 0.7152, 0.0722) 
#define rgbTOxyz mat3(0.6326696, 0.2045558, 0.1269946,0.2284569, 0.7373523, 0.0341908,0.0000000, 0.0095142, 0.8156958) 
#define xyzTOrgb mat3(1.7552599, -0.4836786, -0.2530000,-0.5441336, 1.5068789, 0.0215528,0.0063467, -0.0175761, 1.2256959) 
#define White 0.928
layout (location = 0) out vec4 FragColor;

subroutine vec4 postProc();
subroutine uniform postProc process;

in vec2 texCoord;

uniform sampler2D frameTexture;
uniform sampler2D blurTexture;
uniform sampler2D depthTexture;
uniform float AveLum;
uniform float Weight[MAX_BLUR_WIDTH];
uniform int PixOffset[MAX_BLUR_WIDTH];
uniform int screenHeight;
uniform int screenWidth;
uniform int blurWidth;
uniform float blurStartEdge;
uniform float blurEndEdge;
uniform float bloomThreshold;

float luma(vec3 color)
{
	return dot(lum, color);
}

subroutine(postProc)
vec4 HDR()
{
	vec2 texC = vec2(texCoord.x, 1 - texCoord.y);
	/*Sampling texture*/
	vec4 color = texture(frameTexture, texC);
	/*Convert to XYZ format*/
	vec3 xyzCol = rgbTOxyz * vec3(color);

	/*Convert to xyY format*/
	float xyzSum = xyzCol.x + xyzCol.y + xyzCol.z;
	vec3 xyYCol = vec3(0.0);
	if (xyzSum > 0.0) //Except zero division
	{
		xyYCol = vec3(xyzCol.x / xyzSum, xyzCol.y / xyzSum, xyzCol.y);
	}
	/*Apply tone compression operator*/
	float L = (Exposure * xyYCol.z) / AveLum;

	L = (L * (1 + L / (White * White) )) / (1 + L);

	/*Save new  brightness, converting back to XYZ*/
	if (xyYCol.y > 0.0) {
		xyzCol.x = (L * xyYCol.x) / (xyYCol.y);
		xyzCol.y = L;
		xyzCol.z = (L * (1 - xyYCol.x - xyYCol.y)) / xyYCol.y;
	}
	vec4 hdrColor = vec4(xyzTOrgb * xyzCol, 1.0);
	return hdrColor;
}

//Blur vertical Pass
subroutine(postProc)
vec4 blur1()
{
	vec2 texC = vec2(texCoord.x, 1 - texCoord.y);
	float dy = 1.0 / float(screenHeight);
	vec4 sum = texture(frameTexture, texC) * Weight[0];
	for( int i = 1; i < blurWidth; i++ )
	{
		sum += texture( frameTexture, texC + vec2(0.0, PixOffset[i]) * dy ) * Weight[i];
		sum += texture( frameTexture, texC - vec2(0.0, PixOffset[i]) * dy ) * Weight[i];
	}
	return sum;
}

//Gauss horizontal Pass
subroutine(postProc)
vec4 blur2()
{
	vec2 texC = vec2(texCoord.x, 1 - texCoord.y);
	float dx = 1.0 / float(screenWidth);
	vec4 sum = texture(frameTexture, texC) * Weight[0];
	for( int i = 1; i < blurWidth; i++ )
	{
		sum += texture( frameTexture,texC + vec2(PixOffset[i], 0.0) * dx ) * Weight[i];
		sum += texture( frameTexture,texC - vec2(PixOffset[i], 0.0) * dx ) * Weight[i];
	}
	return sum;
}

//Depth of Field blur
subroutine (postProc)
vec4 DoF()
{
	vec2 texC = vec2(texCoord.x, 1 - texCoord.y);
	float dx = 1.0 / float(screenWidth);
	vec4 sum = texture(blurTexture, texC) * Weight[0];
	for( int i = 1; i < blurWidth; i++ )
	{
		sum += texture( blurTexture,texC + vec2(PixOffset[i], 0.0) * dx ) * Weight[i];
		sum += texture( blurTexture,texC - vec2(PixOffset[i], 0.0) * dx ) * Weight[i];
	}
	vec4 defaultColor = texture(frameTexture, texC);
	vec4 depthColor = texture(depthTexture, texC);
	float depthComponent = (depthColor.r + depthColor.g + depthColor.b) / 3;
	float stepValue =  smoothstep(blurStartEdge ,blurEndEdge , depthComponent);
	return mix( sum, defaultColor, stepValue);
}

//Bloom (get bright parts of image)
subroutine (postProc)
vec4 bloom1()
{
	vec2 texC = vec2(texCoord.x , 1 - texCoord.y);
	vec4 imgColor = texture(frameTexture, texC);
	if (luma(imgColor.rgb) > bloomThreshold) return imgColor;
	else return vec4(0);
}

//Gauss horizontal Pass + default image
subroutine(postProc)
vec4 bloom2()
{
	vec2 texC = vec2(texCoord.x, 1 - texCoord.y);
	vec4 defaultT = texture(frameTexture, texC);
	float dx = 1.0 / float(screenWidth);
	vec4 sum = texture(blurTexture, texC) * Weight[0];
	for( int i = 1; i < blurWidth; i++ )
	{
		sum += texture( blurTexture,texC + vec2(PixOffset[i], 0.0) * dx ) * Weight[i];
		sum += texture( blurTexture,texC - vec2(PixOffset[i], 0.0) * dx ) * Weight[i];
	}
	return sum + defaultT;
}

//Default image sampling
subroutine(postProc)
vec4 simple()
{
	vec2 texC = vec2(texCoord.x, 1 - texCoord.y);
	vec4 texColor = texture(frameTexture, texC);
	return texColor;
}

void main(void)
{
	FragColor = process();
}
