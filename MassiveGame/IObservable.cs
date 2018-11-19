namespace MassiveGame.Core.GameCore.Entities
{
    public interface IObservable
    {
        void NotifyAdded();
        void NotifyRemoved();
    }
}