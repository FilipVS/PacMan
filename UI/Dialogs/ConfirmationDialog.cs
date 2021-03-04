using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.UI
{
    /// <summary>
    /// Dialog used for getting confirmation from the user (whikle saving changes...)
    /// Returns an enum DialogResult
    /// </summary>
    public class ConfirmationDialog : UserDialog
    {
        private readonly Vector2D PROMPT_POSITION = new Vector2D(1, 1);
        private readonly ConsoleColor PROMPT_FOREGROUND_COLOR = ConsoleColor.White;
        private readonly ConsoleColor PROMPT_BACKGROUND_COLOR = ConsoleColor.Red;


        private readonly Vector2D BUTTON_YES_POSITION = new Vector2D(1, 2);
        private readonly Vector2D BUTTON_NO_POSITION = new Vector2D(1, 3);
        private readonly Vector2D BUTTON_CANCEL_POSITION = new Vector2D(1, 4);

        private const ConsoleColor BUTTON_HIGHLIGHTED_FOREGROUND = ConsoleColor.Black;
        private const ConsoleColor BUTTON_HIGHLIGHTED_BACKGROUND = ConsoleColor.Cyan;

        #region Constructors
        public ConfirmationDialog(ConfirmationOptions options, string prompt) : base(new List<ConsoleKey>() {MenuKeyBindings.CursorUp, MenuKeyBindings.CursorUpSecondary, MenuKeyBindings.CursorDown, MenuKeyBindings.CursorDownSecondary, MenuKeyBindings.ClickKey })
        {
            buttonYes = null;
            buttonNo = null;
            buttonCancel = null;

            // Create label with the prompt to the user
            LabelPrompt = new Label(prompt, HorizontalAlignment.Custom, PROMPT_POSITION, PROMPT_FOREGROUND_COLOR, PROMPT_BACKGROUND_COLOR);

            // Create the buttons
            ButtonList = new List<Button>();
            if(options == ConfirmationOptions.YesNo || options == ConfirmationOptions.YesNoCancel)
            {
                buttonYes = new Button("Yes", HorizontalAlignment.Custom, BUTTON_YES_POSITION, BUTTON_HIGHLIGHTED_FOREGROUND, BUTTON_HIGHLIGHTED_BACKGROUND);
                buttonNo = new Button("No", HorizontalAlignment.Custom, BUTTON_NO_POSITION, BUTTON_HIGHLIGHTED_FOREGROUND, BUTTON_HIGHLIGHTED_BACKGROUND);

                buttonYes.OnClick += ButtonPressed;
                buttonNo.OnClick += ButtonPressed;

                ButtonList.Add(buttonYes);
                ButtonList.Add(buttonNo);
            }
            if (options == ConfirmationOptions.YesNoCancel)
            {
                buttonCancel = new Button("Cancel", HorizontalAlignment.Custom, BUTTON_CANCEL_POSITION, BUTTON_HIGHLIGHTED_FOREGROUND, BUTTON_HIGHLIGHTED_BACKGROUND);

                buttonCancel.OnClick += ButtonPressed;

                ButtonList.Add(buttonCancel);
            }

            // Subscribe input-related events
            InputManager.KeyPressed += HandleInput;
        }
        #endregion

        #region Properties
        RunningState CurrentState = RunningState.Off;

        int Highlighted { get; set; } = -1;

        List<Button> ButtonList { get; set; }

        Label LabelPrompt { get; set; }

        Button buttonYes { get; set; }
        Button buttonNo { get; set; }
        Button buttonCancel { get; set; }
        #endregion

        #region Methods
        public override void Run()
        {
            Print();
            MoveHighlightDown();

            CurrentState = RunningState.On;
            RunningState previousRunningState = RunningState.Off;

            while (true)
            {
                if (CurrentState == previousRunningState)
                    Thread.Sleep(MAIN_THREAD_UPDATE_FREQUENCY);
                else
                {
                    previousRunningState = CurrentState;

                    switch (CurrentState)
                    {
                        case RunningState.On:
                            AbortThreads(false);
                            StartThreads(false);
                            break;
                        case RunningState.ButtonPressed:
                            AbortThreads(true);
                            return;
                        default:
                            break;
                    }
                }
            }
        }

        private void ButtonPressed(object sender, EventArgs args)
        {
            if (sender == buttonYes)
                DialogResult = DialogResult.Yes;
            else if (sender == buttonNo)
                DialogResult = DialogResult.No;
            else if (sender == buttonCancel)
                DialogResult = DialogResult.Cancel;

            CurrentState = RunningState.ButtonPressed;
        }

        private void Print()
        {
            Console.Clear();

            LabelPrompt.Print();

            if (buttonYes != null)
                buttonYes.Print();
            if (buttonNo != null)
                buttonNo.Print();
            if (buttonCancel != null)
                buttonCancel.Print();

            Console.CursorVisible = false;
        }

        private void HandleInput(object sender, KeyEventArgs args)
        {
            if (args.keyPressed == MenuKeyBindings.CursorUp || args.keyPressed == MenuKeyBindings.CursorUpSecondary)
                MoveHighlightUp();

            if (args.keyPressed == MenuKeyBindings.CursorDown || args.keyPressed == MenuKeyBindings.CursorDownSecondary)
                MoveHighlightDown();

            if (args.keyPressed == MenuKeyBindings.ClickKey)
                PressButton();
        }

        private void MoveHighlightUp()
        {
            // When there is already a highlighted button, it gets unhighlighted
            if (Highlighted > -1 && Highlighted < ButtonList.Count)
                ButtonList[Highlighted].PrintHighlighted(false);

            // Highlights new button
            Highlighted--;
            if (Highlighted < 0)
                Highlighted = (ButtonList.Count - 1);

            ButtonList[Highlighted].PrintHighlighted(true);
        }

        private void MoveHighlightDown()
        {
            // When there is already a highlighted button, it gets unhighlighted
            if (Highlighted > -1 && Highlighted < ButtonList.Count)
                ButtonList[Highlighted].PrintHighlighted(false);

            // Highlights new button
            Highlighted++;
            if (Highlighted > (ButtonList.Count - 1))
                Highlighted = 0;

            ButtonList[Highlighted].PrintHighlighted(true);
        }

        private void PressButton()
        {
            ButtonList[Highlighted].Clicked(this, EventArgs.Empty);
        }
        #endregion

        enum RunningState
        {
            Off,
            On,
            ButtonPressed
        }
    }

    public enum ConfirmationOptions
    {
        YesNo,
        YesNoCancel
    }
}
