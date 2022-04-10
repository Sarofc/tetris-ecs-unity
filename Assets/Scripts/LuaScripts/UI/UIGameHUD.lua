require("Core/System")

local M = class("UIGameHUD", target)

function M:OnAwake()
    M.self = self

    self:Listen(CS.Tetris.TetrisScoreEventArgs.s_EventID, M.UpdateUI)
    self:Listen(CS.Tetris.TetrisLineClearArgs.s_EventID, M.TextVFX)
end

function M:OnUpdate(dt)
    if M.self.gameCtx ~= nil then
        M.self.tmptxt_time.text = string.format("%.2f", M.self.gameCtx.gameTime)
    end
end

M.UpdateUI = function(sender, args)
    M.self.tmptxt_level.text = args.level
    M.self.tmptxt_score.text = args.score
    M.self.tmptxt_line.text = args.line
end

-- 文字效果
M.TextVFX = function(sender, args)
    local isTSpin = args.isTSpin
    local isMini = args.isMini
    local line = args.line
    local isB2B = args.isB2B
    local ren = args.ren

    if line == 4 then
        M:ProcessAnimation(M.self.go_Tetris)
    end

    if isTSpin then
        if line > 0 then
            M:ProcessAnimation(M.self.go_TSpin)
        end

        if line == 1 then
            M:ProcessAnimation(M.self.go_Single)
        end

        if line == 2 then
            M:ProcessAnimation(M.self.go_Double)
        end

        if line == 3 then
            M:ProcessAnimation(M.self.go_Triple)
        end

        if isMini then
            M:ProcessAnimation(M.self.go_Mini)
        end
    end

    if isB2B then
        M:ProcessAnimation(M.self.go_B2B)
    end

    if ren <= 0 then
        if M.self.tmptxt_Ren.gameObject.activeSelf == true then
            M.self.tmptxt_Ren.gameObject:SetActive(false)
        end
    else
        if M.self.tmptxt_Ren.gameObject.activeSelf == false then
            M.self.tmptxt_Ren.gameObject:SetActive(true)
        else
            M.self.tmptxt_Ren.gameObject:SetActive(false)
            M.self.tmptxt_Ren.gameObject:SetActive(true)
        end

        M.self.tmptxt_Ren.text = "Ren " .. ren
    end
end

function M:ProcessAnimation(go)
    go:SetActive(false)
    go:SetActive(true)
end

return M
