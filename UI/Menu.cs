using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Setnicka.AuxiliaryClasses;


namespace Setnicka.UI
{
    /// <summary>
    /// This is the base class for all menus
    /// It is basically a collection of UI elements
    /// Items can be added, they can't be removed or shifted
    /// When drawn, each item will be printed on a separate line starting at the top of the screen   
    /// </summary>
    public class Menu
    {
        private const HorizontalAlignment DEFAULT_ALLIGNMENT = HorizontalAlignment.Center;

        #region Events
        internal event EventHandler<ActionEventArgs> PerformAction;

        internal event EventHandler ExitMenu;
        #endregion

        #region Properties
        // This is the main collection, storing all the elements
        private List<IUIElement> MenuList { get; set; } = new List<IUIElement>();

        private bool ContainsHighlightableElement { get; set; } = false;

        private int CurrentlyHighlighted { get; set; }
        #endregion

        #region Methods
        internal void PrintMenu()
        {
            Console.Clear();

            CurrentlyHighlighted = -1;

            for(int i = 0; i < MenuList.Count; i++)
            {
                MenuList[i].Print();

                // This highlights the top Highlightable element
                if (ContainsHighlightableElement && CurrentlyHighlighted < 0 && MenuList[i] is IHighlightableUIElement highlightable)
                {
                    highlightable.PrintHighlighted(true);
                    CurrentlyHighlighted = i;
                }
            }

        }

        public void AddUIElement(IUIElement element)
        {
            // Elements in menus cannot have custom allignment
            if (element.HorizontalAlignment == HorizontalAlignment.Custom)
                element.HorizontalAlignment = DEFAULT_ALLIGNMENT;

            // The new element is set to be rendered at the end of the current menu
            element.RenderPosition = new Vector2D(-1, MenuList.Count);

            if (element is IHighlightableUIElement)
                ContainsHighlightableElement = true;

            MenuList.Add(element);
        }

        public void AddUIElementRange(IEnumerable<IUIElement> elementRange)
        {
            foreach (IUIElement element in elementRange)
                AddUIElement(element);
        }

        /// <summary>
        /// Used for exiting the menu, using an elemt like button
        /// </summary>
        public void DoExitMenu(object sender, EventArgs eventArgs)
        {
            ExitMenu(sender, eventArgs);
        }

        /// <summary>
        /// Used for moving to submenu, using an elemt like button
        /// </summary>
        public void DoPerformAction(object sender, EventArgs eventArgs)
        {
            if (sender is IActionable actionable)
                if (actionable.Action != null)
                    PerformAction(sender, new ActionEventArgs(actionable.Action));
        }

        internal void KeyInteraction(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.keyPressed == MenuKeyBindings.CursorUp || keyEventArgs.keyPressed == MenuKeyBindings.CursorUpSecondary)
                MoveHighlightUp();
            else if (keyEventArgs.keyPressed == MenuKeyBindings.CursorDown || keyEventArgs.keyPressed == MenuKeyBindings.CursorDownSecondary)
                MoveHighlightDown();
            else if (keyEventArgs.keyPressed == MenuKeyBindings.ClickKey)
                ClickOnHighlightedElement(sender, keyEventArgs);

            SendInputToHighlightedElement(sender, keyEventArgs);
        }

        private void MoveHighlightUp()
        {
            // This checks, whether there is highlightable element above the currently highlighted
            for(int i = (CurrentlyHighlighted - 1); i > -1; i--)
            {
                if(MenuList[i] is IHighlightableUIElement highlightable)
                {
                    if (MenuList[CurrentlyHighlighted] is IHighlightableUIElement currentlyHighlighted)
                    {
                        currentlyHighlighted.OnElementUnhighlighted();
                        HighlightElement(highlightable, currentlyHighlighted);
                    }
                    else
                        HighlightElement(highlightable);
                    CurrentlyHighlighted = i;

                    return;
                }
            }

            // If not, it checks for highlightable element from the bottom
            for(int i = (MenuList.Count - 1); i > CurrentlyHighlighted; i--)
            {
                if (MenuList[i] is IHighlightableUIElement highlightable)
                {
                    if (MenuList[CurrentlyHighlighted] is IHighlightableUIElement currentlyHighlighted)
                    {
                        currentlyHighlighted.OnElementUnhighlighted();
                        HighlightElement(highlightable, currentlyHighlighted);
                    }
                    else
                        HighlightElement(highlightable);
                    CurrentlyHighlighted = i;

                    return;
                }
            }
        }

        private void MoveHighlightDown()
        {
            // This checks, whether there is highlightable element below the currently highlighted
            for (int i = (CurrentlyHighlighted + 1); i < (MenuList.Count); i++)
            {
                if(MenuList[i] is IHighlightableUIElement highlightable)
                {
                    if (MenuList[CurrentlyHighlighted] is IHighlightableUIElement currentlyHighlighted)
                    {
                        currentlyHighlighted.OnElementUnhighlighted();
                        HighlightElement(highlightable, currentlyHighlighted);
                    }
                    else
                        HighlightElement(highlightable);
                    CurrentlyHighlighted = i;

                    return;
                }
            }

            // If not, it checks for highlightable element from the top
            for (int i = 0; i < CurrentlyHighlighted; i++)
            {
                if (MenuList[i] is IHighlightableUIElement highlightable)
                {
                    if (MenuList[CurrentlyHighlighted] is IHighlightableUIElement currentlyHighlighted)
                    {
                        currentlyHighlighted.OnElementUnhighlighted();
                        HighlightElement(highlightable, currentlyHighlighted);
                    }
                    else
                        HighlightElement(highlightable);
                    CurrentlyHighlighted = i;

                    return;
                }
            }
        }

        private void HighlightElement(IHighlightableUIElement elementToHighlight, IHighlightableUIElement elementToUnhighlight = null)
        {
            elementToHighlight.PrintHighlighted(true);

            if (elementToUnhighlight != null)
                elementToUnhighlight.PrintHighlighted(false);
        }

        private void ClickOnHighlightedElement(object sender, KeyEventArgs eventArgs)
        {
            if(CurrentlyHighlighted > -1 && CurrentlyHighlighted < MenuList.Count)
            {
                if (MenuList[CurrentlyHighlighted] is IClickableUIElement clickable)
                    clickable.Clicked(sender, eventArgs);
            }
        }

        private void SendInputToHighlightedElement(object sender, KeyEventArgs eventArgs)
        {
            if(CurrentlyHighlighted > -1 && CurrentlyHighlighted < MenuList.Count)
            {
                if (MenuList[CurrentlyHighlighted] is IInputtableUIElement inputtableUI)
                    inputtableUI.HandleInput(sender, eventArgs, true);
            }
        }
        #endregion
    }
}
