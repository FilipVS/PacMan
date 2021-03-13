using Setnicka.AuxiliaryClasses;

namespace Setnicka.UI
{
    /// <summary>
    /// Classes implementing this have the abbility to handle KeyEventArgs input
    /// </summary>
    public interface IInputtableUIElement : IHighlightableUIElement
    {
        void HandleInput(object sender, KeyEventArgs input, bool reprintHighlighted);
    }
}
