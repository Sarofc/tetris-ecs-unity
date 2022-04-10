require("Core/System")

local UIManager = CS.Saro.UI.UIManager.Current

local M = class("UIGameOverPanel", target)

function M:OnAwake()
    self:Listen(self.btn_replay.onClick, M.OnClick_Replay)
    self:Listen(self.btn_setting.onClick, M.OnClick_Setting)
    self:Listen(self.btn_about.onClick, M.OnClick_About)
    self:Listen(self.btn_quit.onClick, CS.Saro.Main.Quit)
end

function M.OnClick_Replay()
    UIManager:AddToast("未开启")
end

function M.OnClick_Setting()
    UIManager:OpenUIAsync("UISetting")
end

function M.OnClick_About()
    UIManager:OpenUIAsync("UIAboutPanel")
end

return M
