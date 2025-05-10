using System;
using JetBrains.Annotations;
using Project.Runtime.Scripts.General;
using UnityEngine;

namespace Treasures
{
    public class TreasureInputHandler : MonoBehaviour
    {
        private Action _onClick;
        
        public void Setup([CanBeNull] Action onClick)
        {
            _onClick = onClick;
        }
        
        private void OnMouseDown()
        {
            _onClick?.Invoke();
        }
        
        private void OnMouseEnter()
        {
            CursorManager.Instance.SetInteractCursor();
        }
        
        private void OnMouseExit()
        {
            CursorManager.Instance.SetDefaultCursor();
        }
    }
}
