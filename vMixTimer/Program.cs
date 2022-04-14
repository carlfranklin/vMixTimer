// See https://aka.ms/new-console-template for more information
Console.WriteLine("vMix Timer by Carl Franklin");
Console.WriteLine("Kicks off a vMix event at a specified time.");
Console.WriteLine("https://github.com/carlfranklin/vmixtimer");
Console.WriteLine("----------------------------------------------");
Console.WriteLine("");
Console.WriteLine($"The current time is {DateTime.Now.ToShortTimeString()}");
Console.WriteLine("Enter start time in 24-hour format (HH:MM:SS):");

var startTimeString = "";
DateTime startTime = DateTime.Now;

bool valid = false;
while (!valid)
{
    startTimeString = Console.ReadLine();
    try
    {
        startTime = DateTime.Parse(startTimeString);
        valid = true;
    }
    catch(Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

Console.WriteLine($"Streaming will start at {startTime.ToLongTimeString()}");
Console.WriteLine("Enter desired duration of the stream in HH:MM::SS format:");

valid = false;
var durationString = "";
TimeSpan duration = TimeSpan.FromSeconds(0);
DateTime endTime = DateTime.Now;
while (!valid)
{
    durationString = Console.ReadLine();
    try
    {
        duration = TimeSpan.Parse(durationString);
        endTime = startTime.Add(duration);
        valid = true;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}


Console.WriteLine($"Streaming will start at {startTime.ToLongTimeString()}");
Console.WriteLine($"Streaming will end at {endTime.ToLongTimeString()}");

Console.WriteLine("Cue up the input in the orange frame on the left, and press ENTER when ready to start the countdown.");
Console.ReadLine();
var pos = Console.GetCursorPosition();
var top = pos.Top;

while(true)
{
    if (DateTime.Now >= startTime)
        break;

    // wait one second
    Task.Delay(1000).GetAwaiter().GetResult();
    // display the countdown
    var timeleft = startTime - DateTime.Now;
    Console.SetCursorPosition(0, top);
    Console.Write($"Stream will start in {Convert.ToInt32(Math.Abs(timeleft.TotalSeconds))} seconds    ");
}
Console.WriteLine();

string baseUrl = "http://127.0.0.1:8088/API/";
var startStreamingUrl = baseUrl + "?Function=StartStreaming";
var stopStreamingUrl = baseUrl + "?Function=StopStreaming";
var cutUrl = baseUrl + "?Function=Cut";

// start streaming
SendCommand(startStreamingUrl).GetAwaiter().GetResult();
// wait 3 seconds
Task.Delay(3000).GetAwaiter().GetResult();
// cut to input
SendCommand(cutUrl).GetAwaiter().GetResult();
// wait 3 seconds
Task.Delay(3000).GetAwaiter().GetResult();

Console.WriteLine("Streaming");
pos = Console.GetCursorPosition();
top = pos.Top;

while (true)
{
    if (DateTime.Now >= endTime)
        break;

    // wait one second
    Task.Delay(1000).GetAwaiter().GetResult();
    // display the countdown
    var timeleft = endTime - DateTime.Now;
    Console.SetCursorPosition(0, top);
    Console.Write($"Stream will start in {Convert.ToInt32(Math.Abs(timeleft.TotalSeconds))} seconds    ");
}
Console.WriteLine();

// stop streaming
SendCommand(stopStreamingUrl).GetAwaiter().GetResult();
// wait one second
Task.Delay(1000).GetAwaiter().GetResult();
Console.WriteLine("Streaming has stopped.");

return;


async Task SendCommand(string url)
{
    using (var client = new HttpClient())
    {
        try
        {
            await client.GetStringAsync(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    await Task.Delay(100);
}