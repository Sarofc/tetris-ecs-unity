require("Core/System")

local GameSettings = require("Settings/GameSettings")

-- print("lua main start!")
local UIManager = CS.Saro.UI.UIManager.Current

GameSettings:ApplySettings()

UIManager:OpenUIAsync("UIStartPanel")

-- UIManager:OpenUIAsync("UISetting")
