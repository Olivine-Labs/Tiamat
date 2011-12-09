using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Router;

namespace Router
{
    class Program
    {
        static void Main(string[] args)
        {
            Router server = new Router();
            server.Start();
            Console.ReadLine();
            server.Stop();
        }
    }
}
