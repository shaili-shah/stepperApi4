using System.Web.Http;

namespace StepperWebApI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //var container = new Container();

            //container.RegisterWebApiRequest<IRepositoryAsync<Category>, Repository<Category>>();
            //container.RegisterWebApiRequest<ICategoryService, CategoryService>();
            //container.RegisterWebApiRequest<IDataContextAsync>(() => new MyContext());
            //container.Verify();

            //config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);



        }
    }
}
