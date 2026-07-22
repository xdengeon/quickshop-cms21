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

	public bool UnmountAllCarParts(CarLoader carloader)
	{
		// Peel the whole car apart, inner parts included: keep instantly unmounting every part
		// that still can be, until a full pass removes nothing new (parts blocked by outer ones
		// become removable once those come off). The pass cap guarantees the loop always ends.
		bool removedAny = true;
		int safety = 0;
		while (removedAny && safety++ < 25)
		{
			removedAny = false;
			PartScript[] parts = carloader.root.GetComponentsInChildren<PartScript>();
			for (int i = 0; i < parts.Length; i++)
			{
				PartScript part = parts[i];
				if (!part.IsUnmounted && part.canBeUnmount)
				{
					part.FastUnmount();
					if (part.IsUnmounted)
					{
						removedAny = true;
					}
				}
			}
		}
		return true;
	}

	public bool MountAllCarParts(CarLoader carloader)
	{
		// Install parts we own onto the car's empty slots. Like unmounting, we go in passes -
		// mounting an outer part can expose an inner slot - until a full pass mounts nothing new.
		// For each slot we pick the best item we own (tuned 3-star, then tuned, then regular;
		// higher quality first) so the good parts are the ones that go on.
		//
		// NOTE: the game's mount path (ActionMount/SelectedToMount) is UI-driven and could not be
		// fully verified offline; this drives the most plausible entry points and may need tweaks.
		if (Singleton<Inventory>.Instance == null)
		{
			return true;
		}
		Inventory inventory = Singleton<Inventory>.Instance;
		GameScript gameScript = GameScript.Get();
		bool mountedAny = true;
		int safety = 0;
		while (mountedAny && safety++ < 25)
		{
			mountedAny = false;
			PartScript[] parts = carloader.root.GetComponentsInChildren<PartScript>();
			for (int i = 0; i < parts.Length; i++)
			{
				PartScript part = parts[i];
				if (!part.IsUnmounted)
				{
					continue;
				}
				Item best = FindBestOwnedPart(inventory, part.GetID());
				if (best == null)
				{
					continue;
				}
				gameScript.SelectedToMount = best;
				part.ActionMount(false);
				if (!part.IsUnmounted)
				{
					mountedAny = true;
				}
			}
		}
		return true;
	}

	// Best inventory item that fits the given slot id, ranked tuned-3-star > tuned > regular
	// (higher quality wins within a group). Returns null when nothing we own fits the slot.
	private static Item FindBestOwnedPart(Inventory inventory, string slotId)
	{
		if (string.IsNullOrEmpty(slotId))
		{
			return null;
		}
		Item best = null;
		int bestRank = int.MinValue;
		var enumerator = inventory.GetItems().GetEnumerator();
		while (enumerator.MoveNext())
		{
			Item item = enumerator.Current;
			if (item == null || item.GetNormalID() != slotId)
			{
				continue;
			}
			int rank = PartRank(item);
			if (rank > bestRank)
			{
				bestRank = rank;
				best = item;
			}
		}
		return best;
	}

	// Priority score: tuned 3-star highest, then any tuned, then regular; ties broken by quality.
	private static int PartRank(Item item)
	{
		string id = ((BaseItem)item).ID;
		bool tuned = id != null && id.StartsWith("t_");
		int tier = ((tuned && item.Quality >= 3) ? 3 : (tuned ? 2 : 1));
		return tier * 100 + item.Quality;
	}

	public bool GetMissingItems(CarLoader carloader, int PartAutoUpgrade, double pricepercentage, double PartFixedScrap, bool autoBuyTuned)
	{
		PartScript[] array = carloader.root.GetComponentsInChildren<PartScript>();
		if (Singleton<GameInventory>.Instance == null)
		{
			return true;
		}
		GameInventory gameInventory = Singleton<GameInventory>.Instance;
		// Total only counts the parts we are actually going to buy (the unmounted ones),
		// each priced at the exact id we will buy - tuned when available and enabled.
		double num = 0.0;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].IsUnmounted)
			{
				continue;
			}
			string buyId = GetBuyId(gameInventory, array[i].GetIDWithTuned(), autoBuyTuned);
			num += Math.Ceiling(pricepercentage / 100.0 * (double)gameInventory.GetItemProperty(buyId).Price);
		}
		if (num > (double)GlobalData.PlayerMoney)
		{
			UIManager.Get().ShowPopup("QiuckShop Mod:", "Not enought money!", (PopupType)3);
			return true;
		}
		double num3 = 0.0;
		for (int j = 0; j < array.Length; j++)
		{
			if (!array[j].IsUnmounted)
			{
				continue;
			}
			string buyId = GetBuyId(gameInventory, array[j].GetIDWithTuned(), autoBuyTuned);
			Item val = new Item();
			((BaseItem)val).ID = buyId;
			QuickShopComponent.ApplyWheelData(val, buyId, carloader, array[j]);
			// The quality upgrade (and its scrap cost) stays tied to the auto-upgrade toggle,
			// independent of whether we bought the tuned or the plain version of the part.
			if (PartAutoUpgrade == 1 && num3 <= (double)GlobalData.PlayerScraps)
			{
				val.Quality = 3;
				if (num3 + PartFixedScrap <= (double)GlobalData.PlayerScraps)
				{
					num3 += PartFixedScrap;
				}
			}
			((BaseItem)val).MakeNewUID();
			Singleton<Inventory>.Instance.Add(val, true);
		}
		GlobalData.AddPlayerScraps(-(int)num3);
		GlobalData.AddPlayerMoney(-(int)num);
		return true;
	}

	// Prefer the tuned variant ("t_" + id) when the toggle is on and the game actually has a
	// tuned version of the part (its shop entry has a non-zero price); otherwise the plain id.
	private static string GetBuyId(GameInventory gameInventory, string id, bool autoBuyTuned)
	{
		if (autoBuyTuned)
		{
			string tunedId = "t_" + id;
			if (gameInventory.GetItemProperty(tunedId).Price != 0)
			{
				return tunedId;
			}
		}
		return id;
	}
}
