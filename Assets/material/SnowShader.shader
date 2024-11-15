Shader "Custom/SnowShader"
{
    Properties
    {
        _Color ("Snow Color", Color) = (1, 1, 1, 1)
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _NormalMap ("Normal Map", 2D) = "bump" { }
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            float _Glossiness;
            float4 _Color;
            sampler2D _MainTex;
            sampler2D _NormalMap;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = _Color;
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 texColor = tex2D(_MainTex, i.uv);
                half3 normal = tex2D(_NormalMap, i.uv).rgb;
                normal = normalize(normal * 2.0 - 1.0);
                
                // Tạo màu tuyết (có thể điều chỉnh theo ý muốn)
                half3 snowColor = lerp(texColor.rgb, _Color.rgb, 0.8);
                return half4(snowColor, 1.0);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
