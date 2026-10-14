// Stand-in for a missing Ladybug shader (original GUID 0a2a3c3bbc1e05746bc1cc54637b1c49).
// Used by 2 material(s), e.g. "MapTreesSeparator". Replace with the original shader if it is found.
Shader "Ladybug/Recovered/Curved/0a2a3c3b"
{
    Properties
    {
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _BendAmount ("Bend Amount", Vector) = (0,-8,0,0)
        _BendOrigin ("Bend Origin", Vector) = (0,0,0,0)
        _BendFalloff ("Bend Falloff", Float) = 200
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
            #define LB_BEND 1
            #define LB_LIGHTMAP 1
            #define LB_CUTOUT 1
            #include "LadybugRecovered.cginc"
            ENDCG
        }
    }
    Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
}
