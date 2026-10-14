// Stand-in for a missing Ladybug shader (original GUID f9a7efc8bc223444a9661bfa481cd7d1).
// Used by 2 material(s), e.g. "CosmoBugMask_Material". Replace with the original shader if it is found.
Shader "Ladybug/Recovered/Character/f9a7efc8"
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
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
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
    Fallback Off
}
