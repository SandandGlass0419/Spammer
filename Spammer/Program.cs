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

        public EndLineParam endlParam = new("%endl");
        public CtrlVParam ctrlvParam = new("%ctrlv");
        public RandIntParam rintParam = new("%randint");
        public RandFloatParam rfloatParam = new("%randfloat");
        public RandTimeParam rtimeParam = new("%randtime");
        public RandRgbParam rrgbParam = new("%randrgb");
        public RandGUIDParam rguidParam = new("%randguid");

        public MySpammer(string text, int repeatSize, int sleepTime)
        {
            this.repeatSize = repeatSize;
            this.sleepTime = sleepTime;
            this.Core = new(endlParam, ctrlvParam, rintParam, rfloatParam, rtimeParam, rrgbParam, rguidParam);
            Core.ParseContent(text);
        }

        public void Run()
        {
            for (int current = 0; current < repeatSize; current++)
            {
                Core.OutputContent();

                Thread.Sleep(sleepTime);
            }
        }
    }

    public class EndLineParam(string parameter) : CustomParam(parameter)
    {
        public override void InvokeAction()
        {
            Simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.SHIFT, VirtualKeyCode.RETURN);
        }
    }

    public class CtrlVParam(string parameter) : CustomParam(parameter)
    {
        public override void InvokeAction()
        {
            Simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
        }
    }

    public class RandIntParam : CustomParam
    {
        Random rng = new();
        public int minValue;
        public int maxValue;

        public RandIntParam(string parameter, int minValue = int.MinValue, int maxValue = int.MaxValue) : base(parameter)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public override void InvokeAction()
        {
            Simulator.Keyboard.TextEntry(rng.Next(minValue, maxValue).ToString());
        }
    }

    public class RandFloatParam(string parameter) : CustomParam(parameter)
    {
        Random rng = new();

        public override void InvokeAction()
        {
            Simulator.Keyboard.TextEntry(rng.NextSingle().ToString());
        }
    }

    public class RandTimeParam(string parameter) : CustomParam(parameter)
    {
        Random rng = new();

        public override void InvokeAction()
        {
            Simulator.Keyboard.TextEntry(new DateTime(rng.NextInt64(DateTime.MinValue.Ticks, DateTime.MaxValue.Ticks)).ToString());
        }
    }

    public class RandGUIDParam(string parameter) : CustomParam(parameter)
    {
        public override void InvokeAction()
        {
            Simulator.Keyboard.TextEntry(Guid.NewGuid().ToString());
        }
    }

    public class RandRgbParam(string parameter) : CustomParam(parameter)
    {
        Random rng = new();

        public override void InvokeAction()
        {
            Simulator.Keyboard.TextEntry($"#{rng.GetHexString(6)}");
        }
    }

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

    public abstract class CustomParam
    {
        public string Parameter
        {
            get;
            set => field = IsParam(value) ? value : indicator + value;
        }

        public const char indicator = '%';

        public InputSimulator Simulator = new();

        public CustomParam(string parameter)
        {
            this.Parameter = parameter;
        }

        public static bool IsParam(string entry) => entry.StartsWith(indicator);

        public abstract void InvokeAction();
    }
}