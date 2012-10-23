#include"RootEffect.fx"

Texture xTexture;
//sampler TextureSampler : register(s1) = sampler_state { texture = <xTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; };
sampler TextureSampler : register(s1) = sampler_state { texture = <xTexture>; };

PixelToFrame SimplePixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;
    
    float4 c = tex2D(TextureSampler, PSIn.TexCoords);
    
    Output.Color = c;
    Output.Color *= PSIn.Color;

	Output.Color = mul(ColorMatrix, Output.Color);

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

technique PivotTechnique
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER PivotVertexShader();
        PixelShader = compile PIXEL_SHADER SimplePixelShader();
    }
}