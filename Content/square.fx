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

float2 desloc;
float2 size;

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.pos = mul(input.pos, WorldViewProjection);
	output.col = input.col;
	output.tex = input.tex;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 pos = (input.tex - .5) * 2.;
	float x = abs(pos.x - desloc.x) - size.x;// -desloc.x - size.x;
	float y = abs(pos.y - desloc.y) - size.y;// -desloc.x - size.x;
	float d = max(x, y);
	float s = smoothstep(.01, 0, d);
	float4 col = float4(s, s, s, 1);
	return col;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};