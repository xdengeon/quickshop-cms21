using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace QuickShop;

public class Config
{
	public void CreateConfigFile()
	{
		JsonConfig jsonConfig = new JsonConfig
		{
			NormalKeybind = "B",
			TuningKeybind = "N",
			EnableAutoPartUpgradeKeybind = "Insert",
			FuncitonModeChangeKeybind = "PageUp",
			FunctionModeKeybind = "PageDown",
			CheatModeChangeKeybind = "Home",
			CheatModeKeybind = "End",
			EnabledNormal = true,
			EnabledTuning = true,
			EnabledCheatMode = false,
			PricePercentage = 100,
			KeybindPartFixedScrap = 60,
			EnabledAutoBuyRequiredParts = true,
			EnabledAutoBuyTuned = true,
			ConfigVersion = 1.06
		};
		using StreamWriter streamWriter = File.CreateText("Mods/config.json");
		new JsonSerializer
		{
			Formatting = (Formatting)1
		}.Serialize((TextWriter)streamWriter, (object)jsonConfig);
	}

	public void CreatePartsFile()
	{
		List<JsonReqParts> list = new List<JsonReqParts>();
		JsonReqParts item = new JsonReqParts
		{
			MainPart = "zaciskHamulcowy_1",
			AditionalParts = "zaciskHamulcowy_tloczek_1",
			Enabled = true
		};
		list.Add(item);
		item = new JsonReqParts
		{
			MainPart = "tlok_1",
			AditionalParts = "tlok_1_pierscienie_1",
			Enabled = true
		};
		list.Add(item);
		item = new JsonReqParts
		{
			MainPart = "wahaczDol_double",
			AditionalParts = "tuleja_1,tuleja_1",
			Enabled = true
		};
		list.Add(item);
		item = new JsonReqParts
		{
			MainPart = "wahaczDolny_1",
			AditionalParts = "tuleja_1,tuleja_1",
			Enabled = true
		};
		list.Add(item);
		item = new JsonReqParts
		{
			MainPart = "wahaczTyl_2",
			AditionalParts = "tulejaMala_1,tulejaMala_1",
			Enabled = true
		};
		list.Add(item);
		item = new JsonReqParts
		{
			MainPart = "amortyzator_double",
			AditionalParts = "sprezynnaPrzod_1,czapkaAmorPrzod_1",
			Enabled = true
		};
		list.Add(item);
		item = new JsonReqParts
		{
			MainPart = "wahaczKrotkiTyl_1",
			AditionalParts = "tuleja_1,tulejaMala_1",
			Enabled = true
		};
		list.Add(item);
		item = new JsonReqParts
		{
			MainPart = "amortyzatorPrzod_1",
			AditionalParts = "sprezynnaPrzod_1,czapkaAmorPrzod_1",
			Enabled = true
		};
		list.Add(item);
		item = new JsonReqParts
		{
			MainPart = "wentylatorChlodnicy_2",
			AditionalParts = "wentylatorChlodnicy_2_fan_1,wentylatorChlodnicy_2_fan_2",
			Enabled = true
		};
		list.Add(item);
		ListJsonReqParts listJsonReqParts = new ListJsonReqParts
		{
			JsonReqParts = list
		};
		using StreamWriter streamWriter = File.CreateText("Mods/parts.json");
		new JsonSerializer
		{
			Formatting = (Formatting)1
		}.Serialize((TextWriter)streamWriter, (object)listJsonReqParts);
	}

	public bool CheckConfigFile()
	{
		if (File.Exists("Mods/config.json"))
		{
			return true;
		}
		return false;
	}

	public bool CheckPartsFile()
	{
		if (File.Exists("Mods/parts.json"))
		{
			return true;
		}
		return false;
	}

	public JsonConfig GetConfigFile()
	{
		string path = "Mods/config.json";
		JsonConfig jsonConfig = JsonConvert.DeserializeObject<JsonConfig>(File.ReadAllText(path));
		if (jsonConfig.ConfigVersion != 1.06)
		{
			CreateConfigFile();
			jsonConfig = JsonConvert.DeserializeObject<JsonConfig>(File.ReadAllText(path));
		}
		return jsonConfig;
	}

	public ListJsonReqParts GetPartFile()
	{
		return JsonConvert.DeserializeObject<ListJsonReqParts>(File.ReadAllText("Mods/parts.json"));
	}
}
