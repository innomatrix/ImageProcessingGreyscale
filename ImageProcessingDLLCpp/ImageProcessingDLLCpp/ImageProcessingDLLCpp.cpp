#include "pch.h"
#include "stdafx.h"
#include "ImageProcessingDLLCpp.h"
//#include "Windows.h"
//#include <vector>

using namespace std;

#define DllExport extern "C" __declspec(dllexport)

struct ImageInfo
{
	unsigned char* data;
	int size;
	int width;
	int height;
};

struct Pixel { unsigned char red; unsigned char green; unsigned char blue; };

//DllExport byte[] ToGreyscaleCPP(unsigned char* source, int dataLen, ImageInfo& sourceInfo)
DllExport void ToGreyscaleCPP(uint8_t * source, int dataLen, ImageInfo& sourceInfo)
{
	//	R 0.21    215  (215.04)
	//	G 0.72    737  (737.28)
	//	B 0.07    22   (21.504)

	// following OpenCV implementation: https://github.com/sumsuddin/ImageProcessingOpenCV -> handler for memory release
	// sourceInfo.data = (unsigned char*)calloc(sourceInfo.size, sizeof(unsigned char));
	// std::copy(sourceInfo.begin(), bytes.end(), sourceInfo.data);

	for (int p = 0; p < dataLen; ++p) {
		uint8_t pixel = source[p];

		switch (p % 3)
		{
		case 0:
			pixel = pixel * 22 >> 10; //B
			break;

		case 1:
			pixel = pixel * 737 >> 10; //G 
			break;

		case 2:
			pixel = pixel * 215 >> 10; //R
			break;

		}
	}
}

DllExport bool ReleaseMemoryFromCpp(unsigned char* buf)
{
	if (buf == NULL)
		return false;

	free(buf);
	return true;
}