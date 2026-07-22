using System;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace QuickShop;

public class QuickShopComponent : MonoBehaviour
{
	public static int CheatMode;

	public static int FunctionMode;

	public static int PartAutoUpgrade;

	public QuickShopComponent(IntPtr ptr)
		: base(ptr)
	{
		MelonLogger.Log("[QuickShop] Entered Constructor");
	}

	[HarmonyPostfix]
	public static void Awake()
	{
		MelonLogger.Log("[QuickShop] I'm Awake!");
	}

	[HarmonyPostfix]
	public static void Start()
	{
		MelonLogger.Log("[QuickShop] I'm Starting Up...");
	}

	[HarmonyPostfix]
	public static void Update()
	{
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Invalid comparison between Unknown and I4
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Invalid comparison between Unknown and I4
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Invalid comparison between Unknown and I4
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Invalid comparison between Unknown and I4
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Invalid comparison between Unknown and I4
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Invalid comparison between Unknown and I4
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Invalid comparison between Unknown and I4
		Config config = new Config();
		if (!config.CheckConfigFile())
		{
			config.CreateConfigFile();
		}
		if (!config.CheckPartsFile())
		{
			config.CreatePartsFile();
		}
		string normalKeybind = config.GetConfigFile().NormalKeybind;
		string tuningKeybind = config.GetConfigFile().TuningKeybind;
		bool enabledNormal = config.GetConfigFile().EnabledNormal;
		bool enabledTuning = config.GetConfigFile().EnabledTuning;
		double num = config.GetConfigFile().PricePercentage;
		bool enabledAutoBuyRequiredParts = config.GetConfigFile().EnabledAutoBuyRequiredParts;
		string cheatModeChangeKeybind = config.GetConfigFile().CheatModeChangeKeybind;
		string cheatModeKeybind = config.GetConfigFile().CheatModeKeybind;
		bool enabledCheatMode = config.GetConfigFile().EnabledCheatMode;
		string enableAutoPartUpgradeKeybind = config.GetConfigFile().EnableAutoPartUpgradeKeybind;
		double partFixedScrap = config.GetConfigFile().KeybindPartFixedScrap;
		string funcitonModeChangeKeybind = config.GetConfigFile().FuncitonModeChangeKeybind;
		string functionModeKeybind = config.GetConfigFile().FunctionModeKeybind;
		ListJsonReqParts partFile = config.GetPartFile();
		if (num == 0.0)
		{
			num = 1.0;
		}
		if (Input.GetKeyInt((KeyCode)Enum.Parse(typeof(KeyCode), enableAutoPartUpgradeKeybind)) && (int)Event.current.type == 4)
		{
			if (PartAutoUpgrade == 0)
			{
				PartAutoUpgrade = 1;
				UIManager.Get().ShowPopup("QiuckShop Func:", "Auto Part Upgrade Enabled!", (PopupType)3);
			}
			else
			{
				PartAutoUpgrade = 0;
				UIManager.Get().ShowPopup("QiuckShop Func:", "Auto Part Upgrade Disabled!", (PopupType)3);
			}
		}
		if (Input.GetKeyInt((KeyCode)Enum.Parse(typeof(KeyCode), funcitonModeChangeKeybind)) && (int)Event.current.type == 4)
		{
			if (FunctionMode == 0)
			{
				FunctionMode = 1;
			}
			else if (FunctionMode == 1)
			{
				FunctionMode = 0;
			}
			if (FunctionMode == 0)
			{
				UIManager.Get().ShowPopup("QiuckShop Func:", "Unmount All Body Parts Mode Activated!", (PopupType)0);
			}
			if (FunctionMode == 1)
			{
				UIManager.Get().ShowPopup("QiuckShop Func:", "Get Missing Parts Mode Activated!", (PopupType)0);
			}
		}
		if (Input.GetKeyInt((KeyCode)Enum.Parse(typeof(KeyCode), functionModeKeybind)) && (int)Event.current.type == 4)
		{
			if (FunctionMode == 0)
			{
				new FunctionModes().UnmountAllBodyParts(GameScript.Get().GetIOMouseOverCarLoader2());
			}
			if (FunctionMode == 1)
			{
				new FunctionModes().GetMissingItems(GameScript.Get().GetIOMouseOverCarLoader2(), PartAutoUpgrade, num, partFixedScrap);
			}
		}
		if (enabledCheatMode)
		{
			if (Input.GetKeyInt((KeyCode)Enum.Parse(typeof(KeyCode), cheatModeKeybind)) && (int)Event.current.type == 4)
			{
				if (CheatMode == 0)
				{
					CarLoader iOMouseOverCarLoader = GameScript.Get().GetIOMouseOverCarLoader2();
					new CheatModes().ReloadCar(iOMouseOverCarLoader);
				}
				if (CheatMode == 1)
				{
					GlobalData.AddPlayerMoney(1000);
				}
				if (CheatMode == 2)
				{
					GlobalData.AddPlayerScraps(100);
				}
				if (CheatMode == 3)
				{
					GlobalData.AddPlayerExp(100, false);
				}
				if (CheatMode == 4)
				{
					GameScript.Get().GetIOMouseOverCarLoader2().ExamineAllParts();
				}
				Event.current.Use();
			}
			if (Input.GetKeyInt((KeyCode)Enum.Parse(typeof(KeyCode), cheatModeChangeKeybind)) && (int)Event.current.type == 4)
			{
				if (CheatMode == 0)
				{
					CheatMode++;
				}
				else if (CheatMode == 1)
				{
					CheatMode++;
				}
				else if (CheatMode == 2)
				{
					CheatMode++;
				}
				else if (CheatMode == 3)
				{
					CheatMode++;
				}
				else if (CheatMode == 4)
				{
					CheatMode = 0;
				}
				if (CheatMode == 0)
				{
					UIManager.Get().ShowPopup("QiuckShop Cheat:", "Body Instant Fix Mode Activated!", (PopupType)3);
				}
				if (CheatMode == 1)
				{
					UIManager.Get().ShowPopup("QiuckShop Cheat:", "Money Cheat Activated!", (PopupType)3);
				}
				if (CheatMode == 2)
				{
					UIManager.Get().ShowPopup("QiuckShop Cheat:", "Scrap Cheat Activated!", (PopupType)3);
				}
				if (CheatMode == 3)
				{
					UIManager.Get().ShowPopup("QiuckShop Cheat:", "XP Cheat Activated!", (PopupType)3);
				}
				if (CheatMode == 4)
				{
					UIManager.Get().ShowPopup("QiuckShop Cheat:", "Examine All Cheat Activated!", (PopupType)3);
				}
			}
		}
		if (enabledNormal && Input.GetKeyInt((KeyCode)Enum.Parse(typeof(KeyCode), normalKeybind)) && (int)Event.current.type == 4)
		{
			NormalPartBuy(GameScript.Get().GetPartMouseOver().GetIDWithTuned(), num, enabledAutoBuyRequiredParts, partFile, partFixedScrap);
			Event.current.Use();
		}
		if (enabledTuning && Input.GetKeyInt((KeyCode)Enum.Parse(typeof(KeyCode), tuningKeybind)) && (int)Event.current.type == 4)
		{
			TuningPartBuy("t_" + GameScript.Get().GetPartMouseOver().GetIDWithTuned(), num, enabledAutoBuyRequiredParts, partFixedScrap);
			Event.current.Use();
		}
	}

