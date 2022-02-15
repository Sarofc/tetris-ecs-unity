require("Core/System")

local UIComponent = CS.Saro.UI.UIComponent.Current

local M = class("UIStartPanel", target)

function M:OnAwake()
    self:Listen(self.btn_start.onClick, M.OnClick_start)
    self:Listen(self.btn_setting.onClick, M.OnClick_setting)
    self:Listen(self.btn_about.onClick, M.OnClick_about)
    self:Listen(self.btn_quit.onClick, M.OnClick_quit)
end

-- function M:OnStart()
-- end

function M.OnClick_start()
    print("click start")
    CS.Saro.SceneManagerLuaEx.LoadSceneAsync("Assets/Res/Scenes/Gaming.unity")
end

function M.OnClick_setting()
    UIComponent:OpenUIAsync("UISetting")
end

function M.OnClick_about()
    UIComponent:OpenUIAsync("UIAboutPanel")
end

function M.OnClick_quit()
    CS.Saro.Main:Quit()
end

return M
