using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setnicka.UI
{
    /// <summary>
    /// This is the base class for all menus
    /// It is basically a collection of UI elements
    /// Items can be added, they can't be removed or shifted
    /// When drawn, each item will be printed on a separate line starting at the top of the screen   
    /// </summary>
    class Menu
    {
        public const HorizontalAllignment DEFAULT_ALLIGNMENT = HorizontalAllignment.Center;

        #region Properties
        // This is the main collection, storing all the elements
        public List<IUIElement> MenuList { get; private set; } = new List<IUIElement>();

        private bool ContainsHighlightableElement { get; set; } = false;

        private int CurrentlyHighlighted { get; set; }
        #endregion

        #region Methods
        public void PrintMenu()
        {
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
            if (element.HorizontalAllignment == HorizontalAllignment.Custom)
                element.HorizontalAllignment = DEFAULT_ALLIGNMENT;

            // The new element is set to be rendered at the end of the current menu
            element.RenderPosition = new Vector2D(-1, MenuList.Count);

            if (element is IHighlightableUIElement)
                ContainsHighlightableElement = true;

            MenuList.Add(element);
        }

        public void KeyInteraction(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.keyPressed == MenuKeyBindings.CursorUp || keyEventArgs.keyPressed == MenuKeyBindings.CursorUpSecondary)
                MoveHighlightUp();
            else if (keyEventArgs.keyPressed == MenuKeyBindings.CursorDown || keyEventArgs.keyPressed == MenuKeyBindings.CursorDownSecondary)
                MoveHighlightDown();
            else if (keyEventArgs.keyPressed == MenuKeyBindings.ClickKey)
                ClickOnHighlightedElement(sender, keyEventArgs);
        }

        private void MoveHighlightUp()
        {
            // This checks, whether there is highlightable element above the currently highlighted
            for(int i = (CurrentlyHighlighted - 1); i > -1; i--)
            {
                if(MenuList[i] is IHighlightableUIElement highlightable)
                {
                    if (MenuList[CurrentlyHighlighted] is IHighlightableUIElement currentlyHighlighted)
                        HighlightElement(highlightable, currentlyHighlighted);
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
                        HighlightElement(highlightable, currentlyHighlighted);
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
                        HighlightElement(highlightable, currentlyHighlighted);
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
                        HighlightElement(highlightable, currentlyHighlighted);
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
        #endregion
    }
}
