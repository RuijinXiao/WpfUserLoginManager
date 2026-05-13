# WpfUserLoginManager

一个基于 C#、WPF 和 MVVM 的本地用户登录与用户管理工具。项目采用深色界面风格，用户数据保存为 CSV，登录配置保存为 JSON，适合作为小型桌面管理软件的基础模板继续扩展。

## 项目特性

- WPF 桌面应用，目标框架为 `.NET 8`
- 使用 MVVM 结构组织代码
- 深色科技风界面，主色为黑色，强调色为橙色
- 无系统标题栏窗口，支持拖拽移动
- 主界面支持用户名、密码登录
- 可配置是否需要密码登录
- 内置超级管理员账号
- 支持 5 级权限用户进入用户管理界面
- 用户数据保存为 CSV 文件
- 登录信息保存为 JSON 文件
- 普通用户密码使用 SHA256 哈希保存
- 用户管理支持新增、修改、删除、查询、刷新
- 新增和修改用户时使用独立弹窗
- 密码输入框支持眼睛图标显示/隐藏明文
- 应用程序和窗口配置了 `.ico` 图标资源

## 技术栈

- C#
- WPF
- .NET 8
- MVVM
- CSV 本地持久化
- JSON 本地配置
- SHA256 密码哈希

## 运行环境

建议环境：

- Windows 10 或更高版本
- Visual Studio 2026
- .NET 8 SDK 或更高版本

命令行构建：

```powershell
dotnet build WpfUserLoginManager.slnx
```

运行方式：

1. 使用 Visual Studio 打开 `WpfUserLoginManager.slnx`
2. 选择 Debug 或 Release 配置
3. 点击启动运行

## 默认账号

项目内置超级管理员账号，定义位置：

```text
Services/SuperAdminProvider.cs
```

当前默认值：

```text
用户名：superadmin
密码：SuperAdmin
权限等级：5
```

超级管理员不保存在 CSV 中，也不会写入登录 JSON。

## 权限规则

用户等级范围为 `1-5`。

- `1-4` 级用户：普通用户
- `5` 级用户：高级权限用户
- 超级管理员：代码内置账号，固定拥有 5 级权限

当前逻辑：

- 超级管理员登录后直接进入用户管理界面
- 5 级普通用户登录成功后，会提示是否进入用户管理界面
- 普通用户登录成功后会写入当前登录信息

## 登录配置

用户管理界面中提供“登录时需要密码”设置。

配置文件保存位置：

```text
程序运行目录/Config/appsettings.json
```

配置格式：

```json
{
  "RequirePassword": true
}
```

规则：

- `RequirePassword = true`：登录时需要用户名和密码
- `RequirePassword = false`：登录时只需要用户名

主登录界面会在启动时读取该配置：

- 需要密码时显示密码框
- 不需要密码时隐藏密码框

## 数据文件

### 用户数据

用户数据保存为 CSV：

```text
程序运行目录/Data/users.csv
```

字段：

```text
UserId,UserName,PasswordHash,Level,CreatedAt,LastLoginAt
```

说明：

- `UserId` 是显示序号，每次保存时会从 `1` 开始重新编号
- `UserName` 是用户名
- `PasswordHash` 是 SHA256 密码哈希
- `Level` 是权限等级，范围为 `1-5`
- `CreatedAt` 是创建时间
- `LastLoginAt` 是最后登录时间

### 登录记录

普通用户登录成功后写入：

```text
程序运行目录/Result/current_login.json
```

记录内容包括：

- 用户ID
- 用户名
- 等级
- 是否超级管理员
- 登录时间

注意：超级管理员登录不会写入该 JSON。

## 用户管理功能

用户管理界面支持：

- 查看所有用户
- 新增用户
- 修改选中用户
- 删除选中用户
- 按用户名或序号查询用户
- 刷新用户列表
- 切换是否需要密码登录
- 退出用户管理界面并返回登录界面

新增用户：

- 弹出用户编辑窗口
- 输入用户名、密码、等级
- 密码保存为 SHA256 哈希
- 操作前会弹出确认提示

