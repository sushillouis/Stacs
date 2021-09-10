Shader "QuantumTheory/QT-SkyBoxFog"
{
	Properties 
	{
_SkyBoxTex("SkyBox Texture", 2D) = "black" {}
_Color("Sky Color", Color) = (0.5019608,0.5019608,0.5019608,1)
_FogColor("Fog Color", Color) = (0.2559852,0.3522863,0.513986,1)
_FogDensity("Fog Density", Range(0,5) ) = 1
_FogDistance("Fog Distance", Range(0,10) ) = 1
_Clarity("Clarity", Range(0,10) ) = 0

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Background"
"IgnoreProjector"="False"
"RenderType"="Background"

		}

		
Cull Back
ZWrite Off
ZTest LEqual
ColorMask RGBA
Fog{
Mode Off
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  noambient nolightmap 
#pragma target 2.0


sampler2D _SkyBoxTex;
float4 _Color;
float4 _FogColor;
float _FogDensity;
float _FogDistance;
float _Clarity;

			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
				half4 Custom;
			};
			
			inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
half3 spec = light.a * s.Gloss;
half4 c;
c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
c.a = s.Alpha;
return c;

			}

			inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				half3 h = normalize (lightDir + viewDir);
				
				half diff = max (0, dot ( lightDir, s.Normal ));
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0);
				
				half4 res;
				res.rgb = _LightColor0.rgb * diff;
				res.w = spec * Luminance (_LightColor0.rgb);
				res *= atten * 2.0;

				return LightingBlinnPhongEditor_PrePass( s, res );
			}
			
			struct Input {
				float2 uv_SkyBoxTex;
float4 color : COLOR;

			};

			void vert (inout appdata_full v, out Input o) {
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);


			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Custom = 0.0;
				
float4 Tex2D0=tex2D(_SkyBoxTex,(IN.uv_SkyBoxTex.xyxy).xy);
float4 Multiply1=_Color * float4( 2,2,2,2 );
float4 Multiply0=Tex2D0 * Multiply1;
float4 Split0=IN.color;
float4 Pow0=pow(float4( Split0.w, Split0.w, Split0.w, Split0.w),_FogDistance.xxxx);
float4 Multiply2=Pow0 * _FogDensity.xxxx;
float4 Subtract0=Multiply2 - _Clarity.xxxx;
float4 Saturate0=saturate(Subtract0);
float4 Lerp0=lerp(Multiply0,_FogColor,Saturate0);
float4 Master0_0_NoInput = float4(0,0,0,0);
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Emission = Lerp0;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}