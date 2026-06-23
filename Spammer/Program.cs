namespace Spammer;

internal class Program
{
    static void Main(string[] args)
    {
        Spammer tool = CreateSpammer(args);

        Console.WriteLine("Waiting 5 sec before starting.");
        Thread.Sleep(5000);

        tool.Run();
    }

    static Spammer CreateSpammer(string[] args)
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

public class Spammer
{
    public int repeatSize;
    public int sleepTime;

    public SpammerCore Core;

    public EndLineParam endlParam = new("%endl");
    public CtrlVParam ctrlvParam = new("%ctrlv");
    public UpArrowParam upParam = new("%up");
    public RandIntParam rintParam = new("%rint");
    public RandFloatParam rfloatParam = new("%rfloat");
    public RandTimeParam rtimeParam = new("%rtime");
    public RandRgbParam rrgbParam = new("%rrgb");
    public RandGUIDParam rguidParam = new("%rguid");

    public Spammer(string text, int repeatSize, int sleepTime)
    {
        this.repeatSize = repeatSize;
        this.sleepTime = sleepTime;
        this.Core = new(endlParam, ctrlvParam, upParam, rintParam, rfloatParam, rtimeParam, rrgbParam, rguidParam);
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