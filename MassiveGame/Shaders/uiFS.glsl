#version 400

layout (location = 0) out vec4 FragColor;

in vec2 texCoords;

uniform sampler2D uiTexture;
uniform sampler2D frameTexture;


void main(void){

    vec4 color = texture(uiTexture, texCoords);
	FragColor = color;
}

/*
vec4 invColor = vec4(vec3(1) - texture(uiTexture, texCoords).xyz, 1.0);
vec3 colorCorrection = invColor.xyz * 5;
#define magnitude 0.5
#define edgeMagnitude 0.8
#define center vec2(0.5, 0.5)
*/