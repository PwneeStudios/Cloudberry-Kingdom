#include "RootEffect.fx"

Texture xTexture;
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

technique Simplest
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER SimplestVertexShader();
        PixelShader = compile PIXEL_SHADER BwPixelShader();
    }
}