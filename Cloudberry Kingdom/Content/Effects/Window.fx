#include"RootEffect.fx"

Texture xTexture;
//sampler TextureSampler : register(s1) = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; };
//sampler BackTextureSampler : register(s0) = sampler_state { texture = <ExtraTexture1> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; };
//sampler MaskTextureSampler : register(s2) = sampler_state { texture = <ExtraTexture2> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; };

sampler TextureSampler : register(s1) = sampler_state { texture = <xTexture>; };
sampler BackTextureSampler : register(s0) = sampler_state { texture = <ExtraTexture1>; };
sampler MaskTextureSampler : register(s2) = sampler_state { texture = <ExtraTexture2>; };

PixelToFrame SimplePixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;
    
    float4 c = tex2D(TextureSampler, PSIn.TexCoords);
    float mask = tex2D(MaskTextureSampler, PSIn.TexCoords).g;

    Output.Color = c;
    Output.Color *= PSIn.Color;

	float2 coordinates = (float2(PSIn.Position2D.x, -PSIn.Position2D.y) + float2(1, 1)) / 2;
	float4 back_clr = tex2D(BackTextureSampler, coordinates);

	if (mask > 0)
	{
		float a = Output.Color.a;
		Output.Color = (1 - a) * back_clr + Output.Color;
		//Output.Color.a = back_clr.a + a;
	}

    return Output;
}

PixelToFrame DepthVelocityPixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;        

	Output.Color = float4(PSIn.Color.a,PSIn.Color.g,PSIn.Color.b,1);
	        
    return Output;
}

technique Simplest
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER SimplestVertexShader();
        PixelShader = compile PIXEL_SHADER SimplePixelShader();
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

technique Suck
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER SuckVertexShader();
        PixelShader = compile PIXEL_SHADER SimplePixelShader();
    }
}

technique PushOut
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER PushOutVertexShader();
        PixelShader = compile PIXEL_SHADER SimplePixelShader();
    }
}

technique PivotTechnique
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER PivotVertexShader();
        PixelShader = compile PIXEL_SHADER SimplePixelShader();
    }
}