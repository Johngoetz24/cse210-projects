public class SimpleGoal : Goal
{
    private bool _isComplete;

    public SimpleGoal(string name, string description, int points)
        : base(name, description, points) { }

    public void SetComplete(bool complete) => _isComplete = complete;

    public override int RecordEvent()
    {
        if (!_isComplete)
        {
            _isComplete = true;
            SetCompletedAt();
            return GetPoints();
        }
        return 0;
    }

    public override bool IsComplete() => _isComplete;

    public override string GetStatus()
    {
        return _isComplete
            ? $"[X] {GetName()} - {GetDescription()} (Completed: {GetCompletedAt()})"
            : $"[ ] {GetName()} - {GetDescription()}";
    }

    public override string SaveFormat()
    {
        return $"Simple|{GetName()}|{GetDescription()}|{GetPoints()}|{_isComplete}|{GetCompletedAt()}";
    }
}