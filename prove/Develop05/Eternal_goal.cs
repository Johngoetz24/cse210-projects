public class EternalGoal : Goal
{
    public EternalGoal(string name, string description, int points)
        : base(name, description, points) { }

    public override int RecordEvent() => GetPoints();
    public override bool IsComplete() => false;

    public override string GetStatus()
    {
        return $"[∞] {GetName()} - {GetDescription()}";
    }

    public override string SaveFormat()
    {
        return $"Eternal|{GetName()}|{GetDescription()}|{GetPoints()}";
    }
}
