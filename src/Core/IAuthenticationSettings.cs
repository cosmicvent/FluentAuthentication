using System.Web.Mvc;

namespace Cosmicvent.FluentAuthentication {
    public interface IAuthenticationSettings {
        void AddRule(IControllerRule rule);
        IControllerRule<T> AddRule<T>() where T : IController;
        bool HasAccess(string controller, string action, object role);
        void Reset();
        bool HasAnonymousAccess(string controllerName, string actionName);
    }
}