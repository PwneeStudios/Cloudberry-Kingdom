#include"RootEffect.fx"
//float t;

Texture xTexture;
//sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = clamp; AddressV = clamp;};
sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; };// AddressU = clamp; AddressV = clamp;};
texture SceneTexture;

sampler SceneSampler : register(s0) = sampler_state
{
    Texture = (SceneTexture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    
    AddressU = Clamp;
    AddressV = Clamp;
};

PixelToFrame FireballShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;
    
    float2 uv = PSIn.TexCoords;
    
    uv.y = 1.2 * (pow(uv.x,3) / 1.7 + .6) * (uv.y - .5) + .5;
    //uv.y = .85 * (pow(1-uv.x,.5) / 1.7 + .6) * (uv.y - .5) + .5;
    //uv.x *= .7;
    
	uv.x = pow(uv.x, .2 + .8 * (1 - uv.x));

	//uv.y = .9 / max(.7,pow(uv.x,.75)) * (uv.y + (.04 * pow(uv.x,1) + .03)*sin(uv.x*10 + 25*t) - .5) + .5;
	uv.y = .9 / max(.7,pow(uv.x,.75)) * (uv.y + (.023 * pow(uv.x,1) + .017)*sin(uv.x*10 + 25*t) - .5) + .5;    	
    
    float4 C = tex2D(TextureSampler, uv);
    
    C.r *= 1.2;
    C *= 1.3;
    
    Output.Color = C * PSIn.Color;
        
    return Output;
}


technique Simplest
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER SimplestVertexShader();
        PixelShader = compile PIXEL_SHADER FireballShader();
    }
}

technique DepthVelocityInfo
{
    pass Pass0
    {
		VertexShader = compile VERTEX_SHADER SimplestVertexShader();
        PixelShader = compile PIXEL_SHADER FireballShader();
    }
}