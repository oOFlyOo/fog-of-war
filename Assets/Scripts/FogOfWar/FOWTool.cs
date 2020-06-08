
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FowOfWar
{
    public class FOWTool : MonoBehaviour
    {
        [SerializeField, Tooltip("已探索颜色")]
        private Color _unexploredColor;
        [SerializeField, Tooltip("未探索颜色")]
        private Color _exploredColor;

        [SerializeField, Tooltip("相对区域大小，全局控制")] 
        private float _radiusOffset; 
    
        // Start is called before the first frame update
        void Start()
        {
            _unexploredColor = FOWSystem.instance.UnexploredColor;
            _exploredColor = FOWSystem.instance.ExploredColor;
            _radiusOffset = FOWSystem.instance.radiusOffset;
        }

        // Update is called once per frame
        void Update()
        {
            FOWSystem.instance.UnexploredColor = _unexploredColor;
            FOWSystem.instance.ExploredColor = _exploredColor;
            FOWSystem.instance.radiusOffset = _radiusOffset;
        }

        [ContextMenu("重置显示信息")]
        private void ResetData()
        {
            // FOWSystem.instance.Cancel();
            FOWSystem.instance.Begin();
        }
    
        [ContextMenu("添加显示对象")]
        private void AddReveal()
        {
            FOWUtil.instance.AddObjRevealer(transform, 10);
        }
    }
}

#endif