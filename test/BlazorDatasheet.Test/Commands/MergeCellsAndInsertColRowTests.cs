﻿using BlazorDatasheet.Commands;
using BlazorDatasheet.Data;
using BlazorDatasheet.DataStructures.Geometry;
using NUnit.Framework;

namespace BlazorDatasheet.Test.Commands;

public class MergeCellsAndInsertColRowTests
{
    /*
     We need to test correct behaviour on inserting columns and rows into sheet with merged cells.
     After insert merged region should shift if inserted row is above it or inserted column is before
     or inserted into merged region.

     Initial data

           0  1  2  3  4
       0 |  |  |  |  |  |
       1 |  |U |  |  |  |
       2 |  |  |M    |  |
       3 |  |  |     |  |
       4 |  |  |  |  |  |

     */
    [Test]
    public void Insert_Row_Above_Then_Undo_Correct()
    {
        var sheet = new Sheet(5, 5);
        sheet.TrySetCellValue(1, 1, "U");
        sheet.TrySetCellValue(2, 2, "M");
        sheet.Merges.Add(sheet.Range(2, 3, 2, 3));

        /*
               0  1  2  3  4
           0 |  |  |  |  |  |
       --> 1 |  | U|  |  |  |
           2 |  |  | M   |  |
           3 |  |  |     |  |
           4 |  |  |  |  |  |
           5 |  |  |  |  |  |

         */

        Assert.True(sheet.Merges.IsInsideMerge(2, 2));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(3, 3));

        Assert.False(sheet.Merges.IsInsideMerge(0, 0));
        Assert.False(sheet.Merges.IsInsideMerge(4, 4));

        Assert.AreEqual("M", sheet.GetValue(2, 2));
        Assert.AreEqual(null, sheet.GetValue(3, 3));
        Assert.AreEqual("U", sheet.GetValue(1, 1));

        sheet.InsertRowAt(1);
        /*
               0  1  2  3  4
           0 |  |  |  |  |  |
           1 |  |  |  |  |  |
           2 |  |U |  |  |  |
           3 |  |  |M    |  |
           4 |  |  |     |  |
           5 |  |  |  |  |  |

         */

        Assert.True(sheet.Merges.IsInsideMerge(3, 2));
        Assert.True(sheet.Merges.IsInsideMerge(3, 3));
        Assert.True(sheet.Merges.IsInsideMerge(4, 2));
        Assert.True(sheet.Merges.IsInsideMerge(4, 3));

        Assert.False(sheet.Merges.IsInsideMerge(0, 0));
        Assert.False(sheet.Merges.IsInsideMerge(2, 2));
        Assert.False(sheet.Merges.IsInsideMerge(5, 2));

        Assert.AreEqual("M", sheet.GetValue(3, 2));
        Assert.AreEqual(null, sheet.GetValue(4, 3));
        Assert.AreEqual("U", sheet.GetValue(2, 1));

        sheet.Commands.Undo();

