//#define PIXEL_SHADER ps_3_0
//#define VERTEX_SHADER vs_3_0

#define PIXEL_SHADER ps_2_0
#define VERTEX_SHADER vs_1_1

float4 xCameraPos;
float xCameraAngle;
float xCameraAspect;

float2 FlipVector;
float2 FlipCenter, Pivot;
float t;
float Illumination;

float4x4 ColorMatrix;

Texture ExtraTexture1;
Texture ExtraTexture2;

struct VertexToPixel
{
    float4 Position     : POSITION0;    
    float4 Color		: COLOR0;
    float2 TexCoords    : TEXCOORD0;
    float2 Position2D   : TEXCOORD2;
};

struct PixelToFrame
{
    float4 Color        : COLOR0;
};

VertexToPixel SimplestVertexShader( float2 inPos : POSITION0, float2 inTexCoords : TEXCOORD0, float4 inColor : COLOR0)//, float inDepth : POSITION1)
{
    VertexToPixel Output = (VertexToPixel)0;    

    Output.Position.xy = inPos;
    Output.Position.w = 1;
    
	inPos.x = FlipCenter.x - FlipVector.x * (inPos.x - FlipCenter.x);
	inPos.y = FlipCenter.y - FlipVector.y * (inPos.y - FlipCenter.y);

    Output.Position.x = (inPos.x - xCameraPos.x) / xCameraAspect * xCameraPos.z;
    Output.Position.y = (inPos.y - xCameraPos.y) * xCameraPos.w;

	Output.Position2D = Output.Position.xy;

    Output.TexCoords = inTexCoords;
    Output.Color = inColor;

	Output.Color.rgb *= Illumination;
    
    return Output;
}

VertexToPixel PivotVertexShader( float2 inPos : POSITION0, float2 inTexCoords : TEXCOORD0, float4 inColor : COLOR0)//, float inDepth : POSITION1)
{
    VertexToPixel Output = (VertexToPixel)0;
    
    float c = cos(xCameraAngle);
    float s = sin(xCameraAngle);
    float2 dif = inPos - Pivot;
    inPos.x = dif.x * c - dif.y * s;
    inPos.y = dif.x * s + dif.y * c;
    inPos += Pivot;

    Output.Position.xy = inPos;
    Output.Position.w = 1;
    
	inPos.x = FlipCenter.x - FlipVector.x * (inPos.x - FlipCenter.x);
	inPos.y = FlipCenter.y - FlipVector.y * (inPos.y - FlipCenter.y);

    Output.Position.x = (inPos.x - xCameraPos.x) / xCameraAspect * xCameraPos.z;
    Output.Position.y = (inPos.y - xCameraPos.y) * xCameraPos.w;
    
    Output.TexCoords = inTexCoords;
    Output.Color = inColor;
    
    return Output;
}