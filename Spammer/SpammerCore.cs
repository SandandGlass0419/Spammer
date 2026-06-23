using WindowsInput;
using WindowsInput.Native;

namespace Spammer;

public class SpammerCore
{
    public List<string[]> Content = new();

    InputSimulator Simulator = new();

    public CustomParam[] customParams;

    public SpammerCore(params CustomParam[] customParams)
    {
        this.customParams = customParams;
    }

    public void ParseContent(in string text)
    {
        string[] entries = text.Split('\n');

        foreach (var entry in entries)
        {
            Content.Add(ParseParams([entry]));
        }
    }

    public string[] ParseParams(string[] entry)
    {
        string[] newEntry = entry;

        foreach (var param in customParams)
        {
            newEntry = ParseParam(newEntry, param);
        }

        return newEntry;
    }

    public string[] ParseParam(string[] entry, CustomParam param)
    {
        List<string> parsed = new();

        foreach (var phrase in entry)
        {
            string[] splitPhrase = phrase.Split(param.Parameter);

            parsed.Add(splitPhrase[0]);

            if (splitPhrase.Length < 2) continue;

            for (int i = 1; i < splitPhrase.Length; i++)
            {
                parsed.Add(param.Parameter);
                parsed.Add(splitPhrase[i]);
            }
        }

        return parsed.ToArray();
    }

    public void OutputContent()
    {
        Content.ForEach(OutputEntry);
    }

    public void OutputEntry(string[] parsedEntry)
    {
        foreach (var phrase in parsedEntry)
        {
            if (String.IsNullOrEmpty(phrase)) continue;

            if (CustomParam.IsParam(phrase) && Contains(phrase, out var param))
            {
                param.InvokeAction();
            }

            else
            {
                Simulator.Keyboard.TextEntry(phrase);
            }
        }

        Simulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
    }

    private bool Contains(string phrase, out CustomParam? param)
    {
        param = customParams.FirstOrDefault(p => phrase == p.Parameter);

        return param != null;
    }
}