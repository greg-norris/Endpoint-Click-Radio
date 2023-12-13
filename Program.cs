using EndpointClickRadio;
using GHIElectronics.Endpoint.Pins;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using GHIElectronics.Endpoint.Devices.Display;
using SkiaSharp;

var gpioDriver = new LibGpiodDriver((int)STM32MP1.Port.D);
var gpioController = new GpioController(PinNumberingScheme.Logical, gpioDriver);
gpioController.OpenPin(14, PinMode.Output);
gpioController.Write(14, PinValue.High); // low is on

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



var reset = STM32MP1.GpioPin.PF4;
var cs = STM32MP1.GpioPin.PA14;
var i2cbus = STM32MP1.I2c.I2c4.ToString();

FM_Click radio = new FM_Click(reset, cs);
double currentStation = 94.7;
int volume = 255;
radio.Channel = currentStation;
radio.Volume = volume;

using (var screen = new SKCanvas(bitmap))
{
    //Create Black Screen 
    screen.DrawColor(SKColors.Black);
    screen.Clear(SKColors.Black);

    // Draw text
    using (SKPaint text = new SKPaint())
    {
        text.Color = SKColors.Yellow;
        text.IsAntialias = true;
        text.StrokeWidth = 2;
        text.Style = SKPaintStyle.Stroke;

        //SKFont Text - 
        SKFont font = new SKFont();
        font.Size = 22;
        font.ScaleX = 2;
        SKTextBlob textBlob = SKTextBlob.Create(currentStation.ToString(), font);
        screen.DrawText(textBlob, 50, 100, text);
    }
    // Flush to screen
    var data = bitmap.Copy(SKColorType.Rgb565).Bytes;
    displayController.Flush(data);
    Thread.Sleep(1);
}