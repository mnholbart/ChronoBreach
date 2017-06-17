// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/DissolvingObject" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Off

		CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
#pragma exclude_renderers gles
		#pragma surface surf Lambert vertex:vert addshadow 
		#pragma target 3.0

		sampler2D _MainTex;
		float4 _BoxSize;
		float4 _BoxPos;

		struct Input {
			float2 uv_MainTex;
			float3 pos_ws : TEXCOORD4;
			//float3x4 box_verts1 : TEXCOORD5;
		};
		
		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			//compute object vertices position in world space
			o.pos_ws = mul(unity_ObjectToWorld, v.vertex).xyz;
		}
		
		void surf (Input i, inout SurfaceOutput o) {
			//Multiplying by half gives me a distance between the center of the box and it's side
			_BoxSize *= 0.5;
			
			//This matrix stores the 4 vertex positions of the box (at ground level)
			//One vertex has position computed at the top of the cube so I can get height information for my discard function :)
			float4x3 box_verts1 = float4x3(_BoxPos.x - _BoxSize.x, _BoxPos.y - _BoxSize.y, _BoxPos.z - _BoxSize.z, //left-back vert BOTTOM
										   _BoxPos.x - _BoxSize.x, _BoxPos.y - _BoxSize.y, _BoxPos.z + _BoxSize.z, //left-front vert BOTTOM
										   _BoxPos.x + _BoxSize.x, _BoxPos.y - _BoxSize.y, _BoxPos.z - _BoxSize.z, //right-back vert BOTTOM
										   _BoxPos.x + _BoxSize.x, _BoxPos.y + _BoxSize.y, _BoxPos.z + _BoxSize.z); //right-front vert TOP
			
			//XYZ combined
			if (i.pos_ws.x >= box_verts1[0].x 
			&& i.pos_ws.x <= box_verts1[2].x 
			&& i.pos_ws.z >= box_verts1[2].z 
			&& i.pos_ws.z <= box_verts1[3].z 
			&& i.pos_ws.y >= box_verts1[0].y 
			&& i.pos_ws.y <= box_verts1[3].y)
			{
				fixed4 c = tex2D(_MainTex, i.uv_MainTex);
				o.Albedo = c.rgb;
				o.Alpha = 1;
			}
			else 
			{
				discard;
				//o.Albedo = half3(0, 0, 0);
				//o.Alpha = 0;
			}
			

		}
		ENDCG
	} 
}
