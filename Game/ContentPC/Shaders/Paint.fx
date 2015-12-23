#include "RootEffect.fx"

Texture xTexture;
sampler TextureSampler : register(s1) = sampler_state { texture = <xTexture>; };

texture SceneTexture;
sampler SceneSampler : register(s0) = sampler_state
{
    Texture = (SceneTexture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    
    AddressU = Clamp;
    AddressV = Clamp;
};

PixelToFrame PaintPixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;
    
    Output.Color = tex2D(TextureSampler, PSIn.TexCoords);
    Output.Color *= PSIn.Color;
    Output.Color.a *= tex2D(SceneSampler, PSIn.TexCoords).a;

	// Premultiply the alpha
	Output.Color.rgb *= Output.Color.a;

    return Output;
}

technique Simplest
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER SimplestVertexShader();
        PixelShader = compile PIXEL_SHADER PaintPixelShader();
    }
}