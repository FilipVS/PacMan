using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setnicka.UI
{
    /// <summary>
    /// The Run method is run from the Main - I can use internal components here while testing them from Program
    /// </summary>
    public static class TestRun
    {
        public static void Run()
        {
            List<ConsoleKey> numbers = new List<ConsoleKey>() { ConsoleKey.D0, ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4, ConsoleKey.D5, ConsoleKey.D6, ConsoleKey.D7, ConsoleKey.D8, ConsoleKey.D9, ConsoleKey.NumPad0, ConsoleKey.NumPad1, ConsoleKey.NumPad2, ConsoleKey.NumPad3, ConsoleKey.NumPad4, ConsoleKey.NumPad5, ConsoleKey.NumPad6, ConsoleKey.NumPad7, ConsoleKey.NumPad8, ConsoleKey.NumPad9 };

            Menu subsubMenu = new Menu();
            subsubMenu.AddUIElement(new Label("Submenu 2", HorizontalAlignment.Center, 0, ConsoleColor.Magenta, ConsoleColor.Black));
            subsubMenu.AddUIElement(new Label("Buttons that don't work", HorizontalAlignment.Center, 0, ConsoleColor.Yellow, ConsoleColor.Black));
            subsubMenu.AddUIElement(new Button("Button 1", HorizontalAlignment.Center, 0, ConsoleColor.Red, ConsoleColor.Green));
            subsubMenu.AddUIElement(new Button("Button 2", HorizontalAlignment.Center, 0, ConsoleColor.Red, ConsoleColor.Green));
            subsubMenu.AddUIElement(new Label("", HorizontalAlignment.Center, 0));
            Button btnExit = new Button("Button Exit", HorizontalAlignment.Center, 0, ConsoleColor.Red, ConsoleColor.Green);
            btnExit.OnClick += subsubMenu.DoExitMenu;
            subsubMenu.AddUIElement(btnExit);
            MenuManager subsubMenuManager = new MenuManager(subsubMenu);


            Menu subMenu = new Menu();
            subMenu.AddUIElement(new Label("Submenu", HorizontalAlignment.Center, 0, ConsoleColor.Magenta, ConsoleColor.Black));
            subMenu.AddUIElement(new Label("Buttons that don't work", HorizontalAlignment.Center, 0, ConsoleColor.Yellow, ConsoleColor.Black));
            subMenu.AddUIElement(new Button("Button 1", HorizontalAlignment.Center, 0, ConsoleColor.Red, ConsoleColor.Green));
            subMenu.AddUIElement(new Button("Button 2", HorizontalAlignment.Center, 0, ConsoleColor.Red, ConsoleColor.Green));
            subMenu.AddUIElement(new Label("", HorizontalAlignment.Center, 0));
            Button subbtnExit = new Button("Button Exit", HorizontalAlignment.Center, 0, ConsoleColor.Red, ConsoleColor.Green);
            Button subsubBtn = new Button("Button Move to submenu", HorizontalAlignment.Center, 0, ConsoleColor.Red, ConsoleColor.Green, subsubMenuManager.Run);
            subsubBtn.OnClick += subMenu.DoPerformAction;
            subMenu.AddUIElement(subsubBtn);
            subMenu.AddUIElement(new Label("", HorizontalAlignment.Center, 0));
            subbtnExit.OnClick += subMenu.DoExitMenu;
            subMenu.AddUIElement(subbtnExit);
            MenuManager subMenuManager = new MenuManager(subMenu);

            Menu menu = new Menu();
            menu.AddUIElement(new Label("Menu 1", HorizontalAlignment.Center, 0, ConsoleColor.Magenta, ConsoleColor.Black));
            menu.AddUIElement(new Label("Buttons that don't work", HorizontalAlignment.Center, 0, ConsoleColor.Yellow, ConsoleColor.Black));
            menu.AddUIElement(new Button("Button 1", HorizontalAlignment.Center, 0, ConsoleColor.Red, ConsoleColor.Green));
            menu.AddUIElement(new Button("Button 2", HorizontalAlignment.Center, 0, ConsoleColor.Red, ConsoleColor.Green));
            menu.AddUIElement(new Label("", HorizontalAlignment.Center, 0));
            menu.AddUIElement(new Label("Buttons that actually work", HorizontalAlignment.Center, 0, ConsoleColor.Yellow, ConsoleColor.Black));
            Button buttonDo = new Button("Button Do", HorizontalAlignment.Center, 0, ConsoleColor.Red, ConsoleColor.Green);
            Button buttonUndo = new Button("Button Undo", HorizontalAlignment.Center, 0, ConsoleColor.Red, ConsoleColor.Green);
            menu.AddUIElement(new NumberField("X size of level: ", 3, HorizontalAlignment.Center, 0, ConsoleColor.Red, ConsoleColor.Green));
            Button buttonMoveToSubmenu = new Button("Button Move to submenu", HorizontalAlignment.Center, 0, ConsoleColor.Red, ConsoleColor.Green, subMenuManager.Run);
            Button buttonExit = new Button("Button Exit", HorizontalAlignment.Center, 0, ConsoleColor.Red, ConsoleColor.Green);
            buttonDo.OnClick += Do;
            buttonUndo.OnClick += Undo;
            buttonMoveToSubmenu.OnClick += menu.DoPerformAction;
            buttonExit.OnClick += menu.DoExitMenu;
            menu.AddUIElementRange(new List<IUIElement>() { buttonDo, buttonUndo, buttonMoveToSubmenu, new Label("", HorizontalAlignment.Center, 0), buttonExit });

            MenuManager menuManager = new MenuManager(menu, numbers);
            menuManager.Run();


            Console.SetCursorPosition(0, 0);
            Console.WriteLine("I'm here! Menu system closed succesfully.");
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
