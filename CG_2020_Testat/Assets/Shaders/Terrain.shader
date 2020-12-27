Shader "CG_Lecture/DisplacementMapShader"
{
	// Tutorial - Vertex und Fragment Shader examples: https://docs.unity3d.com/Manual/SL-VertexFragmentShaderExamples.html
	// DOC: How to write vertex and fragment shaders: https://docs.unity3d.com/Manual/SL-ShaderPrograms.html

	// Property Definition --> Visible in IDE
	Properties
	{
				 [HideInInspector]_HeightMap ("Height Map", 2D) = "white"{}
				 [HideInInspector]_MoistureMap ("Moisture Map", 2D) = "white"{}
				_ColorMap ("Color Map", 2D) = "normal"{}
				_WaveNormalMap1 ("Wave Normal Map 1", 2D) = "normal"{}
				_WaveNormalMap2 ("Wave Normal Map 2", 2D) = "normal"{}
				_WaveSpeed("Wave Animation Speed", Range(0, 1)) = 0.5
                _TerrainScale ("Terrain Scale", Range(0, 1)) = 0.1
				_SeeLevelScale ("See Level Scale", Range(0, 1)) = 0.5
				// Ambiente Reflektanz
				_Ka("Ambient Reflectance", Range(0, 1)) = 0.5
				// Diffuse Reflektanz
				_Kd("Diffuse Reflectance", Range(0, 1)) = 0.5
				// Spekulare Reflektanz
				_Ks("Specular Reflectance", Range(0, 1)) = 0.5
				// Shininess
				_Shininess("Shininess", Range(0, 2)) = 1
	}

	// A Shader can contain one or more SubShaders, which are primarily used to implement shaders for different GPU capabilities
	SubShader
	{
		// Subshaders use tags to tell how and when
		// they expect to be rendered to the rendering engine.
		// https://docs.unity3d.com/Manual/SL-SubShaderTags.html
		Tags { "RenderType"="Opaque" }

		// Each SubShader is composed of a number of passes, and each Pass represents an execution of the vertex and fragment
		// code for the same object rendered with the material of the shader
		Pass
		{
			// CGPROGRAM ... ENDCG
			// These keywords surround portions of HLSL code within the vertex and fragment shaders
			CGPROGRAM

			// Definition shaders used and their function names
			#pragma vertex vert
			#pragma fragment frag

			// Builtin Includes
			// https://docs.unity3d.com/Manual/SL-BuiltinIncludes.html
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			sampler2D _HeightMap;
			sampler2D _MoistureMap;
			sampler2D _ColorMap;
			float _TerrainScale;
			float _SeeLevelScale;
			float _Ka, _Kd, _Ks;
			float _Shininess;
			sampler2D _WaveNormalMap1;
			sampler2D _WaveNormalMap2;
			float _WaveSpeed;


			struct v2f
			{
				// SV_POSITION: Shader semantic for position in Clip Space: https://docs.unity3d.com/Manual/SL-ShaderSemantics.html?_ga=2.64760810.432960686.1524081652-394573263.1524081652
				float4 vertex : SV_POSITION;
				float4 col : COLOR;
				float seeLevelOffset : CUSTOM;

				half3 worldNormal : NORMAL;
				half3 worldViewDir : TEXCOORD1;

				half3 texcoord : TEXCOORD0;
			};


			// VERTEX SHADER
			// https://docs.unity3d.com/Manual/SL-VertexProgramInputs.html
			// http://wiki.unity3d.com/index.php?title=Shader_Code
			v2f vert (appdata_full v)
			{
				v2f o;

				//TODO: get vertex Data
				o.vertex = v.vertex;
				o.texcoord = v.texcoord;

				// Access height-map-texture and extract hight map value
				fixed4 disVal = tex2Dlod(_HeightMap, float4(v.texcoord.xy, 0, 0));

				// Access moisture-map-texture and extract moistureness
				fixed4 mosVal = tex2Dlod(_MoistureMap, float4(v.texcoord.xy, 0, 0));
		
				//TODO: displace vertex by the value of the height map along the normal vector
				if(disVal.x <= _SeeLevelScale) 
				{
					disVal.xyz = _SeeLevelScale;
				}

				o.vertex.xyz += _TerrainScale * v.normal * disVal.x * 0.01f;		

				//TODO: Convert Vertex Data from Object to Clip Space
				o.vertex = UnityObjectToClipPos(o.vertex);

				// Access color-map-texture and extract color
				fixed4 colVal = tex2Dlod(_ColorMap, float4(mosVal.x, (disVal.x-(_SeeLevelScale*0.9)), 0, 0));

				//TODO: set texture value as color.
				o.col = colVal;

				// For Lambert / Phong Shading
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldViewDir = normalize(WorldSpaceViewDir(v.vertex));
				o.seeLevelOffset = disVal.x-_SeeLevelScale;

				return o;
			}

			// FRAGMENT / PIXEL SHADER
			// SV_Target: Shader semantic render target (SV_Target = SV_Target0): https://docs.unity3d.com/Manual/SL-ShaderSemantics.html?_ga=2.64760810.432960686.1524081652-394573263.1524081652
			fixed4 frag (v2f i) : SV_Target
			{
				// Wasser/Wellen-Animation wenn im vertex shader unter dem seeLevelOffset liegt
				if(i.seeLevelOffset <= 0)
				{ 
					// für die Animation werden zwei verschiedene normalen Karten verwendet. Eine Karte wird mit der Zeit in X und die andere in Y Richtung verschoben um eine schönere Wellen-Animation zu erhalten.
					// Hierzu wird zur Position der _WaveSpeed Faktor multipliziert mit der Zeit seit die Szene geladen wurde (geteilt durch 5 für langsamere Geschwindigkeit) addiert
					half3 waveNormal = normalize(UnpackNormal(tex2D(_WaveNormalMap1, float2(i.texcoord.x + _WaveSpeed * _Time.x/5, i.texcoord.y))) + UnpackNormal(tex2D(_WaveNormalMap2, float2(i.texcoord.x, i.texcoord.y + _WaveSpeed * _Time.x/5))));
					// Anschließend werden die Normalen der Animation zu den "normalen" Normalen addiert und normiert.
					i.worldNormal = normalize(i.worldNormal + waveNormal);
				}

				// Ambiente Licht Farbe
				// das gesamte ambiente Licht der Szene wird durch die Funktion ShadeSH9 (Teil von UnityCG.cginc) ausgewertet
				// Dazu werden die homogenen Oberflächen Normalen in Welt-Koordinaten verwendet.
				float4 ambLight = float4(ShadeSH9(half4(i.worldNormal,1)),1);


				// Standard Diffuse (Lambert) Shading
				// Gewichtung durch Skalarprodukt (Dot-Produkt) zwischen Normalen-Vektor
				// Richtung der Beleuchtungsquelle
				
				// WICHTIG: Bei Direktionalem Licht gibt _WorldSpaceLightPos0 die Richtung der Lichtquelle an. 
				// Bei Anderen Lichtquellen gibt es die Homogenen Koordinaten der Lichtquelle in Welt-Koordinaten an.
				// https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
                half nl = max(0, dot(i.worldNormal, _WorldSpaceLightPos0.xyz));
                
				// Diffuser Anteil multipliziert mit der Lichtfarbe
                float4 diffLight = nl * _LightColor0;


				float3 worldSpaceReflection = reflect(normalize(-_WorldSpaceLightPos0.xyz), i.worldNormal);
				half re = pow(max(dot(worldSpaceReflection, i.worldViewDir), 0), _Shininess);

				// Spekularer Anteil multipliziert mit der Lichtfarbe
				float spekLight = re * _LightColor0;

				// Nutze die definierte Farbe.
                fixed4 color = i.col;
				
				// Multiplikation der Grundfarbe mit dem Ambienten- und dem Diffusions-Anteil
				// Der Diffuse und Ambiente Anteil wird jeweils mit der entsprechenden Reflektanz der Oberfläche (_Ka, _Kd) gewichtet.
				if(i.seeLevelOffset > 0) color = color * ( ambLight * _Ka + diffLight * _Kd );
				else color = color * ( ambLight * _Ka + diffLight * _Kd + spekLight * _Ks );
				
				// Zusätzliche Addition des spekularen Anteils, multipliziert mit der entsprechenden Reflektanz
				// TODO

                return saturate(color);
			}
			ENDCG
		}
	}
}
