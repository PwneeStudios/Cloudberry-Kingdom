#include"RootEffect.fx"

Texture xTexture;
//sampler TextureSampler : register(s1) = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; };
sampler TextureSampler : register(s1) = sampler_state { texture = <xTexture>; };

PixelToFrame SimplePixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;
    
    float4 c = tex2D(TextureSampler, PSIn.TexCoords);
    
    Output.Color = c;
    Output.Color *= PSIn.Color;

	if (c.g > c.r + .025 && c.g > c.b + .025)
	{
		//float angle = atan2(2 * c.r - c.g - c.b, 1.732 * (c.g - c.b));
		//angle = (angle + t) % 6.28;


	/*
		float bottom = min(c.r, c.b);
		float top = c.g;
		float mid = max(c.r, c.b);

		float spread = top - bottom;
		float angle = (mid - bottom) / spread;

		angle = (angle + t) % 3;
		if (angle < 1)
		{
			c.g = top;
			c.b = bottom + spread * (1 - cos(t)) / 2;
			c.r = bottom;
		}
		else if (angle < 2)
		{
			c.b = top;
			c.g = bottom + spread * (1 - cos(t)) / 2;
			c.r = bottom;
		}
		else
		{
			c.r = top;
			c.b = bottom + spread * (1 - cos(t)) / 2;
			c.g = bottom;
		}

		Output.Color.rgb = c.rgb;*/

		Output.Color = mul(ColorMatrix, Output.Color);
	}

	/*
	// Rotate
	float3 c = Output.Color;
	float3 v = float3(.57735,.57735,.57735);
	
	float trig2 = cos(a);
	float trig1 = 1 - trig2;
	float trig3 = sin(a);
	float dot = trig1 * (c.x * v.x + c.y * v.y + c.z * c.z);

	Output.Color.xyz = float3(
		v.x * dot + c.x * trig2 + (-v.z * c.y + v.y * c.z) * trig3,
		v.y * dot + c.y * trig2 + (v.z * c.x - v.x * c.z) * trig3,
		v.z * dot + c.z * trig2 + (-v.y * c.x + v.x * c.y) * trig3);
		*/

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

technique Suck
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER SuckVertexShader();
        PixelShader = compile PIXEL_SHADER SimplePixelShader();
    }
}

technique PushOut
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER PushOutVertexShader();
        PixelShader = compile PIXEL_SHADER SimplePixelShader();
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