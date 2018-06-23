#version 400

#define zNearPlane 0.1 
#define zFarPlane 1000
layout (location = 0) out vec4 FragColor;

in vec2 texCoords;

uniform sampler2D uiTexture;
uniform bool bDepthTexture;

float ToLinearDepth(float nonLinearDepth)
{
    float linearDepth = 2.0 * zNearPlane * zFarPlane / (zFarPlane + zNearPlane - nonLinearDepth * (zFarPlane - zNearPlane));
    return linearDepth;
}

void main(void){

    vec4 color = texture(uiTexture, texCoords);

   //if (bDepthTexture)
   //{
   //    float depth = (2.0 * color.r) - 1.0;
   //    color = vec4(ToLinearDepth(depth));
   //}

	FragColor = color;
}
