TinyDI is a simple static source-generated DI container/service locator. It was brought into existence by my need to have an easy way to inject into my services, primarily in a Unity modding environment.

For such modding cases the source-generated aspect is a good fit, as it means that TinyDI is portable, yet doesn't require any runtime dependencies.

---

## Currently supported features

1. [x] Simple service registration
2. [x] Service resolution
3. [x] Singleton service lifetime
4. [x] Configurable namespace and accessibility for generated classes

Note that there is no such thing as building the container at runtime. You can register services at any time, use to your own discretion.\
At this time, collection injection is not supported nor is the ability to register multiple services of the same type.

Services also only resolve by the exact type they were registered with.

```csharp
    DI.instance.Register<MyService>();
    
    // This works
    IMyService myService = DI.instance.Resolve<MyService>();
    
    // This does not
    IMyService myService = DI.instance.Resolve<IMyService>();
```