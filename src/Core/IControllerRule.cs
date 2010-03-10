using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Cosmicvent.FluentAuthentication {
    public interface IControllerRule<T> : IControllerRule where T : IController {
        IControllerRule<T> AddAction(string action);
        IControllerRule<T> AddAction(Expression<Func<T, ActionResult>> expression);
        IControllerRule<T> AddRole(object role);
    }

    public interface IControllerRule {
        bool Matches(string controller, string action);
        bool HasAccess(object role, IRoleComparer comparer);
        bool HasAnonymousAccess();
    }
}