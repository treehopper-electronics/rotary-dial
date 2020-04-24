using SimWinInput;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Treehopper;
using Treehopper.Libraries.Displays;
using Treehopper.Libraries.Input;


namespace rotary_dial
{
    public class RotaryDial
    {
[DllImport("user32.dll")]
public static extern void mouse_event(uint flags, int x, int y, int data, int extraInfo);

        public async Task Run()
        {
            var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();
            var driver = new Apa102(board.Spi, 32);
            driver.AutoFlush = false;
            var encoder = new RotaryEncoder(board.Pins[13], board.Pins[12], 4);
            long oldPosition = 0;
            encoder.PositionChanged += async (s, e) =>
            {
                var encoderAngle = 360 * e.NewPosition / 20f; // 20 clicks per rotation
                for (int i = 0; i < driver.Leds.Count; i++)
                {
                    driver.Leds[i].SetHsl((360f / driver.Leds.Count) * i + encoderAngle, 100, 5);
                }

                await driver.FlushAsync();
                
                if (e.NewPosition > oldPosition)
                {
                    mouse_event(0x0800, 0, 0, -120, 0);
                }

                if (e.NewPosition < oldPosition)
                {
                    mouse_event(0x0800, 0, 0, 120, 0);
                }

                oldPosition = e.NewPosition;
            };
        }
    }
}
