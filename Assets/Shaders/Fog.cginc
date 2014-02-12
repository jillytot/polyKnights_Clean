uniform float _FogDistance;
uniform float _FogIntensity;
uniform fixed4 _FogColor;

float computeFogAmount(float4 wpos) {
	float3 offset = (_WorldSpaceCameraPos.xyz - wpos.xyz) * float3(1, 0.15, 1);
	return saturate(1-exp(-_FogIntensity * length(offset) + _FogDistance));
	
}
