#include"RootEffect.fx"

Texture xTexture;
sampler TextureSampler : register(s1) = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; };//AddressU = wrap; AddressV = clamp;};

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
        VertexShader = compile vs_3_0 SimplestVertexShader();
        PixelShader = compile ps_3_0 PaintPixelShader();
    }
}

technique DepthVelocityInfo
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 SimplestVertexShader();
        PixelShader = compile ps_3_0 DepthVelocityPixelShader();
    }
}

technique Outline
{
    pass Pass0
    {
		VertexShader = compile vs_3_0 SimplestVertexShader();
        PixelShader = compile ps_3_0 PaintPixelShader();
    }
}