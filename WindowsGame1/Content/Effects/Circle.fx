#include"RootEffect.fx"
Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = clamp; AddressV = clamp;};

PixelToFrame CirclePixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;
    
    float d = (PSIn.TexCoords.x - .5) * (PSIn.TexCoords.x - .5) + (PSIn.TexCoords.y - .5) * (PSIn.TexCoords.y - .5);
    float4 baseColor = tex2D(TextureSampler, PSIn.TexCoords);
    
    Output.Color = PSIn.Color;
    //Output.Color *= baseColor;
	
	Output.Color.a *= saturate(100*(.25 - d));
    //Output.Color *= PSIn.Color;

	// Premultiply the alpha
	Output.Color.rgb *= Output.Color.a;
	        
    return Output;
}

PixelToFrame DepthVelocityPixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;
        
	float d = (PSIn.TexCoords.x - .5) * (PSIn.TexCoords.x - .5) + (PSIn.TexCoords.y - .5) * (PSIn.TexCoords.y - .5);
     
	Output.Color = float4(PSIn.Color.a,PSIn.Color.g,PSIn.Color.b,1);
	Output.Color.ra *= saturate(10000*(.25 - d));
		
    return Output;
}

PixelToFrame OutlinePixelShader(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;        

	float2 uv = PSIn.TexCoords;
	float4 Color = PSIn.Color;

	float d = .003;

	float l = tex2D(TextureSampler, uv).r;
	float l1 = tex2D(TextureSampler, uv + float2(-1, -1) * d).r;
	float l2 = tex2D(TextureSampler, uv + float2(1, 1) * d).r;
	float l3 = tex2D(TextureSampler, uv + float2(-1, 1) * d).r;
	float l4 = tex2D(TextureSampler, uv + float2(1, -1) * d).r;
	
	float v = (abs(l - l1) + abs(l - l2) + abs(l - l3) + abs(l - l4) + abs(l1 - l2) + abs(l3 - l4)) / 6;
	v = saturate(saturate(v - .05) * 3);

	Color = v * float4(.75,.5,.25,1) + (1 - v) * Color;

	// Premultiply the alpha
	Color.rgb *= Color.a;

	Output.Color = Color;

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
        PixelShader = compile ps_3_0 DepthVelocityPixelShader();
    }
}

technique PivotTechnique
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 PivotVertexShader();
        PixelShader = compile ps_3_0 CirclePixelShader();
    }
}