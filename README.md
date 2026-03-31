# 《火力全开》Unity 项目骨架

这是一个基于 Unity 2022 LTS 的 3D 射击游戏基础工程，面向 PC 平台，包含玩家控制、武器系统、敌人 AI、HUD、交互和对象池等核心模块。

## 已实现内容

- 第一人称玩家移动、视角控制、跳跃和重力
- 射线式武器系统，支持开火、换弹、切枪和基础命中判定
- `ScriptableObject` 武器配置
- 基于 `NavMeshAgent` 的敌人巡逻、索敌、追击和近战攻击
- 生命值、伤害、死亡和击杀得分事件
- 基于 UI Toolkit 的 HUD 模板
- 简单敌人生成器和对象池
- Unity Editor 菜单，一键生成示例武器资源

## 目录结构

- `Assets/Scripts/Core`：事件总线、对象池等基础设施
- `Assets/Scripts/Player`：玩家控制与交互
- `Assets/Scripts/Combat`：生命值、伤害与武器逻辑
- `Assets/Scripts/AI`：敌人 AI 与刷怪
- `Assets/Scripts/UI`：HUD 控制器
- `Assets/Scripts/Editor`：编辑器快捷工具
- `Assets/UI`：UI Toolkit 的 UXML/USS 模板
- `Assets/Scenes`：场景搭建说明

## Unity 打开方式

1. 使用 Unity Hub 选择 `Open`。
2. 指向当前目录 `E:\毕设`。
3. 使用 Unity `2022.3.x LTS` 打开。
4. 首次打开后等待 Package Manager 导入依赖。

## 建议的场景层级

1. `GameRoot`
2. `Player`
3. `Main Camera`
4. `Directional Light`
5. `Environment`
6. `EnemySpawners`
7. `UIDocument`

## 快速挂载步骤

### 玩家

1. 创建一个 `Capsule` 作为玩家对象，命名为 `Player`。
2. 添加 `CharacterController`。
3. 添加脚本 `PlayerController`、`InteractionDetector`、`Health`。
4. 将 `Health.Is Player` 勾选。
5. 给 `Player` 对象打上 `Player` Tag。
6. 把 `Main Camera` 拖到 `PlayerController.Player Camera`。
7. 将相机作为玩家子物体，位置大约设置为 `(0, 1.6, 0)`。
8. 在玩家上挂载 `WeaponController`，把相机拖到 `Aim Camera`。

### 武器

1. 在 Unity 菜单点击 `火力全开/Generate Sample Assets`。
2. 生成后把 `Assets/Data/Weapons/AssaultRifle.asset` 拖入 `WeaponController.Loadout`。
3. 可继续复制该资源做手枪、霰弹枪等变体。

### 敌人

1. 创建一个带 `CapsuleCollider` 的敌人物体。
2. 添加 `NavMeshAgent`、`Health`、`EnemyAIController`。
3. 给敌人设置 `Score Value On Death`。
4. 用几个空物体作为巡逻点，拖给 `Patrol Points`。
5. 给地面烘焙 NavMesh。

### 交互物

1. 创建一个门或任意可旋转物体。
2. 挂载 `RotatingDoorInteractable`。
3. 玩家靠近后按 `E` 即可触发开关门演示。

### HUD

1. 创建 `Panel Settings` 资源。
2. 新建一个空物体 `HUD`，添加 `UIDocument` 和 `HUDController`。
3. 将 `Assets/UI/HUD.uxml` 指定到 `UIDocument.Source Asset`。
4. 将 `Panel Settings` 指定到 `UIDocument.Panel Settings`。
5. 用 UI Builder 打开 `Assets/UI/HUD.uxml`，把 `Assets/UI/HUD.uss` 加入 StyleSheets 列表。

## 默认操作

- `WASD`：移动
- `Mouse`：视角
- `Space`：跳跃
- `Left Mouse`：射击
- `R`：换弹
- `1/2/3`：切换武器
- `E`：交互

## 后续建议

- 用 Animator 给玩家与敌人接入动作状态机
- 为不同武器增加开镜、后坐力、散布和音效
- 扩展敌人行为树，实现掩体、包抄和远程攻击
- 补充开始菜单、结算页和关卡数据保存
