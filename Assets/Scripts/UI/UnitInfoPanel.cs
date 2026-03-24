using UnityEngine;
using System.Collections;

public class UnitInfoPanel : MonoBehaviour
{
    [SerializeField] private UINumber currentHealthNum;
    [SerializeField] private UINumber maxHealthNum;
    [SerializeField] private UINumber strength;
    [SerializeField] private UINumber movementRange;

    public void ShowPanel(Unit currUnit)
    {
        UnitInfo info = UnitDatabase.Instance.GetUnitInfo(currUnit.ID);
        currentHealthNum.UpdateDigit(currUnit.GetHealth());
        maxHealthNum.UpdateDigit(info.baseHealth);
        strength.UpdateDigit(info.strength);
        movementRange.UpdateDigit(info.moveRange);

    }

}
