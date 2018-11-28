#version 400

#define zNearPlane 0.1 
#define zFarPlane 900
layout (location = 0) out vec4 FragColor;

in vec2 texCoords;

uniform sampler2D uiTexture;
uniform bool bPerspectiveDepthTexture = false;
uniform bool bSeparated = false;

float ToLinearDepth(float nonLinearDepth)
{
    float linearDepth = (2.0 * zNearPlane) / (zFarPlane + zNearPlane - nonLinearDepth * (zFarPlane - zNearPlane));
    return linearDepth;
}

void main(void){

    vec4 color = texture(uiTexture, texCoords);

   if (bPerspectiveDepthTexture)
   {
       float depth = (2.0 * color.r) - 1.0;
       float linearDepth = ToLinearDepth(depth);
       color = vec4(linearDepth, linearDepth, linearDepth, 1);
   }
   else if (bSeparated && texCoords.x < 0.5)
   {
		discard;
   }

	FragColor = color;
}
