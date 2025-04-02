// WaveCalculationShader.shader
Shader "Custom/WaveCalculationShader"
{
    Properties
    {
        _Amplitude ("Amplitude", Float) = 1.0
        _Length ("Length", Float) = 2.0
        _Speed ("Speed", Float) = 1.0
        _Offset ("Offset", Float) = 0.0
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

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float _Amplitude;
            float _Length;
            float _Speed;
            float _Offset;

            v2f vert (appdata v)
            {
                v2f o;
                float3 pos = v.vertex.xyz;
                pos.y = _Amplitude * sin(pos.x / _Length + _Offset) * cos(pos.z / _Length + _Offset);
                o.pos = UnityObjectToClipPos(float4(pos, 1.0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(0, 0, 0, 1); // No fragment color needed
            }
            ENDCG
        }
    }
}
