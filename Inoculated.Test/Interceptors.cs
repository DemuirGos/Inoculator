﻿using Inoculator.Attributes;
using Inoculator.Builder;
using System.Collections;
using System.Diagnostics;
using System.Reflection;


public class DateAttribute : InterceptorAttribute
{
    private Stopwatch watch = new();
    public override void OnEntry(MethodData method)
    {
        Console.WriteLine($"started : {DateTime.UtcNow}");
    }

    public override void OnExit(MethodData method)
    {
        Console.WriteLine($"ended : {DateTime.UtcNow}");
    }
}

public class CallCountAttribute : InterceptorAttribute
{
    public static Dictionary<string, int> CallCounter = new Dictionary<string, int>();
    public override void OnEntry(MethodData method)
    {
        if(!CallCounter.ContainsKey(method.MethodName))
            CallCounter.Add(method.MethodName, 0);
        CallCounter[method.MethodName]++;
    }
}

public class UpdateStaticClassAttribute<T> : InterceptorAttribute
{
    private FieldInfo targetField;
    public UpdateStaticClassAttribute() {
        var assembly = typeof(T).Assembly;
        // get static field of type Name U
        var type = assembly.GetType(typeof(T).FullName);
        targetField = type.GetField("CallCount");
    }
    public static Dictionary<string, int> CallCounter = new Dictionary<string, int>();
    public override void OnEntry(MethodData method)
    {
        targetField.SetValue(null, 23);
    }
}


public class MemoizeAttribute : RewriterAttribute
{
    private static Dictionary<int, ParameterData> cache = new Dictionary<int, ParameterData>();
    public int StringifyAndHash(object[] parameters) {
        var result = "";
        foreach(var parameter in parameters) {
            result += parameter.ToString();
        }
        return result.GetHashCode();
    }
    Engine<Program.Entry> engine = new();
    public override MethodData OnCall(MethodData method)
    {
        var argumentsHash = StringifyAndHash(method.Parameters);
        if(method.MethodBehaviour is not MethodData.MethodType.Iter && cache.ContainsKey(argumentsHash)) {
            method.ReturnValue = cache[argumentsHash];
        } else {
            method = engine.Invoke(method);
        }
        cache.TryAdd(argumentsHash, method.ReturnValue);
        Console.WriteLine(method);
        return method;
    }
}