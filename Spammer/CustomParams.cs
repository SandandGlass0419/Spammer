using WindowsInput;
using WindowsInput.Native;

namespace Spammer;

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

public class UpArrowParam(string parameter) : CustomParam(parameter)
{
    public override void InvokeAction()
    {
        Simulator.Keyboard.KeyPress(VirtualKeyCode.UP);
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