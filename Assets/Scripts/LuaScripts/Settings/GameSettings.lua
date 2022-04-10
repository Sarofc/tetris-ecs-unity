local GameSettings = class("GameSettings", target)

local SaveManager = CS.Tetris.Save.SaveManager.Current

local SoundManager = CS.Saro.Audio.SoundManager.Current
local LocalizationManager = CS.Saro.Localization.LocalizationManager.Current

function GameSettings:ApplySettings()
    SaveManager:Load()

    local settings = SaveManager:GetSaveData(typeof(CS.Tetris.Save.GameSettings))

    SoundManager.VolumeBGM = settings.volumeBGM
    SoundManager.VolumeSE = settings.volumeSE

    LocalizationManager:SetLanguage(settings.language)
end

function GameSettings:StoreSettings()
    local settings = SaveManager:GetSaveData(typeof(CS.Tetris.Save.GameSettings))

    settings.volumeBGM = SoundManager.VolumeBGM
    settings.volumeSE = SoundManager.VolumeSE

    settings.language = LocalizationManager:GetLanguage()

    SaveManager:Save()
end

return GameSettings
