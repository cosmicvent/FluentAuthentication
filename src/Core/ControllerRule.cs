using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;


namespace Cosmicvent.FluentAuthentication {
    public class ControllerRule<T> : IControllerRule<T> where T : IController {

        private readonly List<string> _actions;
        private readonly List<object> _roles;
        private readonly string _controller;

        public ControllerRule() {
            _controller = GetControllerName(typeof(T).Name);
            _actions = new List<string>();
            _roles = new List<object>();
        }

        public IControllerRule<T> AddAction(string action) {
            _actions.Add(action);
            return this;
        }

        public IControllerRule<T> AddAction(Expression<Func<T, ActionResult>> expression) {
            var callExpression = expression.Body as MethodCallExpression;
            if (callExpression != null) {
                _actions.Add(callExpression.Method.Name);
            } else {
                throw new ArgumentException("Not a method call");
            }
            return this;
        }

        public IControllerRule<T> AddRole(object role) {
            _roles.Add(role);
            return this;
        }

        public bool Matches(string controller, string action) {

            if (!_controller.Equals(controller, StringComparison.OrdinalIgnoreCase) && !_controller.Equals(GetControllerName(controller), StringComparison.OrdinalIgnoreCase)) {
                return false;
            }
            //If there are no actions added this rule applies to all the actions
            if (_actions.Count == 0) {
                return true;
            }

            foreach (var a in _actions) {
                if (a.Equals(action, StringComparison.OrdinalIgnoreCase)) {
                    return true;
                }
            }

            return false;
        }

        public bool HasAccess(object role, IRoleComparer comparer) {
            foreach (var r in _roles) {
                if (comparer.AreEqual(r, role)) {
                    return true;
                }
            }
            return false;
        }

        public bool HasAnonymousAccess() {
            return _roles.Count == 0;
        }

        private string GetControllerName(string name) {
            return name.Replace("Controller", "");
        }
    }
}