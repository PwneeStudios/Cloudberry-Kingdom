#include"RootEffect.fx"

Texture xTexture;
//sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; };
sampler TextureSampler = sampler_state { texture = <xTexture>; };

texture SceneTexture;

sampler SceneSampler : register(s0) = sampler_state
{
    Texture = (SceneTexture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    
    AddressU = Clamp;
    AddressV = Clamp;
};

PixelToFrame VanillaPixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;

    Output.Color = PSIn.Color;
	//Output.Color.a = tex2D(TextureSampler, PSIn.TexCoords).a;
	Output.Color *= tex2D(TextureSampler, PSIn.TexCoords).a;

    return Output;
}

PixelToFrame OutlinePixelShader(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;
	return Output;
	/*
	float2 uv = PSIn.TexCoords;
	float4 Color = PSIn.Color;

	float d = .003;

	float l = tex2D(TextureSampler, uv).r;
	float l1 = tex2D(TextureSampler, uv + float2(-1, -1) * d).r;
	float l2 = tex2D(TextureSampler, uv + float2(1, 1) * d).r;
	float l3 = tex2D(TextureSampler, uv + float2(-1, 1) * d).r;
	float l4 = tex2D(TextureSampler, uv + float2(1, -1) * d).r;
	
	float v = (abs(l - l1) + abs(l - l2) + abs(l - l3) + abs(l - l4) + abs(l1 - l2) + abs(l3 - l4)) / 6;
	v = saturate(saturate(v - .05) * 3);

	//float v1 = abs(tex2D(SceneSampler, uv + float2(-1, -1) * d) - tex2D(SceneSampler, uv + float2(1, 1) * d));
	//float v2 = abs(tex2D(SceneSampler, uv + float2(1, -1) * d) - tex2D(SceneSampler, uv + float2(-1, 1) * d));
	//float v3 = abs(tex2D(SceneSampler, uv + float2(-1.4, 0) * d) - tex2D(SceneSampler, uv + float2(1.4, 0) * d));
	//float v4 = abs(tex2D(SceneSampler, uv + float2(0, -1.4) * d) - tex2D(SceneSampler, uv + float2(0, 1.4) * d));
	
	//float v = saturate((v1 + v2 + v3 + v4) / 8 - .1) * 2;
	//float v = saturate((v1 + v2) / 4 - .1) * 2;
	

	Color = v * float4(.75,.5,.25,1) + (1 - v) * Color;

	// Premultiply the alpha
	Color.rgb *= Color.a;

	Output.Color = Color;

	return Output;
	*/
}

technique Simplest
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER SimplestVertexShader();
        PixelShader = compile PIXEL_SHADER VanillaPixelShader();
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

technique PivotTechnique
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER PivotVertexShader();
        PixelShader = compile PIXEL_SHADER VanillaPixelShader();
    }
}