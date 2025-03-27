Shader "Custom/Waves"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0


    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float _Offset;

        float4 _Waves[8];
        float _ArraySize;

        float3 GerstnerWave(float4 wave, float3 p, inout float3 tangent, inout float3 binormal)
        {
		    float steepness = wave.z;
		    float wavelength = wave.w;
		    float k = 2 * UNITY_PI / wavelength;
			float c = sqrt(9.8 / k);
			float2 d = normalize(wave.xy);
			float f = k * (dot(d, p.xz) - c * _Offset);
			float a = steepness / k;   

			tangent += float3(
				-d.x * d.x * (steepness * sin(f)),
				d.x * (steepness * cos(f)),
				-d.x * d.y * (steepness * sin(f))
			);

			binormal += float3(
				-d.x * d.y * (steepness * sin(f)),
				d.y * (steepness * cos(f)),
				-d.y * d.y * (steepness * sin(f))
			);	

			// return float3(
			// 	d.x * (a * cos(f)),
			// 	a * sin(f),
			// 	d.y * (a * cos(f))
			// );

            //doesnt move xz axis
			return float3(
				0,
				a * sin(f),
				0
			);
        }

        void vert(inout appdata_full vertexData, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

			float3 gridPoint = vertexData.vertex.xyz;
			float3 tangent = float3(1, 0, 0);
			float3 binormal = float3(0, 0, 1);
			float3 p = gridPoint;

            for (int i = 0; i < _ArraySize; i++)
            {
                p += GerstnerWave(_Waves[i], gridPoint, tangent, binormal);
            }
            
            vertexData.vertex.xyz = p;

			float3 normal = normalize(cross(binormal, tangent));
            // vertexData.normal = normal;

            o.worldPos = mul(unity_ObjectToWorld, float4(p, 1.0)).xyz;
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 worldPos = IN.worldPos; // World position of the fragment
            float3 ddxPos = ddx(worldPos);
            float3 ddyPos = ddy(worldPos);
            float3 normal = normalize(cross(ddxPos, ddyPos)); // Per-fragment normal
            o.Normal = normal;

            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
