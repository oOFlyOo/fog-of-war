using System.Collections;
using UnityEngine;

/// <summary>
/// 说明：FOW表现层渲染脚本
/// 
/// @by wsh 2017-05-20
/// </summary>

public class FOWRender : MonoBehaviour
{
    Material mMat;

    public bool IsActive
    {
        get
        {
            return gameObject.activeSelf;
        }
    }

    private readonly int MainTexID = Shader.PropertyToID("_MainTex");
    private readonly int UnExploredID = Shader.PropertyToID("_Unexplored");
    private readonly int ExploredHashID = Shader.PropertyToID("_Explored");
    private readonly int BlendFactorID = Shader.PropertyToID("_BlendFactor");

    public void Activate(bool active)
    {
        gameObject.SetActive(active);
    }

    void Start()
    {
        UpdateRenderer();
    }

    public void UpdateRenderer()
    {
        if (mMat == null)
        {
            MeshRenderer render = GetComponentInChildren<MeshRenderer>();
            if (render != null)
            {
                mMat = render.sharedMaterial;
            }
        }

        if (mMat == null)
        {
            enabled = false;
            return;
        }

        UpdateColor();

        StartCoroutine(WaitSetTexture());
    }

    /// <summary>
    /// 注意实现必须先激活才能更新
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitSetTexture()
    {
        while (!FOWSystem.instance.texture)
        {
            yield return null;
        }
        
        mMat.SetTexture(MainTexID, FOWSystem.instance.texture);
    }

    private void UpdateColor()
    {
        if (FOWSystem.instance.enableFog)
        {
            mMat.SetColor(UnExploredID, FOWSystem.instance.UnexploredColor);
        }
        else
        {
            mMat.SetColor(UnExploredID, FOWSystem.instance.ExploredColor);
        }
        mMat.SetColor(ExploredHashID, FOWSystem.instance.ExploredColor);
    }

    void OnWillRenderObject()
    {
        if (mMat != null && FOWSystem.instance.texture != null)
        {
            mMat.SetFloat(BlendFactorID, FOWSystem.instance.blendFactor);
            
            #if UNITY_EDITOR
            UpdateColor();
            #endif
        }
    }
}
