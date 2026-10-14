// Stand-in for a missing Ladybug shader (original GUID 4c0656ce01fca0443a627f5f5ce8fdb8).
// Used by 2 material(s), e.g. "Up +Y". Replace with the original shader if it is found.
Shader "Ladybug/Recovered/Sky/4c0656ce"
{
    Properties
    {
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
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
            #define LB_NOFOG 1
            #include "LadybugRecovered.cginc"
            ENDCG
        }
    }
    Fallback "Legacy Shaders/VertexLit"
}
