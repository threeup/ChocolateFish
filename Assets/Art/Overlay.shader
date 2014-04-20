Shader "DebugOverlay" {
  SubShader {
    
    Tags { "Queue"="Overlay" "RenderType"="Transparent" } 
    
    Pass {
    	
    	Name "BASE"
        
        ZTest Always

        Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		struct v2f {
			float4 pos : SV_POSITION;
			fixed4 color : COLOR;
		};

		v2f vert (appdata_full v)
		{
			v2f o;
			float4 outpos = v.vertex;

			// prevent z-fighting along the y  add 0 to 0.1
			outpos.y += (v.color.x*100 + v.color.y*10 + v.color.z)/1100;	
			
			o.pos = mul (UNITY_MATRIX_MVP, outpos);
			o.color = v.color;
			return o;
		}

		fixed4 frag (v2f i) : COLOR0 
		{ 
			return i.color; 
		}
		ENDCG
    }
  } 
}