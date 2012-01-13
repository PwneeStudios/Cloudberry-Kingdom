#include"RootEffect.fx"
Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = wrap; AddressV = wrap;};

texture SceneTexture;

sampler SceneSampler : register(s0) = sampler_state
{
    Texture = (SceneTexture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    
    AddressU = Clamp;
    AddressV = Clamp;
};


PixelToFrame CirclePixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;
    
    float d = (PSIn.TexCoords.x - .5) * (PSIn.TexCoords.x - .5) + (PSIn.TexCoords.y - .5) * (PSIn.TexCoords.y - .5);
    
//    PSIn.TexCoords += .07 * (PSIn.TexCoords - float2(.5,.5)) * 1. / (saturate(pow((.55 - d),2)) + .001);
    float4 baseColor = tex2D(TextureSampler, PSIn.TexCoords);
    
    Output.Color = baseColor;
    Output.Color *= PSIn.Color;
	
	Output.Color.a *= saturate(100*(.25 - d));
	Output.Color.a *= 1.25 * pow(d / .25, 2);
	//Output.Color.a = int(6 * Output.Color.a) / 6.0;

	// Premultiply the alpha
	Output.Color.rgb *= Output.Color.a;
        
    return Output;
}


technique Simplest
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 SimplestVertexShader();
        PixelShader = compile ps_3_0 CirclePixelShader();
    }
}

technique DepthVelocityInfo
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 SimplestVertexShader();
        PixelShader = compile ps_3_0 CirclePixelShader();
    }
}
