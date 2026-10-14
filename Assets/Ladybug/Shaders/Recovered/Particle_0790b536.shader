// Stand-in for a missing Ladybug shader (original GUID 0790b536fc2db6d4598ff35958463e70).
// Used by 59 material(s), e.g. "WallrunFeet FX". Replace with the original shader if it is found.
Shader "Ladybug/Recovered/Particle/0790b536"
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
