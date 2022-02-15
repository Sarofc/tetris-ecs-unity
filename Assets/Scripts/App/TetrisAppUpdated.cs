using Cysharp.Threading.Tasks;
using Saro;
using Saro.Audio;
using Saro.EventDef;
using Saro.UI;
using Saro.XAsset;
using System;
using System.Collections.Generic;

namespace Tetris
{
    public sealed class TetrisAppUpdated : FEvent<AppUpdated>
    {
        protected override async UniTask Run(AppUpdated a)
        {
            XAssetComponent.Current.Initialize();

            //await SoundComponent.Current.InitializeAsync(XAssetComponent.Current, "Assets/Res/Audios/");
            //await UIComponent.Current.InitializeAsync(XAssetComponent.Current, "Assets/Res/Prefab/UI/");
        }
    }
}
