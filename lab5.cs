using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string directoryPath = "путь к директории";
        string errorWordsFilePath = "путь к файлу словарю";

        Dictionary<string, string> errorWords = LoadErrorWords(errorWordsFilePath);

        if (errorWords == null)
        {
            Console.WriteLine("Не удалось загрузить словарь ошибочных слов.");
            return;
        }

        try
        {
            foreach (string filePath in Directory.GetFiles(directoryPath))
            {
                string text = File.ReadAllText(filePath);

                foreach (var errorWord in errorWords)
                {
                    text = text.Replace(errorWord.Key, errorWord.Value);
                }

                text = Regex.Replace(text, @"\(\d{3}\) \d{3}-\d{2}-\d{2}", "+380 12 345 67 89");

                File.WriteAllText(filePath, text);
            }

            Console.WriteLine("Обработка завершена.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    static Dictionary<string, string> LoadErrorWords(string filePath)
    {
        try
        {
            Dictionary<string, string> errorWords = new Dictionary<string, string>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split('-');
                    if (parts.Length >= 2)
                    {
                        errorWords[parts[0]] = parts[1];
                    }
                }
            }

            return errorWords;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
