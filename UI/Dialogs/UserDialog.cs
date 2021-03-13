using System;
using System.Collections.Generic;
using System.Threading;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.UI
{
    /// <summary>
    /// Abstract class for user dialogs (dialog for confirmation...)
    /// </summary>
    public abstract class UserDialog
    {
        // Delay between main thread updates
        protected const int MAIN_THREAD_UPDATE_FREQUENCY = 10;

        #region Constructors
        public UserDialog(IEnumerable<ConsoleKey> keysOfInterest)
        {
            InputManager = new InputManager(keysOfInterest);
        }
        #endregion

        #region Properties
        protected InputManager InputManager { get; set; }

        protected Thread InputManagerThread;

        public DialogResult DialogResult { get; protected set; }
        #endregion

        #region Methods
        public abstract void Run();

         /// <summary>
        /// Sets up the thread to run the input manager and starts it
        /// </summary>
        protected void StartThreads(bool startNewInputManager = true)
        {
            if (InputManagerThread == null || startNewInputManager)
            {
                InputManagerThread = new Thread(InputManager.CheckForInput);
                InputManagerThread.Start();
            }
        }

        /// <summary>
        /// Aborts the thread that runs the input manager
        /// </summary>
        protected void AbortThreads(bool abortInputManager = true)
        {
            if (abortInputManager && InputManagerThread != null)
            {
                InputManagerThread.Abort();
                InputManagerThread = null;
            }
        }
        #endregion
    }

    public enum DialogResult
    {
        Unknown,
        Yes,
        No,
        Cancel
    }
}
