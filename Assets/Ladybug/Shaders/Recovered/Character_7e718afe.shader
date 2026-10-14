// Stand-in for a missing Ladybug shader (original GUID 7e718afe1cc236d48bf4a8070b2adfef).
// Used by 15 material(s), e.g. "Cat_Noir_UI_Diffuse". Replace with the original shader if it is found.
Shader "Ladybug/Recovered/Character/7e718afe"
{
    Properties
    {
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
        _FresnelRange ("Fresnel Range", Float) = 4
        _FresnelIntensity ("Fresnel Intensity", Float) = 0.7
        _GreyBlend ("Grey Blend", Range(0,1)) = 0
        _GreyScaleColor ("Grey Scale Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #define LB_LIT 1
            #define LB_RIM 1
            #define LB_GREY 1
            #include "LadybugRecovered.cginc"
            ENDCG
        }
    }
    Fallback "Legacy Shaders/VertexLit"
}
