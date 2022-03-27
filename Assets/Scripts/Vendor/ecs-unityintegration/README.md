[![gitter](https://img.shields.io/gitter/room/leopotam/ecs.svg)](https://gitter.im/leopotam/ecs)
[![license](https://img.shields.io/github/license/Leopotam/ecs-ui.svg)](https://github.com/Leopotam/ecs-ui/blob/develop/LICENSE)
# Unity integration for Entity Component System framework
[Unity integration](https://github.com/Leopotam/ecs-unityintegration) for [ECS framework](https://github.com/Leopotam/ecs).

> **Important! It's "structs-based" version, if you search "classes-based" version - check [classes-based branch](https://github.com/Leopotam/ecs-unityintegration/tree/classes-based)!**

> C#7.3 or above required for this framework.

> Tested on unity 2019.1 (dependent on Unity engine) and contains assembly definition for compiling to separate assembly file for performance reason.

> Dependent on [ECS framework](https://github.com/Leopotam/ecs) - ECS framework should be imported to unity project first.

# Installation

## As unity module
This repository can be installed as unity module directly from git url. In this way new line should be added to `Packages/manifest.json`:
```
"com.leopotam.ecs-unityintegration": "https://github.com/Leopotam/ecs-unityintegration.git",
```
By default last released version will be used. If you need trunk / developing version then `develop` name of branch should be added after hash:
```
"com.leopotam.ecs-unityintegration": "https://github.com/Leopotam/ecs-unityintegration.git#develop",
```

## As source
If you can't / don't want to use unity modules, code can be downloaded as sources archive of required release from [Releases page](`https://github.com/Leopotam/ecs-unityintegration/releases`).

# Editor integration

## EcsWorld observer
Integration can be processed with one call of `LeopotamGroup.Ecs.UnityIntegration.EcsWorldObserver.Create()` metod - this call should be wrapped to `#if UNITY_EDITOR` preprocessor define:
```csharp
public class Startup : MonoBehaviour {
    EcsSystems _systems;

    void Start () {
        var world = new EcsWorld ();
        
#if UNITY_EDITOR
        UnityIntegration.EcsWorldObserver.Create (world);
#endif  
        _systems = new EcsSystems(world)
            .Add (new RunSystem1());
            // Additional initialization here...
        _systems.Initialize ();
    }
}
```

Observer **must** be created before any entity will be created in ecs-world.

## EcsSystems observer
Integration can be processed with one call of `LeopotamGroup.Ecs.UnityIntegration.EcsSystemsObserver.Create()` metod - this call should be wrapped to `#if UNITY_EDITOR` preprocessor define:
```csharp
public class Startup : MonoBehaviour {
    EcsSystems _systems;

    void Start () {
        var world = new EcsWorld ();
        
#if UNITY_EDITOR
        UnityIntegration.EcsWorldObserver.Create (world);
#endif        
        _systems = new EcsSystems(world)
            .Add (new RunSystem1());
            // Additional initialization here...
        _systems.Initialize ();
#if UNITY_EDITOR
        UnityIntegration.EcsSystemsObserver.Create (_systems);
#endif
    }
}
```

# FAQ

### I can't edit component fields at any ecs-entity observer.
By design, observer works as readonly copy of ecs world data - you can copy value, but not change it.

### I want to create custom inspector view for my component.
Custom component `MyComponent1`:
```csharp
public enum MyEnum { True, False }

public class MyComponent1 {
    public MyEnum State;
    public string Name;
}
```
Inspector for `MyComponent1` (should be placed in `Editor` folder):
```csharp
class MyComponent1Inspector : IEcsComponentInspector {
    Type IEcsComponentInspector.GetFieldType () {
        return typeof (MyComponent1);
    }

    void IEcsComponentInspector.OnGUI (string label, object value, EcsWorld world, ref EcsEntity entityId) {
        var component = value as MyComponent1;
        EditorGUILayout.LabelField (label, EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.EnumPopup ("State", component.State);
        EditorGUILayout.TextField ("Name", component.Name);
        EditorGUI.indentLevel--;
    }
}
```

# License
The software released under the terms of the [MIT license](./LICENSE.md).

No support or any guarantees, no personal help.

# Special thanks (List sorted in back order, from high to low donations)
* [VirtualMaestro](https://github.com/VirtualMaestro)
* [Korchoon](https://github.com/korchoon)
* [PureEmDe](https://github.com/PureEmDee)
* [SH42913](https://github.com/SH42913)
* [Svetlozar Valchev](https://github.com/SvetlozarValchev)
* [Óscar F Gómez S](https://github.com/Racso)