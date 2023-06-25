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

public class xLuaEnv {
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

    //Test
    #endregion

}