using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebScrappingApi.Routng_Configuration
{
    public class ControllerRouteConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            foreach (var action in controller.Actions)
            {
                // Modify the action route to use the action name as the route
                action.Selectors[0].AttributeRouteModel.Template = action.ActionMethod.Name;
            }
        }
    }

}
