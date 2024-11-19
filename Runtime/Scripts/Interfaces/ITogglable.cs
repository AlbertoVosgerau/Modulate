namespace DandyDino.Modulate
{
    public interface ITogglable
    {
        public bool IsEnabled { get; }
        public void SetEnabled(bool isEnabled);
    }
}