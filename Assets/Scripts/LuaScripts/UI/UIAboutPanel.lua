require("Core/System")

local M = class("UIAboutPanel", target)

function M:OnAwake()
    M._close = function()
        self:Close()
    end

    self:Listen(self.btn_close.onClick, M._close)
end

return M
