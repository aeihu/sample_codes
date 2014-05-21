require "AudioEngine" 
require "cocos2d" 

-- for CCLuaEngine traceback
function __G__TRACKBACK__(msg)
    print("----------------------------------------")
    print("LUA ERROR: " .. tostring(msg) .. "\n")
    print(debug.traceback())
    print("----------------------------------------")
end

local function main()
    -- avoid memory leak
    collectgarbage("setpause", 100)
    collectgarbage("setstepmul", 5000)

    local cclog = function(...)
        print(string.format(...))
    end

    ---------------

    local leftPaddle = CCSprite:create("band.png")
    local rightPaddle = CCSprite:create("band.png")

    local ball = CCSprite:create("ball.png")
    ball:setPosition(ccp(CCDirector:sharedDirector():getVisibleSize().width/2,100))

    local gameWidth = CCDirector:sharedDirector():getVisibleSize().width;
    local gameHeight = CCDirector:sharedDirector():getVisibleSize().height;

    math.randomseed(os.time())
    local ballRadius = 10
    local ballAngle = math.random(0,359) * 2 * 3.14 / 360
    local paddleSize = rightPaddle:getContentSize()
    local rightPaddleSpeed  = 0;
    local paddleSpeed = 2;

    leftPaddle:setPosition(ccp(0+paddleSize.width/2,100))
    rightPaddle:setPosition(ccp(CCDirector:sharedDirector():getVisibleSize().width-paddleSize.width/2,100))

    local function tick()
        if ball.isPaused then return end

        math.randomseed(os.time())
        local x, y = ball:getPosition()
        local lx, ly = leftPaddle:getPosition()
        local rx, ry = rightPaddle:getPosition()

        if (x - ballRadius < 0) then
        --    isPlaying = false;
        --    pauseMessage.setString("You lost !\nPress space to restart or\nescape to exit");
        end

        if x + ballRadius > gameWidth then
            --isPlaying = false;
            --pauseMessage.setString("You won !\nPress space to restart or\nescape to exit");
        end

        if y - ballRadius < 0 then
            ballAngle = -ballAngle;
            ball:setPosition(x, ballRadius + 0.1);
        end

        if y + ballRadius > gameHeight then
            ballAngle = -ballAngle;
            ball:setPosition(x, gameHeight - ballRadius - 0.1);
        end

        -- Move the computer's paddle
        if (((rightPaddleSpeed < 0) and (ry - paddleSize.height / 2 > 5)) or
            ((rightPaddleSpeed > 0) and (ry + paddleSize.height / 2 < gameHeight - 5))) then
        
            rightPaddle:setPosition(rx, ry + rightPaddleSpeed);
            leftPaddle:setPosition(lx, ry + rightPaddleSpeed);
        end

        -- Update the computer's paddle direction according to the ball position

        if y + ballRadius > ry + paddleSize.height / 2 then
            rightPaddleSpeed = paddleSpeed;
        elseif y - ballRadius < ry - paddleSize.height / 2 then
            rightPaddleSpeed = -paddleSpeed;
        else
            rightPaddleSpeed = 0;
        end

        ball:setPosition(ccp(x+math.cos(ballAngle)*2,y+math.sin(ballAngle)*2))
        --Left Paddle
        if x - ballRadius < lx + paddleSize.width / 2 and 
            x - ballRadius > lx and
            y + ballRadius >= ly - paddleSize.height / 2 and
            y - ballRadius <= ly + paddleSize.height / 2 then
 
            if y > ly then
                ballAngle = 3.14 - ballAngle + math.random(0,19) * 3.14 / 180;
            else
                ballAngle = 3.14 - ballAngle - math.random(0,19) * 3.14 / 180;
            end

            ball:setPosition(lx + ballRadius + paddleSize.width / 2 + 0.1, y);
        end

        --Right Paddle
        if x + ballRadius > rx - paddleSize.width / 2 and
            x + ballRadius < rx and
            y + ballRadius >= ry - paddleSize.height / 2 and
            y - ballRadius <= ry + paddleSize.height / 2 then

            if y > ry then
                ballAngle = 3.14 - ballAngle + (math.random(0,19)) * 3.14 / 180;
            else
                ballAngle = 3.14 - ballAngle - (math.random(0,19)) * 3.14 / 180;
            end

            ball:setPosition(rx - ballRadius - paddleSize.width / 2 - 0.1, y);
        end
    end

    CCDirector:sharedDirector():getScheduler():scheduleScriptFunc(tick, 0, false)

    local layer = CCLayer:create()
    layer:addChild(leftPaddle)
    layer:addChild(rightPaddle)
    layer:addChild(ball)
    -- run
    local sceneGame = CCScene:create()
    sceneGame:addChild(layer)
    CCDirector:sharedDirector():runWithScene(sceneGame)
end

xpcall(main, __G__TRACKBACK__)
