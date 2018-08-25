#version 400

layout (points ) in;
layout (points, max_vertices = 3) out;

uniform mat4 worldMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
uniform mat4 skeletonMatrices[3];

out vec4 color;

void main()
{
    mat4 worldViewProjectionMatrix = projectionMatrix * viewMatrix * worldMatrix;

   for (int i = 0; i < 3; i++)
   {
        color = vec4(1, 0, 0, 1);
        switch (i)
        {
            case 0: color = vec4(1, 1, 0, 1); break;
            case 1: color = vec4(0, 1, 0, 1); break;
            case 2: color = vec4(0, 0, 1, 1); break;
            case 3: color = vec4(1, 1, 1, 1); break;
            case 4: color = vec4(1, 0, 0, 1); break;
            case 5: color = vec4(1, 0, 0, 1); break;
            case 6: color = vec4(1, 0, 0, 1); break;
            case 7: color = vec4(1, 0, 0, 1); break;
            case 8: color = vec4(1, 0, 0, 1); break;
            case 9: color = vec4(1, 0, 0, 1); break;
            case 10: color = vec4(1, 0, 0, 1); break;
            case 11: color = vec4(1, 0, 0, 1); break;
            case 12: color = vec4(1, 0, 0, 1); break;
            case 13: color = vec4(1, 0, 0, 1); break;
            case 14: color = vec4(1, 0, 0, 1); break;
            case 15: color = vec4(1, 0, 0, 1); break;
        }
        mat4 boneWorldViewProjectionMatrix = worldViewProjectionMatrix * skeletonMatrices[i];
        gl_Position = boneWorldViewProjectionMatrix * gl_in[0].gl_Position;
        EmitVertex();
        EndPrimitive();
   }
}