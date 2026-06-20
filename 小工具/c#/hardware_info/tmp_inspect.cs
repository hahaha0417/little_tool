using System;
using LibreHardwareMonitor.Hardware;

var computer = new Computer
{
    IsCpuEnabled = true
};
computer.Open();

var visitor = new UpdateVisitor();
computer.Accept(visitor);
System.Threading.Thread.Sleep(1000);
computer.Accept(visitor);

foreach (var hardware in computer.Hardware)
{
    Console.WriteLine($"Hardware: {hardware.HardwareType} / {hardware.Name}");
    Dump(hardware, 1);
}

computer.Close();

static void Dump(IHardware hardware, int level)
{
    var pad = new string(' ', level * 2);
    foreach (var sensor in hardware.Sensors)
    {
        Console.WriteLine($"{pad}Sensor: Type={sensor.SensorType}, Name={sensor.Name}, Value={sensor.Value}");
    }

    foreach (var sub in hardware.SubHardware)
    {
        sub.Update();
        Console.WriteLine($"{pad}SubHardware: {sub.HardwareType} / {sub.Name}");
        Dump(sub, level + 1);
    }
}

sealed class UpdateVisitor : IVisitor
{
    public void VisitComputer(IComputer computer) => computer.Traverse(this);
    public void VisitHardware(IHardware hardware)
    {
        hardware.Update();
        foreach (var sub in hardware.SubHardware)
        {
            sub.Accept(this);
        }
    }
    public void VisitSensor(ISensor sensor) { }
    public void VisitParameter(IParameter parameter) { }
}
