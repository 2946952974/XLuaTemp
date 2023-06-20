# 知识

- [简介 · GitBook (shenjun-coder.github.io)](https://shenjun-coder.github.io/LuaBook/)
- 学习网站

## 冷更新

- 发布平台，测试后更新数据，==先关闭应用再进行更新==

## 热更新

- 无需将代码重新打包至AppStore，即可更新客户端
- 苹果禁止了C#反射操作，不允许热更新，只允许使用AssetBundle进行资源更新

### 为何要热更新

- 缩短用户获取新版应用客户端流程
- 发布到AppStore周期难控制
- 大型应用，更新成本大
- 需要不重新下载，不停机状态下实现热更新

## 不同平台

- Android，将代码作为TextAeest打进AB包，运行时调用AssemblyDll代码
- 更新相应的AssetBundle即可实现热更新
- IOS（lua）
- 新方案：Unity+Lua插件

![image-20220819174555460](C:\Users\程速琦\AppData\Roaming\Typora\typora-user-images\image-20220819174555460.png)

![image-20220819174647527](C:\Users\程速琦\AppData\Roaming\Typora\typora-user-images\image-20220819174647527.png)

- 从服务器down到可写目录下

# XLua学习

- GIthub下载地址

- [Releases · Tencent/xLua · GitHub](https://github.com/Tencent/xLua/releases)

## 第一个执行

```csharp
/****************************************************
    文件：FirstXlua.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：XLua
*****************************************************/

using UnityEngine;
using XLua;//需要引用命名空间

public class FirstXlua : MonoBehaviour 
{
    private void Start() {
        //Lua 是解释型语言,需要获得lua解析器
        //Xlua解析器获得
        LuaEnv env = new LuaEnv();

        //解析器运行lua代码,把字符串当成Lua代码执行
        env.DoString("print('Helloworld')");

        //解析器释放
        env.Dispose();
    }
}
```

![image-20220819180924211](C:\Users\程速琦\AppData\Roaming\Typora\typora-user-images\image-20220819180924211.png)

- 代表调用成功

## 01Lua调用C#

```csharp
/****************************************************
    文件：DoString.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：Lua调用C#
*****************************************************/

using UnityEngine;
using XLua;
public class DoString : MonoBehaviour 
{
    private void Start() {
        LuaCallCsharpCode();
    }
    public void LuaCallCsharpCode() {
        LuaEnv env = new LuaEnv();
        //Lua调用C#代码(cs.命名空间.类名.方法名(参数))
        env.DoString("CS.UnityEngine.Debug.Log('form Lua')");
        env.Dispose();
    }
}
```

## 02获取Lua返回值给C#



```csharp
/// <summary>
/// 返回值
/// </summary>
public void LuaReturnData() {
  LuaEnv env = new LuaEnv();
  object[] data=env.DoString("return 100,true");
  Debug.Log("Lua的第一个返回值"+data[0].ToString());
  Debug.Log("Lua的第二个返回值"+data[1].ToString());
  env.Dispose();
}
```

## 03 加载文件

- 需要 把Lua文件放在 StreamingAssets文件中
- ==已过时==

```csharp
/****************************************************
    文件：Loader.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：加载器
*****************************************************/

using UnityEngine;
using XLua;

public class Loader : MonoBehaviour 
{

    private void Start() {
        LuaEnv env = new LuaEnv();
        //内置加载器会扫描预制的目录，查找是否存在test.lua
        env.DoString("require('test')");
        env.Dispose();
    }

}
```

![image-20220820090859326](C:\Users\程速琦\AppData\Roaming\Typora\typora-user-images\image-20220820090859326.png)

## 04自定义加载器

```csharp
public void MyLoader() {
  LuaEnv env = new LuaEnv();

  //将我定义的加载器，加入到xlua的解析环境中
  env.AddLoader(ProjectLoader);
  env.DoString("require('test1')");
  env.Dispose();
}

//自定义加载器
//自定义加载器会先于系统内置架子啊其，当自定义加载器加载到文件后，后续加载器则不会继续执行
//当lua代码执行require时，自定义加载器会尝试加载
//参数：被加载Lua路径
//如果没有加载到文件，记得返回空
public byte[] ProjectLoader(ref string filepath) {
  //filepath来自于require（“文件名”）
  //需要构造路径，才能将require加载的文件指定到我们想要的lua路径下去
  //路径可以任意定制，Lua代码可以放到AB包内
  string path = Application.dataPath;//Assest
  path = path.Substring(0, path.Length - 7)+"/DataPath/Lua/"+filepath+".lua";//Lua文件路径
  //将lua文件读成字节数组
  //Xlua的解析环境，会执行我们自定义的加载器返回的lua代码
  if (File.Exists(path)) {
    return File.ReadAllBytes(path);
  }
  else {
    return null;
  }
}
```

## 全局控制加载器

```csharp
/****************************************************
    文件：xLuaEnx.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：lua环境
*****************************************************/

using System.IO;
using UnityEngine;
using XLua;

public class xLuaEnv
{
    #region Single
    private static xLuaEnv _Instance = null;

    public static xLuaEnv Instance {
        get {
            if (_Instance == null) {
                _Instance = new xLuaEnv();
            }

            return _Instance;
        }
    }
    #endregion
    #region Create LuaEnv
    private LuaEnv _Env;
    private xLuaEnv() {
        _Env = new LuaEnv();
        _Env.AddLoader(ProjectLoader);
    }
    #endregion
    #region Loader
    private byte[] ProjectLoader(ref string filepath) {
        string path = Application.dataPath;//Assest
        path = path.Substring(0, path.Length - 7) + "/DataPath/Lua/" + filepath + ".lua";//Lua文件路径
        if (File.Exists(path)) {
            return File.ReadAllBytes(path);
        }
        else {
            return null;
        }
    }
    #endregion
    #region Free LuaEnv
    public void Free() {
        _Env.Dispose();
        _Instance = null;
    }
    #endregion

    #region Run Lua
    public object[] DoString(string code) {
        return _Env.DoString(code);
    }
 //返回Lua环境的全局变量
    public LuaTable Global {
        get {
            return _Env.Global;
        }
    }
    #endregion
}
```

# Lua调用C#

- Lua调用C#
- C#调用Lua

## 01 调用C#静态

```lua
print("LuaCallStatic")
-- Lua调用静态类
-- CS.命名空间.类名.属性
print(CS.TestStatic.ID)

-- 给静态属性赋值
CS.TestStatic.Name="Admin"
print(CS.TestStatic.Name)

-- 静态成员方法调用
-- CS.命名空间.类名.方法名()
print(CS.TestStatic.Output())

-- 默认值
CS.TestStatic.Default()
-- lua传递赋值
CS.TestStatic.Default("def")
```

```csharp
/****************************************************
    文件：LuaCallStatic.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：Lua调用静态
*****************************************************/

using UnityEngine;

public static class TestStatic {
    public static int ID = 99;

    public static string Name {
        get;
        set;
    }
    public static string Output() {
        return "static";
    }
    public static void Default(string str = "adb") {
        Debug.Log(str);
    }

}


public class LuaCallStatic : MonoBehaviour 
{
    private void Start() {
        xLuaEnv.Instance.DoString("require('C2L/LuaCallStatic')");
    }
    private void OnDestroy() {
        xLuaEnv.Instance.Free();
    }
}
```

## 02 创建实例化

- 能用C#实现的，Lua都能实现

```lua
-- LuaCallObject.lua
-- Lua 实例化类
-- C# Npc obj=new Npc()
local obj=CS.Npc()
obj.HP=100
print(obj.HP)

local obj1=CS.Npc("admin")
print(obj1.Name)

-- 表方法希望调用表成员变量（表：函数（））
-- 对象调用成员变量时隐形使用this，等同于lua的self
print(obj1:Output())

-- Lua实例化GameObject
CS.UnityEngine.GameObject("LuaCreateGO")
go:AddComponent(typeof(CS.UnityEngine.BoxCollider))
```

```csharp
/****************************************************
    文件：LuaCallObject.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：Lua调用Object
*****************************************************/

using UnityEngine;
public class Npc {
    public string Name;
    public int HP {
        get;
        set;
    }
    public Npc() {

    }
    public Npc(string name) {
        this.Name = name;
    }
    public void Output() {
        Debug.Log(Name);
    }
}

public class LuaCallObject : MonoBehaviour 
{
    private void Start() {
        xLuaEnv.Instance.DoString("require('C2L/LuaCallObject')");
    }
    private void OnDestroy() {
        xLuaEnv.Instance.Free();
    }
}
```

## 03调用结构体

```lua
-- LuaCallStruct.lua
-- 和对象调用保持一致
local obj=CS.TestStruct()

obj.Name="admin"

print(obj.Name)
print(obj:Output())
```

```csharp
/****************************************************
    文件：LuaCallStruct.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：Lua调用结构体
*****************************************************/

using UnityEngine;

public struct TestStruct {
    public string Name;

    public string Output() {
        return Name;
    }
}

public class LuaCallStruct : MonoBehaviour 
{
    private void Start() {
        xLuaEnv.Instance.DoString("require('C2L/LuaCallStruct')");
    }
    private void OnDestroy() {
        xLuaEnv.Instance.Free();
    }
}
```

## 04调用枚举

```lua
--LuaCallEnum.lua
-- CS.命名空间.枚举名.枚举值
-- 枚举获得时userdata自定义数据类型，
--获得其他语言数据类型是就是userdata
print(CS.TestEnum.LoL)
print(CS.TestEnum.Dota2)
print(type(CS.TestEnum.LoL))

-- 转换获得枚举值
print("Casr:"..CS.TestEnum.__CastFrom(0):ToString())
print("Cast:"..CS.TestEnum.__CastFrom("Dota2"):ToString())


```

```csharp
/****************************************************
    文件：LuaCallEnum.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：Lua调用枚举
*****************************************************/

using UnityEngine;
public enum TestEnum {
    LoL=0,

    Dota2
}

public class LuaCallEnum : MonoBehaviour 
{
    private void Start() {
        xLuaEnv.Instance.DoString("require('C2L/LuaCallEnum')");
    }
    private void OnDestroy() {
        xLuaEnv.Instance.Free();
    }
}
```

## 05重载

```lua
--LuaCallOverLoad
-- 数字重载
CS.TestOverLoad.Test(99)
-- 字符串重载
CS.TestOverLoad.Test("admin")
-- 不同参数重载
CS.TestOverLoad.Test(100,"root")
```

```csharp
/****************************************************
    文件：LuaCallOverLoad.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：Luad调用重载
*****************************************************/

using UnityEngine;
public class TestOverLoad {
    public static void Test(int id) {
        Debug.Log("Int:"+id);
    }
    public static void Test(string id) {
        Debug.Log("string:" + id);
    }
    public static void Test(int id,string name) {
        Debug.Log("Two::" + id+name);
    }
}

public class LuaCallOverLoad : MonoBehaviour 
{
    private void Start() {
        xLuaEnv.Instance.DoString("require('C2L/LuaCallOverLoad')");
    }
    private void OnDestroy() {
        xLuaEnv.Instance.Free();
    }
}
```

## 06继承

```lua
-- LuaCallBase.lua
-- 调用father
local father=CS.Father()
print(father.Name)

father:Overide()

-- 调用Child
local child=CS.Child()
print(child.Name)
child:Talk()
child:Overide()

```

```csharp
/****************************************************
    文件：LuaCallBase.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：继承
*****************************************************/

using UnityEngine;

public class Father {
    public string Name = "father";

    public void Talk() {
        Debug.Log("这是父类方法");
    }
    public virtual void Overide() {
        Debug.Log("这是父类虚方法");
    }
}
public class Child : Father {

    public override void Overide() {
        Debug.Log("这是子类重写方法");
    }
}



public class LuaCallBase : MonoBehaviour 
{
    private void Start() {
        xLuaEnv.Instance.DoString("require('C2L/LuaCallBase')");
    }
    private void OnDestroy() {
        xLuaEnv.Instance.Free();
    }
}
```

## 07 内扩展

```lua
-- LuaCallExtend.lua

-- 获取对象
local obj=CS.TestExtend()

obj:Output()
-- 用不了，需要特性
obj:Show()
```

```csharp
/****************************************************
    文件：LuaCallExtend.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：Lua内扩展
*****************************************************/

using UnityEngine;
using XLua;
public class TestExtend {
    public void Output() {
        Debug.Log("类本身带的方法");
    }
}

//内扩展需要给扩展方法编写的静态类添加特性
[LuaCallCSharp]
public static class MyExtend {
    public static void Show(this TestExtend obj) {
        Debug.Log("内扩展实现方法");
    }
    public static void Show(this Object obj) {
        Debug.Log("内扩展实现方法");
    }
}


public class LuaCallExtend : MonoBehaviour 
{
    private void Start() {
        xLuaEnv.Instance.DoString("require('C2L/LuaCallExtend')");
    }
    private void OnDestroy() {
        xLuaEnv.Instance.Free();
    }
}
```

08委托

```lua
--LuaCallDelegate.lua

CS.TestDelegate.Static=CS.TestDelegate.StaticFunc
CS.TestDelegate.Static()
-- Lua中如果添加了函数到静态委托变量中，在委托不再使用后
-- 一定要释放添加的委托函数
CS.TestDelegate.Static = nil
-----------------------------------------
local func=function()
	print("这是Lua的函数")
end
-- 覆盖添加委托
CS.TestDelegate.Static=func
-- 确定已经添加过回调函数
CS.TestDelegate.Static=CS.TestDelegate.Static+func
CS.TestDelegate.Static=CS.TestDelegate.Static-func
CS.TestDelegate.Static()
CS.TestDelegate.Static = nil
-----------------------------------------
-- -- 调用前判定
-- if(CS.TestDelegate.Static~=nil)
-- then
	-- CS.TestDelegate.Static()
-- end

-- -- 根据委托判定赋值方法
-- if(CS.TestDelegate.Static==nil)
-- then
	-- CS.TestDelegate.Static=func
-- else
	-- CS.TestDelegate.Static=CS.TestDelegate.Static+func
-- end
-----------------------------------------

-- 对象的委托，可以不置为nil
local obj=CS.TestDelegate()
obj.Dynamic=func
obj.Dynamic()
obj.Dynamic=nil
```

```csharp
/****************************************************
    文件：LuaCallDelegate.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：Lua调用委托
*****************************************************/

using UnityEngine;
public delegate void DelegateLua();

public class TestDelegate {
    public static DelegateLua Static;
    public static DelegateLua Dynamic;

    public static void StaticFunc() {
        Debug.Log("C#静态成员函数");
    }
    public static void ClearEnvent() {
        Debug.Log("释放委托");
        Static = null;
        Dynamic = null;
    }

}


public class LuaCallDelegate : MonoBehaviour 
{
    private void Start() {
        xLuaEnv.Instance.DoString("require('C2L/LuaCallDelegate')");
    }
    private void OnDestroy() {
        xLuaEnv.Instance.Free();
    }
}
```

## 09事件

```lua
-- LuaCallEvent.lua
-- C#调用事件，TestEvent.Static+=TestEvent.StaticFunc

-- Lua添加事件
CS.TestEvent.Static("+",CS.TestEvent.StaticFunc)
CS.TestEvent.CallStatic()
CS.TestEvent.Static("-",CS.TestEvent.StaticFunc)

-- 添加动态成员变量
local func=function()
	print("来自于Lua的回调函数")
end

local obj=CS.TestEvent()
obj:Dynamic("+",func)
obj:CallDynamic()
obj:Dynamic("-",func)
```

```csharp
/****************************************************
    文件：LuaCallEvent.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：Lua调用事件
*****************************************************/

using UnityEngine;
public delegate void EventLua();
public class TestEvent {
    public static event EventLua Static;

    public static void StaticFunc() {
        Debug.Log("这是静态函数");
    }

    public static void CallStatic() {
        if (Static != null) {
            Static();
        }
    }
    public event EventLua Dynamic;

    public void CallDynamic() {
        if (Dynamic != null) {
            Dynamic();
        }
    }
}



public class LuaCallEvent : MonoBehaviour 
{
    private void Start() {
        xLuaEnv.Instance.DoString("require('C2L/LuaCallEvent')");
    }
    private void OnDestroy() {
        xLuaEnv.Instance.Free();
    }
}
```

## 10泛型

- Lua不支持泛型
- typeof 替代泛型

```lua
-- LuaCallGenericType.lua

local obj=CS.TestGenericType()
obj:Output(99)
obj:Output("admin")

-- xlua实现了typeof关键子，
--可以用类型方法替代unity内置的泛型
local go=CS.UnityEngine.GameObject("LuaCreate")
go:AddComponent(typeof(CS.UnityEngine.BoxCollider))
```

```csharp
/****************************************************
    文件：LuaCallGenericType.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：Lua调用泛型
*****************************************************/

using UnityEngine;

public class TestGenericType {
    public void Output<T>(T data) {
        Debug.Log("泛型方法" + data.ToString());
    }
    public void Output(float data) {
        Output<float>(data);
    }
    public void Output(string data) {
        Output<string>(data);
    }

}

public class LuaCallGenericType : MonoBehaviour 
{
    private void Start() {
        xLuaEnv.Instance.DoString("require('C2L/LuaCallGenericType')");
    }
    private void OnDestroy() {
        xLuaEnv.Instance.Free();
    }
}
```

## 11 Ref Out

```lua
-- LuaCallOutRef.lua
local r1=CS.TestOutRef.Func1()
print(r1)

-- C# out返回值，会赋值给Lua第二个接受返回值变量
local out2
local r2,out1=CS.TestOutRef.Func2("admin",out2)
print(r2,out1,out2)

-- C# ref返回值，会赋值给Lua第二个接受返回值变量
local ref2
local r3,ref1=CS.TestOutRef.Func3("root",ref2)
print(r3,ref1,ref2)

-- 即使out，ref作为第一个参数
-- 结果也会以lua多个返回值返回
-- r4=是return的，ref3是ref返回值
local ref4
local r4,ref3=CS.TestOutRef.Func4(ref4,"test")
print(r4,ref3,ref4)
```

```csharp
/****************************************************
    文件：LuaCallOutRef.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：Lua调用out ref
*****************************************************/

using UnityEngine;
public class TestOutRef {
    public static string Func1() {
        return "Func1";
    }
    public static string Func2(string str1,out string str2) {
        str2 = "Func2 out";
        return "Func2";
    }
    public static string Func3(string str1,ref string str2) {
        str2 = "Func3 ref";
        return "Func3";
    }
    public static string Func4(ref string str1, ref string str2) {
        str1 = "Func4 ref";
        return "Func4";
    }

}

public class LuaCallOutRef : MonoBehaviour 
{
    private void Start() {
        xLuaEnv.Instance.DoString("require('C2L/LuaCallOutRef')");
    }
    private void OnDestroy() {
        xLuaEnv.Instance.Free();
    }
}
```

# C#调用Lua

- Unity是基于C#语言开发，生命周期都是基于C#实现
- Xlua不存在Unity相关，生命周期函数
- 可以实现C#作为Unity原始调用，再使用C#调用Lua对应的方法

## 01获取全局变量

```lua
-- CSharpCallVariable.lua
--return 100

-- 隐性做了{num=100,rate=99.99,isWoman=false,name="admin"}
num=100

rate=99.99

isWoman=false

name="admin"
```

```csharp
/****************************************************
    文件：CSharpCallVariable.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：Nothing
*****************************************************/

using UnityEngine;
using XLua;

public class CSharpCallVariable : MonoBehaviour 
{
    private void Start() {
        object[] data = xLuaEnv.Instance.DoString("return require('L2C/CSharpCallVariable')");
        //Debug.Log(data[0]);

        //LuaEnv提供了一个成员变量Global，它可以用于C#获取Lua的全局变量
        //Global的数据类型是C#实现的LuaTable，LuaTable是xLua实现的C#和Lua中表对应的数据结构
        //Xlua将Lua中的全局变量以Table的方式全部存储再Globle中
        
        
        //LuaTable是C#的数据对象，和Lua中的Table映射
        LuaTable g = xLuaEnv.Instance.Global;

        //从Lua中的全局变量提取出来
        //参数：Lua全局变量的名称
        //泛型：Lua全局变量的类型
        int num=g.Get<int>("num");
        float rate=g.Get<float>("rate");
        bool isWoman= g.Get<bool>("isWoman");
        string  name = g.Get<string>("name");
        Debug.Log(num+"    "+rate+"   "+isWoman+"    "+name);
    }
    private void OnDestroy() {
        xLuaEnv.Instance.Free();
    }
}
```



## 02调用Lua函数

- 版本问题
- 谢谢，解决了。Unity 2019需要在 Project Settings->Player->Api Compatibility Level 设置成 .NET 4.x

```lua
-- CSharpCallFunction.lua

func1=function()
	print("这是Lua的func1")
end

func2=function(name)
	print("这是Lua的func2,参数："..name)
end

func3=function()
	return "这是Lua的func3"
end

func4=function()
	return "这是Lua的func4",100
end
```

```csharp
/****************************************************
    文件：CSharpCallFunction.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：C#调用Lua函数
*****************************************************/

using UnityEngine;
using XLua;



public class CSharpCallFunction : MonoBehaviour 
{
    public delegate void Func1();
    public delegate void Func2(string name);
    public delegate string Func3();

    [CSharpCallLua]//映射产生时，xlua提示添加的
    public delegate void Func4(out string name, out int id);

    private void Start() {
        object[] data = xLuaEnv.Instance.DoString("return require('L2C/CSharpCallFunction')");
        
        LuaTable g = xLuaEnv.Instance.Global;

        //Lua的函数，会导出为C#的委托类型
        Func1 func1 = g.Get<Func1>("func1");
        func1();

        //向Lua传递数数
        Func2 func2 = g.Get<Func2>("func2");
        func2("admin");

        //接受Lua函数返回值
        Func3 func3 = g.Get<Func3>("func3");
        Debug.Log(func3());

        //Lua多返回值
        Func4 func4 =g.Get<Func4>("func4");
        string name;
        int id;
        func4(out name, out id);
        Debug.Log(name + " , " + id); 
    }
    private void OnDestroy() {
        xLuaEnv.Instance.Free();
    }
}
```



## 03调用LuaTable

```lua
-- CSharpCallTable

Core={
	Name="Core",
	age=19,
	isWoman=false,
}
Core.ID=100
Core.Func1=function(name)
	print("这是Core的Func1函数，接受"..name)
end
Core.Func3=function(self)
	print("这是Core的Func3函数，成员"..self.Name..self.age..self.isWoman..self.ID)
end
Core.Func4=function(self)
	print("这是Core的Func4函数，接受"..self.Name)
end
```

```csharp
/****************************************************
    文件：CSharpCallTable.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：C#调用LuaTable
*****************************************************/

using UnityEngine;
using XLua;

public delegate void OneStringParams(string name);
public delegate void TransSelf(LuaTable table);

public class CSharpCallTable : MonoBehaviour 
{
    private void Start() {
        xLuaEnv.Instance.DoString("return require('L2C/CSharpCallTable')");
        LuaTable g = xLuaEnv.Instance.Global;

        //获取Name
        //参数：Table中索引名
        //类型：索引对应类型
        LuaTable core = g.Get<LuaTable>("Core");

        string name = core.Get<string>("Name");
        Debug.Log(name);

        core.Set<string, string>("Name", "Setadmin");
        OneStringParams osp = core.Get<OneStringParams>("Func1");
        osp("admin");

        //相当于“：”调用
        TransSelf ts = core.Get<TransSelf>("Func4");
        ts(core);

    }
}
```

- 第二种方法（最好的），使用结构体

![image-20220820233332898](C:\Users\程速琦\AppData\Roaming\Typora\typora-user-images\image-20220820233332898.png)

```lua
 student = {
        name = "xm", age = 18, Sex = "man",
        80, 90, 95,
        getSex = function(self)
            return "人妖"
        end,

        totalScore = function(self, a, b)
            return a + b;
        end
    }
```

```csharp
 /*
             * xLua复杂值类型（struct）的默认传递方式是引用传递，这种方式要求先对值类型boxing，传递给lua，
             * lua使用后释放该引用。由于值类型每次boxing将产生一个新对象，当lua侧使用完毕释放该对象的引用时，
             * 则产生一次gc。为此，xLua实现了一套struct的gc优化方案，您只要通过简单的配置，
             * 则可以实现满足条件的struct传递到lua侧无gc。
             * 
             * struct需要满足什么条件？
             * 1、struct允许嵌套其它struct，但它以及它嵌套的struct只能包含这几种基本类型：
             * byte、sbyte、short、ushort、int、uint、long、ulong、float、double；
             * 例如UnityEngine定义的大多数值类型：Vector系列，Quaternion，Color。。。均满足条件，
             * 或者用户自定义的一些struct
             * 2、该struct配置了GCOptimize属性（对于常用的UnityEngine的几个struct，
             * Vector系列，Quaternion，Color。。。均已经配置了该属性），这个属性可以通过配置文件或者C# Attribute实现；
             * 3、使用到该struct的地方，需要添加到生成代码列表；
             */
            [GCOptimize]
            [LuaCallCSharp]
            struct Student
            {
                public string name;
                public int age;
                public string Sex { get; set; }  // 无法对应table中的键

                public override string ToString()
                {
                    return string.Format("name : {0}, age : {1}, sex : {2}", name, age, Sex);
                }
            }
```

## 04用结构体映射

```lua
-- CSharpCallSturct

Core={}
Core.ID=100
Core.Name="root"
Core.IsWoman=false

Core.Func1=function(name)
	print("这是Core的Func1函数，接受"..name)
end
Core.Func2=function()
	return "这是Core表的Func2函数"
end

Core.Func3=function(self)
	print("这是Core的Func3函数，成员"..self.Name..self.isWoman..self.ID)
end
function Core:Func4()
	print("这是Core的Func4函数，接受"..self.Name)
end
```



```csharp
/****************************************************
    文件：CSharpCallStruct.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：C#调用LuaTable,Struct映射
*****************************************************/

using UnityEngine;
using XLua;

public delegate void OneStringParams(string name);
public delegate string OneStringReturn();
public delegate void TransSelf(LuaTable table);
[CSharpCallLua]
public delegate void TransMy(LuaCore table);

//Lua映射Table过来，需要有个结构体进行对应
//Lua的Table导出到C#的结构体，运行时无GC
[GCOptimize]
public struct LuaCore {
    public int ID;
    public string Name;
    public bool IsWoman;

    public OneStringParams Func1;
    public OneStringReturn Func2;
    public TransMy Func3;
    public TransMy Func4;//需要同名
}

public class CSharpCallStruct : MonoBehaviour 
{
    private void Start() {
        xLuaEnv.Instance.DoString("return require('L2C/CSharpCallSturct')");
        //UseLuaTable();
        UseStruct();
    }
    public void UseLuaTable() {
        LuaTable g = xLuaEnv.Instance.Global;
        //获取Name
        //参数：Table中索引名
        //类型：索引对应类型
        LuaTable core = g.Get<LuaTable>("Core");

        string name = core.Get<string>("Name");
        Debug.Log(name);

        core.Set<string, string>("Name", "Setadmin");
        OneStringParams osp = core.Get<OneStringParams>("Func1");
        osp("admin");

        //相当于“：”调用
        TransSelf ts = core.Get<TransSelf>("Func4");
        ts(core);
    }

    public void UseStruct() {
        LuaTable g = xLuaEnv.Instance.Global;

        //将Lua的Table导出为Core
        LuaCore core=g.Get<LuaCore>("Core");
        Debug.Log(core.Name);

        core.Func4(core);
    }
    private void OnDestroy() {
        xLuaEnv.Instance.Free();
    }
}
```





# 联系起来

- 练习Unity的Update等
- 用Lua实现操作



# 记录

- P3节课，xLua环境控制

- [25 xLua环境控制-火星网校_哔哩哔哩_bilibili](https://www.bilibili.com/video/BV1JT411V7V1?p=3&spm_id_from=pageDriver&vd_source=10eb4c0909b5ae5bf9174349594eae1e)

- 后续再看，还剩实战部分

- [35 周作业3-火星网校_哔哩哔哩_bilibili](https://www.bilibili.com/video/BV1of4y1f7fh?p=5&vd_source=10eb4c0909b5ae5bf9174349594eae1e)