// Stand-in for a missing Ladybug shader (original GUID 05466961816717e4886be8441e3e3629).
// Used by 4 material(s), e.g. "SkyboxCylinderMap". Replace with the original shader if it is found.
Shader "Ladybug/Recovered/Sky/05466961"
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
