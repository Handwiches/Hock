Shader "Metkis/HologramCullOff" {
    Properties {
        _Color ("Main Color", Color) = (30,70,80,0)
        _ColorNoise ("Noise Color", Color) = (30,70,80,0)
        _ImageTex ("Main Image", 2D) = "white" {}
        _MainTex ("Scanlines", 2D) = "white" {}
        _NoiseTex ("Displacement Map", 2D) = "white" {}
        _SecondaryTex ("Noise Map", 2D) = "white" {}
        _TimeScale ("Motion Speed",  Range (0, 10)) = 4
        _Displacement("Motion Displacement",  Range (0, 1)) = 0.3
        _Cutoff("Image Alpha Cutoff",  Range (0, 1)) = 0.3
        _Illumination("Illumination",  Range (0, 2)) = 0.3
        _XScrollSpeed ("X Scanline Speed",  Range (-50, 50) ) = 0
        _YScrollSpeed ("Y Scanline Speed", Range (-50, 50) ) = 1
    }
    
    SubShader {
    Cull Off

        Tags{"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 200 
         // Render into depth buffer only
Pass {
ZWrite Off 
Cull Off
Blend SrcAlpha OneMinusSrcAlpha
 ColorMask 0         }
        CGPROGRAM
        #pragma surface surf Lambert vertex:vert alpha
        #pragma target 3.0
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        sampler2D _NoiseTex;
        sampler2D _ImageTex;
        sampler2D _SecondaryTex;



        float _XScrollSpeed;
        float _YScrollSpeed;
     

        float4 _Color;
        float4 _ColorNoise;
        float _Displacement;
        float _TimeScale;
        float _Illumination;
        float _Cutoff;


        struct Input {
            float2 uv_MainTex;
            float2 uv_SecondaryTex;
            float2 uv_ImageTex;

            INTERNAL_DATA
        };

            
        void vert(inout appdata_full v)
        {
            #if !defined(SHADER_API_OPENGL)
            float timeFrac = frac(_Time*_TimeScale);
            float2 texCoord = float2(v.texcoord.x  + -timeFrac , v.texcoord.y + -timeFrac );
            float2 texCoord2 = float2(v.texcoord.x  + -timeFrac , v.texcoord.y + -timeFrac );
            // average the noise out.  This essetially allows any texture to be used as noise.
            float4 noise = tex2Dlod(_NoiseTex, float4(texCoord,0,0)).rgba;
            float displacement = (((noise.x + noise.z + noise.y)/4 - 0.3) * _Displacement);
            v.vertex.xyz = v.vertex.xyz + (normalize(v.normal.xyz) * displacement );

            #endif
        }
          
        void surf(Input IN, inout SurfaceOutput o) 
        {

            fixed2 scrollUV = IN.uv_MainTex ;
            float timeFrac = _Time;
            fixed xScrollValue = timeFrac * _XScrollSpeed;
            fixed yScrollValue = timeFrac * _YScrollSpeed;

            fixed2 scrollUV2 = IN.uv_SecondaryTex ;
            float timeFrac2 = _Time*24;
            fixed xScrollValue2 = timeFrac2 * 22;
            fixed yScrollValue2 = timeFrac2 * 22;
                                    scrollUV2 += fixed2( xScrollValue2, yScrollValue2 );

            float4 NoiseColor =  tex2D(_SecondaryTex , scrollUV2 ).rgba * _ColorNoise.rgba;

            scrollUV += fixed2( xScrollValue, yScrollValue );

            half4 c = tex2D( _MainTex, scrollUV)  * _Color;
            clip(tex2D(_ImageTex, IN.uv_ImageTex).a - _Cutoff - -o.Albedo);
            o.Albedo =  ((tex2D(_MainTex , scrollUV ).rgb) * _Color.rgb * tex2D(_ImageTex, IN.uv_ImageTex).rgba) + NoiseColor.rgb ;
            o.Emission =   o.Albedo * _Illumination;
            o.Alpha = _Color.a * tex2D(_MainTex , scrollUV ).rgb;
            c.a = _Color.a;

        }
        ENDCG
    }
    Fallback "Diffuse"
}