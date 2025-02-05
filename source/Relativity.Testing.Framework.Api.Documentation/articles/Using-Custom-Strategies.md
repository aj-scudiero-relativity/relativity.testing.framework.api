﻿# Create a custom strategy

All strategies should be based on some interface, most common actions already have interfaces so you should check to see if one exists before duplicating it.
For a list of available interfaces, please check out the folder found [here](https://github.com/relativitydev/relativity.testing.framework/tree/master/source/Relativity.Testing.Framework/Strategies).

After you implement the needed interface you should register your strategy, in RTF we use the Castle Windsor package for it.
Let's say you want to implement a new get strategy for keyword search which uses a new model with fewer properties, named NewKeywordSearch.
For example, it can look like:

```
[ObjectTypeName("KeywordSearch")]
public class NewKeywordSearch : NamedArtifact
{
}
```
And then you implement a new strategy, for example like this:

```
public class NewGetByIdStrategy : IGetWorkspaceEntityByIdStrategy<NewKeywordSearch>
{
    public NewKeywordSearch Get(int workspaceId, int entityId)
    {
        return new NewKeywordSearch();
    }
}
```

Now you have a new strategy but RTF doesn't know how to resolve it.
For that, you need to implement a new class that will implement two interfaces: [IRelativityComponent](https://relativitydev.github.io/relativity.testing.framework/api/Relativity.Testing.Framework.IRelativityComponent.html) and [IWindsorInstaller](https://github.com/castleproject/Windsor/blob/master/docs/installers.md).
In that class you can write some logic to register your strategies, lets name it TestComponent.

```
public class TestComponent : IRelativityComponent, IWindsorInstaller
{
    public void Ensure(IWindsorContainer container)
    {
        // There is nothing to ensure here.
    }
 
    public void Initialize(IWindsorContainer container)
    {
        container.Install(this);
    }
 
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
        container.Register(Component.For<IGetWorkspaceEntityByIdStrategy<NewKeywordSearch>>().
            ImplementedBy<NewGetByIdStrategy>().
            LifestyleSingleton());
    }
}
```

Now we have a class wich can register our new strategy, to make this action you need to rely on this component before using strategy.
Better to do this action in OneTimeSetup like here, but you can do it even in the test like in the example below:

```
[Test]
public void ResolveNewStrategy()
{
    RelativityFacade.Instance.RelyOn<TestComponent>();
    var service = RelativityFacade.Instance.Resolve<IGetWorkspaceEntityByIdStrategy<NewKeywordSearch>>();
 
    var keywordSearch = service.Get(-1, -1);
}
```

# Create a custom strategy with auto clean up

If you want to implement create a strategy with auto clean up then you should implement tho strategies: create and delete.
Create is a strategy that should be based on [CreateWorkspaceEntityStrategy](https://github.com/relativitydev/relativity.testing.framework.api/blob/master/source/Relativity.Testing.Framework.Api/Strategies/CreateWorkspaceEntityStrategy%601.cs) for workspace artifacts or [CreateStrategy](https://relativitydev.github.io/relativity.testing.framework/api/Relativity.Testing.Framework.Strategies.CreateStrategy-1.html) for admin artifacts.
Delete strategy should be based on [DeleteByIdStrategy](https://relativitydev.github.io/relativity.testing.framework/api/Relativity.Testing.Framework.Strategies.DeleteByIdStrategy-1.html) for admin artifacts or [DeleteWorkspaceEntityByIdStrategy](https://relativitydev.github.io/relativity.testing.framework/api/Relativity.Testing.Framework.Strategies.DeleteWorkspaceEntityByIdStrategy-1.html) for workspace artifacts.

```
public class CustomDeleteStrategy : DeleteWorkspaceEntityByIdStrategy<CustomModel>
{
    protected override void DoDelete(int workspaceId, int entityId)
    {
        //some actions
    }
}
```

```
public class CustomCreateStrategy : CreateWorkspaceEntityStrategy<CustomModel>
{
    protected override CustomModel DoCreate(int workspaceId, CustomModel entity)
    {
        //some actions
    }
}
```

If you implement this to strategies then this artifact will be removed in OneTimeTearDown or in TearDown.

---
**NOTE**

Don't forget to register new strategies, and don't use for it abstract classes.
It's better to use the generic interfaces that already exist, when possible.
e.g. [ICreateWorkspaceEntityStrategy](https://github.com/relativitydev/relativity.testing.framework.api/blob/master/source/Relativity.Testing.Framework.Api/Strategies/ICreateWorkspaceEntityStrategy%601.cs) for [CreateWorkspaceEntityStrategy](https://github.com/relativitydev/relativity.testing.framework.api/blob/master/source/Relativity.Testing.Framework.Api/Strategies/CreateWorkspaceEntityStrategy%601.cs).

---

# Using REST or other core strategies inside of the custom strategy

Some times we need to use a different strategy inside of the strategy that we are creating.
To accomplish this, create a new constructor that will initialize the needed strategy.

```
public class CustomCreateStrategy : CreateWorkspaceEntityStrategy<CustomModel>
{
    private readonly IRestService _restService;
 
    public CustomCreateStrategy (IRestService restService)
    {
        _restService = restService;
    }
 
    protected override CustomModel DoCreate(int workspaceId, CustomModel entity)
    {
        return _restService.Post<int>("Relativity.Services.Search.ISearchModule/Keyword%20Search%20Manager/CreateSingleAsync", entity);
    }
}
```

# Using Interceptors

Interceptors are used to provide functionality to strategies in a easy and scalable way.
The common [interceptors](/api/Relativity.Testing.Framework.Api.Interceptors.html) get installed to most of the strategies when they are registered.

```
public class TestComponent : IRelativityComponent, IWindsorInstaller
{
    private readonly Type[] _commonInterceptors = new[]
    {
        typeof(ApplicationInsightsInterceptor),
        typeof(LoggingInterceptor)
    };
     
    public void Ensure(IWindsorContainer container)
    {
        // There is nothing to ensure here.
    }
 
    public void Initialize(IWindsorContainer container)
    {
        container.Install(this);
    }
 
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
        container.Register(Component.For<IGetWorkspaceEntityByIdStrategy<NewKeywordSearch>>().
            ImplementedBy<NewGetByIdStrategy>().
            Interceptors(_commonInterceptors).
            LifestyleSingleton());
    }
}
```
