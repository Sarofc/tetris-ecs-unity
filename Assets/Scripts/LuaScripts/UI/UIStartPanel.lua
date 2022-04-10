require("Core/System")

local UIManager = CS.Saro.UI.UIManager.Current

local M = class("UIStartPanel", target)

function M:OnAwake()
    self:Listen(self.btn_start.onClick, M.OnClick_start)
    self:Listen(self.btn_setting.onClick, M.OnClick_setting)
    self:Listen(self.btn_about.onClick, M.OnClick_about)
    self:Listen(self.btn_quit.onClick, M.OnClick_quit)
end

function M:OnStart()
    local AssetInterface = CS.Saro.Main.Resolve(typeof(CS.Saro.Core.IAssetInterface))
    self.tmptxt_version.text = "v." .. AssetInterface:GetAppVersion() .. "." .. AssetInterface:GetResVersion()
end

function M.OnClick_start()
    CS.Saro.SceneManagerLuaEx.LoadSceneAsync("Assets/Res/Scenes/EcsGaming.unity")
end

function M.OnClick_setting()
    -- UIManager:OpenUIAsync(typeof(CS.Tetris.UI.UISetting))
    UIManager:OpenUIAsync("UISetting")
end

function M.OnClick_about()
    UIManager:OpenUIAsync("UIAboutPanel")
end

function M.OnClick_quit()
    CS.Saro.Main.Quit()
end

return M
