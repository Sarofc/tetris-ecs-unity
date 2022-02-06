#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class SaroMainWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Saro.Main);
			Utils.BeginObjectRegister(type, L, translator, 0, 1, 0, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "DumpMonoCallbacks", _m_DumpMonoCallbacks);
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 16, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "Quit", _m_Quit_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AddOnGUIListener", _m_AddOnGUIListener_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RemoveOnGUIListener", _m_RemoveOnGUIListener_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AddOnDraGizmosListener", _m_AddOnDraGizmosListener_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RemoveOnDraGizmosListener", _m_RemoveOnDraGizmosListener_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RunCoroutine", _m_RunCoroutine_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CancelCoroutine", _m_CancelCoroutine_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CancelAllCoroutinesWhithObjectRef", _m_CancelAllCoroutinesWhithObjectRef_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CancelAllCoroutines", _m_CancelAllCoroutines_xlua_st_);
            
			Utils.RegisterFunc(L, Utils.CLS_IDX, "onUpdate", _e_onUpdate);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "onLateUpdate", _e_onLateUpdate);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "onFixedUpdate", _e_onFixedUpdate);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "onApplicationPause", _e_onApplicationPause);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "onApplicationFocus", _e_onApplicationFocus);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "onApplicationQuit", _e_onApplicationQuit);
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					var gen_ret = new Saro.Main();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to Saro.Main constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Quit_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    Saro.Main.Quit(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddOnGUIListener_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.Action _guiDelegate = translator.GetDelegate<System.Action>(L, 1);
                    
                    Saro.Main.AddOnGUIListener( _guiDelegate );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RemoveOnGUIListener_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.Action _guiDelegate = translator.GetDelegate<System.Action>(L, 1);
                    
                    Saro.Main.RemoveOnGUIListener( _guiDelegate );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddOnDraGizmosListener_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.Action _guiDelegate = translator.GetDelegate<System.Action>(L, 1);
                    
                    Saro.Main.AddOnDraGizmosListener( _guiDelegate );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RemoveOnDraGizmosListener_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.Action _guiDelegate = translator.GetDelegate<System.Action>(L, 1);
                    
                    Saro.Main.RemoveOnDraGizmosListener( _guiDelegate );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DumpMonoCallbacks(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Saro.Main gen_to_be_invoked = (Saro.Main)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.DumpMonoCallbacks(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RunCoroutine_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& translator.Assignable<System.Collections.IEnumerator>(L, 1)&& translator.Assignable<object>(L, 2)) 
                {
                    System.Collections.IEnumerator _routine = (System.Collections.IEnumerator)translator.GetObject(L, 1, typeof(System.Collections.IEnumerator));
                    object _objectRef = translator.GetObject(L, 2, typeof(object));
                    
                        var gen_ret = Saro.Main.RunCoroutine( _routine, _objectRef );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 1&& translator.Assignable<System.Collections.IEnumerator>(L, 1)) 
                {
                    System.Collections.IEnumerator _routine = (System.Collections.IEnumerator)translator.GetObject(L, 1, typeof(System.Collections.IEnumerator));
                    
                        var gen_ret = Saro.Main.RunCoroutine( _routine );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Saro.Main.RunCoroutine!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CancelCoroutine_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 1&& translator.Assignable<UnityEngine.Coroutine>(L, 1)) 
                {
                    UnityEngine.Coroutine _routine = (UnityEngine.Coroutine)translator.GetObject(L, 1, typeof(UnityEngine.Coroutine));
                    
                    Saro.Main.CancelCoroutine( _routine );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 1&& translator.Assignable<System.Collections.IEnumerator>(L, 1)) 
                {
                    System.Collections.IEnumerator _routine = (System.Collections.IEnumerator)translator.GetObject(L, 1, typeof(System.Collections.IEnumerator));
                    
                    Saro.Main.CancelCoroutine( _routine );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Saro.Main.CancelCoroutine!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CancelAllCoroutinesWhithObjectRef_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    object _objectRef = translator.GetObject(L, 1, typeof(object));
                    
                    Saro.Main.CancelAllCoroutinesWhithObjectRef( _objectRef );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CancelAllCoroutines_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    Saro.Main.CancelAllCoroutines(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_onUpdate(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
                System.Action gen_delegate = translator.GetDelegate<System.Action>(L, 2);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#2 need System.Action!");
                }
                
				
				if (gen_param_count == 2 && LuaAPI.xlua_is_eq_str(L, 1, "+")) {
					Saro.Main.onUpdate += gen_delegate;
					return 0;
				} 
				
				
				if (gen_param_count == 2 && LuaAPI.xlua_is_eq_str(L, 1, "-")) {
					Saro.Main.onUpdate -= gen_delegate;
					return 0;
				} 
				
			} catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
			return LuaAPI.luaL_error(L, "invalid arguments to Saro.Main.onUpdate!");
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_onLateUpdate(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
                System.Action gen_delegate = translator.GetDelegate<System.Action>(L, 2);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#2 need System.Action!");
                }
                
				
				if (gen_param_count == 2 && LuaAPI.xlua_is_eq_str(L, 1, "+")) {
					Saro.Main.onLateUpdate += gen_delegate;
					return 0;
				} 
				
				
				if (gen_param_count == 2 && LuaAPI.xlua_is_eq_str(L, 1, "-")) {
					Saro.Main.onLateUpdate -= gen_delegate;
					return 0;
				} 
				
			} catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
			return LuaAPI.luaL_error(L, "invalid arguments to Saro.Main.onLateUpdate!");
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_onFixedUpdate(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
                System.Action gen_delegate = translator.GetDelegate<System.Action>(L, 2);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#2 need System.Action!");
                }
                
				
				if (gen_param_count == 2 && LuaAPI.xlua_is_eq_str(L, 1, "+")) {
					Saro.Main.onFixedUpdate += gen_delegate;
					return 0;
				} 
				
				
				if (gen_param_count == 2 && LuaAPI.xlua_is_eq_str(L, 1, "-")) {
					Saro.Main.onFixedUpdate -= gen_delegate;
					return 0;
				} 
				
			} catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
			return LuaAPI.luaL_error(L, "invalid arguments to Saro.Main.onFixedUpdate!");
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_onApplicationPause(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
                System.Action<bool> gen_delegate = translator.GetDelegate<System.Action<bool>>(L, 2);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#2 need System.Action<bool>!");
                }
                
				
				if (gen_param_count == 2 && LuaAPI.xlua_is_eq_str(L, 1, "+")) {
					Saro.Main.onApplicationPause += gen_delegate;
					return 0;
				} 
				
				
				if (gen_param_count == 2 && LuaAPI.xlua_is_eq_str(L, 1, "-")) {
					Saro.Main.onApplicationPause -= gen_delegate;
					return 0;
				} 
				
			} catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
			return LuaAPI.luaL_error(L, "invalid arguments to Saro.Main.onApplicationPause!");
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_onApplicationFocus(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
                System.Action<bool> gen_delegate = translator.GetDelegate<System.Action<bool>>(L, 2);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#2 need System.Action<bool>!");
                }
                
				
				if (gen_param_count == 2 && LuaAPI.xlua_is_eq_str(L, 1, "+")) {
					Saro.Main.onApplicationFocus += gen_delegate;
					return 0;
				} 
				
				
				if (gen_param_count == 2 && LuaAPI.xlua_is_eq_str(L, 1, "-")) {
					Saro.Main.onApplicationFocus -= gen_delegate;
					return 0;
				} 
				
			} catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
			return LuaAPI.luaL_error(L, "invalid arguments to Saro.Main.onApplicationFocus!");
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_onApplicationQuit(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
                System.Action gen_delegate = translator.GetDelegate<System.Action>(L, 2);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#2 need System.Action!");
                }
                
				
				if (gen_param_count == 2 && LuaAPI.xlua_is_eq_str(L, 1, "+")) {
					Saro.Main.onApplicationQuit += gen_delegate;
					return 0;
				} 
				
				
				if (gen_param_count == 2 && LuaAPI.xlua_is_eq_str(L, 1, "-")) {
					Saro.Main.onApplicationQuit -= gen_delegate;
					return 0;
				} 
				
			} catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
			return LuaAPI.luaL_error(L, "invalid arguments to Saro.Main.onApplicationQuit!");
        }
        
    }
}
