namespace QuickShop;

public class JsonConfig
{
	public string NormalKeybind { get; set; }

	public string TuningKeybind { get; set; }

	public string EnableAutoPartUpgradeKeybind { get; set; }

	public string FuncitonModeChangeKeybind { get; set; }

	public string FunctionModeKeybind { get; set; }

	public string CheatModeChangeKeybind { get; set; }

	public string CheatModeKeybind { get; set; }

	public bool EnabledNormal { get; set; }

	public bool EnabledTuning { get; set; }

	public bool EnabledCheatMode { get; set; }

	public int PricePercentage { get; set; }

	public int KeybindPartFixedScrap { get; set; }

	public bool EnabledAutoBuyRequiredParts { get; set; }

	public bool EnabledAutoBuyTuned { get; set; }

	public double ConfigVersion { get; set; }
}
