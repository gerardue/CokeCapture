using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public static class ConvertToCSV
{
    public static void RecordData(string headerRow, string rowData)
    {
        // Define file path
        string filePath = Application.persistentDataPath + "/DBCokeGame.csv";

        // Check if file exists
        bool fileExists = File.Exists(filePath);

        // Create StreamWriter object
        StreamWriter writer = fileExists ? File.AppendText(filePath) : new StreamWriter(filePath);

        // Write header row if file doesn't exist
        if (!fileExists)
        {
            writer.WriteLine(headerRow);
        }
        
        writer.WriteLine(rowData);

        // Close StreamWriter object
        writer.Close();

        // Debug log
        Debug.Log("CSV file written to: " + filePath);
    }
}
