﻿#version 330 core

layout(location = 0) in vec3 aPosition;  
layout(location = 1) in vec3 aColor;

out vec3 ourColor; 

void main(void)
{
    gl_Position = vec4(aPosition, 1.0); 
	ourColor = aColor;
}