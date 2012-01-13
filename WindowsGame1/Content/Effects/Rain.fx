#include"RootEffect.fx"

float2 Vel1, Vel2, Vel3;
float4 SkyColor;
float4 RainColor;

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = wrap; AddressV = wrap;};

PixelToFrame RainPixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;
    
	// Layer 1
	float2 coord1 = PSIn.TexCoords;
	coord1 += t * Vel1;
    float4 rain1 = tex2D(TextureSampler, coord1);

	// Layer 2
	float2 coord2 = PSIn.TexCoords * 1.13;
	coord2 += t * Vel2;
    float4 rain2 = tex2D(TextureSampler, coord2);

	// Layer 3
	float2 coord3 = PSIn.TexCoords * 1.06;
	coord3 += t * Vel3;
    float4 rain3 = tex2D(TextureSampler, coord3);

	float4 rain = (rain1 + rain2 + rain3) / 3;
	rain = RainColor * rain.a;

	rain.rgb *= rain.a;
    
    Output.Color = SkyColor * (1 - rain.a) + rain;
    Output.Color *= PSIn.Color;
	        
    return Output;
}


technique Simplest
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 SimplestVertexShader();
        PixelShader = compile ps_3_0 RainPixelShader();
    }
}