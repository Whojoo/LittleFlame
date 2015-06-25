float4x4 World;
float4x4 View;
float4x4 Projection;

texture waveTexture;
sampler2D waveSampler = sampler_state{
	texture = <waveTexture>;
};

float time = 1;
float waveSpeed = 0.04f;
float alpha = 1;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 wavePosition : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	output.wavePosition = input.UV;
	output.wavePosition.y -= time * waveSpeed;
	
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 wave = tex2D(waveSampler, input.wavePosition) * 2 - 1;
	wave.a = tex2D(waveSampler, input.wavePosition) * alpha;
	wave.rgb *= wave.a;	

	return wave;
}
technique Waves
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}