// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Projector/Circle" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_Power ("Power", float) = 1
		_Center ("Center", vector) = (0, 0, 0)
		_Radius ("Radius", float) = 0.1
		_Center1 ("Center1", vector) = (1, 0, 1)
		_Radius1 ("Radius1", float) = 0.1
		_Center2 ("Center2", vector) = (2, 0, 2)
		_Radius2 ("Radius2", float) = 0.1
		_Center3 ("Center3", vector) = (3, 0, 3)
		_Radius3 ("Radius3", float) = 0.1
	}
	
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			ColorMask RGB
			Blend DstColor One
			Offset -1, -1
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 uvShadow : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				UNITY_FOG_COORDS(2)
				float4 pos : SV_POSITION;
			};
			
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			
			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(vertex);
				o.uvShadow = mul (unity_Projector, vertex);
				o.uvFalloff = mul (unity_ProjectorClip, vertex);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}
			
			fixed4 _Color;
			float _Power;

			fixed4 _Center;
			fixed _Radius;
			fixed4 _Center1;
			fixed _Radius1;
			fixed4 _Center2;
			fixed _Radius2;
			fixed4 _Center3;
			fixed _Radius3;
			
			sampler2D _ShadowTex;
			sampler2D _FalloffTex;
			
			float map(float value, float min1, float max1, float min2, float max2) {
				// Convert the current value to a percentage
				// 0% - min1, 100% - max1
				float perc = (value - min1) / (max1 - min1);

				// Do the same operation backwards with min2 and max2
				return perc * (max2 - min2) + min2;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed2 coord = UNITY_PROJ_COORD(i.uvShadow);
				fixed circle = 0;

				fixed4 centerInObject = mul(unity_WorldToObject, _Center);
				fixed4 centerCoord = UNITY_PROJ_COORD(mul (unity_Projector, centerInObject));
				fixed l = length(coord - fixed2(centerCoord.xy));
				circle = step(l, _Radius);

				fixed4 centerInObject1 = mul(unity_WorldToObject, _Center1);
				fixed4 centerCoord1 = UNITY_PROJ_COORD(mul (unity_Projector, centerInObject1));
				fixed l1 = length(coord - fixed2(centerCoord1.xy));
				circle = max(circle, step(l1, _Radius1));

				fixed4 centerInObject2 = mul(unity_WorldToObject, _Center2);
				fixed4 centerCoord2 = UNITY_PROJ_COORD(mul (unity_Projector, centerInObject2));
				fixed l2 = length(coord - fixed2(centerCoord2.xy));
				circle = max(circle, step(l2, _Radius2));

				fixed4 centerInObject3 = mul(unity_WorldToObject, _Center3);
				fixed4 centerCoord3 = UNITY_PROJ_COORD(mul (unity_Projector, centerInObject3));
				fixed l3 = length(coord - fixed2(centerCoord3.xy));
				circle = max(circle, step(l3, _Radius3));

				fixed color = circle * _Power;

				return fixed4(color, color, color, 0.5);

				// fixed4 texS = tex2Dproj (_ShadowTex, );
				// texS.rgb *= _Color.rgb;
				// texS.a = 1.0-texS.a;
	
				// fixed4 texF = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
				// fixed4 res = texS * texF.a;

				// UNITY_APPLY_FOG_COLOR(i.fogCoord, res, fixed4(0,0,0,0));
				// return res;
			}
			
			ENDCG
		}
	}
}
