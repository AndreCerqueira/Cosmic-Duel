Shader "Custom/ScrollingSpriteShader_Horizontal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScrollSpeed ("Horizontal Scroll Speed", Float) = 0.5
        [MainColor] _Color ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Name "ScrollingPass"
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

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

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float _ScrollSpeed;
            float4 _Color;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                uv.x += _Time.y * _ScrollSpeed; // scroll horizontal
                uv.x = frac(uv.x); // mantém no intervalo [0,1]
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * _Color;
                return col;
            }

            ENDHLSL
        }
    }
}
