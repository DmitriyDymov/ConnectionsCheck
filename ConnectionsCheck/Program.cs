//------------------------------------------------------------------------------------
// Author: Dmitriy Dymov
// Search classes for non-closed database connections
//------------------------------------------------------------------------------------
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");

string? dir;

bool everythingIsFine = true;

if (args.Length == 0)
{
    Console.WriteLine("Required parameters not specified");
    Console.WriteLine("Enter the directory you want to search in");
    dir = Console.ReadLine();
    Console.WriteLine("---------------------------------------------");
}
else
{
    dir = args[0];
}


if (string.IsNullOrEmpty(dir))
{
    Console.WriteLine("Dir must have a value");
    Console.WriteLine("---------------------------------------------");
    return;
}

if (!Directory.Exists(dir))
{
    Console.WriteLine("Directory does not exist");
    Console.WriteLine("---------------------------------------------");
    return;
}

SearchConnections(dir);

if (everythingIsFine)
{
    Console.WriteLine("Everything is fine");
    Console.WriteLine("---------------------------------------------");
}

void SearchConnections(string folderPath)
{
    var files = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

    foreach (var file in files)
    {
        AnalyzeFile(file);
    }
}

void AnalyzeFile(string filePath)
{
    try
    {
        var lines = File.ReadAllLines(filePath);
        var openCount = 0;
        var closeCount = 0;

        foreach (var l in lines)
        {
            var openMatch = Regex.Match(l, @"\DbConnect\.Get");
            if (openMatch.Success)
            {
                openCount++;
            }

            var closeMatch = Regex.Match(l, @"\.Close\(\)");
            if (closeMatch.Success)
            {
                closeCount++;
            }
        }

        // Проверка на равенство количества открытых и закрытых соединений
        if (openCount == closeCount) return;

        everythingIsFine = false;

        Console.WriteLine($"Файл: {filePath}");
        Console.WriteLine($"Несовпадение в методе: {GetMethodName(filePath)}");
        Console.WriteLine($"Открытых соединений: {openCount}, Закрытых соединений: {closeCount}");
        Console.WriteLine("---------------------------------------------");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при анализе файла {filePath}: {ex.Message}");
    }
}

static string GetMethodName(string filePath)
{
    // Пример: Извлекаем имя метода из пути файла
    var parts = filePath.Split(Path.DirectorySeparatorChar);
    var fileName = parts[^1];
    var methodName = fileName.Split('.')[0]; // Предполагаем, что имя файла = имя метода
    return methodName;
}

