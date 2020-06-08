using UnityEngine;
using System.Collections;

namespace FowOfWar
{
    public class FOWTest : MonoBehaviour
    {
        // Use this for initialization
        void Start () {
            // fow系统启动
            FOWUtil.instance.Startup();
            FOWUtil.instance.SetFOWData(transform.position);
            FOWUtil.instance.Begin();
        
            FOWUtil.instance.CreatePlane(null);
            FOWUtil.instance.AddObjRevealer(GameObject.FindGameObjectWithTag("Player").transform, 10);
        }
    }
}