using UnityEngine;
using System.Collections;

namespace Waggle
{

public class GameEvent {}

public class UpdateDiamondMultiplierEvent : GameEvent
{
	public int multiplier;
	public UpdateDiamondMultiplierEvent(int multiplier)
	{
		this.multiplier = multiplier;
	}
}
public class UpdateBuffCountEvent : GameEvent
{
	public int buffSpinCount;
	public UpdateBuffCountEvent(int buffSpinCount)
	{
		this.buffSpinCount = buffSpinCount;
	}
}

public class UpdateBonusSpinCountEvent : GameEvent
{
	public int bonusSpinCount;
	public UpdateBonusSpinCountEvent(int bonusSpinCount)
	{
		this.bonusSpinCount = bonusSpinCount;
	}
}

public class BonusSpinButtonClickEvent : GameEvent {}
public class MainUIEnableEvent : GameEvent 
{
	public bool isEnabled;
	public MainUIEnableEvent(bool isEnabled)
	{
		this.isEnabled = isEnabled;
	}
}



public class ChangeSymbolEvent : GameEvent 
{
	public int column;
	public int row;
	
	public ChangeSymbolEvent(int column, int row = -1)
	{
		this.column = column;
		this.row 	= row;
	}
}

public class SpinBigWheelEvent : GameEvent
{

}


}