修改用户：

- 弹出用户编辑窗口
- 可修改用户名、密码、等级
- 密码留空表示不修改原密码
- 操作前会弹出确认提示

删除用户：

- 删除当前选中的用户
- 操作前会弹出确认提示
- 保存后用户序号会重新从 `1` 开始排列

## 界面说明

项目包含三个主要窗口：

```text
MainWindow.xaml              登录界面
UserManagementWindow.xaml    用户管理界面
UserEditWindow.xaml          新增/修改用户弹窗
```

窗口特性：

- 使用 `WindowStyle="None"` 隐藏系统标题栏
- 使用 `ResizeMode="NoResize"` 固定窗口尺寸
- 通过鼠标拖拽根容器移动窗口
- 登录界面提供登录和退出按钮
- 用户管理界面提供刷新和退出按钮

## 图标资源

图标文件位于：

```text
Assets/
```

当前使用：

- `account.ico`：应用程序图标、登录窗口图标
- `user-group.ico`：用户管理窗口图标
- `account-plus.ico`：新增/修改用户窗口图标

应用程序图标配置在：

```text
WpfUserLoginManager.csproj
```

```xml
<ApplicationIcon>Assets\account.ico</ApplicationIcon>
```

## 项目结构

```text
WpfUserLoginManager/
├── Assets/                     图标资源
├── Commands/                   命令封装
│   └── RelayCommand.cs
├── Models/                     数据模型
│   ├── AppSettings.cs
│   ├── AppUser.cs
│   └── LoginRecord.cs
├── Services/                   业务服务
│   ├── AppSettingsService.cs
│   ├── AuthenticationService.cs
│   ├── LoginRecordService.cs
│   ├── PasswordHasher.cs
│   ├── SuperAdminProvider.cs
│   └── UserCsvService.cs
├── ViewModels/                 视图模型
│   ├── LoginViewModel.cs
│   ├── UserManagementViewModel.cs
│   └── ViewModelBase.cs
├── App.xaml                    全局样式资源
├── MainWindow.xaml             登录窗口
├── UserManagementWindow.xaml   用户管理窗口
├── UserEditWindow.xaml         用户编辑窗口
├── WpfUserLoginManager.csproj
└── WpfUserLoginManager.slnx
```

## 核心代码说明

### 认证

认证逻辑位于：

```text
Services/AuthenticationService.cs
```

职责：

- 读取登录配置
- 判断是否需要校验密码
- 校验超级管理员账号
- 校验 CSV 中的普通用户
- 更新普通用户最后登录时间

### 密码哈希

密码哈希逻辑位于：

```text
Services/PasswordHasher.cs
```

使用 SHA256：

```csharp
SHA256.HashData(Encoding.UTF8.GetBytes(password))
```

### CSV 读写

CSV 读写逻辑位于：

```text
Services/UserCsvService.cs
```

职责：

- 创建默认 CSV 文件
- 读取用户列表
- 保存用户列表
- 处理 CSV 转义
- 保存时重新生成用户序号

### 登录记录

登录记录逻辑位于：

```text
Services/LoginRecordService.cs
```

职责：

- 创建 `Result` 文件夹
- 写入 `current_login.json`

## 已知注意事项

- 当前数据存储为本地 CSV，适合小型本地工具，不适合多人并发写入
- SHA256 未加盐，正式生产环境建议升级为带盐哈希或专用密码哈希算法
- 超级管理员账号写在代码中，后续可改为安全配置或初始化流程
- 当前没有角色表，权限只通过 `Level` 数字判断
- 当前没有完整日志系统，可后续增加操作审计日志

## 后续可扩展方向

- 增加用户启用/禁用状态
- 增加操作日志
- 增加密码重置功能
- 增加用户搜索条件
- 增加导入/导出用户
- 增加数据库存储，例如 SQLite
- 增加配置界面
- 增加打包发布流程
- 增加单元测试
- 增加更完整的权限控制

## GitHub 仓库

```text
https://github.com/RuijinXiao/WpfUserLoginManager
```
