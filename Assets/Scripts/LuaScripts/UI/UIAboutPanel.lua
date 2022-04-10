require("Core/System")

local M = class("UIAboutPanel", target)

function M:OnAwake()
    M._close = function()
        self:Close()
    end

    self:Listen(self.btn_close.onClick, M._close)
    self:Listen(self.btn_icon.onClick, M.OpenUrl)
end

function M:OnStart()
    local img_icon = self.btn_icon:GetComponent(typeof(CS.UnityEngine.UI.Image))
    -- TODO 看lua如何支持异步 await
    -- 如果后面要继续使用lua，则需要等csharp层api稳定下来后，新增一个lua专用api
    img_icon.sprite =
        self.AssetLoader:LoadAssetRef("Assets/Arts/Textures/UI/icon_github.png", typeof(CS.UnityEngine.Sprite))

    -- 测试
    -- self.AssetLoader:LoadAssetRef("Assets/Arts/Textures/UI/icon_github.png", typeof(CS.UnityEngine.Sprite))
    -- self.AssetLoader:LoadAsset("Assets/Arts/Textures/UI/icon_github.png", typeof(CS.UnityEngine.Sprite))
end

function M.OpenUrl()
    CS.UnityEngine.Application:OpenURL("https://sarofc.gitee.io")
end

return M
