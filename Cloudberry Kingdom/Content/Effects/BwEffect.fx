#include"RootEffect.fx"
float2 OutlineScale;
float4 OutlineColor, InsideColor;

Texture xTexture;
//sampler TextureSampler : register(s1) = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; };
sampler TextureSampler : register(s1) = sampler_state { texture = <xTexture>; };

texture SceneTexture;
sampler SceneSampler : register(s0) = sampler_state { Texture = (SceneTexture); };

float3 _bw = float3(.3, .59, .11);

PixelToFrame BwPixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;
    
    float4 baseColor = tex2D(TextureSampler, PSIn.TexCoords);
    
    Output.Color = baseColor;
    Output.Color *= PSIn.Color;

	float bw = 
		Output.Color.r * _bw.r + 
		Output.Color.g * _bw.g + 
		Output.Color.b * _bw.b;

	Output.Color.rgb = float3(bw, bw, bw);

    return Output;
}

PixelToFrame DepthVelocityPixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;        

	// This commented line should allow textured quads' transparency to effect the final transparency
	//PSIn.Color.a *= tex2D(TextureSampler, PSIn.TexCoords).a;

	Output.Color = float4(PSIn.Color.a,PSIn.Color.g,PSIn.Color.b,1);
	        
    return Output;
}

float4 OutlinePixelShader(float4 color : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
/*
	float d = .055 * OutlineScale;
	float d2 = d / 16;
	
	float _a = tex2D(SceneSampler, uv + float2(-1, -1) * d2).r;
	float _b = tex2D(SceneSampler, uv + float2(-1, 1) * d2).r;
	float _c = tex2D(SceneSampler, uv + float2(1, -1) * d2).r;
	float _d = tex2D(SceneSampler, uv + float2(1, 1) * d2).r;
	
	float BodyIndicator = (_a + _b + _c + _d) / 4;
	BodyIndicator *= BodyIndicator;
		
	float2 _uv = tex2D(SceneSampler, uv).gb;
	float4 Color = tex2D(TextureSampler, _uv);
	
	float l = tex2D(SceneSampler, uv).r;
	
	float v = step(0.001, l);
	v += abs(l - tex2D(SceneSampler, uv + float2(1     ,      0) * d * 1.25).r);
	v += abs(l - tex2D(SceneSampler, uv + float2(.92388, .38268) * d * 1.25).r);
	v += abs(l - tex2D(SceneSampler, uv + float2(.70711, .70711) * d * 1.25).r);
	v += abs(l - tex2D(SceneSampler, uv + float2(.38268, .92388) * d * 1.25).r);
		
	v += abs(l - tex2D(SceneSampler, uv + float2(0 ,  1) * d * 1.25).r);
	v += abs(l - tex2D(SceneSampler, uv + float2(-.92388, .38268) * d * 1.25).r);
	v += abs(l - tex2D(SceneSampler, uv + float2(-.70711, .70711) * d * 1.25).r);
	v += abs(l - tex2D(SceneSampler, uv + float2(-.38268, .92388) * d * 1.25).r);

	v += abs(l - tex2D(SceneSampler, uv + float2(-1,  0) * d * 1.25).r);
	v += abs(l - tex2D(SceneSampler, uv + float2(.92388, -.38268) * d * 1.25).r);
	v += abs(l - tex2D(SceneSampler, uv + float2(.70711, -.70711) * d * 1.25).r);
	v += abs(l - tex2D(SceneSampler, uv + float2(.38268, -.92388) * d * 1.25).r);
		
	v += abs(l - tex2D(SceneSampler, uv + float2(0 , -1) * d * 1.25).r);
	v += abs(l - tex2D(SceneSampler, uv + float2(-.92388, -.38268) * d * 1.25).r);
	v += abs(l - tex2D(SceneSampler, uv + float2(-.70711, -.70711) * d * 1.25).r);
	v += abs(l - tex2D(SceneSampler, uv + float2(-.38268, -.92388) * d * 1.25).r);
		
	v *= 20. / 16.;
				
	v /= 3.4;

	v = clamp(v, 0, 1);
	v *= v;
	
	Color *= InsideColor;
	
	float4 Outline = OutlineColor;
	Outline.a *= v;
	Color = (1 - BodyIndicator) * Outline + BodyIndicator * Color;
	
	// Premultiply the alpha
	Color.rgb *= Color.a;

	return Color;
*/
	return color;
}

