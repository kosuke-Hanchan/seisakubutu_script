Shader "Custom/CircleFadeShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Fade ("Fade", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Fade;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.texcoord - 0.5;
                float dist = length(uv);

                // ここを修正して、中央の円が透明になり、周囲が不透明になるようにする
                float alpha = 1 - step(dist, _Fade * 0.5);
                fixed4 col = tex2D(_MainTex, i.texcoord);
                col.a *= alpha;
                return col;
            }
            ENDCG
        }
    }
}
