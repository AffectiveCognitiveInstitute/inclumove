Shader "Particles/Inclumove-Particle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Icon("Icon", 2D) = "black"{}
		_ColorA("Color A", Color) = (1,1,1,1)
		_ColorB("Color B", Color) = (0,0,0,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags
		{ 
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
			"PreviewType" = "Plane"
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
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "UnityUI.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float2 icon_uv: TEXCOORD1;
                float4 vertex : SV_POSITION;
				float4 worldPosition : TEXCOORD2;
            };

            sampler2D _MainTex;
			sampler2D _Icon;
            float4 _MainTex_ST;
			float4 _Icon_ST;
			float4 _ColorA;
			float4 _ColorB;
			float4 _ClipRect;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPosition = v.vertex;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.icon_uv = TRANSFORM_TEX(v.uv, _Icon);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 mask = tex2D(_MainTex, i.uv);
				fixed4 icon = tex2D(_Icon, i.icon_uv);
				fixed4 col = lerp(_ColorA, _ColorB, mask.r);
				col.a = mask.a;

				// Composition: standard alpha blending
				fixed4 outCol =  icon.a * icon + (1 - icon.a) * col;

#ifdef UNITY_UI_CLIP_RECT
				outCol.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
#endif
				return outCol;
            }
            ENDCG
        }
    }
}
