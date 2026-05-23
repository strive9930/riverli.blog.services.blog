# ThriveX 博客系统 - 功能需求规格说明书

> **用途**: 本文档用于指导AI智能体理解和实现ThriveX博客系统的各项功能
> **目标框架**: .NET 8 + ASP.NET Core + Entity Framework Core
> **数据库**: MySQL 8.0

---

## 📋 目录

1. [系统概述](#系统概述)
2. [核心功能模块](#核心功能模块)
3. [详细功能说明](#详细功能说明)
4. [数据模型](#数据模型)
5. [API接口规范](#api接口规范)
6. [业务规则](#业务规则)
7. [权限控制](#权限控制)
8. [第三方集成](#第三方集成)

---

## 系统概述

### 项目定位
ThriveX是一个现代化的个人博客管理系统，提供文章管理、分类标签、评论系统、留言板、相册展示等完整的博客功能。

### 技术架构
- **后端**: Java Spring Boot 2.7.12 → .NET 8 ASP.NET Core
- **ORM**: MyBatis-Plus → Entity Framework Core
- **数据库**: MySQL 8.0
- **认证**: JWT Token
- **文件存储**: 本地存储 + 云存储（阿里云、腾讯云、七牛云等）
- **邮件服务**: SMTP邮件发送

---

## 核心功能模块

### 模块清单

| 模块名称 | 功能描述 | 优先级 |
|---------|---------|--------|
| 用户管理 | 用户注册、登录、信息管理 | P0 |
| 文章管理 | 文章CRUD、发布、加密、置顶 | P0 |
| 分类管理 | 文章分类的增删改查 | P0 |
| 标签管理 | 文章标签的增删改查 | P0 |
| 评论系统 | 文章评论、回复、审核 | P0 |
| 留言墙 | 公开留言板、分类、审核 | P1 |
| 相册管理 | 图片上传、分类展示 | P1 |
| 友情链接 | 友链申请、审核、展示 | P1 |
| 轮播图 | 首页轮播图管理 | P2 |
| 足迹记录 | 个人足迹时间轴 | P2 |
| 备忘录 | 日常记录、便签 | P2 |
| RSS订阅 | RSS源管理和聚合 | P2 |
| 系统配置 | 网站配置、页面配置 | P1 |
| 文件管理 | 文件上传、存储管理 | P0 |
| 邮件通知 | 评论通知、审核通知 | P1 |

---

## 详细功能说明

### 1. 用户管理模块 (User Management)

#### 1.1 功能列表

| 功能点 | 请求方式 | 路径 | 权限 | 描述 |
|-------|---------|------|------|------|
| 用户注册 | POST | /api/user | 公开 | 创建新用户账号 |
| 用户登录 | POST | /api/user/login | 公开 | 用户登录获取Token |
| 获取用户信息 | GET | /api/user/{id} | 登录 | 获取指定用户信息 |
| 修改用户信息 | PATCH | /api/user | 登录 | 修改当前用户信息 |
| 删除用户 | DELETE | /api/user/{id} | 管理员 | 删除指定用户 |
| 批量删除用户 | DELETE | /api/user/batch | 管理员 | 批量删除用户 |
| 用户列表 | POST | /api/user/paging | 管理员 | 分页查询用户列表 |
| 修改密码 | PATCH | /api/user/pass | 登录 | 修改用户密码 |
| Token校验 | GET | /api/user/check | 公开 | 验证Token有效性 |
| 获取作者信息 | GET | /api/user/author | 公开 | 获取博主信息(ID=1) |

#### 1.2 业务规则

**用户注册**:
```
输入验证:
- username: 必填，长度1-50，唯一
- password: 必填，长度6-50，MD5加密存储
- name: 必填，长度1-50
- email: 可选，邮箱格式验证
- avatar: 可选，头像URL
- info: 可选，个人简介，最大255字符

处理流程:
1. 检查用户名是否已存在
2. 对密码进行MD5加密
3. 设置创建时间为当前时间戳(毫秒)
4. 插入数据库
5. 返回成功响应

异常处理:
- 用户名已存在: 返回400错误 "该用户已存在：{username}"
```

**用户登录**:
```
输入:
- username: 用户名
- password: 明文密码

处理流程:
1. 对密码进行MD5加密
2. 查询匹配的用户名和密码
3. 如果用户不存在，返回400错误 "用户名或密码错误"
4. 生成JWT Token（包含用户信息）
5. 删除该用户的旧Token
6. 保存新Token到user_token表
7. 返回用户信息和Token

Token配置:
- 有效期: 43200分钟(30天)
- 签名算法: HS256
- Claims包含: exp, user对象
```

**修改密码**:
```
输入:
- oldUsername: 原用户名
- newUsername: 新用户名
- oldPassword: 原密码(明文)
- newPassword: 新密码(明文)

处理流程:
1. 验证原用户名和原密码是否正确
2. 如果不正确，返回400错误 "用户名或旧密码错误"
3. 更新用户名和新密码(MD5加密)
4. 保存更改
```

#### 1.3 数据模型

```csharp
// User 实体
public class User : BaseModel
{
    public string Username { get; set; }      // 用户名，唯一
    public string Password { get; set; }      // MD5加密后的密码
    public string Name { get; set; }          // 用户昵称
    public string? Email { get; set; }        // 邮箱
    public string? Avatar { get; set; }       // 头像URL
    public string? Info { get; set; }         // 个人简介
}

// UserToken 实体
public class UserToken
{
    public int Id { get; set; }
    public int Uid { get; set; }              // 用户ID
    public string Token { get; set; }         // JWT Token
}
```

---

### 2. 文章管理模块 (Article Management)

#### 2.1 功能列表

| 功能点 | 请求方式 | 路径 | 权限 | 描述 |
|-------|---------|------|------|------|
| 创建文章 | POST | /api/article | 管理员 | 新增文章 |
| 获取文章 | GET | /api/article/{id} | 公开 | 获取文章详情(含密码验证) |
| 修改文章 | PATCH | /api/article/{id} | 管理员 | 编辑文章 |
| 删除文章 | DELETE | /api/article/{id} | 管理员 | 删除文章 |
| 批量删除 | DELETE | /api/article/batch | 管理员 | 批量删除文章 |
| 文章列表 | POST | /api/article/list | 公开 | 获取文章列表(支持筛选) |
| 分页查询 | POST | /api/article/paging | 公开 | 分页查询文章 |
| 增加浏览量 | PATCH | /api/article/view/{id} | 公开 | 文章浏览量+1 |
| 随机文章 | GET | /api/article/random | 公开 | 获取随机文章列表 |
| 归档统计 | GET | /api/article/archive | 公开 | 按年月统计文章数 |
| 搜索文章 | POST | /api/article/search | 公开 | 关键词搜索 |
| 关联数据 | POST | /api/article/data | 管理员 | 批量关联分类标签 |

#### 2.2 业务规则

**文章创建**:
```
输入字段:
- title: 标题，必填
- description: 描述/摘要
- content: 内容，支持Markdown/HTML
- cover: 封面图URL
- config: 配置对象(JSON)
  - isEncrypt: 是否加密(0/1)
  - password: 访问密码(如果加密)
  - status: 状态(publish/hide/draft)
  - isTop: 是否置顶(0/1)
  - isDel: 是否删除(0/1)
- cates: 分类ID列表
- tags: 标签ID列表

处理流程:
1. 验证必填字段
2. 设置默认config值
3. 保存文章基本信息
4. 关联分类(article_cate表)
5. 关联标签(article_tag表)
6. 初始化浏览量为0
```

**文章查询(含权限控制)**:
```
权限判断逻辑:
1. 检查请求头中的Authorization Token
2. 如果是管理员(Token有效):
   - 可以查看所有文章(包括隐藏、删除的)
   - 可以直接查看加密文章内容
3. 如果是普通用户:
   - 只能查看status='publish'且isDel=0的文章
   - 如果文章加密(isEncrypt=1):
     a. 未传递密码: 返回提示 "请输入文章访问密码"
     b. 密码错误: 返回401错误 "文章访问密码错误"
     c. 密码正确: 返回完整内容
   - 如果文章被隐藏: 返回403错误 "该文章已被隐藏"
   - 如果文章被删除: 返回404错误 "该文章已被删除"

返回数据增强:
- prev: 上一篇文章(id, title)
- next: 下一篇文章(id, title)
- viewCount: 浏览量
- cateNames: 分类名称列表
- tagNames: 标签名称列表
```

**文章筛选条件**:
```
支持的筛选参数:
- keyword: 关键词搜索(标题、内容)
- cateId: 分类ID筛选
- tagId: 标签ID筛选
- status: 状态筛选(publish/hide/draft)
- isTop: 是否置顶
- dateRange: 日期范围[start, end]
- sortBy: 排序字段(createTime/viewCount)
- sortOrder: 排序方式(asc/desc)

默认排序:
- 置顶文章优先(isTop DESC)
- 然后按创建时间倒序(createTime DESC)
```

**浏览量统计**:
```
每次访问文章详情时:
1. 调用 /api/article/view/{id}
2. 文章viewCount字段 +1
3. 异步更新，不阻塞主流程
```

#### 2.3 数据模型

```csharp
// Article 实体
public class Article : BaseModel
{
    public string Title { get; set; }           // 标题
    public string? Description { get; set; }    // 摘要
    public string Content { get; set; }         // 内容
    public string? Cover { get; set; }          // 封面图
    public int ViewCount { get; set; } = 0;    // 浏览量
    
    // JSON配置字段
    public ArticleConfig Config { get; set; }   // 文章配置
    
    // 导航属性
    public virtual ICollection<ArticleCategory> Categories { get; set; }
    public virtual ICollection<ArticleTag> Tags { get; set; }
    
    // 非持久化字段
    [NotMapped]
    public object? Prev { get; set; }           // 上一篇
    [NotMapped]
    public object? Next { get; set; }           // 下一篇
}

// ArticleConfig 配置类
public class ArticleConfig
{
    public int IsEncrypt { get; set; } = 0;     // 是否加密
    public string? Password { get; set; }       // 访问密码
    public string Status { get; set; } = "publish"; // 状态
    public int IsTop { get; set; } = 0;         // 是否置顶
    public int IsDel { get; set; } = 0;         // 是否删除
}

// ArticleCategory 关联表
public class ArticleCategory
{
    public int ArticleId { get; set; }
    public int CategoryId { get; set; }
    public virtual Article Article { get; set; }
    public virtual Category Category { get; set; }
}

// ArticleTag 关联表
public class ArticleTag
{
    public int ArticleId { get; set; }
    public int TagId { get; set; }
    public virtual Article Article { get; set; }
    public virtual Tag Tag { get; set; }
}
```

---

### 3. 分类管理模块 (Category Management)

#### 3.1 功能列表

| 功能点 | 请求方式 | 路径 | 权限 | 描述 |
|-------|---------|------|------|------|
| 创建分类 | POST | /api/cate | 管理员 | 新增分类 |
| 获取分类 | GET | /api/cate/{id} | 公开 | 获取分类详情 |
| 修改分类 | PATCH | /api/cate/{id} | 管理员 | 编辑分类 |
| 删除分类 | DELETE | /api/cate/{id} | 管理员 | 删除分类 |
| 批量删除 | DELETE | /api/cate/batch | 管理员 | 批量删除 |
| 分类列表 | POST | /api/cate/list | 公开 | 获取所有分类 |
| 分页查询 | POST | /api/cate/paging | 公开 | 分页查询 |
| 树形结构 | GET | /api/cate/tree | 公开 | 获取分类树(支持父子级) |
| 文章统计 | GET | /api/cate/count | 公开 | 各分类文章数量 |

#### 3.2 业务规则

**分类特性**:
```
- 支持多级分类(通过parentId实现)
- 每个分类可以有父分类
- 删除分类时，需要处理关联的文章:
  方案1: 禁止删除有关联文章的分类
  方案2: 解除关联后删除
- 分类名称唯一性约束

树形结构返回格式:
{
  "id": 1,
  "name": "技术",
  "parentId": 0,
  "children": [
    {
      "id": 2,
      "name": "前端",
      "parentId": 1,
      "children": []
    }
  ]
}
```

#### 3.3 数据模型

```csharp
public class Category : BaseModel
{
    public string Name { get; set; }            // 分类名称
    public string? Icon { get; set; }           // 图标
    public int ParentId { get; set; } = 0;      // 父分类ID，0表示顶级
    
    // 导航属性
    public virtual Category? Parent { get; set; }
    public virtual ICollection<Category> Children { get; set; }
    public virtual ICollection<ArticleCategory> Articles { get; set; }
    
    // 非持久化
    [NotMapped]
    public int ArticleCount { get; set; }       // 文章数量
}
```

---

### 4. 标签管理模块 (Tag Management)

#### 4.1 功能列表

| 功能点 | 请求方式 | 路径 | 权限 | 描述 |
|-------|---------|------|------|------|
| 创建标签 | POST | /api/tag | 管理员 | 新增标签 |
| 获取标签 | GET | /api/tag/{id} | 公开 | 获取标签详情 |
| 修改标签 | PATCH | /api/tag/{id} | 管理员 | 编辑标签 |
| 删除标签 | DELETE | /api/tag/{id} | 管理员 | 删除标签 |
| 批量删除 | DELETE | /api/tag/batch | 管理员 | 批量删除 |
| 标签列表 | POST | /api/tag/list | 公开 | 获取所有标签 |
| 分页查询 | POST | /api/tag/paging | 公开 | 分页查询 |
| 热门标签 | GET | /api/tag/hot | 公开 | 获取热门标签(按使用次数) |

#### 4.2 业务规则

```
- 标签名称唯一
- 标签没有层级关系(扁平结构)
- 删除标签时，自动解除与文章的关联
- 热门标签按关联文章数量排序，取前10个
```

#### 4.3 数据模型

```csharp
public class Tag : BaseModel
{
    public string Name { get; set; }            // 标签名称
    public string? Color { get; set; }          // 标签颜色
    
    // 导航属性
    public virtual ICollection<ArticleTag> Articles { get; set; }
    
    // 非持久化
    [NotMapped]
    public int ArticleCount { get; set; }       // 使用次数
}
```

---

### 5. 评论系统模块 (Comment System)

#### 5.1 功能列表

| 功能点 | 请求方式 | 路径 | 权限 | 描述 |
|-------|---------|------|------|------|
| 发表评论 | POST | /api/comment | 公开 | 发表新评论(需审核) |
| 获取评论 | GET | /api/comment/{id} | 公开 | 获取评论详情 |
| 删除评论 | DELETE | /api/comment/{id} | 管理员 | 删除评论 |
| 批量删除 | DELETE | /api/comment/batch | 管理员 | 批量删除 |
| 评论列表 | POST | /api/comment/list | 公开 | 获取评论列表(文章/全站) |
| 分页查询 | POST | /api/comment/paging | 管理员 | 后台分页管理 |
| 审核评论 | PATCH | /api/comment/audit/{id} | 管理员 | 通过/拒绝评论 |
| 回复评论 | POST | /api/comment/reply | 公开 | 回复某条评论 |

#### 5.2 业务规则

**评论发表**:
```
输入字段:
- articleId: 文章ID(如果是文章评论)
- content: 评论内容，必填
- email: 评论者邮箱，必填
- name: 评论者昵称，必填
- url: 评论者网站，可选
- parentId: 父评论ID(回复时填写)，0表示顶级评论
- avatar: 头像URL，可选

处理流程:
1. 验证必填字段
2. 如果email已在系统中存在，自动填充name和avatar
3. 设置审核状态为pending(待审核)
4. 保存评论
5. 发送邮件通知给博主(如果配置了邮箱)
6. 如果是回复评论，发送邮件通知给被回复者

防垃圾策略:
- IP限流: 同一IP每分钟最多5条评论
- 内容过滤: 检测敏感词
- 邮箱验证: 邮箱格式必须正确
```

**评论查询**:
```
筛选条件:
- articleId: 指定文章的评论
- status: 审核状态(approved/pending/rejected)
- type: 评论类型(article/wall)

返回数据结构:
- 树形结构: 顶级评论 + 回复列表
- 每条评论包含:
  - 评论者信息(name, email, avatar, url)
  - 评论内容和时间
  - 回复列表(replies)
  - 点赞数(如果有)

分页:
- 默认每页10条
- 支持按时间排序
```

**评论审核**:
```
审核状态:
- pending: 待审核(默认)
- approved: 已通过(前台显示)
- rejected: 已拒绝

审核操作:
1. 管理员修改status字段
2. 如果通过，发送邮件通知评论者
3. 如果拒绝，可选择填写拒绝原因
```

#### 5.3 数据模型

```csharp
public class Comment : BaseModel
{
    public int? ArticleId { get; set; }         // 文章ID(可为空，用于留言板)
    public string Content { get; set; }         // 评论内容
    public string Email { get; set; }           // 评论者邮箱
    public string Name { get; set; }            // 评论者昵称
    public string? Url { get; set; }            // 评论者网站
    public string? Avatar { get; set; }         // 头像
    public int ParentId { get; set; } = 0;      // 父评论ID
    public string Status { get; set; } = "pending"; // 审核状态
    public string? Ip { get; set; }             // IP地址
    public string? UserAgent { get; set; }      // 浏览器信息
    
    // 导航属性
    public virtual Article? Article { get; set; }
    public virtual Comment? Parent { get; set; }
    public virtual ICollection<Comment> Replies { get; set; }
    
    // 非持久化
    [NotMapped]
    public List<Comment> Children { get; set; } // 子评论列表
}
```

---

### 6. 留言墙模块 (Wall Management)

#### 6.1 功能列表

| 功能点 | 请求方式 | 路径 | 权限 | 描述 |
|-------|---------|------|------|------|
| 发表留言 | POST | /api/wall | 公开 | 发表新留言 |
| 获取留言 | GET | /api/wall/{id} | 公开 | 获取留言详情 |
| 删除留言 | DELETE | /api/wall/{id} | 管理员 | 删除留言 |
| 批量删除 | DELETE | /api/wall/batch | 管理员 | 批量删除 |
| 留言列表 | POST | /api/wall/list | 公开 | 获取留言列表 |
| 分页查询 | POST | /api/wall/paging | 管理员 | 后台分页管理 |
| 留言分类 | GET | /api/wall/cates | 公开 | 获取留言分类 |
| 发送邮件 | POST | /api/wall/email | 公开 | 给博主发邮件 |

#### 6.2 业务规则

```
留言墙特点:
- 类似评论区，但独立于文章
- 支持分类(心情、祝福、建议等)
- 需要审核后才能显示
- 支持匿名留言
- 可以给博主发送邮件

留言分类:
- 由wall_cate表管理
- 每个留言属于一个分类
- 分类可自定义
```

#### 6.3 数据模型

```csharp
public class Wall : BaseModel
{
    public string Content { get; set; }         // 留言内容
    public string Email { get; set; }           // 邮箱
    public string Name { get; set; }            // 昵称
    public string? Url { get; set; }            // 网站
    public string? Avatar { get; set; }         // 头像
    public int CateId { get; set; }             // 分类ID
    public string Status { get; set; } = "pending"; // 审核状态
    public string? Ip { get; set; }
    
    // 导航属性
    public virtual WallCate Category { get; set; }
}

public class WallCate : BaseModel
{
    public string Name { get; set; }            // 分类名称
    public string? Icon { get; set; }           // 图标
    public string? Color { get; set; }          // 颜色
}
```

---

### 7. 相册管理模块 (Album Management)

#### 7.1 功能列表

| 功能点 | 请求方式 | 路径 | 权限 | 描述 |
|-------|---------|------|------|------|
| 创建相册分类 | POST | /api/album/cate | 管理员 | 新增相册分类 |
| 获取分类 | GET | /api/album/cate/{id} | 公开 | 获取分类详情 |
| 修改分类 | PATCH | /api/album/cate/{id} | 管理员 | 编辑分类 |
| 删除分类 | DELETE | /api/album/cate/{id} | 管理员 | 删除分类及图片 |
| 分类列表 | GET | /api/album/cates | 公开 | 获取所有分类 |
| 上传图片 | POST | /api/album/image | 管理员 | 上传照片到相册 |
| 获取图片 | GET | /api/album/image/{id} | 公开 | 获取图片详情 |
| 删除图片 | DELETE | /api/album/image/{id} | 管理员 | 删除图片 |
| 图片列表 | POST | /api/album/images | 公开 | 获取分类下的图片 |

#### 7.2 业务规则

```
相册结构:
- album_cate: 相册分类(如旅行、生活、工作)
- album_image: 具体图片，属于某个分类

图片上传:
- 支持多图上传
- 自动生成缩略图
- 存储到配置的存储平台(本地/云存储)
- 记录图片元数据(大小、尺寸、上传时间)

图片展示:
- 支持懒加载
- 点击查看大图
- 支持幻灯片播放
```

#### 7.3 数据模型

```csharp
public class AlbumCate : BaseModel
{
    public string Name { get; set; }            // 分类名称
    public string? Description { get; set; }    // 描述
    public string? Cover { get; set; }          // 封面图
    
    // 导航属性
    public virtual ICollection<AlbumImage> Images { get; set; }
}

public class AlbumImage : BaseModel
{
    public int CateId { get; set; }             // 分类ID
    public string Url { get; set; }             // 图片URL
    public string? Thumbnail { get; set; }      // 缩略图URL
    public string? Description { get; set; }    // 描述
    public int Size { get; set; }               // 文件大小(字节)
    public int Width { get; set; }              // 宽度
    public int Height { get; set; }             // 高度
    
    // 导航属性
    public virtual AlbumCate Category { get; set; }
}
```

---

### 8. 友情链接模块 (Link Management)

#### 8.1 功能列表

| 功能点 | 请求方式 | 路径 | 权限 | 描述 |
|-------|---------|------|------|------|
| 申请友链 | POST | /api/link | 公开 | 提交友链申请 |
| 获取友链 | GET | /api/link/{id} | 公开 | 获取友链详情 |
| 修改友链 | PATCH | /api/link/{id} | 管理员 | 编辑友链 |
| 删除友链 | DELETE | /api/link/{id} | 管理员 | 删除友链 |
| 批量删除 | DELETE | /api/link/batch | 管理员 | 批量删除 |
| 友链列表 | POST | /api/link/list | 公开 | 获取友链列表 |
| 分页查询 | POST | /api/link/paging | 管理员 | 后台分页管理 |
| 友链类型 | GET | /api/link/types | 公开 | 获取友链分类 |

#### 8.2 业务规则

```
友链申请:
- 需要填写: 网站名称、URL、Logo、描述
- 默认状态为pending(待审核)
- 管理员审核后显示

友链类型:
- 技术博客、朋友站点、推荐网站等
- 由link_type表管理

展示规则:
- 只显示approved状态的友链
- 可按类型分组展示
- 定期检查链接有效性(可选)
```

#### 8.3 数据模型

```csharp
public class Link : BaseModel
{
    public string Name { get; set; }            // 网站名称
    public string Url { get; set; }             // 网站URL
    public string? Logo { get; set; }           // Logo URL
    public string? Description { get; set; }    // 描述
    public int TypeId { get; set; }             // 类型ID
    public string Status { get; set; } = "pending"; // 审核状态
    public string? Email { get; set; }          // 站长邮箱
    
    // 导航属性
    public virtual LinkType Type { get; set; }
}

public class LinkType : BaseModel
{
    public string Name { get; set; }            // 类型名称
    public string? Icon { get; set; }           // 图标
}
```

---

### 9. 轮播图模块 (Swiper Management)

#### 9.1 功能列表

| 功能点 | 请求方式 | 路径 | 权限 | 描述 |
|-------|---------|------|------|------|
| 创建轮播图 | POST | /api/swiper | 管理员 | 新增轮播图 |
| 获取轮播图 | GET | /api/swiper/{id} | 公开 | 获取详情 |
| 修改轮播图 | PATCH | /api/swiper/{id} | 管理员 | 编辑轮播图 |
| 删除轮播图 | DELETE | /api/swiper/{id} | 管理员 | 删除轮播图 |
| 轮播图列表 | GET | /api/swipers | 公开 | 获取所有轮播图 |

#### 9.2 业务规则

```
轮播图字段:
- image: 图片URL
- title: 标题
- description: 描述
- url: 点击跳转链接
- sort: 排序号(越小越靠前)
- isActive: 是否启用

展示规则:
- 只返回isActive=true的轮播图
- 按sort字段升序排列
- 限制最多显示10张
```

#### 9.3 数据模型

```csharp
public class Swiper : BaseModel
{
    public string Image { get; set; }           // 图片URL
    public string? Title { get; set; }          // 标题
    public string? Description { get; set; }    // 描述
    public string? Url { get; set; }            // 跳转链接
    public int Sort { get; set; } = 0;          // 排序
    public bool IsActive { get; set; } = true;  // 是否启用
}
```

---

### 10. 足迹记录模块 (Footprint Management)

#### 10.1 功能列表

| 功能点 | 请求方式 | 路径 | 权限 | 描述 |
|-------|---------|------|------|------|
| 创建足迹 | POST | /api/footprint | 管理员 | 新增足迹 |
| 获取足迹 | GET | /api/footprint/{id} | 公开 | 获取详情 |
| 修改足迹 | PATCH | /api/footprint/{id} | 管理员 | 编辑足迹 |
| 删除足迹 | DELETE | /api/footprint/{id} | 管理员 | 删除足迹 |
| 足迹列表 | GET | /api/footprints | 公开 | 获取所有足迹(时间轴) |

#### 10.2 业务规则

```
足迹特点:
- 时间轴形式展示
- 包含: 时间、地点、内容、图片
- 按时间倒序排列
- 可在地图上展示(如果有经纬度)

足迹字段:
- time: 发生时间
- location: 地点名称
- content: 内容描述
- images: 图片列表(JSON数组)
- longitude: 经度(可选)
- latitude: 纬度(可选)
```

#### 10.3 数据模型

```csharp
public class Footprint : BaseModel
{
    public DateTime Time { get; set; }          // 发生时间
    public string Location { get; set; }        // 地点
    public string Content { get; set; }         // 内容
    public string Images { get; set; } = "[]";  // 图片JSON数组
    public decimal? Longitude { get; set; }     // 经度
    public decimal? Latitude { get; set; }      // 纬度
}
```

---

### 11. 备忘录模块 (Record Management)

#### 11.1 功能列表

| 功能点 | 请求方式 | 路径 | 权限 | 描述 |
|-------|---------|------|------|------|
| 创建备忘 | POST | /api/record | 管理员 | 新增备忘 |
| 获取备忘 | GET | /api/record/{id} | 公开 | 获取详情 |
| 修改备忘 | PATCH | /api/record/{id} | 管理员 | 编辑备忘 |
| 删除备忘 | DELETE | /api/record/{id} | 管理员 | 删除备忘 |
| 备忘列表 | GET | /api/records | 公开 | 获取所有备忘 |

#### 11.2 业务规则

```
备忘录特点:
- 类似微博/动态
- 简短的文字记录
- 可配图
- 按时间倒序展示

字段:
- content: 文本内容
- images: 图片列表(JSON数组)
- createTime: 创建时间
```

#### 11.3 数据模型

```csharp
public class Record : BaseModel
{
    public string Content { get; set; }         // 内容
    public string Images { get; set; } = "[]";  // 图片JSON数组
}
```

---

### 12. RSS订阅模块 (RSS Management)

#### 12.1 功能列表

| 功能点 | 请求方式 | 路径 | 权限 | 描述 |
|-------|---------|------|------|------|
| 添加RSS源 | POST | /api/rss | 管理员 | 新增RSS订阅源 |
| 获取RSS | GET | /api/rss/{id} | 公开 | 获取详情 |
| 修改RSS | PATCH | /api/rss/{id} | 管理员 | 编辑RSS源 |
| 删除RSS | DELETE | /api/rss/{id} | 管理员 | 删除RSS源 |
| RSS列表 | GET | /api/rsses | 公开 | 获取所有RSS源 |
| 抓取内容 | POST | /api/rss/fetch/{id} | 管理员 | 手动抓取最新文章 |
| 定时抓取 | - | 后台任务 | 系统 | 定时自动抓取 |

#### 12.2 业务规则

```
RSS功能:
- 订阅外部博客/网站的RSS feed
- 定时抓取最新内容
- 聚合展示在自己的博客上
- 可作为内容来源参考

RSS字段:
- name: 订阅源名称
- url: RSS feed地址
- logo: 网站Logo
- description: 描述
- lastFetchTime: 最后抓取时间
- isActive: 是否启用

定时任务:
- 每小时检查一次所有启用的RSS源
- 解析RSS XML
- 提取最新文章
- 存储到本地(可选)或直接展示
```

#### 12.3 数据模型

```csharp
public class Rss : BaseModel
{
    public string Name { get; set; }            // 订阅源名称
    public string Url { get; set; }             // RSS地址
    public string? Logo { get; set; }           // Logo
    public string? Description { get; set; }    // 描述
    public DateTime? LastFetchTime { get; set; } // 最后抓取时间
    public bool IsActive { get; set; } = true;  // 是否启用
}
```

---

### 13. 系统配置模块 (Configuration Management)

#### 13.1 功能列表

| 功能点 | 请求方式 | 路径 | 权限 | 描述 |
|-------|---------|------|------|------|
| 获取配置 | GET | /api/config/{name} | 公开 | 获取指定配置 |
| 更新配置 | PATCH | /api/config/{name} | 管理员 | 更新配置值 |
| 配置列表 | GET | /api/configs | 管理员 | 获取所有配置 |
| 页面配置 | GET | /api/page/config | 公开 | 获取页面个性化配置 |
| 更新JSON值 | PATCH | /api/config/json | 管理员 | 更新JSON类型配置 |

#### 13.2 业务规则

```
配置类型:
1. 系统配置(config表):
   - 网站基本信息(标题、描述、Logo)
   - SEO配置
   - 社交链接
   - 统计代码
   
2. 环境配置(env_config表):
   - 邮箱配置
   - 存储配置
   - 第三方API密钥
   
3. 页面配置(page_config表):
   - 首页布局
   - 侧边栏组件
   - 主题设置

配置存储:
- key-value形式
- value可以是字符串或JSON对象
- 使用JacksonTypeHandler处理JSON字段
```

#### 13.3 数据模型

```csharp
// 通用配置
public class Config : BaseModel
{
    public string Name { get; set; }            // 配置键名
    public string Value { get; set; }           // 配置值(JSON字符串)
    public string? Notes { get; set; }          // 备注
}

// 环境配置
public class EnvConfig : BaseModel
{
    public string Name { get; set; }            // 配置名称
    public string Value { get; set; }           // 配置值
}

// 页面配置
public class PageConfig : BaseModel
{
    public string Name { get; set; }            // 页面标识
    public string Config { get; set; }          // 页面配置JSON
}
```

---

### 14. 文件管理模块 (File Management)

#### 14.1 功能列表

| 功能点 | 请求方式 | 路径 | 权限 | 描述 |
|-------|---------|------|------|------|
| 上传文件 | POST | /api/file/upload | 管理员 | 上传文件 |
| 删除文件 | DELETE | /api/file/{id} | 管理员 | 删除文件记录 |
| 文件列表 | POST | /api/file/list | 管理员 | 获取文件列表 |
| 存储配置 | GET | /api/oss | 管理员 | 获取存储配置 |
| 更新配置 | PATCH | /api/oss/{id} | 管理员 | 更新存储配置 |

#### 14.2 业务规则

```
文件存储平台支持:
1. Local: 本地存储
2. Aliyun: 阿里云OSS
3. Tencent: 腾讯云COS
4. Qiniu: 七牛云Kodo
5. Huawei: 华为云OBS
6. Minio: Minio对象存储

上传流程:
1. 选择存储平台
2. 验证文件类型和大小
3. 生成唯一文件名
4. 上传到对应平台
5. 记录文件信息到file_detail表
6. 返回文件URL

文件限制:
- 图片: JPG, PNG, GIF, WebP, 最大10MB
- 文档: PDF, DOC, XLS, 最大50MB
- 其他类型可配置
```

#### 14.3 数据模型

```csharp
// OSS配置
public class Oss : BaseModel
{
    public string Platform { get; set; }        // 平台类型(local/aliyun/tencent等)
    public string AccessKey { get; set; }       // AccessKey
    public string SecretKey { get; set; }       // SecretKey
    public string Bucket { get; set; }          // 存储桶名称
    public string Domain { get; set; }          // 访问域名
    public string BasePath { get; set; }        // 基础路径
    public bool IsDefault { get; set; }         // 是否默认平台
}

// 文件详情
public class FileDetail : BaseModel
{
    public string FileName { get; set; }        // 原始文件名
    public string FilePath { get; set; }        // 存储路径
    public string FileUrl { get; set; }         // 访问URL
    public long FileSize { get; set; }          // 文件大小
    public string FileType { get; set; }        // 文件类型
    public string Platform { get; set; }        // 存储平台
}
```

---

## API接口规范

### 统一响应格式

```json
{
  "code": 200,
  "message": "操作成功",
  "data": {}
}
```

**状态码说明**:
- `200`: 成功
- `400`: 请求参数错误
- `401`: 未授权(Token无效)
- `403`: 禁止访问(权限不足)
- `404`: 资源不存在
- `429`: 请求过于频繁(限流)
- `500`: 服务器内部错误

**特殊业务状态码**:
- `610`: 文章不需要访问密码
- `611`: 文章已被隐藏
- `612`: 请输入文章访问密码
- `613`: 文章访问密码错误

### 分页响应格式

```json
{
  "code": 200,
  "message": "查询成功",
  "data": {
    "records": [],
    "total": 100,
    "size": 10,
    "current": 1,
    "pages": 10
  }
}
```

### 请求头规范

```
Content-Type: application/json
Authorization: Bearer {token}  // 需要认证的接口
```

---

## 业务规则汇总

### 1. 权限控制规则

**角色定义**:
- `游客`: 无需登录，只能查看公开内容
- `用户`: 登录后，可以评论、留言(需审核)
- `管理员`: 拥有所有权限，可以管理所有内容

**权限判断逻辑**:
```csharp
// 从Token中提取用户信息
var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
var claims = JwtHelper.ValidateToken(token);
var isAdmin = claims != null && ValidateAdmin(claims);

if (!isAdmin)
{
    // 普通用户权限限制
    if (article.Status == "hide") 
        throw new BusinessException(403, "该文章已被隐藏");
    if (article.Config.IsDel == 1)
        throw new BusinessException(404, "该文章已被删除");
}
```

### 2. 数据验证规则

**通用验证**:
- 所有必填字段不能为空
- 邮箱格式必须符合RFC标准
- URL必须是合法的HTTP/HTTPS地址
- 字符串长度不能超过数据库字段限制

**密码安全**:
- 密码使用MD5加密存储
- 不在日志中输出明文密码
- API返回时将密码字段替换为提示文本

### 3. 限流规则

**限流策略**:
```csharp
[RateLimit(MaxRequests = 10, TimeWindow = 60)] // 每分钟10次
public IActionResult SomeAction()
{
    // ...
}

// 实现逻辑:
- 基于IP地址限流
- 使用内存字典存储请求记录
- 滑动窗口算法
- 超过限制返回429状态码
```

**限流端点**:
- `/api/user/author`: 获取作者信息
- `/api/comment`: 发表评论
- `/api/wall`: 发表留言

### 4. 邮件通知规则

**触发场景**:
1. 新评论提交 → 通知博主
2. 评论审核通过 → 通知评论者
3. 留言提交 → 通知博主
4. 友链申请 → 通知博主

**邮件模板**:
- `comment_email.html`: 评论通知
- `dismiss_email.html`: 审核拒绝通知
- `wall_email.html`: 留言通知

**邮件配置**:
```json
{
  "Email": {
    "Host": "smtp.qq.com",
    "Port": 465,
    "Username": "xxx@qq.com",
    "Password": "授权码",
    "EnableSsl": true
  }
}
```

---

## 第三方集成

### 1. 文件存储服务

**集成x-file-storage库** (.NET版本需找替代或自行实现)

支持的平台:
- Local (本地)
- Aliyun OSS
- Tencent COS
- Qiniu Kodo
- Huawei OBS
- Minio

**配置示例**:
```json
{
  "FileStorage": {
    "DefaultPlatform": "local",
    "Platforms": {
      "Local": {
        "BasePath": "wwwroot/uploads",
        "BaseUrl": "/uploads"
      },
      "Aliyun": {
        "AccessKey": "xxx",
        "SecretKey": "xxx",
        "Bucket": "my-bucket",
        "Endpoint": "oss-cn-hangzhou.aliyuncs.com",
        "Domain": "https://cdn.example.com"
      }
    }
  }
}
```

### 2. JWT认证

**配置**:
```json
{
  "Jwt": {
    "SecretKey": "your-secret-key-min-32-chars",
    "Issuer": "ThriveX",
    "Audience": "ThriveX.Client",
    "ExpirationMinutes": 43200
  }
}
```

**Token生成**:
```csharp
var token = JwtHelper.GenerateToken(user);
// 包含用户信息的Claims
```

### 3. 邮件服务

**SMTP配置**:
- QQ邮箱: smtp.qq.com:465 (SSL)
- 163邮箱: smtp.163.com:465 (SSL)
- Gmail: smtp.gmail.com:587 (STARTTLS)

**注意事项**:
- QQ邮箱需要使用授权码，不是登录密码
- 需要在邮箱设置中开启SMTP服务

---

## 数据库表结构

### 核心表清单

| 表名 | 说明 | 主要字段 |
|-----|------|---------|
| user | 用户表 | id, username, password, name, email, avatar |
| user_token | Token表 | id, uid, token |
| article | 文章表 | id, title, content, cover, view_count, config |
| category | 分类表 | id, name, icon, parent_id |
| tag | 标签表 | id, name, color |
| article_cate | 文章-分类关联 | article_id, category_id |
| article_tag | 文章-标签关联 | article_id, tag_id |
| article_config | 文章配置 | id, article_id, is_encrypt, password, status |
| comment | 评论表 | id, article_id, content, email, name, parent_id, status |
| wall | 留言表 | id, content, email, name, cate_id, status |
| wall_cate | 留言分类 | id, name, icon, color |
| album_cate | 相册分类 | id, name, description, cover |
| album_image | 相册图片 | id, cate_id, url, thumbnail, size |
| link | 友链表 | id, name, url, logo, type_id, status |
| link_type | 友链类型 | id, name, icon |
| swiper | 轮播图 | id, image, title, url, sort, is_active |
| footprint | 足迹 | id, time, location, content, images |
| record | 备忘 | id, content, images |
| rss | RSS订阅 | id, name, url, logo, last_fetch_time |
| config | 系统配置 | id, name, value, notes |
| env_config | 环境配置 | id, name, value |
| page_config | 页面配置 | id, name, config |
| oss | 存储配置 | id, platform, access_key, secret_key, bucket |
| file_detail | 文件详情 | id, file_name, file_path, file_url, size |

---

## 开发 checklist

### 基础架构
- [ ] 创建ASP.NET Core Web API项目
- [ ] 配置Entity Framework Core + MySQL
- [ ] 配置JWT认证
- [ ] 配置Swagger文档
- [ ] 配置CORS跨域
- [ ] 创建统一响应格式
- [ ] 创建全局异常处理
- [ ] 创建日志配置

### 数据层
- [ ] 创建所有实体类(Entity)
- [ ] 创建DbContext
- [ ] 配置实体关系
- [ ] 创建Repository基类
- [ ] 创建特定Repository
- [ ] 执行数据库迁移

### 业务层
- [ ] 创建Service接口
- [ ] 实现UserService
- [ ] 实现ArticleService
- [ ] 实现CategoryService
- [ ] 实现TagService
- [ ] 实现CommentService
- [ ] 实现其他Service...

### API层
- [ ] 创建UserController
- [ ] 创建ArticleController
- [ ] 创建CategoryController
- [ ] 创建TagController
- [ ] 创建CommentController
- [ ] 创建其他Controller...
- [ ] 添加Swagger注解
- [ ] 添加权限验证

### 工具类
- [ ] 实现JwtHelper
- [ ] 实现PasswordHelper (MD5)
- [ ] 实现EmailService
- [ ] 实现FileStorageService
- [ ] 实现RateLimit中间件
- [ ] 实现分页辅助类

### 测试
- [ ] 单元测试(Service层)
- [ ] 集成测试(Controller层)
- [ ] API接口测试
- [ ] 性能测试

### 部署
- [ ] 编写Dockerfile
- [ ] 编写docker-compose.yml
- [ ] 配置生产环境变量
- [ ] 配置Nginx反向代理(可选)
- [ ] 编写部署文档

---

## 注意事项

### 1. 与原Java版本的兼容性

**必须保持一致**:
- 数据库表结构完全相同
- 密码加密方式(MD5)相同
- JWT Token格式相同
- API接口路径和参数相同
- 响应格式相同

**可以优化**:
- 代码结构和命名规范
- 异步编程模型
- 错误处理机制
- 性能优化

### 2. 安全性考虑

**必须实现**:
- SQL注入防护(EF Core参数化查询)
- XSS防护(输出编码)
- CSRF防护(Token验证)
- 密码加密存储
- HTTPS传输
- API限流
- 敏感信息脱敏

### 3. 性能优化

**建议实现**:
- 数据库索引优化
- Redis缓存(热点数据)
- 图片CDN加速
- 静态资源压缩
- 响应数据压缩(Gzip)
- 数据库连接池配置
- 异步I/O操作

### 4. 可维护性

**最佳实践**:
- 统一的代码风格
- 完整的注释文档
- 合理的日志记录
- 完善的异常处理
- 自动化测试覆盖
- CI/CD流水线

---

**文档版本**: 1.0  
**创建日期**: 2026-05-20  
**适用对象**: AI智能体、开发人员  
**更新频率**: 随需求变更同步更新

