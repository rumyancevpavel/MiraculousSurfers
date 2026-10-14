// Stand-in for a missing Ladybug shader (original GUID b86feb2616e7c1945be176bdef47fad9).
// Used by 23 material(s), e.g. "mat_Hotel_Chunk1_Floor". Replace with the original shader if it is found.
Shader "Ladybug/Recovered/Env/b86feb26"
{
    Properties
    {
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Lightmap ("Lightmap (UV2)", 2D) = "white" {}
        _LightmapIntensity ("Lightmap Intensity", Float) = 1
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
            #define LB_LIGHTMAP 1
            #define LB_CUTOUT 1
            #include "LadybugRecovered.cginc"
            ENDCG
        }
    }
    Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
}
