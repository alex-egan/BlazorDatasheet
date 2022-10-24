using BlazorDatasheet.Util;

namespace BlazorDatasheet.Data;

/// <summary>
/// A column region. This region does not have an end position.
/// </summary>
public class RowRegion : Region
{
    public RowRegion(int rowStart, int rowEnd) : base(rowStart, rowEnd, 0, int.MaxValue)
    {
    }
    public override IRegion Copy()
    {
        return new RowRegion(TopLeft.Row, BottomRight.Row);
    }

    public override void ExtendTo(int row, int col, IRegion? regionLimit = null)
    {
        if (regionLimit == null)
        {
            End = new CellPosition(row, int.MaxValue);
            SetOrderedBounds();
            return;
        }

        var newRowEnd = SheetMath
            .ClampInt(regionLimit.TopLeft.Row, regionLimit.BottomRight.Row, row);

        End = new CellPosition(newRowEnd, int.MaxValue);
        SetOrderedBounds();
    }
}