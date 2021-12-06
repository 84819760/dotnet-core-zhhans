namespace DotNetCore_zhHans.Db.Import;

internal class ProgressData : NotifyPropertyChanged
{
    public double Maximum { get; set; }

    public double Value { get; set; }

    public double Progress { get; set; }

    public int Digits { get; init; } = 4;

    public string? Title { get; set; }

    public void AddToMaximum(double vlaue = 1)
    {
        Maximum += vlaue;
        ChangedHandler();
    }

    public void AddToValue(double vlaue = 1)
    {
        Value += vlaue;
        ChangedHandler();
    }

    public void ChangedHandler() => Progress = Math.Round(Value / Maximum, Digits);
}
