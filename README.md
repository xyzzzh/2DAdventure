# 2DAdventure
Ref: https://learn.u3d.cn/tutorial/2DAdventure

2023.3.21：完成人物移动、翻转及跳跃

2023.3.26：完成【第二章 Player 人物创建】

- UnityEvent事件系统
- InputSystem
- Animation 动画状态机

有限状态机：一个时间段内，只执行一个特定的状态

UnityEvent实现流程
> 1. 在Scripts下创建XXXSO.cs
> 2. 在Events下创建对应Event
> 3. 创建负责control的c#，挂载到触发Event的GameObject上
> 4. 在control.cs中编写触发广播(RaisedEvent)的逻辑代码
> 5. 在负责监听的C#编写监听Event及监听后执行逻辑的代码(OnEnable中注册方法等)