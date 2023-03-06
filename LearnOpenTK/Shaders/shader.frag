#version 330 core

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D ourTexture1;

void main()
{
	outputColor = texture(ourTexture1, texCoord);
}