using Battle;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 说明：角色视野
/// 
/// @by wsh 2017-05-20
/// </summary>

public class FOWObjectRevealer : FOWRevealer
{
    private Transform _objTrans;

    static public new FOWObjectRevealer Get()
    {
        return ClassObjPool<FOWObjectRevealer>.Get();
    }


    public override void OnInit()
    {
        base.OnInit();
        
        _objTrans = null;
    }

    public override void OnRelease()
    {
        _objTrans = null;

        base.OnRelease();
    }

    public void InitInfo(Transform trans, float radius)
    {
        _objTrans = trans;
        m_radius = radius;
        m_isValid = true;

        Update(0);
    }
    
    public override void Update(int deltaMS)
    {
        m_position = _objTrans.position;
    }
}