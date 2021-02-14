using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setnicka.UI
{
    public static class TestRun
    {
        public static void Run()
        {
            Menu menu = new Menu();
            menu.AddUIElement(new Lable("Menu 1", HorizontalAllignment.Center, 0, ConsoleColor.Magenta, ConsoleColor.Black));
            menu.AddUIElement(new Lable("Welcome to my menu", HorizontalAllignment.Center, 0, ConsoleColor.Yellow, ConsoleColor.Black));
            menu.AddUIElement(new Button("Button 1", HorizontalAllignment.Center, 0, ConsoleColor.Red, ConsoleColor.Green));
            menu.AddUIElement(new Button("Button 2", HorizontalAllignment.Center, 0, ConsoleColor.Red, ConsoleColor.Green));
            menu.AddUIElement(new Lable("", HorizontalAllignment.Center, 0));
            menu.AddUIElement(new Lable("Buttons that actually work", HorizontalAllignment.Center, 0, ConsoleColor.Yellow, ConsoleColor.Black));
            Button buttonDo = new Button("Button Do", HorizontalAllignment.Center, 0, ConsoleColor.Red, ConsoleColor.Green);
            Button buttonUndo = new Button("Button Undo", HorizontalAllignment.Center, 0, ConsoleColor.Red, ConsoleColor.Green);
            buttonDo.OnClick += Do;
            buttonUndo.OnClick += Undo;
            menu.AddUIElement(buttonDo);
            menu.AddUIElement(buttonUndo);

            MenuManager menuManager = new MenuManager(menu);
            menuManager.Run();


            Console.ReadKey();
        }

        static void Do(object sender, EventArgs eventArgs)
        {
            Console.SetCursorPosition(0, 0);
            Console.Write("Do!");
        }

        static void Undo(object sender, EventArgs eventArgs)
        {
            ConsoleColor previousForeground = Console.ForegroundColor;
            ConsoleColor previousBackground = Console.BackgroundColor;

            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = Console.BackgroundColor;
            Console.Write("Do!");

            Console.ForegroundColor = previousForeground;
            Console.BackgroundColor = previousBackground;

        }
    }
}