	public static void NormalPartBuy(string ItemId, double pricepercentage, bool autobuyreqparts, ListJsonReqParts ReqParts, double PartFixedScrap)
	{
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		num += PriceCalculation(ItemId, pricepercentage);
		if (ItemId == null)
		{
			return;
		}
		Item val = CreateItem(ItemId);
		if (autobuyreqparts)
		{
			for (int i = 0; i < ReqParts.JsonReqParts.Count; i++)
			{
				if (!ReqParts.JsonReqParts[i].Enabled || !ItemId.Equals(ReqParts.JsonReqParts[i].MainPart) || !(ReqParts.JsonReqParts[i].AditionalParts != "") || ReqParts.JsonReqParts[i].AditionalParts == null)
				{
					continue;
				}
				string[] array = ReqParts.JsonReqParts[i].AditionalParts.Split(',');
				foreach (string text in array)
				{
					if (text != null && text != "")
					{
						num += PriceCalculation(text, pricepercentage);
					}
				}
			}
		}
		ApplyWheelData(val, ItemId, GameScript.Get().GetIOMouseOverCarLoader2());

		if (GlobalData.PlayerMoney < num)
		{
			UIManager.Get().ShowPopup("QiuckShop Mod:", "Not enought money!", (PopupType)3);
			return;
		}
		if (autobuyreqparts)
		{
			for (int k = 0; k < ReqParts.JsonReqParts.Count; k++)
			{
				if (!ReqParts.JsonReqParts[k].Enabled || !ItemId.Equals(ReqParts.JsonReqParts[k].MainPart) || !(ReqParts.JsonReqParts[k].AditionalParts != "") || ReqParts.JsonReqParts[k].AditionalParts == null)
				{
					continue;
				}
				string[] array = ReqParts.JsonReqParts[k].AditionalParts.Split(',');
				foreach (string text2 in array)
				{
					if (text2 == null || !(text2 != ""))
					{
						continue;
					}
					Item val2 = CreateItem(text2);
					if (PartAutoUpgrade == 1)
					{
						if ((double)GlobalData.PlayerScraps >= PartFixedScrap)
						{
							val2.Quality = 3;
							GlobalData.AddPlayerScraps(-(int)PartFixedScrap);
						}
						else
						{
							UIManager.Get().ShowPopup("QiuckShop Mod:", "Not enought Scraps! Item not upgraded. ", (PopupType)3);
						}
					}
					Singleton<Inventory>.Instance.Add(val2, true);
				}
			}
		}
		if (PartAutoUpgrade == 1)
		{
			if ((double)GlobalData.PlayerScraps >= PartFixedScrap)
			{
				val.Quality = 3;
				GlobalData.AddPlayerScraps(-(int)PartFixedScrap);
			}
			else
			{
				UIManager.Get().ShowPopup("QiuckShop Mod:", "Not enought Scraps! Item not upgraded. ", (PopupType)3);
			}
		}
		Singleton<Inventory>.Instance.Add(val, true);
		GlobalData.AddPlayerMoney(-num);
		UIManager.Get().ShowPopup("QiuckShop Mod:", "Part cost: " + num, (PopupType)3);
	}

