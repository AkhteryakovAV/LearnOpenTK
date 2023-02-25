#version 330 core

out vec4 outputColor;

in vec2 texCoord;
in vec3 ourColor; 
uniform sampler2D ourTexture;

void main()
{
	outputColor = texture(ourTexture, texCoord) * vec4(ourColor, 1.0f);
}