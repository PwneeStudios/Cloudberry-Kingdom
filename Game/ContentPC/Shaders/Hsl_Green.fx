#include "RootEffect.fx"

Texture xTexture;
sampler TextureSampler : register(s1) = sampler_state { texture = <xTexture>; };

PixelToFrame SimplePixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;
    
    float4 c = tex2D(TextureSampler, PSIn.TexCoords);
    
    Output.Color = c;
    Output.Color *= PSIn.Color;

	if (c.g > c.r + .025 && c.g > c.b + .025)
		Output.Color = mul(ColorMatrix, Output.Color);

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