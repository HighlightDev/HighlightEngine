#version 400

#define HAS_PREVIOUS_STAGE 1
layout (location = 0) out vec4 FragColor;

#if HAS_PREVIOUS_STAGE
    uniform sampler2D previousPostProcessResultSampler;
#endif

uniform sampler2D bluredTexture;

uniform int numSamples;
uniform float weight;
uniform float density;
uniform float decay;
uniform float exposure;
uniform vec2 radialPosition;

in vec2 texCoord;

void main()
{
    vec4 result = texture(bluredTexture, texCoord);

    vec2 deltaTextCoord = vec2(texCoord - radialPosition);
    vec2 offsetTexCoords = texCoord;
    deltaTextCoord *= (1.0 /  numSamples) * density;
    float illuminationDecay = 1.0;

    for(int i = 0; i < numSamples ; i++)
    {
        offsetTexCoords -= deltaTextCoord;
        vec4 color = texture(bluredTexture, offsetTexCoords);
        color *= illuminationDecay * weight;
        result += color;
        illuminationDecay *= decay;
    }

    //Reducing contrast
    result *= exposure;

#if HAS_PREVIOUS_STAGE
    //Add light shafts result to previous post process result
    result += texture(previousPostProcessResultSampler, texCoord);
#endif

    FragColor = result;
}
