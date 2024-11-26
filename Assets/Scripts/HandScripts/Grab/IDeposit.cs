namespace HandScripts.Grab
{
    public interface IDeposit
    {
        void OnDeposit(IHandGrabable grabable);
        string GetDepositKey();
    }
}