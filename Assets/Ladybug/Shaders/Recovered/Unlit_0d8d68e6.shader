// Stand-in for a missing Ladybug shader (original GUID 0d8d68e6c50d83c4d92b511f53ca7307).
// Used by 2 material(s), e.g. "TreeObstacles_Trees". Replace with the original shader if it is found.
Shader "Ladybug/Recovered/Unlit/0d8d68e6"
{
    Properties
    {
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #define LB_CUTOUT 1
            #include "LadybugRecovered.cginc"
            ENDCG
        }
    }
    Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
}