float4 RefinedOutlinePixelShader(float4 color : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
/*
	float d = .055;
		
	float d2 = .9 * d / 16;

	float _a = tex2D(SceneSampler, uv + float2(-1, -1) * d2).r;
	float _b = tex2D(SceneSampler, uv + float2(-1, 1) * d2).r;
	float _c = tex2D(SceneSampler, uv + float2(1, -1) * d2).r;
	float _d = tex2D(SceneSampler, uv + float2(1, 1) * d2).r;
	
	float BodyIndicator = (_a + _b + _c + _d) / 4;
	BodyIndicator *= BodyIndicator;

	float2 _uv = tex2D(SceneSampler, uv).gb;
	float4 Color = tex2D(TextureSampler, _uv);

	float l = tex2D(SceneSampler, uv).r;
	
	float v = 0;
	if (l > 0)
		v = 1;
	else
	{
v += abs(l - tex2D(SceneSampler, uv + float2( 1.0 , 0.0 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.995184726671 , 0.0980171403423 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.980785280398 , 0.195090322041 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.956940335721 , 0.290284677291 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.923879532492 , 0.382683432412 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.881921264318 , 0.471396736883 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.83146961226 , 0.555570233084 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.773010453306 , 0.634393284233 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.707106781114 , 0.707106781259 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.634393284074 , 0.773010453436 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.555570232913 , 0.831469612374 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.471396736702 , 0.881921264415 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.382683432223 , 0.92387953257 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.290284677095 , 0.956940335781 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.19509032184 , 0.980785280438 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.0980171401382 , 0.995184726691 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -2.05103428515e-10 , 1.0 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.0980171405464 , 0.995184726651 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.195090322242 , 0.980785280358 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.290284677488 , 0.956940335662 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.382683432602 , 0.923879532413 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.471396737063 , 0.881921264221 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.555570233254 , 0.831469612146 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.634393284392 , 0.773010453176 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.707106781404 , 0.707106780969 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.773010453566 , 0.634393283916 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.831469612488 , 0.555570232742 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.881921264512 , 0.471396736521 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.923879532649 , 0.382683432033 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.95694033584 , 0.290284676899 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.980785280478 , 0.195090321639 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.995184726711 , 0.0980171399341 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -1.0 , -4.10206857031e-10 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.995184726631 , -0.0980171407505 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.980785280318 , -0.195090322444 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.956940335602 , -0.290284677684 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.923879532335 , -0.382683432791 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.881921264125 , -0.471396737244 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.831469612032 , -0.555570233425 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.773010453046 , -0.63439328455 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.707106780824 , -0.707106781549 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.634393283757 , -0.773010453696 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.555570232572 , -0.831469612602 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.47139673634 , -0.881921264608 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.382683431844 , -0.923879532727 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.290284676702 , -0.9569403359 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.195090321438 , -0.980785280518 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( -0.09801713973 , -0.995184726731 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 6.15310285546e-10 , -1.0 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.0980171409547 , -0.995184726611 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.195090322645 , -0.980785280278 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.29028467788 , -0.956940335542 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.382683432981 , -0.923879532256 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.471396737425 , -0.881921264028 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.555570233595 , -0.831469611918 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.634393284709 , -0.773010452915 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.707106781694 , -0.707106780679 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.773010453826 , -0.634393283599 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.831469612716 , -0.555570232401 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.881921264705 , -0.471396736159 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.923879532806 , -0.382683431654 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.956940335959 , -0.290284676506 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.980785280558 , -0.195090321237 ) * d * 1.25).r);
v += abs(l - tex2D(SceneSampler, uv + float2( 0.995184726751 , -0.0980171395259 ) * d * 1.25).r);
		
		v *= 20. / 45.;
		
		v /= 3.4;

		v = clamp(v, 0, 1);
		v *= v;
	}
	
	Color *= InsideColor;
	
	float4 Outline = OutlineColor;
	Outline.a *= v;
	Color = (1 - BodyIndicator) * Outline + BodyIndicator * Color;

	// Premultiply the alpha
	Color.rgb *= Color.a;
	
	return Color;
	*/
	return color;
}

technique Simplest
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER SimplestVertexShader();
        PixelShader = compile PIXEL_SHADER BwPixelShader();
    }
}

technique Suck
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER SuckVertexShader();
        PixelShader = compile PIXEL_SHADER BwPixelShader();
    }
}

technique PushOut
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER PushOutVertexShader();
        PixelShader = compile PIXEL_SHADER BwPixelShader();
    }
}

technique PivotTechnique
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER PivotVertexShader();
        PixelShader = compile PIXEL_SHADER BwPixelShader();
    }
}

technique DepthVelocityInfo
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER SimplestVertexShader();
        PixelShader = compile PIXEL_SHADER DepthVelocityPixelShader();
    }
}

technique Outline
{
    pass Pass0
    {
		VertexShader = compile VERTEX_SHADER SimplestVertexShader();
        PixelShader = compile PIXEL_SHADER OutlinePixelShader();
    }
}

technique RefinedOutline
{
    pass Pass0
    {
		VertexShader = compile VERTEX_SHADER SimplestVertexShader();
        PixelShader = compile PIXEL_SHADER RefinedOutlinePixelShader();
    }
}