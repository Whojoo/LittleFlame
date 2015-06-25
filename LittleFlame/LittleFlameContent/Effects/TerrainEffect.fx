//------- Constants --------
float4x4 xView;
float4x4 xProjection;
float4x4 xWorld;
float3 xLightDirection;
float xAmbient;
bool xEnableLighting;


//------- Texture Samplers --------
Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xTexture0;
sampler TextureSampler0 = sampler_state { texture = <xTexture0> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = wrap; AddressV = wrap;}; 

Texture xTexture1;
sampler TextureSampler1 = sampler_state { texture = <xTexture1> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = wrap; AddressV = wrap;};

Texture xTexture2;
sampler TextureSampler2 = sampler_state { texture = <xTexture2> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xTexture3;
sampler TextureSampler3 = sampler_state { texture = <xTexture3> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xTexture4;
sampler TextureSampler4 = sampler_state { texture = <xTexture4> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xTexture5;
sampler TextureSampler5 = sampler_state { texture = <xTexture5> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xTexture6;
sampler TextureSampler6 = sampler_state { texture = <xTexture6> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xTexture7;
sampler TextureSampler7 = sampler_state { texture = <xTexture7> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};


//------- Technique: Multitextured --------
struct MTVertexToPixel
{
    float4 Position				: POSITION;    
    float4 Color				: COLOR0;
    float3 Normal				: TEXCOORD0;
    float2 TextureCoords		: TEXCOORD1;
    float4 LightDirection		: TEXCOORD2;
    float4 Textures1			: TEXCOORD3;
	float4 Textures2			: TEXCOORD4;
};


struct MTPixelToFrame
{
    float4 Color : COLOR0;
};

MTVertexToPixel MultitexturedVS( float4 inPos : POSITION, float3 inNormal: NORMAL, float2 inTexCoords: TEXCOORD0, float4 inTextures1: TEXCOORD1, float4 inTextures2 : TEXCOORD2)
{    
    MTVertexToPixel Output = (MTVertexToPixel)0;
    float4x4 preViewProjection = mul (xView, xProjection);
    float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
    
    Output.Position = mul(inPos, preWorldViewProjection);
    Output.Normal = mul(normalize(inNormal), xWorld);
    Output.TextureCoords = inTexCoords;
    Output.LightDirection.xyz = -xLightDirection;
    Output.LightDirection.w = 1;    

    Output.Textures1 = inTextures1;
	Output.Textures2 = inTextures2;
    
    return Output;    
}

MTPixelToFrame MultitexturedPS(MTVertexToPixel vtp)
{
    MTPixelToFrame Output = (MTPixelToFrame)0;        
    
    float lightingFactor = 1;

	//If we want light
	if (xEnableLighting)
        lightingFactor = saturate(saturate(dot(vtp.Normal, vtp.LightDirection)) + xAmbient);

	Output.Color =	tex2D(TextureSampler0, vtp.TextureCoords)*vtp.Textures1.w;
	Output.Color += tex2D(TextureSampler1, vtp.TextureCoords)*vtp.Textures1.x;
	Output.Color += tex2D(TextureSampler2, vtp.TextureCoords)*vtp.Textures1.y;
	Output.Color += tex2D(TextureSampler3, vtp.TextureCoords)*vtp.Textures1.z;  
	
	Output.Color += tex2D(TextureSampler4, vtp.TextureCoords)*vtp.Textures2.w;  
	Output.Color += tex2D(TextureSampler5, vtp.TextureCoords)*vtp.Textures2.x;
	Output.Color += tex2D(TextureSampler6, vtp.TextureCoords)*vtp.Textures2.y;
	Output.Color += tex2D(TextureSampler7, vtp.TextureCoords)*vtp.Textures2.z;  

	Output.Color *= lightingFactor;
	Output.Color.r *= 1;
	
    return Output;
}

technique Multitextured
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 MultitexturedVS();
        PixelShader = compile ps_2_0 MultitexturedPS();
    }
}




