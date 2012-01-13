float4 xCameraPos;
float xCameraAngle;
float xCameraAspect;
bool xFlip;
float2 FlipCenter, Pivot;
float t;
float Illumination;

float SuckTime;

struct VertexToPixel
{
    float4 Position     : POSITION0;    
    float4 Color		: COLOR0;
    float2 TexCoords    : TEXCOORD0;
    //float2 Position2D   : TEXCOORD2;
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
    if (xFlip) inPos.x = FlipCenter.x - (inPos.x - FlipCenter.x);
    Output.Position.x = (inPos.x - xCameraPos.x) / xCameraAspect * xCameraPos.z;
    Output.Position.y = (inPos.y - xCameraPos.y) * xCameraPos.w;

    Output.TexCoords = inTexCoords;
    Output.Color = inColor;

	Output.Color.rgb *= Illumination;
    
    return Output;
}

VertexToPixel SuckVertexShader( float2 inPos : POSITION0, float2 inTexCoords : TEXCOORD0, float4 inColor : COLOR0)//, float inDepth : POSITION1)
{
    VertexToPixel Output = (VertexToPixel)0;    

    Output.Position.xy = inPos;
    Output.Position.w = 1;
    if (xFlip) inPos.x = FlipCenter.x - (inPos.x - FlipCenter.x);
    Output.Position.x = (inPos.x - xCameraPos.x) / xCameraAspect * xCameraPos.z;
    Output.Position.y = (inPos.y - xCameraPos.y) * xCameraPos.w;


	float2 e = Output.Position;
	Output.Position.x = cos(t*3.14*1.25) * e.x - sin(t*3.14*1.25) * e.y;
	Output.Position.y = sin(t*3.14*1.25) * e.x + cos(t*3.14*1.25) * e.y;

	//float T = (1 - t % 1);

	//float T = (1.25f*t) % 1;
	float T = (-cos(1.25f*t*3.14) + 1)/2;
	T = 
	(0) * (T - .3) * (T - .6) * (T - 1.) / ((1) * (0 - .3) * (0 - .6) * (0 - 1)) + 
	(T - 0) * (.4) * (T - .6) * (T - 1.) / ((.3 - 0) * (1) * (.3 - .6) * (.3 - 1)) + 
	(T - 0) * (T - .3) * (.65) * (T - 1.) / ((.6 - 0) * (.6 - .3) * (1) * (.6 - 1)) + 
	(T - 0) * (T - .3) * (T - .6) * (1) / ((1 - 0) * (1 - .3) * (1 - .6) * (1));
	T = 1 - T;

	Output.Position.x = Output.Position.x * pow(abs(Output.Position.x), 1 - T) * T;
	Output.Position.y = Output.Position.y * pow(abs(Output.Position.y), 1 - T) * T;

    Output.TexCoords = inTexCoords;
    Output.Color = inColor;

	Output.Color.rgb *= Illumination;
    
    return Output;
}

VertexToPixel PushOutVertexShader( float2 inPos : POSITION0, float2 inTexCoords : TEXCOORD0, float4 inColor : COLOR0)//, float inDepth : POSITION1)
{
    VertexToPixel Output = (VertexToPixel)0;    

    Output.Position.xy = inPos;
    Output.Position.w = 1;
    if (xFlip) inPos.x = FlipCenter.x - (inPos.x - FlipCenter.x);
    Output.Position.x = (inPos.x - xCameraPos.x) / xCameraAspect * xCameraPos.z;
    Output.Position.y = (inPos.y - xCameraPos.y) * xCameraPos.w;


	float T = SuckTime;
	//T = pow(T,1.5);
	T = pow(T,1.7);
	T = 1 - T;
	

	float2 e = Output.Position;
	float l = (e.x*e.x + e.y*e.y);
	l = pow(l,.65);
	float2 d = e / l + float2(0,.5);

//	Output.Position.x = e.x * pow(1*abs(e.x) + .5*t, 1 - T);
	//Output.Position.y = e.y * pow(1*abs(e.y) + 1*t, 1 - T);
	//Output.Position.xy += 2.3 * d * (1 - T);

	Output.Position.x = e.x * pow(1*abs(e.x) + .5*T, 1 - T);
	Output.Position.y = e.y * pow(1*abs(e.y) + 1*T, 1 - T);
	Output.Position.xy += 2.08 * d * (1 - T);

	inColor.rgba *= clamp(pow(3 * l, 1 - T) * pow(T, 1 - T), 0, 1);

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
    if (xFlip) inPos.x = FlipCenter.x - (inPos.x - FlipCenter.x);
    Output.Position.x = (inPos.x - xCameraPos.x) / xCameraAspect * xCameraPos.z;
    Output.Position.y = (inPos.y - xCameraPos.y) * xCameraPos.w;
    
    Output.TexCoords = inTexCoords;
    //Output.Position2D = inPos.xy;
    Output.Color = inColor;
    
    return Output;
}