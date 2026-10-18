// Stand-in for a missing Ladybug shader (original GUID 65f7c3f7b6c78f84a9c53fb7e57de57f).
// Used by 12 material(s), e.g. "ArrowColor". Replace with the original shader if it is found.
Shader "Ladybug/Recovered/CurvedFX/65f7c3f7"
{
    Properties
    {
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _BendAmount ("Bend Amount", Vector) = (0,-8,0,0)
        _BendOrigin ("Bend Origin", Vector) = (0,0,0,0)
        _BendFalloff ("Bend Falloff", Float) = 200
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
            #define LB_BEND 1
            #define LB_VCOLOR 1
            #include "LadybugRecovered.cginc"
            ENDCG
        }
    }
    Fallback Off
}

