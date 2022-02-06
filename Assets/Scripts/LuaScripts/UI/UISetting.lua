require("Core.System")

local SoundComponent = CS.Saro.Audio.SoundComponent.Current

local M = class("UISetting", target)

function M:OnAwake()
    self:Listen(self.slider_bgm.onValueChanged, M.OnBGMChanged)
    self:Listen(self.slider_se.onValueChanged, M.OnSEChanged)
end

function M:OnStart()
    self.slider_bgm.value = SoundComponent.VolumeBGM
    self.slider_se.value = SoundComponent.VolumeSE
end

M.OnSEChanged = function(val)
    -- print("OnSEChanged: " .. val)
    SoundComponent.VolumeSE = val
end

function M.OnBGMChanged(val)
    -- print("OnBGMChanged: " .. val)
    SoundComponent.VolumeBGM = val
end

return M
