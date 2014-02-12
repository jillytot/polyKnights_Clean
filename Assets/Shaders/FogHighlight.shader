Shader "Little Polygon/Fog Highlight" {
	Properties {
		_Color ("Main Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_TintColor ("Highlight Color", Color) = (0, 0, 0, 0.0)
		_MainTex ("Main Texture", 2D) = "white" {}		
	}
	SubShader {
		Tags { "Queue" = "Geometry" }
		Lighting Off Fog { Mode Off }
		Pass {
		
CGPROGRAM
#pragma exclude_renderers ps3 xbox360 flash
#pragma fragmentoption ARB_precision_hint_fastest
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#include "Fog.cginc"

uniform fixed4 _Color;
uniform sampler2D _MainTex;
uniform float4 _MainTex_ST;
uniform fixed4 _TintColor;

//uniform float _TintMod;


struct FragmentInput {
	float4 pos : SV_POSITION;
	half2 uv : TEXCOORD0;
	float fogAmount : TEXCOORD1;
};

FragmentInput vert(
	float4 vertex : POSITION,
	float4 texcoord : TEXCOORD0
) {						
	FragmentInput o;
	float4 wpos = mul(_Object2World, vertex);
	o.pos = mul(UNITY_MATRIX_VP, wpos);
	o.uv = TRANSFORM_TEX( texcoord, _MainTex );
	
	o.fogAmount = computeFogAmount(wpos);

	return o;
}

half4 frag(FragmentInput i) : COLOR {
	return lerp(
		lerp(_Color, _FogColor, i.fogAmount) * tex2D( _MainTex, i.uv ), 
		fixed4(_TintColor.xyz, 1), 
		_TintColor.w
	);
}


ENDCG

		} 	
	}
	FallBack "Diffuse"
}
