namespace MassiveGame.API.ResourcePool.Policies
{
    public abstract class AllocationPolicy<ArgType, ReturnType> : Policy
    {
        // this class is base
        // it's children implement strategy of data allocation and freeing

        public abstract ReturnType AllocateMemory(ArgType arg);

        public abstract void FreeMemory(ReturnType arg);
    }
}
