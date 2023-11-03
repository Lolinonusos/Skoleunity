Shader "Custom/Goblin"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        
        #include "UnityCG.cginc"
        #define UNITY_INDIRECT_DRAW_ARGS IndirectDrawIndexedArgs
        #include "UnityIndirect.cginc"

        Texture2D _MainTex;
        SamplerState samplerMaintTex;
        
        struct v2f {
            float4 pos : SV_POSITION;
            float4 color : COLOR0;
            float2 uv : TEXCOORD0;
        };

        struct Input
        {
            float2 uv_MainTex;
        };

        uniform float4x4 objectToWorld;
        uniform StructuredBuffer<float3> positions;
        //float2 minMaxHeight;

        v2f vert(appdata_base v, uint svInstanceID : SV_InstanceID) {

            InitIndirectDrawArgs(0);
            v2f o;

            const uint instanceID = GetIndirectInstanceID(svInstanceID);

            const float4 wPos = mul(objectToWorld, v.vertex + float4(positions[instanceID][0], positions[instanceID][1], positions[instanceID][2], 1.0f));
            o.pos = mul(UNITY_MATRIX_VP, wPos);

            //o.uv = v.uv;
            o.color = float4(1.0f, 0.0f, 0.0f, 1.0f);//_MainTex.Sample(samplerMaintTex);
            
            return o;
        }

        float4 frag(v2f i) : SV_Target{
            return i.color;
        }        
        ENDCG
        }
    }
    FallBack "Diffuse"
}
