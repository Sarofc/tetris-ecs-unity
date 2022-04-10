require("Core/System")

local GameSettings = require("Settings/GameSettings")

local M = class("UISetting", target)

-- local SaveManager = CS.Tetris.Save.SaveManager.Current
local SoundManager = CS.Saro.Audio.SoundManager.Current
local LocalizationManager = CS.Saro.Localization.LocalizationManager.Current

function M:OnAwake()
    self:Listen(self.slider_bgm.onValueChanged, M.OnBGMChanged)
    self:Listen(self.slider_se.onValueChanged, M.OnSEChanged)
    self:Listen(self.tmpdrop_language.onValueChanged, M.OnLanguageChanged)
end

function M:OnStart()
    self.tmpdrop_language.value = LocalizationManager:GetLanguage()
    self.slider_bgm.value = SoundManager:GetVolumeBGM()
    self.slider_se.value = SoundManager:GetVolumeSE()
end

function M:OnClose()
    GameSettings:StoreSettings()
    -- SoundManager:StoreSettings()
    -- LocalizationManager:StoreSettings()
    -- SaveManager:Save()
end

M.OnSEChanged = function(val)
    -- print("OnSEChanged: " .. val)
    SoundManager:SetVolumeSE(val)
end

function M.OnBGMChanged(val)
    -- print("OnBGMChanged: " .. val)
    SoundManager:SetVolumeBGM(val)
end

M.OnLanguageChanged = function(val)
    -- print("OnLanguageChanged: " .. val)
    LocalizationManager:SetLanguageAsync(val)
end

return M
