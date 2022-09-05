Shader "Tivoli/UI Button"
{
    Properties
    {
        // _MainTex ("Texture", 2D) = "white" {}
        _FrontColor ("Front Color", Color) = (0.91372549,0.11764705882,0.38823529411,0)
        _SideColor ("Side Color", Color) = (0.91372549,0.11764705882,0.38823529411,0)
        _Width ("Width", float) = 1
        _Height ("Height", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct Input
            {
                // float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 localCoord : TEXCOORD1;
                float3 localNormal : NORMAL;
            };

            float4 _FrontColor;
            float4 _SideColor;
            
            float _Width;
            float _Height;

            Input vert (appdata_full v)
            {
                Input o;
                UNITY_INITIALIZE_OUTPUT(Input, o);
                
                o.localCoord = v.vertex.xyz;
                
                if (_Width < 0.5) _Width = 0.5;
                const float halfWidth = _Width / 2 - 0.5;
                
                if (o.localCoord.x < 0)
                {
                    o.localCoord.x -= halfWidth;
                }
                else if (o.localCoord.x > 0)
                {
                    o.localCoord.x += halfWidth;
                }

                if (_Height < 0.5) _Height = 0.5;
                const float halfHeight = _Height / 2 - 0.5;
                
                if (o.localCoord.y < 0)
                {
                    o.localCoord.y -= halfHeight;
                }
                else if (o.localCoord.y > 0)
                {
                    o.localCoord.y += halfHeight;
                }

                o.localCoord *= 0.25;
                
                
                
                o.vertex = UnityObjectToClipPos(o.localCoord);
                
                o.localNormal = v.normal.xyz;
                
                UNITY_TRANSFER_FOG(o,o.vertex);
                
                return o;
            }

            fixed4 frag (Input i) : SV_Target
            {
                // // Blending factor of triplanar mapping
                // float3 bf = normalize(abs(i.localNormal));
                // bf /= dot(bf, (float3)1);
                //
                // // // Triplanar mapping
                // float2 tx = i.localCoord.yz;
                // float2 ty = i.localCoord.zx;
                // float2 tz = i.localCoord.xy;

                
                fixed4 color = _FrontColor;

                if (i.localNormal.z > -0.6 && i.localNormal.z < 0.6)
                {
                    color = _SideColor;
                }

                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, color);
                return color;
            }
            ENDCG
        }
    }
}
