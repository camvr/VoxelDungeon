Shader "SkyboxBlend"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white"{}
		_Color ("Blend Color", Color) = (0, 0, 0, 0)
	}

	SubShader
	{
		Tags{ "RenderType" = "Fade" }

		CGPROGRAM
		#pragma surface surf Lambert
		ENDCG
	}

	CGINCLUDE
	struct Input {
		float2 uv_MainTex;
	};

	sampler2D _MainTex;
	half4 _Color;

	void surf(Input IN, inout SurfaceOutput o)
	{
		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color;
	}
	ENDCG
}