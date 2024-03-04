using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class TextFile
{
    public string FileName { get; set; }
    public string Content { get; set; }

    //беспараметрический конструктор для сериализации
    public TextFile()
    {
    }

    public TextFile(string fileName, string content)
    {
        FileName = fileName;
        Content = content;
    }

    public void SerializeToBinary(string filePath)
    {
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fileStream, this);
        }
    }

    public static TextFile DeserializeFromBinary(string filePath)
    {
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return (TextFile)formatter.Deserialize(fileStream);
        }
    }

    public void SerializeToXml(string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TextFile));
            serializer.Serialize(writer, this);
        }
    }

    public static TextFile DeserializeFromXml(string filePath)
    {
        using (StreamReader reader = new StreamReader(filePath))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TextFile));
            return (TextFile)serializer.Deserialize(reader);
        }
    }
}

public class TextFileSearcher
{
    public List<string> SearchFilesByKeyword(string directoryPath, string keyword)
    {
        List<string> foundFiles = new List<string>();
        string[] files = Directory.GetFiles(directoryPath, "*.txt", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            string content = File.ReadAllText(file);
            if (content.Contains(keyword))
            {
                foundFiles.Add(file);
            }
        }

        return foundFiles;
    }
}

public class TextEditor
{
    private string filePath;
    private string content;
    private Stack<string> history;

    public TextEditor(string filePath)
    {
        this.filePath = filePath;
        this.content = File.ReadAllText(filePath);
        this.history = new Stack<string>();
    }

    public void Save()
    {
        File.WriteAllText(filePath, content);
        history.Push(content);
    }

    public void Undo()
    {
        if (history.Count > 0)
        {
            content = history.Pop();
            File.WriteAllText(filePath, content);
        }
    }

    public void AppendText(string text)
    {
        content += text;
    }

    public void RemoveText(string text)
    {
        content = content.Replace(text, "");
    }

    public string GetContent()
    {
        return content;
    }
}

public class TextFileIndexer
{
    public Dictionary<string, List<string>> IndexFilesByKeywords(string directoryPath, List<string> keywords)
    {
        Dictionary<string, List<string>> index = new Dictionary<string, List<string>>();
        string[] files = Directory.GetFiles(directoryPath, "*.txt", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            string content = File.ReadAllText(file);
            foreach (string keyword in keywords)
            {
                if (content.Contains(keyword))
                {
                    if (!index.ContainsKey(keyword))
                    {
                        index[keyword] = new List<string>();
                    }
                    index[keyword].Add(file);
                }
            }
        }

        return index;
    }
}

class Program
{
    static void Main(string[] args)
    {
        string filePath = "example.txt";
        TextEditor textEditor = new TextEditor(filePath);
        textEditor.AppendText("Hello, world!");
        textEditor.Save();

        Console.WriteLine("Содержимое файла: " + textEditor.GetContent());

        textEditor.RemoveText("world");
        textEditor.Undo();

        Console.WriteLine("Содержимое файла после отмены: " + textEditor.GetContent());

        TextFile textFile = new TextFile("example.txt", "Hello, world!");
        textFile.SerializeToBinary("example.bin");
        TextFile deserializedTextFile = TextFile.DeserializeFromBinary("example.bin");
        Console.WriteLine("Десериализованное содержимое двоичного файла: " + deserializedTextFile.Content);

        textFile.SerializeToXml("example.xml");
        TextFile deserializedTextFileXml = TextFile.DeserializeFromXml("example.xml");
        Console.WriteLine("Десериализованное содержимое из XML: " + deserializedTextFileXml.Content);

        TextFileSearcher textFileSearcher = new TextFileSearcher();
        List<string> foundFiles = textFileSearcher.SearchFilesByKeyword(".", "Hello");
        Console.WriteLine("Файлы, содержащие 'Hello':");
        foreach (string foundFile in foundFiles)
        {
            Console.WriteLine(foundFile);
        }

        TextFileIndexer textFileIndexer = new TextFileIndexer();
        Dictionary<string, List<string>> index = textFileIndexer.IndexFilesByKeywords(".", new List<string> { "Hello" });
        Console.WriteLine("Индекс файлов по ключевому слову 'Hello':");
        foreach (var entry in index)
        {
            Console.WriteLine($"Ключевое слово: {entry.Key}");
            foreach (var file in entry.Value)
            {
                Console.WriteLine($" - {file}");
            }
        }
    }
}
