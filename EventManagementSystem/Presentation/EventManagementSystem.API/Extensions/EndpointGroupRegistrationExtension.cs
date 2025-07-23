namespace EventManagementSystem.API.Extensions
{

    using EventManagementSystem.API.Interface;
    using System.Reflection;

    public static class EndpointGroupRegistrationExtension
    {
        public static void RegisterAllEndpointGroups(this IEndpointRouteBuilder app)
        {
            var endpointGroupTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IEndpointGroup).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in endpointGroupTypes)
            {
                try
                {
                    if (Activator.CreateInstance(type) is IEndpointGroup group)
                    {
                        group.MapEndpoints(app);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
