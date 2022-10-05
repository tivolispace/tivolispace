Shader "Maki/Login Background"
{
    Properties
    {
//        _MainTex ("Main texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            // sampler2D _MainTex;
            // float4 _MainTex_ST;

            #define mod(x,y) (x-y*floor(x/y))

            float3 HSVtoRGB(float3 hsv) {
				// thx shaderforge <3~
				return lerp(float3(1,1,1),saturate(3.0*abs(1.0-2.0*frac(hsv.r+float3(0.0,-1.0/3.0,1.0/3.0)))-1),hsv.g)*hsv.b;
			}
            
            #define TIVOLI_PINK_H 340.0 / 360.0
            #define TIVOLI_PINK_S 0.87
            #define TIVOLI_PINK_V 0.91

            // thank you @tdhooper
            // https://www.shadertoy.com/view/Xt3yDS
            
            float3 hash33(in float3 p)
            {
                const float n = sin(dot(p, float3(7.0, 157.0, 113.0)));
                return frac(float3(2097152.0, 262144.0, 32768.0) * n) * 2.0 - 1.0;
            }

            float tetraNoise(in float3 p)
            {
                float3 i = floor(p + dot(p, float3(0.333333, 0.333333, 0.333333)));
                p -= i - dot(i, float3(0.166666, 0.166666, 0.166666));
                float3 i1 = step(p.yzx, p);
                float3 i2 = max(i1, 1.0 - i1.zxy);
                i1 = min(i1, 1.0 - i1.zxy);
                float3 p1 = p - i1 + 0.166666;
                float3 p2 = p - i2 + 0.333333;
                float3 p3 = p - 0.5;
                float4 v = max(0.5 - float4(dot(p, p), dot(p1, p1), dot(p2, p2), dot(p3, p3)), 0.0);
                float4 d = float4(dot(p, hash33(i)), dot(p1, hash33(i + i1)), dot(p2, hash33(i + i2)), dot(p3, hash33(i + 1.0)));
                return clamp(dot(d, v * v * v * 8.0) * 1.732 + 0.5, 0.0, 1.0);
            }
                                    
            
            float2 smoothRepeatStart(in float x, in float size)
            {
                return float2(
                    mod((x - (size / 2.0)), size),
                    mod(x, size)
                );
            }
            
            float smoothRepeatEnd(in float a, in float b, in float x, in float size)
            {
                return lerp(a, b,
                    smoothstep(
                        0.0, 1.0,
                        sin(x / size * UNITY_PI * 2.0 - UNITY_PI * 0.5) * 0.5 + 0.5
                    )
                );
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // https://www.shadertoy.com/view/Xt3yDS
                
                float2 uv = (i.texcoord.xy);
                uv.x *= _ScreenParams.x / _ScreenParams.y;
                uv.x -= 0.5;
                uv *= 2.0;

                float repeatSize = 4.;
                float speed = 0.01;
                float x = uv.x - mod(_Time.y * speed, repeatSize / 2.0);
                float y = uv.y;
                
                float2 ab; // two sample points on one axis
                
                float noise;
                float noiseA, noiseB;

                // Blend noise at different frequencies, moving in
                // different directions
                
                ab = smoothRepeatStart(x, repeatSize);
                noiseA = tetraNoise(16.+float3(float2(ab.x, uv.y) * 1.2, 0)) * .5;
                noiseB = tetraNoise(16.+float3(float2(ab.y, uv.y) * 1.2, 0)) * .5;
                noise = smoothRepeatEnd(noiseA, noiseB, x, repeatSize);
                
                ab = smoothRepeatStart(y, repeatSize / 2.);
                noiseA = tetraNoise(float3(float2(uv.x, ab.x) * .5, 0)) * 2.;
                noiseB = tetraNoise(float3(float2(uv.x, ab.y) * .5, 0)) * 2.;
                noise *= smoothRepeatEnd(noiseA, noiseB, y, repeatSize / 2.);
                
                ab = smoothRepeatStart(x, repeatSize);
                noiseA = tetraNoise(9.+float3(float2(ab.x, uv.y) * .05, 0)) * 5.;
                noiseB = tetraNoise(9.+float3(float2(ab.y, uv.y) * .05, 0)) * 5.;
                noise *= smoothRepeatEnd(noiseA, noiseB, x, repeatSize);
                
                noise *= .666;
                
                // Blend with a linear gradient, this gives the isolines a
                // common orientation (try changing .6 to 1.)
                noise = lerp(noise, dot(uv, float2(-.66,1.)*.4), .6);

                // Create anti-aliased even weight isolines from the noise...

                // Break the continuous noise into steps
                float spacing = 1./50.;
                float lines = mod(noise, spacing) / spacing;

                 // Convert each step into a bump, or, the sawtooth wave
                // into a triangle wave:
                //
                //     /|    /|
                //   /  |  /  |
                // /    |/    |
                //       
                // to:   
                //       
                //   /\    /\
                //  /  \  /  \ 
                // /    \/    \
                
                lines = min(lines * 2., 1.) - max(lines * 2. - 1., 0.);

                // Scale it by the amount the noise varies over a pixel,
                // factoring in the spacing scaling that was applied.
                // noise is used because it's continuous, if we use lines we'd
                // see stepping artefacts.
                lines /= fwidth(noise / spacing);

                // Double to occupy two pixels and appear smoother
                lines /= 2.;

                // make lines thicker
                lines -= 0.1;
                
                float3 color = lerp(
                    HSVtoRGB(float3(TIVOLI_PINK_H, TIVOLI_PINK_S - 0.4, TIVOLI_PINK_V)),
                    // float3(1,1,1),
                    HSVtoRGB(float3(TIVOLI_PINK_H, TIVOLI_PINK_S, TIVOLI_PINK_V)),
                    clamp(lines, 0, 1)
                );

                return fixed4(pow(color, 2.2), 1);
            }
            ENDCG
        }
    }
}
