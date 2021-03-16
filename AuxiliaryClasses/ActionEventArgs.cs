using System;

namespace Setnicka.AuxiliaryClasses
{
    /// <summary>
    /// Event args that contain the extra information about an Action
    /// </summary>
    public class ActionEventArgs : EventArgs
    {
        public ActionEventArgs(Action action)
        {
            Action = action;
        }

        public Action Action { get; }
    }
}
