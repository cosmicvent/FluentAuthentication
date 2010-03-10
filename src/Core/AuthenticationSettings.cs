using System.Collections.Generic;
using System.Web.Mvc;

namespace Cosmicvent.FluentAuthentication {
    public abstract class AuthenticationSettings : IAuthenticationSettings {

        protected readonly IRoleComparer _roleComparer;

        protected AuthenticationSettings(IRoleComparer roleComparer) {
            _roleComparer = roleComparer;
        }

        private static List<IControllerRule> _rules;
        private readonly object _lockingObject = new object();
        private bool _isConfigured;

        private List<IControllerRule> Rules {
            get {
                if (_rules == null) {
                    lock (_lockingObject) {
                        _rules = new List<IControllerRule>();
                    }
                }
                return _rules;
            }
        }

        public IControllerRule<T> AddRule<T>() where T : IController {
            var rule = new ControllerRule<T>();
            Rules.Add(rule);
            return rule;
        }

        public bool HasAccess(string controller, string action, object role) {
            EnsureRulesAreConfigured();
            if (HasAnonymousAccess(controller, action)) {
                return true;
            }

            var rule = FindMatchingRule(controller, action);
            return rule.HasAccess(role, _roleComparer);
        }

        private void EnsureRulesAreConfigured() {
            if (!_isConfigured) {
                _isConfigured = true;
                lock (_lockingObject) {
                    Configure();
                }
            }
        }

        public void AddRule(IControllerRule rule) {
            Rules.Add(rule);
        }

        protected abstract void Configure();

        public void Reset() {
            _rules = null;
        }

        public bool HasAnonymousAccess(string controllerName, string actionName) {
            EnsureRulesAreConfigured();
            var rule = FindMatchingRule(controllerName, actionName);
            return rule == null || rule.HasAnonymousAccess();
        }

        private IControllerRule FindMatchingRule(string controller, string action) {
            foreach (IControllerRule rule in Rules) {
                bool matches = rule.Matches(controller, action);
                if (!matches) continue;
                return rule;
            }
            return null;
        }
    }
}