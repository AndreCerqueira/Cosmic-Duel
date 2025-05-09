// CardOutlineShader.hlsl (URP compatible, single-pass approximation)

Shader "Custom/CardOutlineURP"
{
    Properties
    {
        _MainTex("Front Texture", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth("Outline Width", Float) = 1

        _NoiseTex("Noise Texture", 2D) = "white" {}
        _RotateSpeed("Rotate Speed", Float) = 20
        _Len("Rotate Direction", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite True 
      
        Pass
        {
            Name "OUTLINE"
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _OutlineColor;
            float _OutlineWidth;
            float _RotateSpeed;
            float _Len;
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 offset = normalize(IN.positionOS.xyz) * _OutlineWidth;
                float3 newPos = IN.positionOS.xyz + offset;
                OUT.positionHCS = TransformObjectToHClip(float4(newPos, 1.0));
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv - float2(0.5, 0.5);
                float angle = _RotateSpeed * _Time.y * _Len;
                float cosA = cos(angle);
                float sinA = sin(angle);
                float2 rotatedUV = float2(
                    uv.x * cosA - uv.y * sinA,
                    uv.x * sinA + uv.y * cosA
                );
                rotatedUV += float2(0.5, 0.5);
                float4 noiseColor = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, rotatedUV);
                return noiseColor * _OutlineColor * 1.5;
            }
            ENDHLSL
        }
    }
    FallBack Off
}