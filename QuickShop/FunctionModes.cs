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

	public bool UnmountAllPass(CarLoader carloader)
	{
		// One disassembly pass: instantly unmount every part that is currently unblocked - bolts
		// come off before the panel they hold, so nothing is left floating. Parts still blocked by
		// outer ones stay put this frame; the game clears their blocked state a frame later, so the
		// caller runs this once per frame until a pass removes nothing. Returns whether it removed
		// anything this pass.
		bool progressed = false;
		PartScript[] parts = carloader.root.GetComponentsInChildren<PartScript>();
		for (int i = 0; i < parts.Length; i++)
		{
			PartScript part = parts[i];
			if (!part.IsUnmounted && part.canBeUnmount && !part.IsBlocked())
			{
				part.FastUnmount();
				if (part.IsUnmounted)
				{
					progressed = true;
				}
			}
		}
		return progressed;
	}

	public bool MountAllPass(CarLoader carloader)
	{
		// One assembly pass: for each empty slot we own a part for, instantly mount the best one
		// (tuned 3-star, then tuned, then regular; higher quality first) and consume it from the
		// inventory. Parts that cannot go on yet just do not take this frame; the caller runs this
		// once per frame until a pass mounts nothing. Returns whether it mounted anything.
		if (Singleton<Inventory>.Instance == null)
		{
			return false;
		}
		Inventory inventory = Singleton<Inventory>.Instance;
		GameScript gameScript = GameScript.Get();
		bool progressed = false;
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
			part.MountByGroup(true);
			if (!part.IsUnmounted)
			{
				inventory.Delete(best);
				progressed = true;
			}
		}
		return progressed;
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
