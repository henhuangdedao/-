Shader "Custom/BackgroundTransition"
{
    Properties
    {
        [Header(Textures)]
        _MainTex ("Texture A (当前)", 2D) = "white" {}
        _NextTex ("Texture B (下一个)", 2D) = "white" {}
        
        [Header(Transition Control)]
        _Blend ("混合度", Range(0, 1)) = 0
        _TransitionSpeed ("过渡速度", Float) = 1.0
        
        [Header(Effect Parameters)]
        _BlurAmount ("模糊强度", Range(0, 0.1)) = 0.01
        _Distortion ("扭曲强度", Range(0, 0.1)) = 0.02
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "Queue"="Geometry"
        }
        
        LOD 100
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
            
            // 纹理
            sampler2D _MainTex;
            sampler2D _NextTex;
            float4 _MainTex_ST;
            
            // 过渡参数
            float _Blend;
            float _TransitionSpeed;
            float _BlurAmount;
            float _Distortion;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
            // 简单模糊采样
            fixed4 sampleBlur(sampler2D tex, float2 uv, float blur)
            {
                fixed4 color = fixed4(0, 0, 0, 0);
                float2 offsets[9] = 
                {
                    float2(-blur, blur),  float2(0, blur),  float2(blur, blur),
                    float2(-blur, 0),     float2(0, 0),     float2(blur, 0),
                    float2(-blur, -blur), float2(0, -blur), float2(blur, -blur)
                };
                float weights[9] = 
                {
                    0.05, 0.1, 0.05,
                    0.1,  0.3, 0.1,
                    0.05, 0.1, 0.05
                };
                
                for(int i = 0; i < 9; i++)
                {
                    color += tex2D(tex, uv + offsets[i]) * weights[i];
                }
                
                return color;
            }
            
            // 噪声函数（用于扭曲效果）
            float noise(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 基础UV
                float2 uv = i.uv;
                
                // 添加扭曲效果
                float2 distortion = float2(
                    sin(uv.y * 10.0 + _Time.y) * 0.01 * _Distortion * _Blend,
                    cos(uv.x * 10.0 + _Time.y) * 0.01 * _Distortion * _Blend
                );
                uv += distortion;
                
                // 采样纹理A（带模糊）
                fixed4 colA = sampleBlur(_MainTex, uv, _BlurAmount * _Blend);
                
                // 采样纹理B（带模糊）
                fixed4 colB = sampleBlur(_NextTex, uv, _BlurAmount * (1.0 - _Blend));
                
                // 交叉淡入淡出
                fixed4 finalColor = lerp(colA, colB, _Blend);
                
                // 添加淡入淡出边缘效果
                float edgeEffect = smoothstep(0.0, 0.2, _Blend) * (1.0 - smoothstep(0.8, 1.0, _Blend));
                finalColor.rgb += edgeEffect * 0.1;
                
                // 应用雾效
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                
                return finalColor;
            }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}