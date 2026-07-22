using Il2CppSystem.Collections.Generic;

namespace QuickShop;

internal class CheatModes
{
	public void ReloadCar(CarLoader carloader)
	{
		List<CarPart> val = new List<CarPart>();
		new List<Part>();
		var enumerator = carloader.carParts.GetEnumerator();
		while (enumerator.MoveNext())
		{
			CarPart current = enumerator.Current;
			current.Unmounted = false;
			current.Condition = 100f;
			current.ConditionPaint = 100f;
			current.Dust = 0f;
			current.Dent = 1f;
			current.StructureCondition = 1f;
			val.Add(current);
		}
		carloader.carParts = val;
		carloader.SaveCarToFile();
		carloader.DeleteCar();
		carloader.LoadCarFromFile();
	}
}
