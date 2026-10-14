// Stand-in for a missing Ladybug shader (original GUID 61209350db961fd468e85534cea73e20).
// Used by 15 material(s), e.g. "Swipe_particlesGlow". Replace with the original shader if it is found.
Shader "Ladybug/Recovered/Particle/61209350"
{
    Properties
    {
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Pass
        {
            Blend SrcAlpha One
            ZWrite Off
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #define LB_PARTICLE 1
            #define LB_ADDITIVE 1
            #include "LadybugRecovered.cginc"
            ENDCG
        }
    }
    Fallback Off
}
