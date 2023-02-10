using Convey;
using Convey.Persistence.MongoDB;

namespace Ntemtem.MainService.Extensions;

public static class ConveyExtension
{
    public static IConveyBuilder RegisterConvey(this IConveyBuilder builder)
    {
        builder.AddMongo();
    
        return builder;
    }
}