#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;

struct VertexShaderInput
{
	float4 pos : POSITION0;
	float4 col : COLOR0;
	float2 tex : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 pos : SV_POSITION;
	float4 col : COLOR0;
	float2 tex : TEXCOORD0;
};

float4 vmin, vmax;

Texture2D tex;
sampler2D tex_sampler = sampler_state
{
	Texture = <tex>;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.pos = mul(input.pos, WorldViewProjection);
	output.col = input.col;
	output.tex = input.tex;

	return output;
}
float normalize(float value, float vmin, float vmax, float newMin, float newMax)
{
	return (value - vmin) * ((newMax - newMin) / (vmax - vmin)) + newMin;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 col = tex2D(tex_sampler, input.tex);
	col.r = normalize(col.r, vmin, vmax, 0., 1.);
	col.g = normalize(col.g, vmin, vmax, 0., 1.);
	col.b = normalize(col.b, vmin, vmax, 0., 1.);
	col.a = 1.;
	return  col * input.col;//
}

float4 normalPS(VertexShaderOutput input) : COLOR
{
	float4 col = tex2D(tex_sampler, input.tex);	
	return  col * input.col;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
	pass P1
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL normalPS();
	}
};