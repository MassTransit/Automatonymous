namespace Stayt.Impl
{
    public class StateImpl<TInstance> :
        State
    {
        public StateImpl(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}