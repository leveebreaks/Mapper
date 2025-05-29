using System;
using System.Collections.Generic;

namespace AMapper;

public static class Mapper
{
    private static Dictionary<(Type, Type), Mapping> _types = new Dictionary<(Type, Type), Mapping>();
    
    public static Mapping<TA, TB> ConfigureMap<TA, TB>() where TB : new() where TA : class
    {
        var mapping = new Mapping<TA, TB>();
        _types[(typeof(TA), typeof(TB))] = mapping;

        return mapping;
    }

    public static TB Map<TB>(object aInstance) where TB : class
    {
        if (aInstance == null)
        {
            throw new ArgumentNullException(nameof(aInstance));
        }
        
        var mapping = _types[(aInstance.GetType(), typeof(TB))];

        return mapping.Convert<TB>(aInstance) as TB;
    }
}