#version 400

layout (location = 0) out vec4 FragColor;

in vec2 texCoords;

uniform sampler2D uiTexture;
uniform sampler2D frameTexture;


void main(void){

    vec4 color = texture(uiTexture, texCoords);
	FragColor = color;
}
