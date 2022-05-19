#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;
float timer;

Texture2D tex;

sampler2D texture_sampler = sampler_state
{
	Texture = <tex>;
	MinFilter = ANISOTROPIC;
	MagFilter = ANISOTROPIC;
	MipFilter = ANISOTROPIC;
};

struct VertexShaderInput
{
	float4 position : POSITION0;
	float3 col : COLOR0;
	float2 tex : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 position : SV_POSITION;
	float3 col : COLOR0;
	float2 tex : TEXCOORD0;
};

VertexShaderOutput imageVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	output.position = mul(input.position, WorldViewProjection);
	output.col = input.col;
	output.tex = input.tex;
	return output;
}

float4 textRenderPS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(texture_sampler, input.tex);	
	return color.rrrr;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL imageVS();
		PixelShader = compile PS_SHADERMODEL textRenderPS();
	}
}