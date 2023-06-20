/****************************************************
    文件：Bootstrap.cs
	作者：别离或雪
    邮箱: 2946952974@qq.com
    日期：#CreateTime#
	功能：启动类
*****************************************************/

using UnityEngine;
using XLua;
[CSharpCallLua]
public delegate void LifeCycle();
[GCOptimize]
public struct LuaBootstrap {
    public LifeCycle Start;
    public LifeCycle Update;
    public LifeCycle OnDestory;
}
public class Bootstrap : MonoBehaviour 
{
    //Lua核心table
    private LuaBootstrap _Bootstrap;
    //调用Lua代码
    private void Start() {

        //防止切换场景时丢失
        DontDestroyOnLoad(gameObject);
        xLuaEnv.Instance.DoString("return require('Bootstrap')");
        //将Lua的核心表Tbale导入到C#
        _Bootstrap = xLuaEnv.Instance.Global.Get<LuaBootstrap>("Bootstrap");

        _Bootstrap.Start();
    }
    private void Update() {
        _Bootstrap.Update();
    }

    
    //释放Lua代码
    private void OnDestroy() {
        _Bootstrap.OnDestory();
        //回调函数需要释放
        _Bootstrap.Start = null;
        _Bootstrap.Update = null;
        _Bootstrap.OnDestory = null;
        xLuaEnv.Instance.Free();
    }
}