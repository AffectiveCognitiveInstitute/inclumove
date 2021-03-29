// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/4-points-gradient-mat" {
	Properties {
		[Header(4 points gradient)]
		col_left_top("col_left_top", Color) = (1,1,1,1)
		col_right_top("col_right_top", Color) = (1,1,1,1)
		col_right_bottom("col_right_bottom", Color) = (1,1,1,1)
		col_left_bottom("col_left_bottom", Color) = (1,1,1,1)

		[Header(gradient behaviours)]
		[Toggle(enable_gradient_luminance)] enable_gradient_luminance("enable_gradient_luminance", Int) = 0
		luminance_intensity("luminance_intensity", Range(0, 1)) = 1
		[Toggle(enable_smooth_step)] enable_smooth_step("enable_smooth_step (slower, better quality)", Int) = 0


		[Space(20)] 

		[Header(default attributes)]
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Cull Off

		Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		#pragma shader_feature enable_gradient_luminance
		#pragma shader_feature enable_smooth_step

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		float4 col_left_bottom;
		float4 col_left_top;
		float4 col_right_top;
		float4 col_right_bottom;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float luminance_intensity;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		float4 cal_4p_grad(float2 coord, float4 c0, float4 c1, float4 c2, float4 c3){
			// original glsl code from
			// https://johnflux.com/2016/03/16/four-point-gradient-as-a-shader/

			float2 p0 = float2(0.0, 0.0); // lb
			float2 p1 = float2(1.0, 0.0); // rb
			float2 p2 = float2(0.0, 1.0); // lt
			float2 p3 = float2(1.0, 1.0); // rt

			float2 Q = p0 - p2;
			float2 R = p1 - p0;
			float2 S = R + p2 - p3;
			float2 T = p0 - coord;

			float u;
			float t;

			if(Q.x == 0.0 && S.x == 0.0) {
				u = -T.x/R.x;
				t = (T.y + u*R.y) / (Q.y + u*S.y);
			} else if(Q.y == 0.0 && S.y == 0.0) {
				u = -T.y/R.y;
				t = (T.x + u*R.x) / (Q.x + u*S.x);
			} else {
				float A = S.x * R.y - R.x * S.y;
				float B = S.x * T.y - T.x * S.y + Q.x*R.y - R.x*Q.y;
				float C = Q.x * T.y - T.x * Q.y;
				// Solve Au^2 + Bu + C = 0
				if(abs(A) < 0.0001)
					u = -C/B;
				else
					u = (-B+sqrt(B*B-4.0*A*C))/(2.0*A);
				t = (T.y + u*R.y) / (Q.y + u*S.y);

			}
			u = clamp(u,0.0,1.0);
			t = clamp(t,0.0,1.0);

			// These two lines smooth out t and u to avoid visual 'lines' at the boundaries.  
			// They can be removed to improve performance at the cost of graphics quality.
			#if enable_smooth_step
				t = smoothstep(0.0, 1.0, t);
				u = smoothstep(0.0, 1.0, u);
			#endif

			float4 colorA = lerp(c0,c1,u);
			float4 colorB = lerp(c2,c3,u);
			return lerp(colorA, colorB, t);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			float4 grad = cal_4p_grad(IN.uv_MainTex, col_right_top, col_left_top, col_right_bottom, col_left_bottom);

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgba * grad.rgba;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			#if enable_gradient_luminance
				o.Emission = grad.rgba * luminance_intensity;
			#endif
			o.Alpha = grad.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}