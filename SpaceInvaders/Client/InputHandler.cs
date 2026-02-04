using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class InputHandler
    {
        public static ConsoleKey? ReadInput()
        {
            if(Console.KeyAvailable)
            {
                return Console.ReadKey(true).Key;
            }
            return null;
        }
    }
}
