Shader "Custom/Tint"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} 
        _TintColor ("Tint Color", Color) = (1,0,0,1)
        _Strength ("Strength", float) = 0.5
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _TintColor;
            float _Strength;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
                                               
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half4 texCol = tex2D(_MainTex, i.uv);
                return _TintColor * _Strength + texCol * (1 - _Strength);
            }
            ENDCG
        }
    }
}
