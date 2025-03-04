using System.Collections.Generic;
using UnityEngine;

public class StatisticsManager
{
    /// <summary>
    /// Manages statistics and renders graphs based on user data. 
    /// It calculates scaled values and adjusts line renderers for 
    /// different statistical graphs.
    /// </summary>
    public void SetSGLines(User user, ref LineRendererHUD line1, ref LineRendererHUD line2, ref LineRendererHUD line3, ref UIGridRenderer grid)
    {
        // Determine the min and max across all SG data sets for proper scaling
        float min = Mathf.Min(FindMinFromList(user.SGValues1), FindMinFromList(user.SGValues2), FindMinFromList(user.SGValues3));
        float max = Mathf.Max(FindMaxFromList(user.SGValues1), FindMaxFromList(user.SGValues2), FindMaxFromList(user.SGValues3));

        // Normalize and assign values to each line renderer
        SetLineValues(AlignValues(user.SGValues1, min, max), ref line1);
        SetLineValues(AlignValues(user.SGValues2, min, max), ref line2);
        SetLineValues(AlignValues(user.SGValues3, min, max), ref line3);
        // Adjust grid size to accommodate the longest data set
        grid.SetGridSize(new Vector2Int(Mathf.Max(user.SGValues1.Count, Mathf.Max(user.SGValues2.Count, user.SGValues3.Count)), 0));
    }

    /// <summary>
    /// Manages statistics and renders graphs based on user data. 
    /// It calculates scaled values and adjusts line renderers for 
    /// different statistical graphs.
    /// </summary>
    public void SetKGLines(User user, ref LineRendererHUD line1, ref LineRendererHUD line2, ref UIGridRenderer grid)
    {

        List<float> panValues = GenerateValuesForKGStat(user.FGTime, user.FGEggs, FindAverageInList(user.FGTime));
        List<float> waterValues = GenerateValuesForKGStat(user.PGTime, user.PGGlass, FindAverageInList(user.PGTime));

        float min = Mathf.Min(FindMinFromList(panValues), FindMinFromList(waterValues));
        float max = Mathf.Max(FindMaxFromList(panValues), FindMaxFromList(waterValues));

        SetLineValues(AlignValues(panValues, min, max), ref line1);
        SetLineValues(AlignValues(waterValues, min, max), ref line2);
        grid.SetGridSize(new Vector2Int(Mathf.Max(panValues.Count, waterValues.Count), 0));
    }

    /// <summary>
    /// Generates values for kitchen game statistics.
    /// </summary>
    private List<float> GenerateValuesForKGStat(List<float> time, List<float> val, float average)
    {
        List<float> values = new();
        for (int i = 0; i < val.Count; i++)
            values.Add(val[i] * (1 / time[i]));
        return values;
    }

    /// <summary>
    /// Calculates the average of a given list.
    /// </summary>
    private float FindAverageInList(List<float> list)
    {
        float sum = 0f;
        foreach (float num in list)
            sum += num;
        return sum / list.Count;
    }

    /// <summary>
    /// Sets the Points of a line renderer based on the input list of values.
    /// </summary>
    private void SetLineValues(List<float> values, ref LineRendererHUD line)
    {
        line.Points.Clear();
        line.Points.Add(new Vector2(0, 0));
        for (int i = 0; i < values.Count; i++)
            line.Points.Add(new Vector2(i + 1, values[i]));
    }

    /// <summary>
    /// Finds the minimum value from a list, returning a high default if the list is empty.
    /// </summary>
    private float FindMinFromList(List<float> list)
    {
        if (list.Count == 0)
            return float.MaxValue;
        else
            return Mathf.Min(list.ToArray());
    }

    /// <summary>
    /// Finds the maximum value from a list, returning a low default if the list is empty.
    /// </summary>
    private float FindMaxFromList(List<float> list)
    {
        if (list.Count == 0)
            return float.MinValue;
        else
            return Mathf.Max(list.ToArray());
    }

    /// <summary>
    /// Aligns and scales values from a list into a specified target range (1 to 11).
    /// </summary>
    private List<float> AlignValues(List<float> originalValues, float minValue, float maxValue)
    {
        List<float> alignedValues = new();
        float range = maxValue - minValue;
        float targetRange = 11f - 1f; // Target range from 1 to 11

        // Handle cases based on the number of items in the original list
        switch (originalValues.Count)
        {
            case 0:
                // If no values exist, return an empty list
                return originalValues;
            case 1:
                // Single value: center within the target range
                if (minValue == maxValue)
                    alignedValues.Add(targetRange / 2);
                else
                {
                    float scaledValue = (originalValues[0] - minValue) / range;
                    float alignedValue = scaledValue * targetRange + 1f;
                    alignedValues.Add(alignedValue);
                }
                return alignedValues;

            default:
                // Multiple values: scale each value within the target range
                foreach (float value in originalValues)
                {
                    float scaledValue = (value - minValue) / range;
                    float alignedValue = scaledValue * targetRange + 1f;
                    alignedValues.Add(alignedValue);
                }
                return alignedValues;
        }
    }
}
