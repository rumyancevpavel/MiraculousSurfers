// Shared code for the stand-in shaders in this folder.
// Each stand-in .shader keeps the GUID of an original Ladybug shader that is missing
// from the project, so existing materials reconnect to it without being modified.
// Features are switched on per shader with LB_* defines before including this file.
// Passes have no LightMode tag, so URP draws them as SRPDefaultUnlit.

#ifndef LADYBUG_RECOVERED_INCLUDED
#define LADYBUG_RECOVERED_INCLUDED

#include "UnityCG.cginc"

sampler2D _MainTex; float4 _MainTex_ST;
fixed4 _Color;
fixed4 _TintColor;
half _Cutoff;
sampler2D _Lightmap;
sampler2D _LMTex;
half _LightmapIntensity;
float4 _BendAmount;
float _BendFalloff;
fixed4 _FresnelColor;
half _FresnelRange;
half _FresnelIntensity;
half _GreyBlend;
fixed4 _GreyScaleColor;
half _GrayscaleAmount;

#ifdef LB_LIT
// URP main light globals
float4 _MainLightPosition;
half4 _MainLightColor;
#endif

struct lb_appdata
{
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
    float2 uv2 : TEXCOORD1;
    fixed4 color : COLOR;
};

struct lb_v2f
{
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    float2 uv2 : TEXCOORD1;
    fixed4 color : COLOR;
    float3 worldNormal : TEXCOORD2;
    float3 worldPos : TEXCOORD3;
    UNITY_FOG_COORDS(4)
};

// Curved-world approximation: pushes geometry along _BendAmount.xy by the squared
// distance ahead of the camera, reaching the full amount at _BendFalloff units.
float3 LB_Bend(float3 worldPos)
{
#ifdef LB_BEND
    if (_BendFalloff > 0)
    {
        float k = max(0, worldPos.z - _WorldSpaceCameraPos.z) / _BendFalloff;
        worldPos.xy += _BendAmount.xy * k * k;
    }
#endif
    return worldPos;
}

lb_v2f vert(lb_appdata v)
{
    lb_v2f o;
    float3 worldPos = LB_Bend(mul(unity_ObjectToWorld, v.vertex).xyz);
    o.pos = UnityWorldToClipPos(worldPos);
    o.worldPos = worldPos;
    o.worldNormal = UnityObjectToWorldNormal(v.normal);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.uv2 = v.uv2;
    o.color = v.color;
    UNITY_TRANSFER_FOG(o, o.pos);
    return o;
}

fixed4 frag(lb_v2f i) : SV_Target
{
    fixed4 tex = tex2D(_MainTex, i.uv);
    fixed4 c;
#if defined(LB_FONT)
    c = _Color * i.color * 2.0 * _TintColor;
    c.a *= tex.a;
#elif defined(LB_PARTICLE)
    c = tex * _Color * i.color * 2.0 * _TintColor;
#elif defined(LB_VCOLOR)
    c = tex * _Color * i.color;
#else
    c = tex * _Color;
#endif

#ifdef LB_CUTOUT
    clip(c.a - _Cutoff);
#endif

#ifdef LB_LIGHTMAP
    c.rgb *= tex2D(_Lightmap, i.uv2).rgb * _LightmapIntensity;
#endif
#ifdef LB_LMTEX
    c.rgb *= tex2D(_LMTex, i.uv2).rgb * _LightmapIntensity;
#endif

    float3 n = normalize(i.worldNormal);
#ifdef LB_LIT
    half ndl = saturate(dot(n, normalize(_MainLightPosition.xyz)));
    c.rgb *= _MainLightColor.rgb * ndl + ShadeSH9(float4(n, 1));
#endif
#ifdef LB_RIM
    half ndv = saturate(dot(n, normalize(UnityWorldSpaceViewDir(i.worldPos))));
    c.rgb += _FresnelColor.rgb * pow(1 - ndv, max(_FresnelRange, 0.01)) * _FresnelIntensity;
#endif
#ifdef LB_GREY
    c.rgb = lerp(c.rgb, Luminance(c.rgb) * _GreyScaleColor.rgb, _GreyBlend);
#endif
#ifdef LB_UIGREY
    c.rgb = lerp(c.rgb, Luminance(c.rgb).xxx, _GrayscaleAmount);
#endif

#if defined(LB_ADDITIVE)
    UNITY_APPLY_FOG_COLOR(i.fogCoord, c, fixed4(0, 0, 0, 0));
#elif !defined(LB_NOFOG)
    UNITY_APPLY_FOG(i.fogCoord, c);
#endif
    return c;
}

#endif
