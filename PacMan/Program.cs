using System;


namespace Setnicka.PacMan
{
    class Program
    {
        static void Main(string[] args)
        {
            AppManager appManager = new AppManager();
            appManager.Run();

            // TODO: Delete
            Console.Clear();
            Console.WriteLine("I'm back at program!");
            Console.ReadKey(true);
        }
    }
}
