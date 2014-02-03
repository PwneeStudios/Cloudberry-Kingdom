#include "RootEffect.fx"

Texture xTexture;
sampler TextureSampler : register(s1) = sampler_state { texture = <xTexture>; };

texture SceneTexture;
sampler SceneSampler : register(s0) = sampler_state { Texture = <SceneTexture>; };

PixelToFrame SimplePixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;
    
    float4 baseColor = tex2D(TextureSampler, PSIn.TexCoords);
    
    Output.Color = baseColor;
    Output.Color *= PSIn.Color;

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