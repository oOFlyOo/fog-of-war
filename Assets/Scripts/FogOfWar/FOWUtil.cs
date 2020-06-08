using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOWUtil : Singleton<FOWUtil>
{
    private const string SHADER_NAME = "HOG/FOWRender";

    private Coroutine _checkTimer;
    private List<IFOWRevealer> _fowRevealers = new List<IFOWRevealer>();
    private Dictionary<Transform, FOWRender> _fowRenders = new Dictionary<Transform, FOWRender>();

    public override void Init()
    {
        base.Init();

        FOWSystem.instance.Startup();
    }

    public override void Dispose()
    {
        FOWSystem.instance.DestroySelf();
    }

    public void SetFOWData(Vector3 worldCenter)
    {
        FOWSystem.instance.UpdateData(worldCenter);
    }

    public void SetFOWData(Vector3 worldCenter, int worldSize, int textureSize)
    {
        FOWSystem.instance.UpdateData(worldCenter, worldSize, textureSize);
    }

    /// <summary>
    /// 临时控制雾效显示
    /// </summary>
    /// <param name="active"></param>
    public void ActiveFOWRender(bool active)
    {
        FOWSystem.instance.enableRender = active;
    }

    public void Begin()
    {
        FOWSystem.instance.Begin();

        _checkTimer = FOWSystem.instance.StartCoroutine(CheckUpdate());
    }

    public void Cancel()
    {
        FOWSystem.instance.Cancel();

        if (_checkTimer != null)
        {
            FOWSystem.instance.StopCoroutine(_checkTimer);
            _checkTimer = null;
        }

        // 如果没有控制移除，这里强制删除
        for (int i = _fowRevealers.Count - 1; i >= 0; i--)
        {
            var revealer = _fowRevealers[i];
            ReleaseRevealer(revealer);
        }
        _fowRevealers.Clear();
        foreach (var trans in _fowRenders.Keys)
        {
            Object.Destroy(trans);
        }
        _fowRenders.Clear();
    }


    private IEnumerator CheckUpdate()
    {
        while (true)
        {
            yield return null;

            // 说明：每个游戏帧更新，这里不做时间限制，实测对游戏帧率优化微乎其微
            UpdateRenders();
            UpdateRevealers((int) Time.unscaledDeltaTime * 10000);
        }
    }

    private void UpdateRenders()
    {
        var renders = _fowRenders.Values;
        foreach (var render in renders)
        {
            ActivateRender(render, FOWSystem.instance.enableRender);
        }
    }


    private void ActivateRender(FOWRender render, bool active)
    {
        if (render != null)
        {
            render.Activate(active);
        }
    }

    protected void UpdateRevealers(int deltaMS)
    {
        for (int i = _fowRevealers.Count - 1; i >= 0; i--)
        {
            var revealer = _fowRevealers[i];
            revealer.Update(deltaMS);
            if (!revealer.IsValid())
            {
                _fowRevealers.RemoveAt(i);
                ReleaseRevealer(revealer);
            }
        }
    }

    public IFOWRevealer AddObjRevealer(Transform trans, float radius)
    {
        var revealer = FOWObjectRevealer.Get();
        revealer.InitInfo(trans, radius);
        FOWSystem.AddRevealer(revealer);
        _fowRevealers.Add(revealer);

        return revealer;
    }

    public void AddTempRevealer(Vector3 position, float radius, int leftMS)
    {
        if (leftMS <= 0)
        {
            return;
        }

        var tmpRevealer = FOWTempRevealer.Get();
        tmpRevealer.InitInfo(position, radius, leftMS);
        FOWSystem.AddRevealer(tmpRevealer);
        _fowRevealers.Add(tmpRevealer);
    }

    public void DelRevealer(IFOWRevealer revealer)
    {
        if (_fowRevealers.Contains(revealer))
        {
            ReleaseRevealer(revealer);
        }
    }

    private void ReleaseRevealer(IFOWRevealer revealer)
    {
        FOWSystem.RemoveRevealer(revealer);
        revealer.Release();
    }

    /// <summary>
    /// 若是有父节点，务必保证归零
    /// </summary>
    /// <param name="parent"></param>
    public Transform CreatePlane(Transform parent)
    {
        var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        var trans = plane.transform;
        trans.SetParent(parent, false);
        trans.localPosition = FOWSystem.instance.WorldCenter;
        trans.localEulerAngles = new Vector3(0, 180, 0);
        var scale = FOWSystem.instance.WorldSize * 0.1f;
        trans.localScale = new Vector3(scale, 1, scale);

        var fowRender = AddFOWRender(plane);
        _fowRenders.Add(trans, fowRender);

        return trans;
    }

    public void DelFOWRender(Transform trans)
    {
        _fowRenders.Remove(trans);
        Object.Destroy(trans);
    }

    private FOWRender AddFOWRender(GameObject go)
    {
        var render = go.GetComponent<Renderer>();
        render.sharedMaterial = CreateMaterial();

        return go.AddComponent<FOWRender>();
    }

    private Material CreateMaterial()
    {
        var mat = new Material(Shader.Find(SHADER_NAME));
        return mat;
    }
}