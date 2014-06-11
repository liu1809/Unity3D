using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NguiBlur : PostEffectsBaseCSharp
{

    [Range(0, 2)]
    public int downsample = 1;

    public enum BlurType
    {
        StandardGauss = 0,
        SgxGauss = 1,
    }

    [Range(0f, 10f)]
    public float blurSize = 3.0f;

    [Range(1, 4)]
    public int blurIterations = 2;

    public BlurType blurType = BlurType.StandardGauss;

    public Transform m_TopLeftRef;

    public Shader blurShader;
    private Material blurMaterial = null;

    public Texture2D tempTexture2D;

    private static List<NguiBlurSprite> blurSpriteList = new List<NguiBlurSprite>();

    public static void AddBlurSprite(NguiBlurSprite _sprite)
    {
        if (!blurSpriteList.Contains(_sprite))
        {
            blurSpriteList.Add(_sprite);
        }
    }
    public static void RemoveBlurSprite(NguiBlurSprite _sprite)
    {
        if (blurSpriteList.Contains(_sprite))
        {
            blurSpriteList.Remove(_sprite);
        }
    }

    public override bool CheckResources()
    {
        CheckSupport(false);

        blurMaterial = CheckShaderAndCreateMaterial(blurShader, blurMaterial);

        if (!isSupported)
        {
            ReportAutoDisable();
        }
        return isSupported;
    }

    void OnDisable()
    {
        if (blurMaterial)
        {
            DestroyImmediate(blurMaterial);
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (CheckResources() == false)
        {
            Graphics.Blit(source, destination);
            return;
        }

        float widthMod = 1.0f / (1.0f * (1 << downsample));

        blurMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod, -blurSize * widthMod, 0.0f, 0.0f));
        source.filterMode = FilterMode.Bilinear;

        int rtW = source.width >> downsample;
        int rtH = source.height >> downsample;

        // downsample
        RenderTexture rt = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);

        rt.filterMode = FilterMode.Bilinear;

        Graphics.Blit(source, rt, blurMaterial, 0);

        int passOffs = (blurType == BlurType.StandardGauss ? 0 : 2);

        for (int i = 0; i < blurIterations; i++)
        {
            float iterationOffs = (i * 1.0f);
            blurMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod + iterationOffs, -blurSize * widthMod - iterationOffs, 0.0f, 0.0f));

            // vertical blur
            RenderTexture rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
            rt2.filterMode = FilterMode.Bilinear;
            Graphics.Blit(rt, rt2, blurMaterial, 1 + passOffs);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;

            // horizontal blur
            rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
            rt2.filterMode = FilterMode.Bilinear;
            Graphics.Blit(rt, rt2, blurMaterial, 2 + passOffs);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;
        }

        

        foreach (NguiBlurSprite one in blurSpriteList)
        {
            if (one != null)
            {
                //Rect rect = GetSpriteRect(one.m_Sprite);
                
                //tempTexture2D = new Texture2D(one.m_Sprite.width, one.m_Sprite.height);

                //tempTexture2D.ReadPixels(rect, 0, 0);

                //tempTexture2D.Apply();

                

                //DestroyImmediate(tempTexture2D);
            }
        }

        RenderTexture.ReleaseTemporary(rt);



    }



    Rect GetSpriteRect(UI2DSprite _uiTexture)
    {
        Vector3 localPos = m_TopLeftRef.InverseTransformPoint(_uiTexture.cachedTransform.position);
        return new Rect(Mathf.FloorToInt(localPos.x), -Mathf.FloorToInt(localPos.y), _uiTexture.width, _uiTexture.height);
    }
}
