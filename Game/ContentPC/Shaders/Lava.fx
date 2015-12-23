#include"RootEffect.fx"

float4 EdgeColor, LavaColor;

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture>; };

Texture xHeight;
sampler DisplacementSampler = sampler_state { texture = <xHeight>; };

PixelToFrame LavaPixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;

	float x = PSIn.TexCoords.x;
	float y = PSIn.TexCoords.y;
	float _t = t / 2.5;

	float2 TexLookup = float2(x, -y);
	float4 c1 = tex2D(TextureSampler, TexLookup);

	float2 coor1; coor1.x = x / 1; coor1.y = 0;
	float h1 = 1.1 * .9 * dot(tex2D(DisplacementSampler, coor1), float4(1., 1./255, 1./65025, 1./160581375));

	//Output.Color = tex2D(DisplacementSampler, coor1);
	//Output.Color = float4(1,1,1,1) * h1;
	//Output.Color.a = 1;
	//return Output;

	float alpha = 1;
	float width = .00275;
	if (y < h1)
		alpha *= (y - h1 + width) / width;
	alpha = clamp(alpha, 0, 1);

	//float y2 = 1 * (y - h1);
	//float r = y2;
	//float3 color = float3(r, .3, .1);
	//color = float3(1, .3, .1);
	float3 color = LavaColor;

	width = .0045;
	if (y < h1 + width)
	{
		float interpolate = (y - h1 - width + width) / width;
		interpolate = clamp(interpolate, 0, 1);
		interpolate *= interpolate;
		color.rgb = interpolate * color + (1 - interpolate) * EdgeColor;
	}

	Output.Color.rgb = color * alpha; Output.Color.a = alpha;
	return Output;

	float4 c = c1;
	c.r = .1;
	c.g = .1;
	c.b = .1;
		
	//c.r *= sin(texCoord.y - h1) / .1;
	c.r *= cos(min(1.5 * (h1 - PSIn.TexCoords.y) / (-.001),1.5));
	c.b *= cos(min(1.5 * (h1 - PSIn.TexCoords.y) / (-.001),1.5));
	c.g *= cos(min(1.5 * (h1 - PSIn.TexCoords.y) / (-.001),1.5));

	h1 *= clamp(cos(.01*(PSIn.TexCoords.y-.1)),0,1);

	float2 TexCoords2 = PSIn.TexCoords;
	TexCoords2.y -= 1.15 * h1 + _t / 140;
	TexCoords2.x += .06 * (cos(2 * h1) + cos(TexCoords2.y)) + _t / 280;
	float4 c2 = tex2D(TextureSampler, TexCoords2);

	Output.Color = c2;
	Output.Color.rgba *= c1.a;
	//Output.Color.a = c1.a;
	return Output;
	
	
	float2 TexCoords3 = PSIn.TexCoords;
	TexCoords3.y -= .8 * h1 + _t / 130;
	TexCoords3.x -= .06 * (cos(2 * h1) + cos(TexCoords3.y)) + _t / 260;
	TexCoords3.x *= -1.2;
	TexCoords3.y *= -1.2;
	float4 c3 = tex2D(TextureSampler, TexCoords3);
	c3.r *= 1.3;

	float2 TexCoords4 = PSIn.TexCoords;
	TexCoords4.y -= 1.05 * h1 + _t / 110;
	TexCoords4.x -= .06 * (cos(2 * h1) + cos(TexCoords4.y)) + _t / 310;
	TexCoords4.x *= 0.9;
	TexCoords4.y *= -0.9;
	float4 c4 = tex2D(TextureSampler, TexCoords3);
	c4.g *= 1.3;

	float l = min(.8, 8 * PSIn.TexCoords.y);
	c.r += (l * 1.9 + .47) * (c2.r + c3.r + c4.r) / 3;
	c.g += (l * .5) * (c2.g + c3.g + c4.g) / 3;
	c.b += (l * .25) * (c2.b + c3.b + c4.b) / 3;

	l = max(.35,PSIn.TexCoords.y / .1);
	c.r = l * c.r + (1 - l) * .8;
	c.g = l * c.g + (1 - l) * .5;
	c.b = l * c.b + (1 - l) * .2;

	//c.rgb = clamp(c.rgb * (2 - l), 0, 1);

	//c.b *= sin(.7 + h1 - PSIn.TexCoords.y);
	//c.g *= sin(.7 + h1 - PSIn.TexCoords.y);
	c.r *= max(0.25,(2 - l));
	c.g *= max(0.08,(2 - l));
	c.b *= max(0.02,(2 - l));
	c.r += .15;
	c.rgb *= .8;

	c.rgb = clamp(1.5*c.rgb,0,1)/1.;

	Output.Color = c;
	Output.Color.rgb *= c.a;
	return Output;
}

PixelToFrame LavaPixelShader2(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;

	float _t = t;
	//float _t = t / 2.5;
	//t /= 2.5;

	float4 c1 = tex2D(TextureSampler, PSIn.TexCoords);
	float2 coor1; coor1.x = PSIn.TexCoords.x / 1; coor1.y = 0;
	float h1 = 1.25 * dot(tex2D(DisplacementSampler, coor1), float4(1., 1./255, 1./65025, 1./160581375))  ;

	//Output.Color = tex2D(DisplacementSampler, coor1);
	//Output.Color = float4(1,1,1,1) * h1;
	//Output.Color.a = 1;
	//return Output;
	
	if (PSIn.TexCoords.y < h1)
		c1.a *= (PSIn.TexCoords.y - h1 + .008) / .008;
	else
		c1.a = 1.;

	float4 c = c1;
	c.r = .1;
	c.g = .1;
	c.b = .1;
		
	//c.r *= sin(texCoord.y - h1) / .1;
	c.r *= cos(min(1.5 * (h1 - PSIn.TexCoords.y) / (-.001),1.5));
	c.b *= cos(min(1.5 * (h1 - PSIn.TexCoords.y) / (-.001),1.5));
	c.g *= cos(min(1.5 * (h1 - PSIn.TexCoords.y) / (-.001),1.5));

	h1 *= clamp(cos(.01*(PSIn.TexCoords.y-.1)),0,1);

	float2 TexCoords2 = PSIn.TexCoords;
	TexCoords2.y -= 1.15 * h1 * (1 - PSIn.TexCoords.y) + _t / 140;
	TexCoords2.x += .06 * (cos(2 * h1) + cos(TexCoords2.y)) + _t / 280;
	float4 c2 = tex2D(TextureSampler, TexCoords2);
	

	//float l = min(.8, 8 * PSIn.TexCoords.y);
	float l = min(.5+5*h1, 8 * PSIn.TexCoords.y);
	c.r += (l * 1.9 + .47) * (c2.r) / 1.5;
	c.g += (l * .25) * (c2.g) / 1.5;
	c.b += (l * .25) * (c2.b) / 1.5;

	l = min(1.,max(h1/2,(PSIn.TexCoords.y-.8*h1) / .025));
	c.r = l * c.r + (1 - l) * 1;//.7;
	c.g = l * c.g + (1 - l) * .2;//.45;
	c.b = l * c.b + (1 - l) * 0;//.2;

	Output.Color = c;
	Output.Color.rgb *= c.a;
	return Output;
}

technique Simplest
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER SimplestVertexShader();
        PixelShader = compile PIXEL_SHADER LavaPixelShader();
    }
}