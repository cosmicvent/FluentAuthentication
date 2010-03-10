namespace Cosmicvent.FluentAuthentication {
    public interface IRoleComparer {
        bool AreEqual(object firstRole, object secondRole);
    }
}