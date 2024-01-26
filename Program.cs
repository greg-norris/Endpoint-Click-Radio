using EndpointClickRadio;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using GHIElectronics.Endpoint.Devices.Display;
using SkiaSharp;
using GHIElectronics.Endpoint.Core;

var backlightPort = EPM815.Gpio.Pin.PD14 / 16;
var backlightPin = EPM815.Gpio.Pin.PD14 % 16;

var gpioDriver = new LibGpiodDriver((int)backlightPort);
var gpioController = new GpioController(PinNumberingScheme.Logical, gpioDriver);
gpioController.OpenPin(backlightPin);
gpioController.SetPinMode(backlightPin, PinMode.Output);
gpioController.Write(backlightPin, PinValue.High);

var screenWidth = 480;
var screenHeight = 272;
SKBitmap bitmap = new SKBitmap(screenWidth, screenHeight, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
bitmap.Erase(SKColors.Transparent);

var configuration = new FBDisplay.ParallelConfiguration()
{
    Clock = 10000,
    Width = 480,
    Hsync_start = 480 + 2,
    Hsync_end = 480 + 2 + 41,
    Htotal = 480 + 2 + 41 + 2,
    Height = 272,
    Vsync_start = 272 + 2,
    Vsync_end = 272 + 2 + 10,
    Vtotal = 272 + 2 + 10 + 2,

};
var fbDisplay = new FBDisplay(configuration);
var displayController = new DisplayController(fbDisplay);

var reset = EPM815.Gpio.Pin.PF4;
var cs = EPM815.Gpio.Pin.PA14;

FM_Click radio = new FM_Click(reset, cs);
double currentStation = 94.7;
int volume = 255;
radio.Channel = currentStation;
radio.Volume = volume;


while (true) { 
using (var screen = new SKCanvas(bitmap))
{
    //Create Black Screen 
    screen.DrawColor(SKColors.Black);
    screen.Clear(SKColors.Black);

        // Draw text
        using (SKPaint text = new SKPaint())
        {
            //text.Color = SKColors.CornflowerBlue;
            text.Color = SKColor.Parse("#FF0977aa");
            text.IsAntialias = true;
            text.StrokeWidth = 2;
            text.Style = SKPaintStyle.Stroke;
            text.TextSize = 50;
            screen.DrawText("FM Radio", 10, 10, text);
        }

        // Flush to screen
        var data = bitmap.Copy(SKColorType.Rgb565).Bytes;
    displayController.Flush(data);
    Thread.Sleep(1);
}
}