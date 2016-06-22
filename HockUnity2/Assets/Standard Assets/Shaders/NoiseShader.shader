Shader "Metkis/Noise" {
    Properties {
        _Color ("Main Color", Color) = (30,70,80,0)
       	_MainTex ("Noise Map", 2D) = "white" {}
       	_SecondTex ("Darkening", 2D) = "white" {}
       	_ThirdTex ("Scanlines", 2D) = "white" {}
       	_FourthTex ("Latent Image", 2D) = "white" {}
       	_ScrollSpeed ("Scroll Speed", Range(-100,100)) = 5
       	_ScrollSpeed2 ("Scroll Speed 2", Range(-30,30)) = 10
       	_Brightness ("Brightness", Range(-1,10)) = 1
       	_ScanIntens ("Scanline Intensity", Range(-1,3)) = 1
       	_LatentImageInt ("Latent Intensity", Range(-1,2)) = 0.3







    }
    
    SubShader {
 Tags {"RenderType" = "Opaque" }
Lighting On
         CGPROGRAM
         #pragma surface surf Lambert vertex:vert
         
        float4 _Color;
        float _ScrollSpeed;
                float _LatentImageInt;

                float _Brightness;

        float _ScrollSpeed2;
                float _ScanIntens;


        sampler2D _MainTex;
        sampler2D _SecondTex;
        sampler2D _ThirdTex;
        sampler2D _FourthTex;

        float2 uv_MainTex;
        float2 uv_SecondTex;
        float2 uv_ThirdTex;
        float2 uv_FourthTex;
        float3 worldPos;

        struct Input {
        float2 uv_MainTex;
        float2 uv_SecondTex;
        float3 worldPos;

                  float2 uv_ThirdTex;
                                    float2 uv_FourthTex;
         };

                 void vert(inout appdata_full v)
        {
            #if !defined(SHADER_API_OPENGL)

            v.vertex.xyz = v.vertex.xyz;

            #endif
        }
 
             void surf (Input IN, inout SurfaceOutput o) {

			fixed2 scrollUV = IN.uv_MainTex;
            float timeFrac = _Time;
            fixed xScrollValue = timeFrac;
            fixed yScrollValue = timeFrac;
            scrollUV += fixed2( xScrollValue*22 , yScrollValue*222);

            fixed2 scrollUV2 = IN.uv_SecondTex;
            float timeFrac2 = _Time;
            fixed xScrollValue2 = timeFrac2;
            fixed yScrollValue2 = timeFrac2;
            scrollUV2 += fixed2( xScrollValue2 , yScrollValue2*_ScrollSpeed);

            fixed2 scrollUV3 = IN.uv_ThirdTex;
            float timeFrac3 = _Time;
            fixed xScrollValue3 = timeFrac3;
            fixed yScrollValue3 = timeFrac3;
            scrollUV3 += fixed2( xScrollValue3 , yScrollValue3*_ScrollSpeed2);

           	 half4 c = tex2D(_MainTex, IN.uv_MainTex) +1;
             o.Albedo =  tex2D(_MainTex , scrollUV)+(tex2D(_ThirdTex , scrollUV3)/2) * tex2D(_SecondTex , scrollUV2) ;
			o.Albedo +=  tex2D(_FourthTex , IN.uv_FourthTex)/2 * _Brightness  * _LatentImageInt ;
			o.Albedo +=  tex2D(_SecondTex , scrollUV2) *_ScanIntens;
o.Albedo = o.Albedo *_Color;

			o.Alpha = 1;




         }

                  ENDCG
    }
    Fallback "Diffuse"
}