float4x4 World;
float4x4 View;
float4x4 Projection;

TextureCube skycube;

sampler cubesampler = sampler_state
{
    texture = <skycube>;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 TexCoord : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection).xyww; //move all vertices to the far clipping plane. W & Z are both 1.0.
    output.TexCoord = input.Position;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return texCUBE(cubesampler, input.TexCoord);
}


technique Skybox
{
    pass Pass1
    {
        CullMode = None;
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
