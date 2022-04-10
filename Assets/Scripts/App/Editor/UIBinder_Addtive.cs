using Saro.UI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tetris.UI
{
    internal class UIBinder_Addtive : IUIBindProcessor
    {
        public Dictionary<string, Type> Binds { get; } = new Dictionary<string, Type>
        {
            { "tmptxt_", typeof(TMP_Text) },
            { "tmpdrop_", typeof(TMP_Dropdown) },
            { "anim_", typeof(Animator) },
        };
    }
}
