Shader "Custom/World UV Test" {

Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex1 ("Cara 1", 2D) = "surface" {}
	_MainTex2 ("Cara 2", 2D) = "surface" {}
	
	_Scale ("Texture Scale", Float) = 0.1
	_Offsetx ("Offset X", Float) = 0.0
	_Offsety ("Offset Y", Float) = 0.0
}

SubShader {

	Tags { "RenderType" = "Opaque" }

	CGPROGRAM
	#pragma surface surf Lambert
	
	struct Input {
		float3 worldNormal;
		float3 worldPos;
	};
	
	sampler2D _MainTex2;
	sampler2D _MainTex1;
 
	float4 _Color;
	float _Scale;
	float _Offsetx;
	float _Offsety;
	
	void surf (Input IN, inout SurfaceOutput o) {
		float2 UV;
		fixed4 c;
		
		UV = IN.worldPos.xz;
		c = tex2D(_MainTex1, UV * _Scale);
		
		if (abs(IN.worldNormal.x) > 0.25) {
			UV = IN.worldPos.zy + float2(_Offsetx, _Offsety);
			c = tex2D(_MainTex1, UV * _Scale);
		} else if (abs(IN.worldNormal.y) > 0.25) {
			UV = IN.worldPos.xz + float2(_Offsetx, _Offsety);
			c = tex2D(_MainTex2, UV * _Scale);
		} else if (abs(IN.worldNormal.z) > 0.25) {
			UV = IN.worldPos.xy + float2(_Offsetx, _Offsety);
			c = tex2D(_MainTex1, UV * _Scale);
		}
		
		o.Albedo = c.rgb * _Color;
	}
	
	ENDCG
} 
	
	Fallback "VertexLit"
}