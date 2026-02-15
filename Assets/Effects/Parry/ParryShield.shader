Shader "Custom/ParryShield"
{
    Properties
    {
        _Color ("Shield Color", Color) = (0.2, 0.6, 1.0, 0.4)
        _EdgeColor ("Edge Color", Color) = (0.4, 0.8, 1.0, 0.8)
        _FresnelPower ("Fresnel Power", Range(0.5, 5)) = 2.0
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 3.0
        _PulseAmount ("Pulse Amount", Range(0, 0.5)) = 0.15
        _ScrollSpeed ("Scroll Speed", Range(0, 5)) = 1.5
        _NoiseScale ("Noise Scale", Range(1, 20)) = 8.0
        _Alpha ("Overall Alpha", Range(0, 1)) = 0.5
        _FadeDirection ("Fade Direction", Range(-1, 1)) = 1.0
        _FadePower ("Fade Power", Range(0.5, 5)) = 1.5
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
            };
            
            float4 _Color;
            float4 _EdgeColor;
            float _FresnelPower;
            float _PulseSpeed;
            float _PulseAmount;
            float _ScrollSpeed;
            float _NoiseScale;
            float _Alpha;
            float _FadeDirection;
            float _FadePower;
            
            // Simple noise function
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }
            
            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                
                float a = hash(i);
                float b = hash(i + float2(1, 0));
                float c = hash(i + float2(0, 1));
                float d = hash(i + float2(1, 1));
                
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.uv;
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                // Fresnel edge glow
                float fresnel = pow(1.0 - saturate(dot(i.viewDir, i.worldNormal)), _FresnelPower);
                
                // Scrolling noise pattern
                float2 noiseUV = i.uv * _NoiseScale + float2(0, _Time.y * _ScrollSpeed);
                float n = noise(noiseUV) * 0.5 + 0.5;
                
                // Pulse
                float pulse = 1.0 + sin(_Time.y * _PulseSpeed) * _PulseAmount;
                
                // Combine colors
                float4 col = lerp(_Color, _EdgeColor, fresnel);
                col.rgb *= pulse;
                col.rgb += fresnel * _EdgeColor.rgb * 0.5;
                
                // Add noise variation
                col.rgb *= (0.8 + n * 0.4);
                
                // Alpha: stronger at edges, weaker in center
                float verticalFade = _FadeDirection > 0 
                    ? pow(1.0 - i.uv.y, _FadePower)   // fade out at top
                    : pow(i.uv.y, _FadePower);          // fade out at bottom
                col.a = saturate((_Color.a + fresnel * 0.6) * _Alpha * pulse * verticalFade);
                
                // Add subtle energy lines
                float lines = sin((i.uv.y + _Time.y * _ScrollSpeed * 0.5) * 30.0) * 0.5 + 0.5;
                col.rgb += lines * _EdgeColor.rgb * 0.1;
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}
