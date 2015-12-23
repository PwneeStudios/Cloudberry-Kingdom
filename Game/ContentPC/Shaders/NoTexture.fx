#include "RootEffect.fx"

Texture xTexture;
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

technique Simplest
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER SimplestVertexShader();
        PixelShader = compile PIXEL_SHADER VanillaPixelShader();
    }
}