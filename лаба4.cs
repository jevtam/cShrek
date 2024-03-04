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

    private Caretaker caretaker;

    public TextFile()
    {
        caretaker = new Caretaker();
    }

    public TextFile(string fileName, string content)
    {
        FileName = fileName;
        Content = content;
        caretaker = new Caretaker();
    }

    public TextFileMemento CreateMemento()
    {
        return new TextFileMemento(FileName, Content);
    }

    public void SetMemento(TextFileMemento memento)
    {
        FileName = memento.FileName;
        Content = memento.Content;
    }

    public void SaveState()
    {
        caretaker.SaveState(CreateMemento());
    }

    public void Undo()
    {
        TextFileMemento memento = caretaker.RestoreState();
        if (memento != null)
        {
            SetMemento(memento);
        }
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

[Serializable]
public class Caretaker
{
    private Stack<TextFileMemento> history;

    public Caretaker()
    {
        history = new Stack<TextFileMemento>();
    }

    public void SaveState(TextFileMemento memento)
    {
        history.Push(memento);
    }

    public TextFileMemento RestoreState()
    {
        if (history.Count > 0)
        {
            return history.Pop();
        }
        else
        {
            return null;
        }
    }
}

[Serializable]
public class TextFileMemento
{
    public string FileName { get; }
    public string Content { get; }

    public TextFileMemento(string fileName, string content)
    {
        FileName = fileName;
        Content = content;
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
    private TextFile textFile;

    public TextEditor(string filePath)
    {
        textFile = new TextFile(filePath, File.ReadAllText(filePath));
    }

    public void Save()
    {
        textFile.SaveState();
        File.WriteAllText(textFile.FileName, textFile.Content);
    }

    public void Undo()
    {
        textFile.Undo();
        File.WriteAllText(textFile.FileName, textFile.Content);
    }

    public void AppendText(string text)
    {
        textFile.Content += text;
    }

    public void RemoveText(string text)
    {
        textFile.Content = textFile.Content.Replace(text, "");
    }

    public string GetContent()
    {
        return textFile.Content;
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
