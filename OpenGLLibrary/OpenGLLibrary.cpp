// ������ DLL �ļ���

#include "stdafx.h"

#include "OpenGLLibrary.h"

#include <gl\glut.h>

using namespace System::Runtime::InteropServices;

int OpenGLLibrary::ZROpenGL::drawRect()
{
	//GL_POINTS����   GL_LINES���� 
	glClear(GL_COLOR_BUFFER_BIT);
	glColor3f(1.0f,0.0f,0.0f);
	glBegin(GL_TRIANGLES);

	glVertex2f(0.0f, 0.0f);
	glVertex2f(0.5f, 0.0f);
	glVertex2f(0.3f, 0.2f);

	glEnd();
	return 0;
}