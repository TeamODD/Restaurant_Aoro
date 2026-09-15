Shader "Custom/CircleReveal"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
        _Radius ("Radius", Range(0,1.5)) = 0
        _Softness ("Softness", Range(0.001,0.2)) = 0.01
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            fixed4 _Color;
            float _Radius;
            float _Softness;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);

                float distanceFromCenter =
                    distance(i.uv, center);

                float alpha = smoothstep(
                    _Radius,
                    _Radius + _Softness,
                    distanceFromCenter
                );

                fixed4 color = _Color;
                color.a *= alpha;

                return color;
            }

            ENDCG
        }
    }
}