	public static void TuningPartBuy(string ItemId, double pricepercentage, bool autobuyreqparts, double PartFixedScrap)
	{
		int price = Singleton<GameInventory>.Instance.GetItemProperty(ItemId).Price;
		int num = 0;
		num += PriceCalculation(ItemId, pricepercentage);
		if (ItemId != "t_" && price != 0)
		{
			Item val = CreateItem(ItemId);
			if (ItemId.Equals("t_tlok_1") && autobuyreqparts)
			{
				num += PriceCalculation("tlok_1_pierscienie_1", pricepercentage);
			}
			if (GlobalData.PlayerMoney < num)
			{
				UIManager.Get().ShowPopup("QiuckShop Mod:", "Not enought money!", (PopupType)3);
				return;
			}
			if (ItemId.Equals("t_tlok_1") && autobuyreqparts)
			{
				Item val2 = CreateItem("tlok_1_pierscienie_1");
				if (PartAutoUpgrade == 1)
				{
					if ((double)GlobalData.PlayerScraps >= PartFixedScrap)
					{
						val2.Quality = 3;
						GlobalData.AddPlayerScraps(-(int)PartFixedScrap);
					}
					else
					{
						UIManager.Get().ShowPopup("QiuckShop Mod:", "Not enought Scraps! Item not upgraded. ", (PopupType)3);
					}
				}
				Singleton<Inventory>.Instance.Add(val2, true);
			}
			if (PartAutoUpgrade == 1)
			{
				if ((double)GlobalData.PlayerScraps >= PartFixedScrap)
				{
					val.Quality = 3;
					GlobalData.AddPlayerScraps(-(int)PartFixedScrap);
				}
				else
				{
					UIManager.Get().ShowPopup("QiuckShop Mod:", "Not enought Scraps! Item not upgraded. ", (PopupType)3);
				}
			}
			Singleton<Inventory>.Instance.Add(val, true);
			GlobalData.AddPlayerMoney(-num);
			UIManager.Get().ShowPopup("QiuckShop Mod:", "Part cost: " + num, (PopupType)3);
		}
		else
		{
			UIManager.Get().ShowPopup("QiuckShop Mod:", "Tuning part not found!", (PopupType)0);
		}
	}

	public static int PriceCalculation(string ItemId, double pricepercentage)
	{
		return Convert.ToInt32(Math.Ceiling(pricepercentage / 100.0 * (double)Singleton<GameInventory>.Instance.GetItemProperty(ItemId).Price));
	}

	public static Item CreateItem(string ItemId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		Item val = new Item
		{
			ID = ItemId
		};
		((BaseItem)val).MakeNewUID();
		return val;
	}

	// Rims and tires must inherit the wheel dimensions the car actually uses, otherwise a
	// freshly created part spawns with a default size/profile that no longer fits the car.
	// The car reports its front spec as "Width/ProfileRSize" (e.g. "205/55R16"), optionally
	// as "front|rear" - we always use the front spec, matching the single-part buy path.
	public static void ApplyWheelData(Item item, string itemId, CarLoader carloader)
	{
		if (itemId == null || (!itemId.Contains("rim_") && !itemId.Contains("tire_")))
		{
			return;
		}
		string tireSize = carloader.GetFrontTireSize();
		if (string.IsNullOrEmpty(tireSize))
		{
			return;
		}
		string spec = (tireSize.Contains("|") ? tireSize.Split('|')[0] : tireSize);
		string[] bySize = spec.Split('R');
		if (bySize.Length < 2)
		{
			return;
		}
		WheelData wheelData = new WheelData
		{
			Size = int.Parse(bySize[1])
		};
		if (itemId.Contains("tire_"))
		{
			string[] byProfile = bySize[0].Split('/');
			if (byProfile.Length < 2)
			{
				return;
			}
			wheelData.Width = int.Parse(byProfile[0]);
			wheelData.Profile = int.Parse(byProfile[1]);
		}
		item.WheelData = wheelData;
	}
}
