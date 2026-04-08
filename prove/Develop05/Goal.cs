public abstract class Goal
{
    private string _name;
    private string _description;
    private int _points;
    private DateTime? _completedAt;

    public Goal(string name, string description, int points)
    {
        _name = name;
        _description = description;
        _points = points;
    }

    public string GetName() => _name;
    public string GetDescription() => _description;
    public int GetPoints() => _points;

    public abstract int RecordEvent();
    public abstract bool IsComplete();
    public abstract string GetStatus();
    public abstract string SaveFormat();

    public void SetCompletedAt()
    {
        if (_completedAt == null)
            _completedAt = DateTime.Now;
    }

    public void SetCompletedAt(DateTime? date)
    {
        _completedAt = date;
    }

    public string GetCompletedAt()
    {
        return _completedAt.HasValue ? _completedAt.Value.ToString("g") : "";
    }
}