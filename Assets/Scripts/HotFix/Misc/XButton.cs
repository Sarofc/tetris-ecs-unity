using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tetris
{
    public sealed class XButton : Button
    {
        public bool PressDown { get; private set; }
        public bool PressUp { get; private set; }
        public bool Pressed => IsPressed();

        public override void OnPointerDown(PointerEventData data)
        {
            base.OnPointerDown(data);

            PressDown = true;
        }

        public override void OnPointerUp(PointerEventData data)
        {
            base.OnPointerUp(data);

            PressUp = true;
        }

        private void LateUpdate()
        {
            PressDown = false;
            PressUp = false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
