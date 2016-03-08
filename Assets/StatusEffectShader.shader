Shader "Unlit/StatusEffectShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Percent("Percent", Range(0, 1.0)) = 0.1
		_Color("Color", Color) = (0,0,0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Percent;
			float4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);

				float radius = 1;
				const float pi = 3.14159;
				float rad = lerp(0.0, 2*pi, _Percent);

				//Grayscale based on percent
				float grayscale = (col.r + col.g + col.b) / 6;
				//col.r = lerp(col.r, grayscale, _Percent);
				//col.g = lerp(col.g, grayscale, _Percent);
				//col.b = lerp(col.b, grayscale, _Percent);

				//First quadrant
				if (_Percent < 0.25) {
					if (i.uv.x > 0.5 && i.uv.y > 0.5 && atan((i.uv.x - 0.5) / (i.uv.y - 0.5)) < rad) {
						col = grayscale;
					}
				}
				//Second quadrant
				else if (_Percent < 0.5) {
					rad = rad % (pi / 2);
					if ((i.uv.x > 0.5 && i.uv.y > 0.5) || 
						(i.uv.x > 0.5 && i.uv.y < 0.5 && atan((0.5 - i.uv.y)/(i.uv.x - 0.5)) < rad)) {
						col = grayscale;
					}
				}
				//Third quadrant
				else if (_Percent < 0.75) {
					rad = rad % (pi / 2);
					if ((i.uv.x > 0.5 && i.uv.y > 0.5) || (i.uv.x > 0.5 && i.uv.y < 0.5) || 
						(i.uv.x < 0.5 && i.uv.y < 0.5 && atan((0.5 - i.uv.x) / (0.5 - i.uv.y)) < rad)) {
						col = grayscale;
					}
				}
				//Fourth quadrant
				else if (_Percent < 1) {
					rad = rad % (pi / 2);
					if (!(i.uv.x < 0.5 && i.uv.y > 0.5) ||
						(i.uv.x < 0.5 && i.uv.y > 0.5) && atan((i.uv.y - 0.5) / (0.5 - i.uv.x)) < rad) {
						col = grayscale;
					}
				}
				else {
					col = grayscale;
				}
				//if (atan(i.uv.x/i.uv.y) < rad) {
				//	col = grayscale;
				//}

				return col;
			}
			ENDCG
		}
	}
}
