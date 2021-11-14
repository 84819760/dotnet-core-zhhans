namespace DotNetCorezhHans.Base
{
    public class IndexProvider
    {
        private int value;

        public int GetId()
        {
            value++;
            return value;
        }
    }
}
