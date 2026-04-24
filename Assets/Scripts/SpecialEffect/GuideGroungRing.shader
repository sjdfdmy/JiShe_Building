Shader "Custom/GuideGroundRingCircular"
{
    Properties
    {
        [HDR] _Color ("Color", Color) = (0, 1, 1, 1)
        _Intensity ("Intensity", Float) = 2
        _PulseSpeed ("Pulse Speed", Float) = 0.5
        _RingWidth ("Ring Width", Range(0, 1)) = 0.2
        _Softness ("Edge Softness", Range(0, 0.5)) = 0.05
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        
        Pass
        {
            ZWrite Off
            Cull Off
            Blend SrcAlpha One
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            float4 _Color;
            float _Intensity;
            float _PulseSpeed;
            float _RingWidth;
            float _Softness;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // UV 中心 (0.5, 0.5)
                float2 centerUV = i.uv - 0.5;
                
                // 圆形裁切：距离中心超过 0.5 就 discard/透明
                float distFromCenter = length(centerUV) * 2.0; // 0~1
                
                // 软边裁切
                float circleMask = 1.0 - smoothstep(1.0 - _Softness * 2, 1.0, distFromCenter);
                
                // 如果完全在圆外，直接丢弃（性能优化）
                if (circleMask <= 0) discard;
                
                // 脉冲动画
                float pulse = frac(_Time.y * _PulseSpeed);
                
                // 扩散圆环
                float ringCenter = pulse;
                float ringDist = abs(distFromCenter - ringCenter);
                float ring = 1.0 - smoothstep(0, _RingWidth, ringDist);
                
                // 中心光
                float center = 1.0 - smoothstep(0, 0.15, distFromCenter);
                
                // 外圈残留
                float edge = 1.0 - smoothstep(0.8, 1.0, distFromCenter);
                
                // 综合
                float mask = max(ring, center * 0.3) + edge * 0.2;
                mask *= circleMask; // 应用圆形裁切
                
                // 颜色
                float brightness = 1.0 + pulse * 0.5;
                float3 finalColor = _Color.rgb * _Intensity * brightness;
                
                return fixed4(finalColor, mask * _Color.a);
            }
            ENDCG
        }
    }
}