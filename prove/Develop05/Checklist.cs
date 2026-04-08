public class ChecklistGoal : Goal
{
    private int _target;
    private int _current;
    private int _bonus;

    public ChecklistGoal(string name, string description, int points, int target, int bonus)
        : base(name, description, points)
    {
        _target = target;
        _bonus = bonus;
    }

    public void SetProgress(int current) => _current = current;

    public override int RecordEvent()
    {
        if (_current < _target)
        {
            _current++;
            if (_current == _target)
            {
                SetCompletedAt();
                return GetPoints() + _bonus;
            }
            return GetPoints();
        }
        return 0;
    }

    public override bool IsComplete() => _current >= _target;

    public override string GetStatus()
    {
        return IsComplete()
            ? $"[X] {GetName()} - {GetDescription()} ({_current}/{_target}) Completed: {GetCompletedAt()}"
            : $"[ ] {GetName()} - {GetDescription()} ({_current}/{_target})";
    }

    public override string SaveFormat()
    {
        return $"Checklist|{GetName()}|{GetDescription()}|{GetPoints()}|{_current}|{_target}|{_bonus}|{GetCompletedAt()}";
    }
}
