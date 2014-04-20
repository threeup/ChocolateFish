Shader "My/BlackLine" {
	
	// A simple shader to draw lines 
	// (backface culling and lightning are turned off)
	
    Properties {
    	
    }
    
    SubShader {
    	Tags { "Queue" = "Transparent" }
        Pass {
        	Blend SrcAlpha OneMinusSrcAlpha 
            ZWrite Off
            ZTest Always
        	Lighting Off
            Cull Off
            BindChannels {
            	Bind "vertex", vertex
            	Bind "color", color 
            }
        }
    }
    
    FallBack "VertexLit"
} 