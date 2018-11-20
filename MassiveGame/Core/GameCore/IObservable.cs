namespace MassiveGame.Core.GameCore
{
    public interface IObservable
    {
        void NotifyAdded();
        void NotifyRemoved();
    }
}