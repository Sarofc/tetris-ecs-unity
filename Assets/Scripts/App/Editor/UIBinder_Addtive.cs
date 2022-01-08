using System;
using System.Collections.Generic;
using Saro.UI;
using TMPro;
using UnityEngine;

namespace Tetris.UI
{
    class UIBinder_Addtive : IUIBindProcessor
    {
        public Dictionary<string, Type> Binds { get; } = new Dictionary<string, Type>
        {
            { "tmptxt_", typeof(TMP_Text) },
            { "anim_", typeof(Animator) },
        };
    }
}
