#version 330

layout (location = 0) out vec4 FragColor;

smooth in vec2 texCoords;

uniform sampler2D lampTexture;

void main(void)
{
	vec4 resultPixel = texture(lampTexture, texCoords);
	if (resultPixel.a < 0.8) discard;
	else 
	{
		FragColor = resultPixel;
	}
}