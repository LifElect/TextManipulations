using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

// Класс текстового файла с возможностью бинарной и XML сериализации/десериализации
[Serializable]
public class TextFile
{
    public string FilePath { get; set; }
    public string Content { get; set; }

    // Бинарная сериализация в файл
    public void BinarySerialize(string fileName)
    {
        FileStream fileStream = new FileStream(fileName, FileMode.Create);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, this);
        fileStream.Close();
    }

    // Бинарная десериализация из файла
    public static TextFile BinaryDeserialize(string fileName)
    {
        FileStream fileStream = new FileStream(fileName, FileMode.Open);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        TextFile textFile = (TextFile)binaryFormatter.Deserialize(fileStream);
        fileStream.Close();
        return textFile;
    }

    // XML сериализация в файл
    public void XmlSerialize(string fileName)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(TextFile));
        StreamWriter streamWriter = new StreamWriter(fileName);
        xmlSerializer.Serialize(streamWriter, this);
        streamWriter.Close();
    }

    // XML десериализация из файла
    public static TextFile XmlDeserialize(string fileName)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(TextFile));
        StreamReader streamReader = new StreamReader(fileName);
        TextFile textFile = (TextFile)xmlSerializer.Deserialize(streamReader);
        streamReader.Close();
        return textFile;
    }
}

// Класс для поиска текстовых файлов по ключевым словам
public class TextFileSearcher
{
    public List<string> SearchFiles(string directory, string[] keywords)
    {
        List<string> result = new List<string>();
        foreach (string file in Directory.GetFiles(directory, "*.txt"))
        {
            string content = File.ReadAllText(file);
            if (keywords.All(keyword => content.Contains(keyword)))
            {
                result.Add(file);
            }
        }
        return result;
    }
}

// Класс, реализующий паттерн "Memento"
public class Originator
{
    private string state;

    public string State
    {
        get { return state; }
        set
        {
            state = value;
            Console.WriteLine("Состояние изменено на: " + state);
        }
    }

    public Memento Save()
    {
        return new Memento(state);
    }

    public void Restore(Memento memento)
    {
        state = memento.GetState();
        Console.WriteLine("Восстановлено состояние: " + state);
    }
}

public class Memento
{
    private readonly string state;

    public Memento(string stateToSave)
    {
        state = stateToSave;
    }

    public string GetState()
    {
        return state;
    }
}

public class Caretaker
{
    private Stack<Memento> mementos = new Stack<Memento>();

    public void SaveState(Memento memento)
    {
        mementos.Push(memento);
    }

    public Memento RestoreState()
    {
        return mementos.Pop();
    }
}

// Консольный редактор текстовых файлов
public class TextFileEditor
{
    private Originator originator;
    private Caretaker caretaker;

    public TextFileEditor()
    {
        originator = new Originator();
        caretaker = new Caretaker();
    }

    public void CreateNewFile(string filePath)
    {
        File.Create(filePath).Close();
        Console.WriteLine("Создан новый файл: " + filePath);
        originator.State = File.ReadAllText(filePath);
        caretaker.SaveState(originator.Save());
    }

    public void OpenFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Файл не существует!");
        }
        else
        {
            Console.WriteLine(File.ReadAllText(filePath));
            originator.State = File.ReadAllText(filePath);
            caretaker.SaveState(originator.Save());
        }
    }

    public void SaveFile(string filePath, string content)
    {
        File.WriteAllText(filePath, content);
        Console.WriteLine("Файл сохранен.");
        originator.State = File.ReadAllText(filePath);
        caretaker.SaveState(originator.Save());
    }

    public void Undo()
    {
        if (caretaker.RestoreState() is Memento memento)
        {
            originator.Restore(memento);
        }
    }
}

// Приложение для индексации текстовых файлов
public class FileIndexer
{
    public Dictionary<string, List<string>> IndexFiles(string directory, string[] keywords)
    {
        Dictionary<string, List<string>> index = new Dictionary<string, List<string>>();

        foreach (string file in Directory.GetFiles(directory, "*.txt"))
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

// Консольное приложение
class Program
{
    static void Main(string[] args)
    {
        TextFileEditor editor = new TextFileEditor();


        while (true)
        {
            Console.Write("Выберите действие (1-4): ");
        Console.WriteLine("Привет! Что вы хотите сделать?");
        Console.WriteLine("1. Создать новый файл");
        Console.WriteLine("2. Открыть существующий файл");
        Console.WriteLine("3. Сохранить файл");
        Console.WriteLine("4. Отменить последнее действие");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Console.Write("Введите путь и имя нового файла: ");
                    string newFilePath = Console.ReadLine();
                    editor.CreateNewFile(newFilePath);
                    break;
                case "2":
                    Console.Write("Введите путь к файлу: ");
                    string openFilePath = Console.ReadLine();
                    editor.OpenFile(openFilePath);
                    break;
                case "3":
                    Console.Write("Введите путь к файлу: ");
                    string saveFilePath = Console.ReadLine();
                    Console.Write("Введите содержимое файла: ");
                    string content = Console.ReadLine();
                    editor.SaveFile(saveFilePath, content);
                    break;
                case "4":
                    editor.Undo();
                    break;
                default:
                    Console.WriteLine("Некорректный ввод. Попробуйте еще раз.");
                    break;
            }
        Console.ReadKey();
        Console.Clear();
        }
    }
}