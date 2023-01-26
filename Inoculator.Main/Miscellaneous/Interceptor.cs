using Inoculator.Builder;
namespace Inoculator.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class InterceptorAttribute : System.Attribute
{
    public InterceptorAttribute() { }
    public virtual void OnEntry(MethodData method) {}
    public virtual void OnException(MethodData method) {}
    public virtual void OnSuccess(MethodData method) {}
    public virtual void OnExit(MethodData method) {}

}