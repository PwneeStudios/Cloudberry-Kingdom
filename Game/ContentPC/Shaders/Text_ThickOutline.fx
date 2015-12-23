#include "RootEffect.fx"

Texture xTexture;
sampler TextureSampler : register(s1) = sampler_state { texture = <xTexture>; };

PixelToFrame SimplePixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;
    
    float val = tex2D(TextureSampler, PSIn.TexCoords).b;

    Output.Color = PSIn.Color * val;

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