namespace MassiveGame.API.ResourcePool.PoolHandling
{
    public interface IPoolObtainable<PoolType>
    {
        PoolType GetPool();
    }
}
