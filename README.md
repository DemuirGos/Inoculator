# Innoculator
An IL code Injector using Ilasm and Ildasm (WIP)
# Limitations
    * Needs Excessive Testing
    * Targeting release Build is not predictable (yet)
# Plan 
    * Automatically Add MSbuild PostBuild event handler
# Usage
* Reference Inoculator.Injecter
* Add to Msbuild :
   ```
   <Target Name="InjectionStep" BeforeTargets="AfterBuild">
       <Exec Command="$(MSbuildProjectDirectory)\$(BaseOutputPath)$(Configuration)\$(TargetFramework)\Inoculator.Injector.exe   $(MSbuildProjectDirectory)\$(BaseOutputPath)$(Configuration)\$(TargetFramework)\$(AssemblyName).dll" />
   </Target>
  ```
* Inherit InterceptorAttribute and override Function lifecycle nodes :  
```csharp
public class ElapsedTimeAttribute : InterceptorAttribute
{
    public override void OnEntry(Metadata method)
    {
        method.EmbededResource = Stopwatch.StartNew();        
    }

    public override void OnExit(Metadata method)
    {
        var sw = (Stopwatch)method.EmbededResource;
        sw.Stop();
        Console.WriteLine($"Method {method.Name} took {sw.ElapsedMilliseconds}ms");
    }
}
```
* Flag function to be injected with code : 
```csharp
static void Main(string[] args) {
    Test();
}

[ElapsedTime, LogEntrency]
public static void Test() {
    int i = 0;
    for (int j = 0; j < 100; j++) {
        i++;
    }
    Console.WriteLine(i);
}
```
* Output :
```
Started Method Test                                                                                                                                           
100                                                                                                                                                           
Method Testtook 4ms
Finished Method Test
```
