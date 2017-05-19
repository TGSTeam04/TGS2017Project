// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/RefractiveOri" {

Properties{
_MainTex("Main", 2D) = "white" { }
_BumpMap("Normals ", 2D) = "bump" {}
_BaseColor("Base color", COLOR) = (.54, .95, .99, 0.5)
_BumpTiling("Bump Tiling", Vector) = (1.0 ,1.0, -2.0, 3.0)
_BumpDirection("Bump Direction & Speed", Vector) = (1.0 ,1.0, -1.0, 1.0)
_BumpStrength("Bump Strength", Float) = 1.0
}

SubShader{
	Pass{
	Zwrite On
	ColorMask A
	Color (1,1,1,0)
}
GrabPass{ "_RefractionTex" }
Pass{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

	sampler2D _MainTex;
	sampler2D _BumpMap;
	sampler2D _RefractionTex;
	uniform float _BumpStrength;
	uniform float4 _BumpTiling;
	uniform float4 _BumpDirection;

	struct appdata {
		float4 vertex   : POSITION;
		float4 texcoord : TEXCOORD0;
	};

	struct v2f {
		float4 pos : SV_POSITION; 
		float4 bumpCoords : TEXCOORD1;
		float4 grabPassPos : TEXCOORD2;
	};

	v2f vert(appdata v) {
		v2f o;

		o.bumpCoords.xyzw = (v.texcoord.xyxy + _Time.xxxx * _BumpDirection.xyzw) * _BumpTiling.xyzw;

		o.pos = UnityObjectToClipPos(v.vertex);

#if UNITY_UV_STARTS_AT_TOP
		float scale = -1.0;
#else
		float scale = 1.0f;
#endif
		o.grabPassPos.xy = (float2(o.pos.x, o.pos.y*scale) + o.pos.w) * 0.5;
		o.grabPassPos.zw = o.pos.zw;

		return o;
	}

	half4 frag(v2f i) : SV_Target{
		half3 bump = (UnpackNormal(tex2D(_BumpMap, i.bumpCoords.xy)) + UnpackNormal(tex2D(_BumpMap, i.bumpCoords.zw))) * 0.5;
		half4 distortOffset = half4(normalize(half3(0, 1, 0) + bump.xxy * _BumpStrength * half3(1, 0, 1)).xz, 0, 0);
		half4 grabWithOffset = i.grabPassPos + distortOffset;
		half4 Color = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(grabWithOffset));
		//Color = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(i.grabPassPos))*Color.w +Color*(1 - Color.w);
		return Color;
	}

	ENDCG
	}
Pass{
	Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha

	CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
	uniform sampler2D _MainTex;
	uniform fixed4 _MainTex_ST;
	uniform float4 _BaseColor;
	sampler2D _BumpMap;
	uniform float4 _BumpTiling;
	uniform float4 _BumpDirection;
	uniform float _BumpStrength;

	struct appdata_ {
		float4 vertex:POSITION;
		float3 normal:NORMAL;
		float2 texcoord:TEXCOORD0;
	};

	struct v2f_ {
		float4 pos : SV_POSITION;
		float2 uv  : TEXCOORD0;
		//float4 bumpCoords : TEXCOORD1;
		//float4 lightDirection : COLOR0;

	};

	v2f_ vert(appdata_ v) {
		v2f_ o;
		o.pos = UnityObjectToClipPos(v.vertex);
		//o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.uv = v.texcoord;

		//o.bumpCoords.xyzw = (v.texcoord.xyxy + _Time.xxxx * _BumpDirection.xyzw) * _BumpTiling.xyzw;

		//float4 wnormal = normalize(mul(v.normal, unity_WorldToObject));
		//float3 n = normalize(wnormal).xyz;
		//float3 t = normalize(cross(n, float3(0, 1, 0)));
		//float3 b = cross(n, t);

		//float4 light = _WorldSpaceLightPos0;
		//o.lightDirection.x = dot(t, light);
		//o.lightDirection.y = dot(b, light);
		//o.lightDirection.z = dot(n, light);
		//o.lightDirection = normalize(o.lightDirection);

		return o;
	}

	half4 frag(v2f_ v) : SV_Target
	{
		//half3 bump = (UnpackNormal(tex2D(_BumpMap, v.bumpCoords.xy)) + UnpackNormal(tex2D(_BumpMap, v.bumpCoords.zw))) * 0.5* _BumpStrength;
		//float3 light = normalize(v.lightDirection).xyz;
		//float  diffuse = max(0, dot(bump, light));
		half4  Color = tex2D(_MainTex, v.uv)*_BaseColor;
		return Color;// *diffuse;
	}
ENDCG
}
}
}