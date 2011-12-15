using System;

namespace Tiamat
{
    internal class Program
    {
        private static void Main( /*string[] args*/)
        {
            var server = new Router.Router();
            server.Start();
            Console.ReadLine();
            server.Stop();
        }
    }
}