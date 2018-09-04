#version 400

#define MaxWeights 3 
#define MaxBones 55
layout(location = 0) in vec3 vertex;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 uv;
layout(location = 4) in vec3 tangent;
layout(location = 5) in vec3 bitangent;
layout(location = 6) in vec3 blendWeights;
layout(location = 7) in ivec3 blendIndices;

uniform mat4 worldMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
uniform mat4 bonesMatrices[MaxBones];

out vec2 texCoords;

void main()
{
    texCoords = uv;

    vec4 localSpaceSkinnedVertex = vec4(0);
    vec4 localSpaceVertex = vec4(vertex, 1.0);

    for (int i = 0; i < MaxWeights; i++)
    {
        int blendIndex = blendIndices[i];
        if (blendIndex < 0)
            continue;

        float blendWeight = blendWeights[i];

        localSpaceSkinnedVertex += ((bonesMatrices[blendIndex]  * localSpaceVertex) * blendWeight);
    }

    vec4 worldSkinnedVertex = worldMatrix * localSpaceSkinnedVertex;

    gl_Position = projectionMatrix * viewMatrix * worldSkinnedVertex;
}
