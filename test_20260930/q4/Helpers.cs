using System;
using System.Collections.Generic;
using System.Text;

namespace test_20260930.q4;

public class Helpers
{
    public static int[] ContructSortedArray(SortedDictionary<int, int> data, int count)
    {
        int[] result = new int[count];
        int index = 0;
        Span<int> resultSpan = result;
        foreach ( var e in data )
        {
            if ( e.Value <= 0 )
            {
                continue;
            }
            resultSpan.Slice(index, e.Value).Fill(e.Key);
            index += e.Value;
        }

        return result;
    }
}
