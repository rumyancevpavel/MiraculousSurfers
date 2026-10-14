// Stand-in for a missing Ladybug shader (original GUID cc1d2a2dab9677c4e9c1957c6ea0abc2).
// Used by 18 material(s), e.g. "Ladybug_Albedo". Replace with the original shader if it is found.
Shader "Ladybug/Recovered/Character/cc1d2a2d"
{
    Properties
    {
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
        _FresnelRange ("Fresnel Range", Float) = 4
        _FresnelIntensity ("Fresnel Intensity", Float) = 0.7
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
            #include "LadybugRecovered.cginc"
            ENDCG
        }
    }
    Fallback "Legacy Shaders/VertexLit"
}
