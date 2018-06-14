#version 400

layout (location = 0) out vec4 FragColor;

//uniform sampler2D frameTexture;
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
		 vec2 texCoords = vec2(texCoord.x, 1 - texCoord.y);
		 vec2 tC = texCoords;
		 vec4 result = texture(bluredTexture, texCoords);

		vec2 deltaTextCoord = vec2( texCoords - radialPosition );
    	vec2 textCoo = texCoords;
    	deltaTextCoord *= (1.0 /  numSamples) * density;
    	float illuminationDecay = 1.0;
	
	
    	for(int i = 0; i < numSamples ; i++)
        {
                 textCoo -= deltaTextCoord;
                 vec4 color = texture(bluredTexture, textCoo );
			
                 color *= illuminationDecay * weight;

                 result += color;

                 illuminationDecay *= decay;
         }

		 //Reducing contrast
         result *= exposure;

		 //Add result to default image
		 //result = texture(frameTexture, texCoords) + result; 

		 FragColor = result;
    }