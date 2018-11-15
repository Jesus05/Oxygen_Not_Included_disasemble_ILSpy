using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Color Adjustments/Color Correction (3D Lookup Texture)")]
	public class ColorCorrectionLookup : PostEffectsBase
	{
		public Shader shader;

		private Material material;

		public Texture3D converted3DLut;

		public Texture3D converted3DLut2;

		public string basedOnTempTex = string.Empty;

		private bool supports3dTextures;

		private void Awake()
		{
			supports3dTextures = SystemInfo.supports3DTextures;
			CheckSupport(false);
		}

		public override bool CheckResources()
		{
			material = CheckShaderAndCreateMaterial(shader, material);
			if (!isSupported || !supports3dTextures)
			{
				ReportAutoDisable();
			}
			return isSupported;
		}

		private void OnDisable()
		{
			if ((bool)material)
			{
				Object.DestroyImmediate(material);
				material = null;
			}
		}

		private void OnDestroy()
		{
			if ((bool)converted3DLut)
			{
				Object.DestroyImmediate(converted3DLut);
			}
			if ((bool)converted3DLut2)
			{
				Object.DestroyImmediate(converted3DLut2);
			}
			converted3DLut = null;
			converted3DLut2 = null;
		}

		public void SetIdentityLut()
		{
			SetIdentityLut(ref converted3DLut);
		}

		public void SetIdentityLut2()
		{
			SetIdentityLut(ref converted3DLut);
		}

		private void SetIdentityLut(ref Texture3D target)
		{
			int num = 16;
			Color[] array = new Color[num * num * num];
			float num2 = 1f / (1f * (float)num - 1f);
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num; j++)
				{
					for (int k = 0; k < num; k++)
					{
						array[i + j * num + k * num * num] = new Color((float)i * 1f * num2, (float)j * 1f * num2, (float)k * 1f * num2, 1f);
					}
				}
			}
			if ((bool)target)
			{
				Object.DestroyImmediate(target);
			}
			target = new Texture3D(num, num, num, TextureFormat.ARGB32, false);
			target.SetPixels(array);
			target.Apply();
			basedOnTempTex = string.Empty;
		}

		public bool ValidDimensions(Texture2D tex2d)
		{
			if (!(bool)tex2d)
			{
				return false;
			}
			int height = tex2d.height;
			if (height != Mathf.FloorToInt(Mathf.Sqrt((float)tex2d.width)))
			{
				return false;
			}
			return true;
		}

		public void Convert(Texture2D temp2DTex, string path)
		{
			Convert(temp2DTex, path, ref converted3DLut);
		}

		public void Convert2(Texture2D temp2DTex, string path)
		{
			Convert(temp2DTex, path, ref converted3DLut2);
		}

		private void Convert(Texture2D temp2DTex, string path, ref Texture3D target)
		{
			if ((bool)temp2DTex)
			{
				int num = temp2DTex.width * temp2DTex.height;
				num = temp2DTex.height;
				if (!ValidDimensions(temp2DTex))
				{
					Debug.LogWarning("The given 2D texture " + temp2DTex.name + " cannot be used as a 3D LUT.", null);
					basedOnTempTex = string.Empty;
				}
				else
				{
					Color[] pixels = temp2DTex.GetPixels();
					Color[] array = new Color[pixels.Length];
					for (int i = 0; i < num; i++)
					{
						for (int j = 0; j < num; j++)
						{
							for (int k = 0; k < num; k++)
							{
								int num2 = num - j - 1;
								array[i + j * num + k * num * num] = pixels[k * num + i + num2 * num * num];
							}
						}
					}
					if ((bool)target)
					{
						Object.DestroyImmediate(target);
					}
					target = new Texture3D(num, num, num, TextureFormat.ARGB32, false);
					target.SetPixels(array);
					target.Apply();
					basedOnTempTex = path;
				}
			}
			else
			{
				Debug.LogError("Couldn't color correct with 3D LUT texture. Image Effect will be disabled.", null);
			}
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!CheckResources() || !supports3dTextures)
			{
				Graphics.Blit(source, destination);
			}
			else
			{
				if ((Object)converted3DLut == (Object)null)
				{
					SetIdentityLut();
				}
				if ((Object)converted3DLut2 == (Object)null)
				{
					SetIdentityLut2();
				}
				int width = converted3DLut.width;
				converted3DLut.wrapMode = TextureWrapMode.Clamp;
				material.SetFloat("_Scale", (float)(width - 1) / (1f * (float)width));
				material.SetFloat("_Offset", 1f / (2f * (float)width));
				material.SetTexture("_ClutTex", converted3DLut);
				material.SetTexture("_ClutTex2", converted3DLut2);
				Graphics.Blit(source, destination, material, (QualitySettings.activeColorSpace == ColorSpace.Linear) ? 1 : 0);
			}
		}
	}
}
