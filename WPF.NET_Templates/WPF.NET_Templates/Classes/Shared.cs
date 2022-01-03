using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF.NET_Templates.Classes
{
    public class Shared
    {
        public static StartWindow_with_pinPanels startWindow_With_PinPanels;
        public static bool logRegWindowsClosed = true;


        // delay execution: https://stackoverflow.com/questions/5904636/best-way-to-create-a-run-once-time-delayed-function-in-c-sharp
        public static async Task Delay(Action action, int timeoutInMilliseconds)
        {
            await Task.Delay(timeoutInMilliseconds);
            action();
        }

    }

}
