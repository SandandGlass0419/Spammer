using WindowsInput;
using WindowsInput.Native;

namespace Spammer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MySpammer tool = CreateSpammer(args);

            Console.WriteLine("Waiting 5 sec before starting.");
            Thread.Sleep(5000);

            tool.Run();
        }

        static MySpammer CreateSpammer(string[] args)
        {
            string filePath;
            int repeatSize;
            int sleepTime;

            if (args.Length != 3)
            {
                Console.WriteLine("Bad Argument count.");
                Console.WriteLine($"Format is \"{nameof(filePath)}\" \"{nameof(repeatSize)}\" \"{nameof(sleepTime)}\"");
                Environment.Exit(1);
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"File '{args[0]}' doesn't exist.");
                Environment.Exit(1);
            }

            filePath = args[0];

            if (!int.TryParse(args[1], out repeatSize))
            {
                Console.WriteLine($"'{args[1]}' isn't an int for argument \"{nameof(repeatSize)}\"");
            }

            if (!int.TryParse(args[2], out sleepTime))
            {
                Console.WriteLine($"'{args[2]}' isn't an int for argument \"{nameof(sleepTime)}\"");
            }

            return new(File.ReadAllText(filePath), repeatSize, sleepTime);
        }
    }

    public class MySpammer
    {
        public int repeatSize;
        public int sleepTime;

        public SpammerCore Core;

        public RandIntParam rintParam = new("%ri", 1, int.MaxValue);
        public RandFloatParam rfloatParam = new("%rf");

        public MySpammer(string text, int repeatSize, int sleepTime)
        {
            this.repeatSize = repeatSize;
            this.sleepTime = sleepTime;
            this.Core = new(text);
        }

        public void Run()
        {
            for (int current = 0; current < repeatSize; current++)
            {
                OutputText();

                Thread.Sleep(sleepTime);
            }
        }

        public void OutputText()
        {
            foreach (var entry in Core.Content)
            {
                Core.OutputEntry(entry, rintParam, rfloatParam);
            }
        }
    }

    public class RandIntParam : CustomParam
    {
        Random rng = new();
        public int minValue;
        public int maxValue;

        public RandIntParam(string parameter, int minValue, int maxValue) : base(parameter)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public override string Factory(int workingIndex)
        {
            return rng.Next(minValue, maxValue).ToString();
        }
    }

    public class RandFloatParam : CustomParam
    {
        Random rng = new();

        public RandFloatParam(string parameter) : base(parameter) {}

        public override string Factory(int workingIndex)
        {
            return rng.NextSingle().ToString();
        }
    }

    public class SpammerCore
    {
        public List<string[]> Content = [];

        InputSimulator Simulator = new();

        public SpammerCore(string text)
        {
            LoadText(text);
        }

        public void LoadText(string text)
        {
            foreach (string entry in text.Split('\n'))
            {
                Content.Add(entry.Split(@"%n"));
            }
        }

        public string[] InsertCustomText(string[] entry, CustomParam customParam)
        {
            List<string> result = new();

            int workingIndex = 0;
            foreach (var line in entry)
            {
                string[] splitLines = line.Split(customParam.parameter);

                if (splitLines.Length < 2)
                {
                    result.Add(splitLines.First());
                    continue;
                }

                string newline = splitLines.First();
                for (int i = 1; i < splitLines.Length; i++)
                {
                    newline += customParam.Factory(workingIndex) + splitLines[i];
                }

                result.Add(newline);
            }

            return result.ToArray();
        }

        public string[] ApplyParams(string[] entry, params CustomParam[] customParams)
        {
            string[] newentry = entry;

            foreach (var param in customParams)
            {
                newentry = InsertCustomText(newentry, param);
            }

            return newentry;
        }

        public void OutputEntry(string[] entry, params CustomParam[] customParams)
        {
            string[] newentry = ApplyParams(entry, customParams);

            foreach (var line in newentry)
            {
                if (!string.IsNullOrEmpty(line))
                { Simulator.Keyboard.TextEntry(line); }

                Simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.SHIFT, VirtualKeyCode.RETURN);
            }

            Simulator.Keyboard.KeyPress(VirtualKeyCode.BACK);
            Simulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
        }
    }

    public abstract class CustomParam
    {
        public string parameter;

        public CustomParam(string parameter)
        {
            this.parameter = parameter;
        }

        public abstract string Factory(int workingIndex);
    }
}
