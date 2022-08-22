using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotNetCoreZhHans.Service.FileHandlers.FileActuators;
internal class XmlCollate
{
    private const StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
    private readonly StringBuilder writerBuffer = new();
    private readonly StringBuilder xmlElement = new();
    private readonly StreamWriter textWriter;

    delegate void CharHandler(ref char value);
    private CharHandler charHandler;

    public XmlCollate(string sourcePath, string targetPath)
    {
        charHandler = FirstHandler;
        textWriter = new(targetPath);
        Run(sourcePath);
        textWriter.Dispose();
    }

    private void Run(string path)
    {
        foreach (var item in GetChars(path))
        {
            var value = item;
            charHandler(ref value);
        }
    }

    private static IEnumerable<char> GetChars(string path)
    {
        using var stream = new StreamReader(path);
        var buffer = new char[8192];
        int readCount;
        while ((readCount = stream.Read(buffer)) > 0)
        {
            for (int i = 0; i < readCount; i++)
            {
                yield return buffer[i];
            }
        }
    }

    private void FirstHandler(ref char value)
    {
        if (value is not '<') return;
        (charHandler = ElementHander)(ref value);
    }

    void ElementHander(ref char value)
    {
        xmlElement.Append(value);
        if (value is not '>') return;
        SetEnd();
        charHandler = DefaultHander;
    }

    private void SetEnd()
    {
        var value = xmlElement.ToString()
            .Replace("<br>", "<br/>", ignoreCase);

        if (value.StartsWith("<member"))
            value = value.Replace("&,", "@,").Replace("&)", "@)");

        xmlElement.Clear();

        if (!IsP(value)) writerBuffer.Append(value);
        textWriter.Write(writerBuffer);
        writerBuffer.Clear();
    }

    private static bool IsP(string value)
    {
        return value.IndexOf("<p ", ignoreCase) > -1 ||
            value.IndexOf("<p>", ignoreCase) > -1 ||
            value.IndexOf("</p>", ignoreCase) > -1;
    }

    void DefaultHander(ref char value)
    {
        if (value is '<') (charHandler = ElementHander)(ref value);
        else writerBuffer.Append(value);
    }
}
