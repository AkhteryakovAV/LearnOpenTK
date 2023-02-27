#version 330 core

out vec4 outputColor;

in vec2 texCoord;
in vec3 ourColor; 
uniform sampler2D ourTexture1;
uniform sampler2D ourTexture2;

void main()
{
	outputColor = mix(texture(ourTexture1, texCoord), texture(ourTexture2, texCoord), 0.2) * vec4(ourColor, 1.0f);
}