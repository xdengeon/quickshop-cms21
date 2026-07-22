using System;
using System.Reflection;
using HarmonyLib;
using MelonLoader;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace QuickShop;

public class QuickShopMod : MelonMod
{
	public override void OnApplicationStart()
	{
		MelonLogger.Msg("[QuickShop] Registering QuickShopComponent in Il2Cpp");
		try
		{
			ClassInjector.RegisterTypeInIl2Cpp<QuickShopComponent>();
			GameObject val = new GameObject("QuickShopObject");
			val.AddComponent<QuickShopComponent>();
			UnityEngine.Object.DontDestroyOnLoad(val);
		}
		catch
		{
			MelonLogger.Msg("[QuickShop] FAILED to Register Il2Cpp Type: QuickShopComponent!");
		}
		try
		{
			HarmonyLib.Harmony harmony = this.HarmonyInstance;
			MethodInfo methodInfo = AccessTools.Method(typeof(GameScript), "Update", (Type[])null, (Type[])null);
			MelonLogger.Msg("[QuickShop] Harmony - Original Method: " + methodInfo.DeclaringType.Name + "." + methodInfo.Name);
			MethodInfo methodInfo2 = AccessTools.Method(typeof(QuickShopComponent), "Update", (Type[])null, (Type[])null);
			MelonLogger.Msg("[QuickShop] Harmony - Postfix Method: " + methodInfo2.DeclaringType.Name + "." + methodInfo2.Name);
			harmony.Patch((MethodBase)methodInfo, (HarmonyMethod)null, new HarmonyMethod(methodInfo2), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
			MelonLogger.Msg("[QuickShop] Harmony - Runtime Patch's Applied");
		}
		catch
		{
			MelonLogger.Msg("[QuickShop] Harmony - FAILED to Apply Patch's!");
		}
	}
}
