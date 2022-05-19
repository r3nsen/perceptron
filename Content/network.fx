#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
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

Texture2D inputs, weigths;
float sign; // positivo ou negativo
sampler2D input_sampler = sampler_state
{
	Texture = <inputs>;
};
sampler2D weigths_sampler = sampler_state
{
	Texture = <weigths>;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.pos = mul(input.pos, WorldViewProjection);
	output.col = input.col;
	output.tex = input.tex;

	return output;
}

float4 feedforward(VertexShaderOutput input) : COLOR
{
	// r           g           b
	// 255 0 0  0 255 0  0 0 255

	float w = tex2D(weigths_sampler, input.tex).r;	
	float o = w * tex2D(input_sampler, input.tex).r;
	
	return float4(o, o, o, 1.);
}

float4 backpropagation(VertexShaderOutput input) : COLOR
{
	float w = tex2D(weigths_sampler, input.tex).r;

	w += tex2D(input_sampler, input.tex).r * sign * .01;	
	return float4(w, w,w, 1.);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL feedforward();
	}
	pass P1
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL backpropagation();
	}
};