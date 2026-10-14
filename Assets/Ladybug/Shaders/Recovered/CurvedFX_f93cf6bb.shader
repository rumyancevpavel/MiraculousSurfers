// Stand-in for a missing Ladybug shader (original GUID f93cf6bbad2369343adef625a8a3f917).
// Used by 2 material(s), e.g. "MapForcefield". Replace with the original shader if it is found.
Shader "Ladybug/Recovered/CurvedFX/f93cf6bb"
{
    Properties
    {
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _BendAmount ("Bend Amount", Vector) = (0,-8,0,0)
        _BendOrigin ("Bend Origin", Vector) = (0,0,0,0)
        _BendFalloff ("Bend Falloff", Float) = 200
        _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
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
            #define LB_PARTICLE 1
            #include "LadybugRecovered.cginc"
            ENDCG
        }
    }
    Fallback Off
}
