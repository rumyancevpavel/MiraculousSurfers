// Stand-in for a missing Ladybug shader (original GUID 0a692500b7456ba4ba42c7506c019d22).
// Used by 2 material(s), e.g. "ChestShadow". Replace with the original shader if it is found.
Shader "Ladybug/Recovered/Particle/0a692500"
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
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #define LB_PARTICLE 1
            #include "LadybugRecovered.cginc"
            ENDCG
        }
    }
    Fallback Off
}
