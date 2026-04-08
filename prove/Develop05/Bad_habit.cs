
public class BadHabitGoal : Goal
{
    private int _count = 0;

    public BadHabitGoal(string name, string description, int points)
        : base(name, description, points) { }

    public void SetCount(int count) => _count = count;

    public override int RecordEvent()
    {
        _count++;
        return -GetPoints();
    }

    public override bool IsComplete() => false;

    public override string GetStatus()
    {
        return $"[!] {GetName()} - {GetDescription()} (Slipped {_count} times)";
    }

    public override string SaveFormat()
    {
        return $"BadHabit|{GetName()}|{GetDescription()}|{GetPoints()}|{_count}";
    }
}
