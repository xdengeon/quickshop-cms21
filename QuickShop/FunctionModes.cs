using System;
using Il2CppSystem.Collections.Generic;
using UnhollowerBaseLib;
using UnityEngine;

namespace QuickShop;

internal class FunctionModes
{
	public bool UnmountAllBodyParts(CarLoader carloader)
	{
		var enumerator = carloader.carParts.GetEnumerator();
		while (enumerator.MoveNext())
		{
			CarPart current = enumerator.Current;
			if (current.name != "body" && current.name != "details")
			{
				carloader.TakeOffCarPart(current.name);
			}
		}
		return true;
	}

	public bool GetMissingItems(CarLoader carloader, int PartAutoUpgrade, double pricepercentage, double PartFixedScrap)
	{
		PartScript[] array = carloader.root.GetComponentsInChildren<PartScript>();
		if (Singleton<GameInventory>.Instance != null)
		{
			double num = 0.0;
			for (int i = 0; i < array.Length; i++)
			{
				double num2 = pricepercentage / 100.0;
				if (PartAutoUpgrade == 1)
				{
					num2 = Math.Ceiling(num2 * (double)Singleton<GameInventory>.Instance.GetItemProperty("t_" + array[i].GetIDWithTuned()).Price);
					if (num2 == 0.0)
					{
						num2 = Math.Ceiling(num2 * (double)Singleton<GameInventory>.Instance.GetItemProperty(array[i].GetIDWithTuned()).Price);
					}
				}
				else
				{
					num2 = Math.Ceiling(num2 * (double)Singleton<GameInventory>.Instance.GetItemProperty(array[i].GetIDWithTuned()).Price);
				}
				num += num2;
			}
			double num3 = 0.0;
			if (num <= (double)GlobalData.PlayerMoney)
			{
				for (int j = 0; j < array.Length; j++)
				{
					if (!array[j].IsUnmounted)
					{
						continue;
					}
					if (PartAutoUpgrade == 1)
					{
						if (Math.Ceiling(pricepercentage / 100.0 * (double)Singleton<GameInventory>.Instance.GetItemProperty("t_" + array[j].GetIDWithTuned()).Price) != 0.0)
						{
							Item val = new Item();
							((BaseItem)val).ID = "t_" + array[j].GetIDWithTuned();
							QuickShopComponent.ApplyWheelData(val, ((BaseItem)val).ID, carloader);
							if (num3 <= (double)GlobalData.PlayerScraps)
							{
								val.Quality = 3;
								if (num3 + PartFixedScrap <= (double)GlobalData.PlayerScraps)
								{
									num3 += PartFixedScrap;
								}
							}
							((BaseItem)val).MakeNewUID();
							Singleton<Inventory>.Instance.Add(val, true);
							continue;
						}
						Item val2 = new Item();
						((BaseItem)val2).ID = array[j].GetIDWithTuned();
						QuickShopComponent.ApplyWheelData(val2, ((BaseItem)val2).ID, carloader);
						if (num3 <= (double)GlobalData.PlayerScraps)
						{
							val2.Quality = 3;
							if (num3 + PartFixedScrap <= (double)GlobalData.PlayerScraps)
							{
								num3 += PartFixedScrap;
							}
						}
						((BaseItem)val2).MakeNewUID();
						Singleton<Inventory>.Instance.Add(val2, true);
					}
					else
					{
						Item val3 = new Item();
						((BaseItem)val3).ID = array[j].GetIDWithTuned();
						QuickShopComponent.ApplyWheelData(val3, ((BaseItem)val3).ID, carloader);
						((BaseItem)val3).MakeNewUID();
						Singleton<Inventory>.Instance.Add(val3, true);
					}
				}
				GlobalData.AddPlayerScraps(-(int)num3);
				GlobalData.AddPlayerMoney(-(int)num);
			}
			else
			{
				UIManager.Get().ShowPopup("QiuckShop Mod:", "Not enought money!", (PopupType)3);
			}
		}
		return true;
	}
}
