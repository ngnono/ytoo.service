namespace com.intime.fashion.data.sync.Wgw.Response.Processor
{
    public class ProcessorFactory
    {
        public static IProcessor CreateProcessor<T>() where T:class,new()
        {
            return new T() as IProcessor;
        }
    }
}
