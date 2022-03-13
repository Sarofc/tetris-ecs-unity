require("Core/System")

local M = class("UISetting", target)

local SoundComponent = CS.Saro.Audio.SoundComponent.Current
local SaveComponent = CS.Tetris.Save.SaveComponent.Current
local LocalizationComponent = CS.Saro.Localization.LocalizationComponent.Current

function M:OnAwake()
    self:Listen(self.slider_bgm.onValueChanged, M.OnBGMChanged)
    self:Listen(self.slider_se.onValueChanged, M.OnSEChanged)
    self:Listen(self.tmpdrop_language.onValueChanged, M.OnLanguageChanged)
end

function M:OnStart()
    self.slider_bgm.value = SoundComponent:GetVolumeBGM()
    self.slider_se.value = SoundComponent:GetVolumeSE()
end

function M:OnClose()
    SoundComponent:StoreSettings()
    SaveComponent:Save()
end

M.OnSEChanged = function(val)
    -- print("OnSEChanged: " .. val)
    SoundComponent:SetVolumeSE(val)
end

function M.OnBGMChanged(val)
    -- print("OnBGMChanged: " .. val)
    SoundComponent:SetVolumeBGM(val)
end

M.OnLanguageChanged = function(val)
    print("OnLanguageChanged: " .. val)
    LocalizationComponent:SetLanguage(val)
end

return M
