#include "RootEffect.fx"

Texture xTexture;
sampler TextureSampler : register(s1) = sampler_state { texture = <xTexture>; };

PixelToFrame LightPixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;
    
    float4 clr = tex2D(TextureSampler, PSIn.TexCoords);
    Output.Color = float4(0, 0, 0, 1 - clr.a);

	// Premultiply the alpha
	//Output.Color.rgb *= Output.Color.a;

    return Output;
}

technique Simplest
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER SimplestVertexShader();
        PixelShader = compile PIXEL_SHADER LightPixelShader();
    }
}