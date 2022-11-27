namespace BlazorDatasheet.Data.Events;

public class ColumnRemovedEventArgs
{
    public int ColumnIndex { get; }

    public ColumnRemovedEventArgs(int columnIndex)
    {
        ColumnIndex = columnIndex;
    }
}