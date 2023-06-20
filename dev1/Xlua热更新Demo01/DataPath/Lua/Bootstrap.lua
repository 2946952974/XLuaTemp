
Bootstrap={}

-- 核心Table,存储所有的控制器
Bootstrap.Controllers={}


Bootstrap.Start=function ()
	--print("Lua Start")
	
	--所有控制器,注册给核心控制器 
	local c=require("Controller/MainMenuController")
	Bootstrap.Controllers["MainMenuController"]=c
end

Bootstrap.Update=function ()
	print("Lua Update")
	-- 遍历所有控制器注册给和新的控制脚本
	for k,v in pairs(Bootstrap.Controllers)
	do
	-- 子类不一定有Update
		if(v.Update~=nil)
		then
			v:Update()
		end
		
	end
end

Bootstrap.OnDestory=function ()
	print("Lua OnDestory")
end

-- 提供一个核心Table用于加载核心控制器
Bootstrap.LoadPage=function(name)
-- 加载controller目录下的脚本 
	local c=require("Controller/"..name)
	Bootstrap.Controllers[name]=c
end


-- print("Bootstrap")
-- -- 加载器
-- require("Prefabes")

-- Prefabes:Load("Test")