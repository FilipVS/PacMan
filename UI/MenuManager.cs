using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Setnicka.UI
{
    /// <summary>
    /// This class is used to manage menus - turn input into action...
    /// </summary>
    class MenuManager
    {
        // Delay between main thread updates
        private const int MAIN_THREAD_UPDATE_FREQUENCY = 10;

        public MenuManager(Menu menu)
        {
            Menu = menu;

            // Initializing the inputManager and subscribing individual event handlers
            List<ConsoleKey> keysOfInterest = new List<ConsoleKey>() { MenuKeyBindings.CursorUp, MenuKeyBindings.CursorUpSecondary, MenuKeyBindings.CursorDown, MenuKeyBindings.CursorDownSecondary, MenuKeyBindings.ClickKey };
            InputManager = new InputManager(keysOfInterest);
            InputManager.KeyPressed += Menu.KeyInteraction;
        }

        #region Properties
        private Menu Menu { get; set; }

        private MenuState CurrentMenuState { get; set; }

        private InputManager InputManager { get; set; } 

        private Thread InputManagerThread { get; set; }
        #endregion

        #region Methods
        public void Run()
        {
            // TODO: Finish
            Menu.PrintMenu();

            CurrentMenuState = MenuState.Running;
            MenuState previousState = MenuState.Off;

            while (true)
            {
                if (previousState == CurrentMenuState)
                    Thread.Sleep(MAIN_THREAD_UPDATE_FREQUENCY);
                else
                {
                    previousState = CurrentMenuState;

                    switch (previousState)
                    {
                        case MenuState.Running:
                            AbortThreads(false);
                            StartThreads(InputManager.CheckForInput, false);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Prints the menu
        /// </summary>
        private void Print()
        {
            Console.Clear();

            Menu.PrintMenu();

            // TODO: Finish (user interface elements...)
        }

        /// <summary>
        /// Sets up the thread to run the input manager and starts it
        /// </summary>
        private void StartThreads(ThreadStart inputThreadStart, bool startNewInputManager = true)
        {
            if (InputManagerThread == null || startNewInputManager)
            {
                InputManagerThread = new Thread(inputThreadStart);
                InputManagerThread.Start();
            }
        }

        /// <summary>
        /// Aborts the thread that runs the input manager
        /// </summary>
        private void AbortThreads(bool abortInputManager = true)
        {
            if (abortInputManager && InputManagerThread != null)
            {
                InputManagerThread.Abort();
                InputManagerThread = null;
            }
        }
        #endregion


        private enum MenuState
        {
            Off,
            Running,
            Escaping
        }
    }
}
