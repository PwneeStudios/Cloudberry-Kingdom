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

float4 PaintPixelShader_SpriteBatch(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 Output = tex2D(SceneSampler, texCoord);
    Output *= color;
    Output.a *= tex2D(TextureSampler, texCoord).a;

	// Premultiply the alpha
	Output *= Output.a;

    return Output;
}

technique Simplest
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PaintPixelShader_SpriteBatch();
    }
}