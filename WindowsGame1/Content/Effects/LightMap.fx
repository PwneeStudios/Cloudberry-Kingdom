#include"RootEffect.fx"

Texture xTexture;
sampler TextureSampler : register(s1) = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; };//AddressU = wrap; AddressV = clamp;};

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
        VertexShader = compile vs_3_0 SimplestVertexShader();
        PixelShader = compile ps_3_0 LightPixelShader();
    }
}