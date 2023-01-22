using System.Reflection;
using events;
using host_domain.Aggregates;

namespace host_domain_tests;

public static class TestHelpers
{
    public static void ApplyEventAgainstAggregate<T1, T2>(this T1 aggregate, T2 registeredEvent) where T1 : Aggregate where T2 : Event
    {
        var methods = aggregate.GetType()
            .GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic);

        var methodInfos = methods
            .First(x => x.GetParameters().First().ParameterType == typeof(T2));

        methodInfos.Invoke(aggregate, new object[] { registeredEvent });
    }
}