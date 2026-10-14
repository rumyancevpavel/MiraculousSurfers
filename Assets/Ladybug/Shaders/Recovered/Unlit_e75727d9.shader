// Stand-in for a missing Ladybug shader (original GUID e75727d9555d9d14ca51d91908c681bc).
// Used by 13 material(s), e.g. "Shield2DMaterial". Replace with the original shader if it is found.
Shader "Ladybug/Recovered/Unlit/e75727d9"
{
    Properties
    {
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
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
            #define LB_VCOLOR 1
            #include "LadybugRecovered.cginc"
            ENDCG
        }
    }
    Fallback Off
}
