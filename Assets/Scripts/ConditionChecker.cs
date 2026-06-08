
public static class ConditionChecker
{
    public static bool Check(ConditionData[] requires, RuntimeConditionData[] currentConditions)
    {
        bool isValid = requires.Length == currentConditions.Length;
        if (isValid == false) return false;

        int n = requires.Length;

        for (int i = 0; i < n; i++)
        {
            if (requires[i].value != currentConditions[i].value)
                return false;
        }
        return true;
    }
    public static bool Check(ConditionData[] requires, RuntimeConditionData currentConditions)
    {
        bool isValid = requires.Length == 1;
        if (isValid == false) return false;

        return requires[0].value == currentConditions.value;
    }
}