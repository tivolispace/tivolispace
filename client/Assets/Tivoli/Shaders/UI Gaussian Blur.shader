Shader "Maki/UI Gaussian Blur"
{   
    Properties
    {
        _Color ("Overlay Color", Color) = (1, 1, 1, 1)
//        _Directions ("Directions (more is better but slower)", Float) = 16.0
//        _Quality ("Quality (more is better but slower)", Float) = 3.0
        _Size ("Size", Float) = 6.0

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        
        _ColorMask ("Color Mask", Float) = 15
        
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        // Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        GrabPass
        {
            "_BackgroundTexture"
        }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID
			    UNITY_VERTEX_OUTPUT_STEREO
                float4 pos : SV_POSITION;
                float4 grabPos : TEXCOORD0;
            };

            v2f vert(appdata_base v) {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
			    UNITY_SETUP_INSTANCE_ID(v);
			    UNITY_TRANSFER_INSTANCE_ID(v,o);
			    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            sampler2D _BackgroundTexture;

            float4 _Color;

            // float _Directions;
            float _Size;

            bool isVR() {
                // USING_STEREO_MATRICES
                #if UNITY_SINGLE_PASS_STEREO
                    return true;
                #else
                    return false;
                #endif
            }
            
            bool isDesktop() {
                return !isVR() && abs(UNITY_MATRIX_V[0].y) < 0.0000005;
            }
            
            half4 frag(v2f i) : SV_Target
            {
                float2 uv = i.grabPos.xy / i.grabPos.w;
                
                // half4 color = tex2Dproj(_BackgroundTexture, i.grabPos);

                float TAU = 6.28318530718;

                float directions = 16.0; // More is better but slower (default 16)
                float quality = 8.0; // More is better but slower (default 3)
                float size = _Size; // Radius (it gets blurrier in vr)

                // float radius = size/float2(1024,1024); // iResolution.xy
                float radius = size/(isVR() ? 1024 : 256);

                half4 blurColor = tex2D(_BackgroundTexture, uv);

                 for( float d=0.0; d<TAU; d+=TAU/directions)
                {
		            for(float i=1.0/quality; i<=1.0; i+=1.0/quality)
                    {
			            blurColor += tex2D(_BackgroundTexture, uv+float2(cos(d),sin(d))*radius*i);		
                    }
                }

                blurColor /= quality * directions - 15.0;
                
                return half4(lerp(blurColor.rgb, _Color.rgb, _Color.a), blurColor.a);
                
                // return half4(blurColor.rgb * lerp(blurColor.rgb, _Color.rgb, _Color.a), blurColor.a);
            }
            ENDCG
        }
    }
}

