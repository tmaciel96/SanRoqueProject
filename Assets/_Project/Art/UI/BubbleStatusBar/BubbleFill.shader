Shader "Custom/BubbleStateBar"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _FillAmount("Fill Amount", Range(0, 1)) = 1
        _FillColor("Fill Color", Color) = (0.3, 0.8, 0.3, 1)
        _EmptyColor("Empty Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
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

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float  _FillAmount;
                half4  _FillColor;
                half4  _EmptyColor;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                //OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);

                // descartamos pixels transparentes (bordes del sprite)
                clip(tex.a - 0.01);

                half isFilled = step(IN.uv.y, _FillAmount);
                half4 color = lerp(_EmptyColor, _FillColor, isFilled);
                color.a = tex.a;

                return color;
                
                
            }
            ENDHLSL
        }
    }
}