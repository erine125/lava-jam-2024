using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dish
{

    // Storage \\

    public string filename;
    public string flavorText;
    public Course course;
    public string[] ingredients;
    public string displayName;

    public Sprite sprite;


    // Exposed \\

    public Dish(string filename, string flavorText, Course course, string[] ingredients)
    {
        this.filename = filename;
        this.flavorText = flavorText;
        this.course = course;
        this.ingredients = ingredients;

        for (int i = 0; i < filename.Length; i++)
        {
            if (i == 0 || filename[i-1] == '-')
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

    public static Dish CreateFromLine(string line)
    {
        string[] entries = line.Split('\t');
        for (int i = 0; i < entries.Length; i++)
        {
            entries[i] = entries[i].Trim();
        }

        Course course = Course.DESSERT;
        if (entries[2] == "appetizer")
        {
            course = Course.APPETIZER;
        }
        else if (entries[2] == "main-course")
        {
            course = Course.MAIN_COURSE;
        }

        List<string> ings = new List<string>();
        for (int i = 3; i < entries.Length; i++)
        {
            if (entries[i].Trim() != "")
            {
                ings.Add(entries[i].Trim());
            }
        }

        return new Dish(entries[0], entries[1], course, ings.ToArray());
    }


    // Structure \\

    public enum Course
    {
        APPETIZER = 1,
        MAIN_COURSE,
        DESSERT
    }

}
