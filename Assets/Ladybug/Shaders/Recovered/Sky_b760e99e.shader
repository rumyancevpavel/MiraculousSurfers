// Stand-in for a missing Ladybug shader (original GUID b760e99e78543d54eacb1855aaf17221).
// Used by 1 material(s), e.g. "SkyboxCylinderStorm". Replace with the original shader if it is found.
Shader "Ladybug/Recovered/Sky/b760e99e"
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