        Assert.True(sheet.Merges.IsInsideMerge(2, 2));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(3, 3));

        Assert.False(sheet.Merges.IsInsideMerge(0, 0));
        Assert.False(sheet.Merges.IsInsideMerge(4, 4));
    }

    [Test]
    public void Insert_Row_Into_Then_Undo_Correct()
    {
        var sheet = new Sheet(5, 5);
        sheet.TrySetCellValue(2, 2, "M");
        sheet.TrySetCellValue(1, 1, "U");

        sheet.Merges.Add(sheet.Range(2, 3, 2, 3));

        Assert.True(sheet.Merges.IsInsideMerge(2, 2));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(3, 3));

        Assert.False(sheet.Merges.IsInsideMerge(0, 0));
        Assert.False(sheet.Merges.IsInsideMerge(4, 4));

        Assert.AreEqual("M", sheet.GetValue(2, 2));
        Assert.AreEqual(null, sheet.GetValue(3, 3));
        Assert.AreEqual("U", sheet.GetValue(1, 1));

        sheet.InsertRowAt(3);
        /*
               0  1  2  3  4
           0 |  |  |  |  |  |
           1 |  |U |  |  |  |
           2 |  |  |M    |  |
           3 |  |  |     |  |
           4 |  |  |     |  |
           5 |  |  |  |  |  |

         */

        Assert.True(sheet.Merges.IsInsideMerge(2, 2));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(2, 2));
        Assert.True(sheet.Merges.IsInsideMerge(3, 3));
        Assert.True(sheet.Merges.IsInsideMerge(4, 2));
        Assert.True(sheet.Merges.IsInsideMerge(4, 3));

        Assert.False(sheet.Merges.IsInsideMerge(0, 0));
        Assert.False(sheet.Merges.IsInsideMerge(1, 2));
        Assert.False(sheet.Merges.IsInsideMerge(5, 2));

        Assert.AreEqual("M", sheet.GetValue(2, 2));
        Assert.AreEqual(null, sheet.GetValue(4, 3));
        Assert.AreEqual("U", sheet.GetValue(1, 1));
    }

    [Test]
    public void Insert_Column_Before_Then_Undo_Correct()
    {
        var sheet = new Sheet(5, 5);
        sheet.TrySetCellValue(2, 2, "M");
        sheet.TrySetCellValue(1, 1, "U");

        sheet.Merges.Add(sheet.Range(2, 3, 2, 3));

        Assert.True(sheet.Merges.IsInsideMerge(2, 2));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(3, 3));

        Assert.False(sheet.Merges.IsInsideMerge(0, 0));
        Assert.False(sheet.Merges.IsInsideMerge(4, 4));

        Assert.AreEqual("M", sheet.GetValue(2, 2));
        Assert.AreEqual(null, sheet.GetValue(3, 3));
        Assert.AreEqual("U", sheet.GetValue(1, 1));

        sheet.InsertColAt(0);

        /*
               0  1  2  3  4  5
           0 |  |  |  |  |  |  |
           1 |  |U |  |  |  |  |
           2 |  |  |  |M    |  |
           3 |  |  |  |     |  |
           4 |  |  |  |  |  |  |

         */

        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(2, 4));
        Assert.True(sheet.Merges.IsInsideMerge(3, 3));
        Assert.True(sheet.Merges.IsInsideMerge(3, 4));

        Assert.False(sheet.Merges.IsInsideMerge(0, 0));
        Assert.False(sheet.Merges.IsInsideMerge(2, 2));
        Assert.False(sheet.Merges.IsInsideMerge(1, 2));
        Assert.False(sheet.Merges.IsInsideMerge(4, 5));

        Assert.AreEqual("M", sheet.GetValue(2, 3));
        Assert.AreEqual(null, sheet.GetValue(2, 4));
        Assert.AreEqual("U", sheet.GetValue(1, 2));

        sheet.Commands.Undo();

        Assert.True(sheet.Merges.IsInsideMerge(2, 2));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(3, 3));

        Assert.False(sheet.Merges.IsInsideMerge(0, 0));
        Assert.False(sheet.Merges.IsInsideMerge(4, 4));
    }

    [Test]
    public void Insert_Column_Into_Then_Undo_Correct()
    {
        var sheet = new Sheet(5, 5);
        sheet.TrySetCellValue(2, 2, "M");
        sheet.TrySetCellValue(1, 1, "U");

        sheet.Merges.Add(sheet.Range(2, 3, 2, 3));

        Assert.True(sheet.Merges.IsInsideMerge(2, 2));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(3, 3));

        Assert.False(sheet.Merges.IsInsideMerge(0, 0));
        Assert.False(sheet.Merges.IsInsideMerge(4, 4));

        Assert.AreEqual("M", sheet.GetValue(2, 2));
        Assert.AreEqual(null, sheet.GetValue(3, 3));
        Assert.AreEqual("U", sheet.GetValue(1, 1));

        sheet.InsertColAt(3);

        /*
               0  1  2  3  4  5
           0 |  |  |  |  |  |  |
           1 |  |U |  |  |  |  |
           2 |  |  |M       |  |
           3 |  |  |        |  |
           4 |  |  |  |  |  |  |

         */

        Assert.True(sheet.Merges.IsInsideMerge(2, 2));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(2, 4));
        Assert.True(sheet.Merges.IsInsideMerge(3, 2));
        Assert.True(sheet.Merges.IsInsideMerge(3, 3));
        Assert.True(sheet.Merges.IsInsideMerge(3, 4));

        Assert.False(sheet.Merges.IsInsideMerge(0, 0));
        Assert.False(sheet.Merges.IsInsideMerge(1, 1));
        Assert.False(sheet.Merges.IsInsideMerge(2, 1));
        Assert.False(sheet.Merges.IsInsideMerge(4, 5));

        Assert.AreEqual("M", sheet.GetValue(2, 2));
        Assert.AreEqual(null, sheet.GetValue(2, 4));
        Assert.AreEqual("U", sheet.GetValue(1, 1));
    }

    [Test]
    public void Remove_Column()
    {
        /*
               0  1  2  3  4  5
           0 |  |  |  |  |  |  |
           1 |  |U |  |  |  |  |
           2 |  |  |M       |  |
           3 |  |  |        |  |
           4 |  |  |  |  |  |  |

         */

        var sheet = new Sheet(5, 5);
        sheet.TrySetCellValue(2, 2, "M");
        sheet.TrySetCellValue(1, 1, "U");

        sheet.Merges.Add(sheet.Range(2, 3, 2, 4));

        sheet.RemoveCol(3);

        /*
               0  1  2  3  4
           0 |  |  |  |  |  |
           1 |  |U |  |  |  |
           2 |  |  |M    |  |
           3 |  |  |     |  |
           4 |  |  |  |  |  |

         */

        Assert.True(sheet.Merges.IsInsideMerge(2, 2));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(3, 2));
        Assert.True(sheet.Merges.IsInsideMerge(3, 3));

        Assert.False(sheet.Merges.IsInsideMerge(0, 0));
        Assert.False(sheet.Merges.IsInsideMerge(1, 1));
        Assert.False(sheet.Merges.IsInsideMerge(2, 4));
        Assert.False(sheet.Merges.IsInsideMerge(4, 4));

        Assert.AreEqual("M", sheet.GetValue(2, 2));
        Assert.AreEqual(null, sheet.GetValue(2, 4));
        Assert.AreEqual("U", sheet.GetValue(1, 1));

        sheet.Commands.Undo();

        Assert.True(sheet.Merges.IsInsideMerge(2, 2));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(2, 4));
        Assert.True(sheet.Merges.IsInsideMerge(3, 2));
        Assert.True(sheet.Merges.IsInsideMerge(3, 3));
        Assert.True(sheet.Merges.IsInsideMerge(3, 4));

        Assert.False(sheet.Merges.IsInsideMerge(0, 0));
        Assert.False(sheet.Merges.IsInsideMerge(1, 1));
        Assert.False(sheet.Merges.IsInsideMerge(2, 1));
        Assert.False(sheet.Merges.IsInsideMerge(4, 5));

        Assert.AreEqual("M", sheet.GetValue(2, 2));
        Assert.AreEqual(null, sheet.GetValue(2, 4));
        Assert.AreEqual("U", sheet.GetValue(1, 1));

        sheet.RemoveCol(0);

        /*
               0  1  2  3  4
           0 |  |  |  |  |  |
           1 |U |  |  |  |  |
           2 |  |M       |  |
           3 |  |        |  |
           4 |  |  |  |  |  |

         */

        Assert.True(sheet.Merges.IsInsideMerge(2, 1));
        Assert.True(sheet.Merges.IsInsideMerge(2, 2));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(3, 1));
        Assert.True(sheet.Merges.IsInsideMerge(3, 2));
        Assert.True(sheet.Merges.IsInsideMerge(3, 3));

        Assert.False(sheet.Merges.IsInsideMerge(0, 0));
        Assert.False(sheet.Merges.IsInsideMerge(1, 1));
        Assert.False(sheet.Merges.IsInsideMerge(2, 4));
        Assert.False(sheet.Merges.IsInsideMerge(4, 4));

        Assert.AreEqual("M", sheet.GetValue(2, 1));
        Assert.AreEqual(null, sheet.GetValue(2, 3));
        Assert.AreEqual("U", sheet.GetValue(1, 0));
    }

    [Test]
    public void Remove_Row()
    {
        /*
               0  1  2  3  4  5
           0 |  |  |  |  |  |  |
           1 |  |U |  |  |  |  |
           2 |  |  |M       |  |
           3 |  |  |        |  |
           4 |  |  |        |  |
           5 |  |  |  |  |  |  |

         */

        var sheet = new Sheet(5, 5);
        sheet.TrySetCellValue(2, 2, "M");
        sheet.TrySetCellValue(1, 1, "U");

        sheet.Merges.Add(sheet.Range(2, 4, 2, 4));

        sheet.RemoveRow(3);

        /*
               0  1  2  3  4  5
           0 |  |  |  |  |  |  |
           1 |  |U |  |  |  |  |
           2 |  |  |M       |  |
           3 |  |  |        |  |
           4 |  |  |  |  |  |  |

         */

        Assert.True(sheet.Merges.IsInsideMerge(2, 2));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(2, 4));
        Assert.True(sheet.Merges.IsInsideMerge(3, 2));
        Assert.True(sheet.Merges.IsInsideMerge(3, 3));
        Assert.True(sheet.Merges.IsInsideMerge(3, 4));

        Assert.False(sheet.Merges.IsInsideMerge(0, 0));
        Assert.False(sheet.Merges.IsInsideMerge(1, 1));
        Assert.False(sheet.Merges.IsInsideMerge(2, 5));
        Assert.False(sheet.Merges.IsInsideMerge(4, 4));

        Assert.AreEqual("M", sheet.GetValue(2, 2));
        Assert.AreEqual(null, sheet.GetValue(2, 3));
        Assert.AreEqual("U", sheet.GetValue(1, 1));

        sheet.Commands.Undo();

        Assert.True(sheet.Merges.IsInsideMerge(2, 2));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));
        Assert.True(sheet.Merges.IsInsideMerge(2, 4));
        Assert.True(sheet.Merges.IsInsideMerge(4, 2));
        Assert.True(sheet.Merges.IsInsideMerge(4, 3));
        Assert.True(sheet.Merges.IsInsideMerge(4, 4));

        Assert.False(sheet.Merges.IsInsideMerge(0, 0));
        Assert.False(sheet.Merges.IsInsideMerge(1, 1));
        Assert.False(sheet.Merges.IsInsideMerge(2, 5));
        Assert.False(sheet.Merges.IsInsideMerge(5, 5));

        sheet.RemoveRow(0);


        /*
               0  1  2  3  4  5
           0 |  |U |  |  |  |  |
           1 |  |  |M       |  |
           2 |  |  |        |  |
           3 |  |  |        |  |
           4 |  |  |  |  |  |  |

         */

        Assert.True(sheet.Merges.IsInsideMerge(1, 2));
        Assert.True(sheet.Merges.IsInsideMerge(1, 3));
        Assert.True(sheet.Merges.IsInsideMerge(1, 4));
        Assert.True(sheet.Merges.IsInsideMerge(3, 2));
        Assert.True(sheet.Merges.IsInsideMerge(3, 3));
        Assert.True(sheet.Merges.IsInsideMerge(3, 4));

        Assert.False(sheet.Merges.IsInsideMerge(0, 0));
        Assert.False(sheet.Merges.IsInsideMerge(0, 3));
        Assert.False(sheet.Merges.IsInsideMerge(1, 5));
        Assert.False(sheet.Merges.IsInsideMerge(3, 5));
        Assert.False(sheet.Merges.IsInsideMerge(5, 5));

        Assert.AreEqual("M", sheet.GetValue(1, 2));
        Assert.AreEqual(null, sheet.GetValue(2, 3));
        Assert.AreEqual("U", sheet.GetValue(0, 1));
    }

    [Test]
    public void Unmerge_Column()
    {
        var sheet = new Sheet(5, 5);
        sheet.TrySetCellValue(2, 2, "M");
        sheet.TrySetCellValue(1, 1, "U");

        sheet.Merges.Add(sheet.Range(2, 2, 2, 3));

        /*
                0  1  2  3  4
            0 |  |  |  |  |  |
            1 |  |U |  |  |  |
            2 |  |  |M    |  |
            3 |  |  |  |  |  |
            4 |  |  |  |  |  |
         */

        Assert.True(sheet.Merges.IsInsideMerge(2, 2));
        Assert.True(sheet.Merges.IsInsideMerge(2, 3));

        Assert.False(sheet.Merges.IsInsideMerge(0, 0));
        Assert.False(sheet.Merges.IsInsideMerge(4, 4));

        Assert.AreEqual("M", sheet.GetValue(2, 2));
        Assert.AreEqual(null, sheet.GetValue(3, 3));
        Assert.AreEqual("U", sheet.GetValue(1, 1));

        sheet.RemoveCol(3);
        /*
                0  1  2  3  4
            0 |  |  |  |  |  |
            1 |  |U |  |  |  |
            2 |  |  |M |  |  |
            3 |  |  |  |  |  |
            4 |  |  |  |  |  |
         */

        Assert.False(sheet.Merges.IsInsideMerge(2, 2));
        Assert.False(sheet.Merges.IsInsideMerge(2, 3));
    }

    [Test]
    public void Unmerge_Row()
    {
        var sheet = new Sheet(5, 5);
        sheet.TrySetCellValue(2, 2, "M");
        sheet.TrySetCellValue(1, 1, "U");

        sheet.Merges.Add(sheet.Range(2, 3, 2, 2));

        /*
                0  1  2  3  4
            0 |  |  |  |  |  |
            1 |  |U |  |  |  |
            2 |  |  |M |  |  |
            3 |  |  |M*|  |  |
            4 |  |  |  |  |  |
         */

        Assert.True(sheet.Merges.IsInsideMerge(2, 2));
        Assert.True(sheet.Merges.IsInsideMerge(3, 2));

        Assert.False(sheet.Merges.IsInsideMerge(0, 0));
        Assert.False(sheet.Merges.IsInsideMerge(4, 4));

        Assert.AreEqual("M", sheet.GetValue(2, 2));
        Assert.AreEqual(null, sheet.GetValue(3, 3));
        Assert.AreEqual("U", sheet.GetValue(1, 1));

        sheet.RemoveRow(3);
        /*
                0  1  2  3  4
            0 |  |  |  |  |  |
            1 |  |U |  |  |  |
            2 |  |  |M |  |  |
            3 |  |  |  |  |  |
         */

        Assert.False(sheet.Merges.IsInsideMerge(2, 2));
        Assert.False(sheet.Merges.IsInsideMerge(3, 2));
    }

    [Test]
    public void Insert_Row_Inside_Merged_Column_Expands_Merge()
    {
        // This case tests when an entire column is merged
        // and a row is inserted inside the merge. The behaviour
        // should be the same as when inserting inside a smaller range
        var sheet = new Sheet(3, 3);
        sheet.TrySetCellValue(0, 1, "M");
        sheet.Merges.Add(new ColumnRegion(1));
        Assert.AreEqual(sheet.GetValue(0, 1), "M");

        sheet.InsertRowAt(0);
        var mergeRegion = sheet.Merges.Get(0, 1);
        Assert.NotNull(mergeRegion);
        Assert.AreEqual(mergeRegion.GetType(), typeof(ColumnRegion));
    }

    [Test]
    public void Insert_Col_Inside_Merged_Row_Expands_Merge()
    {
        // This case tests when an entire row is merged
        // and a col is inserted inside the merge. The behaviour
        // should be the same as when inserting inside a smaller range
        var sheet = new Sheet(3, 3);
        sheet.TrySetCellValue(0, 1, "M");

        sheet.Merges.Add(new RowRegion(1));

        Assert.AreEqual(sheet.GetValue(0, 1), "M");

        sheet.InsertColAt(0);

        var mergeRowRegion = sheet.Merges.Get(1, 0);
        Assert.NotNull(mergeRowRegion);
        Assert.AreEqual(mergeRowRegion.GetType(), typeof(RowRegion));
    }

    [Test]
    public void Insert_Col_At_Merged_Column_Shift_Merge()
    {
        // This case tests when an entire row is merged
        // and a col is inserted inside the merge. The behaviour
        // should be the same as when inserting inside a smaller range
        var sheet = new Sheet(3, 3);
        sheet.TrySetCellValue(0, 1, "M");

        sheet.Merges.Add(new ColumnRegion(2));


        Assert.AreEqual(sheet.GetValue(0, 1), "M");

        sheet.InsertColAt(2);


        var mergeColumnRegion = sheet.Merges.Get(0, 3);
        Assert.NotNull(mergeColumnRegion);
        Assert.AreEqual(mergeColumnRegion.GetType(), typeof(ColumnRegion));
    }

    [Test]
    public void Insert_Row_At_Merged_Row_Shift_Merge()
    {
        // This case tests when an entire row is merged
        // and a col is inserted inside the merge. The behaviour
        // should be the same as when inserting inside a smaller range
        var sheet = new Sheet(3, 3);
        sheet.TrySetCellValue(0, 1, "M");

        sheet.Merges.Add(new RowRegion(1));

        Assert.AreEqual(sheet.GetValue(0, 1), "M");

        sheet.InsertRowAt(1);

        var mergeRowRegion = sheet.Merges.Get(2, 0);
        Assert.NotNull(mergeRowRegion);
        Assert.AreEqual(mergeRowRegion.GetType(), typeof(RowRegion));
    }

    [Test]
    public void Remove_Row_On_Single_Row_Height_Merge_Then_Undo_Works_Correctly()
    {
        // a single row merge should be removed when the row is removed.
        var sheet = new Sheet(10, 10);
        var merge = new Region(1, 1, 1, 2);
        sheet.Merges.Add(merge);
        Assert.True(sheet.Merges.IsInsideMerge(1, 1));

        // Remove the row and there shouldn't be any merges.
        sheet.RemoveRow(1);
        Assert.False(sheet.Merges.Any());

        // Undoing should bring the merged row back
        sheet.Commands.Undo();
        Assert.True(sheet.Merges.IsInsideMerge(1, 1));
    }

    [Test]
    public void Remove_Col_On_Single_Col_Width_Merge_Then_Undo_Works_Correctly()
    {
        // a single row merge should be removed when the row is removed.
        var sheet = new Sheet(10, 10);
        var merge = new Region(1, 2, 1, 1);
        sheet.Merges.Add(merge);
        Assert.True(sheet.Merges.IsInsideMerge(1, 1));

        // Remove the row and there shouldn't be any merges.
        sheet.RemoveCol(1);
        Assert.False(sheet.Merges.Any());

        // Undoing should bring the merged row back
        sheet.Commands.Undo();
        Assert.True(sheet.Merges.IsInsideMerge(1, 1));
    }

    [Test]
    public void Remove_Top_Row_Of_Merge_Then_Undo_Works_Correctly()
    {
        // This is an edge case where we have a merged and remove the top row of the merge.
        // When the removal is undone, the merge should be the same as before.
        var sheet = new Sheet(10, 10);
        sheet.Merges.Add(new Region(2, 5, 2, 3));
        sheet.Commands.ExecuteCommand(new RemoveRowsCommand(2));
        Assert.True(sheet.Merges.IsInsideMerge(2, 2));
        Assert.False(sheet.Merges.IsInsideMerge(5, 2));
        sheet.Commands.Undo();
        Assert.True(sheet.Merges.IsInsideMerge(2, 2));
        Assert.True(sheet.Merges.IsInsideMerge(5, 2));
    }
}