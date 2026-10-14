// Stand-in for a missing Ladybug shader (original GUID e37f44f1a08cf594fb6b3919774527ed).
// Used by 1 material(s), e.g. "LuckyCharm2DMaterial". Replace with the original shader if it is found.
Shader "Ladybug/Recovered/Unlit/e37f44f1"
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
