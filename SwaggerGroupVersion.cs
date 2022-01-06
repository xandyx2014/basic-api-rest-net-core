using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApiAutores
{
    public class SwaggerGroupVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var nameSpaceController = controller.ControllerType.Namespace; // Controller.v1
            var versionApi = nameSpaceController.Split('.').Last().ToLower(); // v1
            controller.ApiExplorer.GroupName = versionApi;
        }
    }
}
