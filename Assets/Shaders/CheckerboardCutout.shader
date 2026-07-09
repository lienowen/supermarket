Shader "Supermarket/CheckerboardCutout"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.08
        _NeutralTolerance ("Neutral Tolerance", Range(0.01,0.2)) = 0.065
        _CheckerTolerance ("Checker Tolerance", Range(0.005,0.08)) = 0.035
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Cutoff;
            float _NeutralTolerance;
            float _CheckerTolerance;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv) * _Color;

                float maxChannel = max(c.r, max(c.g, c.b));
                float minChannel = min(c.r, min(c.g, c.b));
                float chroma = maxChannel - minChannel;
                float luminance = (c.r + c.g + c.b) / 3.0;

                // Generated source art contains a baked light-gray checkerboard.
                // Key only the two checker luminance bands instead of removing every white pixel,
                // preserving most silver and white highlights on the actual object.
                float nearGray = 1.0 - smoothstep(
                    _NeutralTolerance * 0.45,
                    _NeutralTolerance,
                    chroma
                );

                float checkerLight = 1.0 - smoothstep(
                    _CheckerTolerance * 0.35,
                    _CheckerTolerance,
                    abs(luminance - 0.996)
                );

                float checkerGray = 1.0 - smoothstep(
                    _CheckerTolerance * 0.35,
                    _CheckerTolerance,
                    abs(luminance - 0.945)
                );

                float checker = saturate(max(checkerLight, checkerGray) * nearGray);
                c.a *= 1.0 - checker;

                clip(c.a - _Cutoff);
                return c;
            }
            ENDCG
        }
    }

    Fallback "Unlit/Transparent"
}
