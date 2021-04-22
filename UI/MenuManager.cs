using System;
using System.Collections.Generic;
using System.Threading;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.UI
{
    /// <summary>
    /// This class is used to manage Menus - turn input into action...
    /// </summary>
    public class MenuManager
    {
        // Delay between main thread updates
        private const int MAIN_THREAD_UPDATE_FREQUENCY = 10;

        #region Constructors
        /// <summary>
        /// Initializes new instance of MenuManager class
        /// </summary>
        /// <param name="menu">The Menu that the MenuManager should manage</param>
        public MenuManager(Menu menu, IEnumerable<ConsoleKey> inputToHandle = null)
        {
            Menu = menu;

            // Initializing the InputManager and subscribing individual event handlers
            List<ConsoleKey> keysOfInterest = new List<ConsoleKey>() { MenuKeyBindings.CursorUp, MenuKeyBindings.CursorUpSecondary, MenuKeyBindings.CursorDown, MenuKeyBindings.CursorDownSecondary, MenuKeyBindings.ClickKey, MenuKeyBindings.RefreshKey };
            if(inputToHandle != null)
                keysOfInterest.AddRange(inputToHandle);
            InputManager = new InputManager(keysOfInterest);
            InputManager.KeyPressed += Menu.KeyInteraction;
            InputManager.KeyPressed += Refresh;

            // Subscribe menu controlling events
            menu.PerformAction += PerformAction;
            menu.ExitMenu += ExitMenu;
        }
        #endregion

        #region Properties
        private Menu Menu { get; set; }

        private RunningState CurrentMenuState { get; set; }

        private InputManager InputManager { get; set; } 

        private Thread InputManagerThread { get; set; }

        // An action that the menu should perform
        private Action ScheduledAction { get; set; }
        #endregion

        #region Methods
        public void Run()
        {
            Print();

            CurrentMenuState = RunningState.On;
            RunningState previousState = RunningState.Off;

            while (true)
            {
                if (previousState == CurrentMenuState)
                    Thread.Sleep(MAIN_THREAD_UPDATE_FREQUENCY);
                else
                {
                    previousState = CurrentMenuState;

                    switch (CurrentMenuState)
                    {
                        case RunningState.On:
                            AbortThread(false);
                            StartThread(InputManager.CheckForInput, false);
                            break;
                        case RunningState.PerformAction:
                            AbortThread(true);

                            if (ScheduledAction != null)
                                ScheduledAction();
                            Print();

                            CurrentMenuState = previousState = RunningState.On;

                            StartThread(InputManager.CheckForInput, true);
                            break;
                        case RunningState.Escaping:
                            AbortThread(true);
                            return;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Used for moving into the next submenu/performing other actions when the event in Menu is fired
        /// </summary>
        private void PerformAction(object sender, ActionEventArgs eventArgs)
        {
            ScheduledAction = eventArgs.Action;

            CurrentMenuState = RunningState.PerformAction;   
        }

        /// <summary>
        /// Used for exiting the menu when the event in Menu is fired
        /// </summary>
        private void ExitMenu(object sender, EventArgs eventArgs)
        {
            CurrentMenuState = RunningState.Escaping;
        }

        /// <summary>
        /// Used for refreshing
        /// </summary>
        private void Refresh(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.keyPressed != MenuKeyBindings.RefreshKey)
                return;

            Print();
        }

        /// <summary>
        /// Prints the menu
        /// </summary>
        private void Print()
        {
            Console.Clear();

            Menu.PrintMenu();
        }

        /// <summary>
        /// Sets up the thread to run the InputManager and starts it
        /// </summary>
        private void StartThread(ThreadStart inputThreadStart, bool startNewInputManager = true)
        {
            if (InputManagerThread == null || startNewInputManager)
            {
                InputManagerThread = new Thread(inputThreadStart);
                InputManagerThread.Start();
            }
        }

        /// <summary>
        /// Aborts the thread that runs the InputManager
        /// </summary>
        private void AbortThread(bool abortInputManager = true)
        {
            if (abortInputManager && InputManagerThread != null)
            {
                InputManager.AbortManager = true;

                InputManagerThread.Join();

                InputManagerThread = null;
            }
        }
        #endregion


        private enum RunningState
        {
            Off,
            On,
            PerformAction,
            Escaping
        }
    }
}
