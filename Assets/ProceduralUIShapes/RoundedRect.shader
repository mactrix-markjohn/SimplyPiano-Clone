// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "ProceduralUI/RoundedRect"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Main Color", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		
		Pass
		{
			Name "Default"
		CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
				float2 texcoord3 : TEXCOORD3;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 rectSize : TEXCOORD1;
				float4 color : COLOR;
				float4 otherData : COLOR1;
				float2 shadowSettings : TEXCOORD2;
				float2 worldPosition : TEXCOORD3;
				
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;

			fixed4 _TextureSampleAdd;

			bool _UseClipRect;
			float4 _ClipRect;

			bool _UseAlphaClip;
			
			v2f vert(appdata_t v)
			{
				v2f o;
				
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.worldPosition = v.vertex;
				o.vertex = UnityObjectToClipPos(float3(v.vertex.x, v.vertex.y, 0));

				o.uv = v.texcoord;
				o.rectSize = v.texcoord1;
				o.color = v.color;

				float rounded = v.texcoord2.x;
				float thickness = v.texcoord2.y;
				float type = v.texcoord3.x;
				o.otherData = float4(rounded, thickness, type, v.vertex.z);
				o.shadowSettings = float2(v.texcoord3.y, v.vertex.z);

				return o;
			}

			float roundedRectFull(float2 warpedUVs)
			{
				float roundedRectOuter = length(((warpedUVs*2.0 - 1.0)));
				return saturate((1.0 - roundedRectOuter) / fwidth(roundedRectOuter));
			}

			float roundedRectInserted(float2 warpedUVs, float2 uv, float offset, float roundedValue, float2 rectSize, float aaOffset)
			{
				float innerOffset = (1.0 - (offset / roundedValue));
				float roundedRectInner = length(((warpedUVs*2.0 - 1.0) / innerOffset.xx));
				float fillRounded = saturate(((1.0 - roundedRectInner) / fwidth(roundedRectInner)) + aaOffset);

				float2 squareRectInnerOffset = 1.0 - (offset.xx / rectSize) * 2.0;
				float2 temp = (abs((uv * 2.0 - 1.0)) - squareRectInnerOffset);
				float2 squareRectInner = (1.0 - (temp / fwidth(temp)));
				float fillSquare = saturate(min(squareRectInner.x, squareRectInner.y));

				return lerp(fillSquare, fillRounded, saturate((roundedValue - offset) * 100));
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				float2 uv = IN.uv;
				float2 rectSize = IN.rectSize;
				float2 localPosition = uv * rectSize;
				float roundedValue = IN.otherData.x;
				float borderThicknessValue = IN.otherData.y;
				float isBorder = IN.otherData.z;
				float isShadow = IN.shadowSettings.x;
				float shadowBlur = IN.shadowSettings.y;
				float applySpriteToBorder = IN.otherData.w;
				float4 color = IN.color;

				float2 warpedUVs = max(min(((localPosition / roundedValue) * float2(0.5, 0.5)), float2(0.5, 0.5)), (((localPosition - (rectSize - (roundedValue * 2.0).xx)) / roundedValue) / float2(2, 2)));

				float overlapAmount = color.a * 0.5;
				float fill = roundedRectInserted(warpedUVs, uv, borderThicknessValue, roundedValue, rectSize, 0);
				float borderCutout = roundedRectInserted(warpedUVs, uv, borderThicknessValue, roundedValue, rectSize, -overlapAmount);
				float border = roundedRectFull(warpedUVs) - borderCutout;


				//Shadow
				rectSize += float2(shadowBlur * 2, shadowBlur * 2);
				localPosition = uv * rectSize;
				float roundedShadowValue = max(roundedValue + shadowBlur / 2, shadowBlur);
				float2 warpedShadowUVs = max(min(((localPosition / roundedShadowValue) * float2(0.5, 0.5)), float2(0.5, 0.5)), (((localPosition - (rectSize - (roundedShadowValue * 2.0).xx)) / roundedShadowValue) / float2(2, 2)));
				float roundedRectShadow = length(((warpedShadowUVs*2.0 - 1.0)));

				float shadowMaskSharp = saturate((1 - roundedRectShadow) / fwidth(roundedRectShadow));
				float shadowMaskBlured = 1 - smoothstep(1 - shadowBlur / roundedShadowValue, 1, roundedRectShadow);
				float shadowMask = lerp(shadowMaskSharp, shadowMaskBlured, saturate(shadowBlur));

				float4 textureSample = saturate(tex2D(_MainTex, uv) + isShadow + saturate(isBorder - applySpriteToBorder));
				color *= textureSample;

				float alphaFill = fill * (1 - isBorder) * (1 - isShadow);
				float alphaborder = border * isBorder * (1 - isShadow);
				float alphaShadow = shadowMask * isShadow;

				float alpha = max(max(alphaFill, alphaborder), alphaShadow);
				color.a *= alpha;

				#ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
}