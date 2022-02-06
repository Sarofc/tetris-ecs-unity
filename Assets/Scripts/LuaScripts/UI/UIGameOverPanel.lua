require("Core.System")

local UIComponent = CS.Saro.UI.UIComponent.Current

local M = class("UIGameOverPanel", target)

function M:OnAwake()
    self:Listen(self.btn_replay.onClick, M.OnClick_Replay)
    self:Listen(self.btn_setting.onClick, M.OnClick_Setting)
    self:Listen(self.btn_about.onClick, M.OnClick_About)
    self:Listen(self.btn_quit.onClick, CS.Saro.Main.Quit)
end

function M.OnClick_Replay()
    UIComponent:AddToast("未开启")
end

function M.OnClick_Setting()
    UIComponent:OpenUIAsync("UISetting")
end

function M.OnClick_About()
    UIComponent:OpenUIAsync("UIAboutPanel")
end

return M
