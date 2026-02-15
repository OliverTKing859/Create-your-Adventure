#version 460 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTextureCoordinates;
layout (location = 2) in vec4 aInstanceMatrix0;
layout (location = 3) in vec4 aInstanceMatrix1;
layout (location = 4) in vec4 aInstanceMatrix2;
layout (location = 5) in vec4 aInstanceMatrix3;

out vec2 vTextureCoordinates;

uniform mat4 uView;
uniform mat4 uProjection;

void main()
{
    mat4 instanceMatrix = mat4(
        aInstanceMatrix0,
        aInstanceMatrix1,
        aInstanceMatrix2,
        aInstanceMatrix3
        );

    gl_Position = uProjection * uView * instanceMatrix * vec4(aPosition, 1.0);
    vTextureCoordinates = aTextureCoordinates;
}