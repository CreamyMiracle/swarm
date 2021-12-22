using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(63, 63);
            Environment env = new Environment(100, 100, 100, 30);
            env.Run().Wait();
        }
    }
}
