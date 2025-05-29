namespace AMapper;

public static class Extensions
{
    public static TB Map<TB>(this object aInstance) where TB : class
    {
        return Mapper.Map<TB>(aInstance);
    }
}