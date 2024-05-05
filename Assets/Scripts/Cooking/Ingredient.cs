using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Dish;

public class Ingredient
{
    // Storage \\

    public string filename;
    public string flavorText;
    public string displayName;

    public Sprite sprite;


    // Exposed \\

    public Ingredient(string filename, string flavorText)
    {
        this.filename = filename;
        this.flavorText = flavorText;

        for (int i = 0; i < filename.Length; i++)
        {
            if (i == 0 || filename[i - 1] == '-')
            {
                displayName += filename[i].ToString().ToUpper();
            }
            else if (filename[i] == '-')
            {
                displayName += " ";
            }
            else
            {
                displayName += filename[i];
            }
        }
    }

    public static Ingredient CreateFromLine(string line)
    {
        string[] entries = line.Split('\t');
        for (int i = 0; i < entries.Length; i++)
        {
            entries[i] = entries[i].Trim();
        }
        return new Ingredient(entries[0], entries[1]);
    }
}
