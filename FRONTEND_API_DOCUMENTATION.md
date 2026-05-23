# ThriveX 博客系统 - 前端 API 对接文档

> **用途**: 前端开发 / AI 智能体开发参考
> **后端框架**: .NET 9 ASP.NET Core Web API
> **基础地址**: `http://localhost:5002` (直连) / `http://localhost:5000/api/v1/blog` (网关)

---

## 目录

1. [通用约定](#1-通用约定)
2. [文章管理](#2-文章管理)
3. [分类管理](#3-分类管理)
4. [标签管理](#4-标签管理)
5. [评论系统](#5-评论系统)
6. [留言墙](#6-留言墙)
7. [相册管理](#7-相册管理)
8. [友情链接](#8-友情链接)
9. [轮播图](#9-轮播图)
10. [足迹记录](#10-足迹记录)
11. [备忘录](#11-备忘录)
12. [RSS订阅](#12-rss订阅)
13. [系统配置](#13-系统配置)
14. [文件管理](#14-文件管理)

---

## 1. 通用约定

### 1.1 基础URL

```
开发环境: http://localhost:5002
网关代理: http://localhost:5000/api/v1/blog
```

### 1.2 统一响应格式

```json
{
  "success": true,
  "code": 200,
  "message": "操作成功",
  "data": {}
}
```

**状态码说明**:

| code | 含义 |
|------|------|
| 200 | 成功 |
| 400 | 请求参数错误 |
| 401 | 未授权(Token 无效) |
| 403 | 禁止访问(权限不足) |
| 404 | 资源不存在 |
| 500 | 服务器内部错误 |

### 1.3 分页响应格式

```json
{
  "success": true,
  "code": 200,
  "message": "操作成功",
  "timestamp": 1700000000000,
  "pageIndex": 1,
  "pageSize": 10,
  "totalCount": 100,
  "totalPages": 10,
  "hasPreviousPage": false,
  "hasNextPage": true,
  "data": []
}
```

### 1.4 认证方式

**需要认证的接口**在请求头中携带 Token：

```
Authorization: Bearer <jwt_token>
Content-Type: application/json
```

**接口权限标记**:
- 公开 - 无需 Token
- 登录 - 需要有效 Token
- 管理员 - 需要管理员 Token

### 1.5 分页查询通用参数

所有分页接口请求体继承以下参数：

```json
{
  "pageIndex": 1,
  "pageSize": 10,
  "sortField": "createTime desc"
}
```

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| pageIndex | int | 1 | 页码(从1开始) |
| pageSize | int | 10 | 每页条数(最大100) |
| sortField | string | null | 排序字段 |

---

## 2. 文章管理

**路由前缀**: `/api/article`

### 2.1 接口列表

| 功能 | 方法 | 路径 | 权限 |
|------|------|------|------|
| 创建文章 | POST | /api/article | 管理员 |
| 获取文章详情 | GET | /api/article/{id} | 公开 |
| 修改文章 | PATCH | /api/article/{id} | 管理员 |
| 删除文章 | DELETE | /api/article/{id} | 管理员 |
| 批量删除文章 | DELETE | /api/article/batch | 管理员 |
| 文章列表 | POST | /api/article/list | 公开 |
| 分页查询 | POST | /api/article/paging | 公开 |
| 增加浏览量 | PATCH | /api/article/view/{id} | 公开 |
| 随机文章 | GET | /api/article/random | 公开 |
| 归档统计 | GET | /api/article/archive | 公开 |
| 搜索文章 | POST | /api/article/search | 公开 |

### 2.2 接口详情

#### POST /api/article - 创建文章

> 权限: 管理员

**请求体**:
```json
{
  "title": "string (必填)",
  "content": "string (必填, Markdown/HTML)",
  "description": "string (可选, 摘要)",
  "cover": "string (可选, 封面图URL)",
  "config": "{} (JSON字符串, 文章配置)",
  "tags": ["string (标签名列表)"]
}
```

**config 字段说明** (JSON 字符串):
```json
{
  "isEncrypt": 0,
  "password": "",
  "status": "publish",
  "isTop": 0,
  "isDel": 0
}
```

| config字段 | 类型 | 默认值 | 说明 |
|-----------|------|--------|------|
| isEncrypt | int | 0 | 是否加密(0/1) |
| password | string | "" | 访问密码 |
| status | string | "publish" | 状态: publish/hide/draft |
| isTop | int | 0 | 是否置顶(0/1) |
| isDel | int | 0 | 是否删除(0/1) |

**成功响应**:
```json
{
  "success": true,
  "code": 200,
  "message": "操作成功",
  "data": "guid-article-id"
}
```

---

#### GET /api/article/{id}?password={pwd} - 获取文章详情

> 权限: 公开

**查询参数**:
| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| password | string | 否 | 加密文章的访问密码 |

**成功响应**:
```json
{
  "success": true,
  "code": 200,
  "data": {
    "id": "guid",
    "title": "文章标题",
    "content": "文章内容(Markdown/HTML)",
    "description": "文章摘要",
    "cover": "封面图URL",
    "config": "{\"isEncrypt\":0,\"status\":\"publish\"}",
    "viewCount": 100,
    "authorId": "author-guid",
    "authorName": "作者名",
    "createTime": "2026-05-20T10:00:00",
    "cateNames": ["分类1", "分类2"],
    "tagNames": ["标签1", "标签2"],
    "prev": { "id": "guid", "title": "上一篇" },
    "next": { "id": "guid", "title": "下一篇" }
  }
}
```

---

#### PATCH /api/article/{id} - 修改文章

> 权限: 管理员

**请求体**:
```json
{
  "id": "guid (由路由提供, 可省略)",
  "title": "string (必填)",
  "content": "string (必填)",
  "description": "string (可选)",
  "cover": "string (可选)",
  "config": "{} (JSON字符串)",
  "cateIds": [1, 2],
  "tags": ["标签1", "标签2"]
}
```

---

#### DELETE /api/article/batch - 批量删除

> 权限: 管理员

**请求体**:
```json
["guid1", "guid2", "guid3"]
```

---

#### POST /api/article/list - 文章列表(筛选)

> 权限: 公开

**请求体**:
```json
{
  "keyword": "string (可选, 标题/内容搜索)",
  "cateId": 1,
  "tagId": 1,
  "status": "publish",
  "sortBy": "createTime",
  "sortOrder": "desc"
}
```

**响应 data**:
```json
[
  {
    "id": "guid",
    "title": "文章标题",
    "description": "摘要",
    "cover": "封面URL",
    "viewCount": 100,
    "authorName": "作者",
    "createTime": "2026-05-20T10:00:00"
  }
]
```

---

#### POST /api/article/paging - 文章分页查询

> 权限: 公开

继承[分页参数](#15-分页查询通用参数)，额外字段:

```json
{
  "pageIndex": 1,
  "pageSize": 10,
  "keyword": "string (可选)",
  "cateId": 1,
  "tagId": 1,
  "status": "publish"
}
```

**响应**: 标准分页格式，data 为 `ArticleListDto[]`

---

#### PATCH /api/article/view/{id} - 增加浏览量

> 权限: 公开

无需请求体。每次调用浏览量+1。

---

#### GET /api/article/random?count=5 - 随机文章

> 权限: 公开

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| count | int | 5 | 获取数量 |

---

#### GET /api/article/archive - 归档统计

> 权限: 公开

**响应 data**:
```json
{
  "2026年05月": 3,
  "2026年04月": 5
}
```

---

#### POST /api/article/search - 搜索文章

> 权限: 公开

**请求体**:
```json
{
  "keyword": "搜索关键词 (必填)"
}
```

---

## 3. 分类管理

**路由前缀**: `/api/cate`

### 3.1 接口列表

| 功能 | 方法 | 路径 | 权限 |
|------|------|------|------|
| 创建分类 | POST | /api/cate | 管理员 |
| 获取分类 | GET | /api/cate/{id} | 公开 |
| 修改分类 | PATCH | /api/cate/{id} | 管理员 |
| 删除分类 | DELETE | /api/cate/{id} | 管理员 |
| 批量删除 | DELETE | /api/cate/batch | 管理员 |
| 分类列表 | POST | /api/cate/list | 公开 |
| 分页查询 | POST | /api/cate/paging | 公开 |
| 树形结构 | GET | /api/cate/tree | 公开 |
| 文章统计 | GET | /api/cate/count | 公开 |

### 3.2 接口详情

#### POST /api/cate - 创建分类

> 权限: 管理员

```json
{
  "name": "分类名称 (必填)",
  "icon": "图标URL (可选)",
  "parentId": "guid or null (父分类ID, null为顶级)"
}
```

#### GET /api/cate/tree - 分类树

> 权限: 公开

**响应 data**:
```json
[
  {
    "id": "guid",
    "name": "技术",
    "icon": "icon-url",
    "parentId": null,
    "createTime": "2026-05-20T10:00:00",
    "children": [
      {
        "id": "guid",
        "name": "前端",
        "icon": null,
        "parentId": "parent-guid",
        "createTime": "2026-05-20T10:00:00",
        "children": [],
        "articleCount": 0
      }
    ],
    "articleCount": 0
  }
]
```

---

## 4. 标签管理

**路由前缀**: `/api/tag`

### 4.1 接口列表

| 功能 | 方法 | 路径 | 权限 |
|------|------|------|------|
| 创建标签 | POST | /api/tag | 管理员 |
| 获取标签 | GET | /api/tag/{id} | 公开 |
| 修改标签 | PATCH | /api/tag/{id} | 管理员 |
| 删除标签 | DELETE | /api/tag/{id} | 管理员 |
| 批量删除 | DELETE | /api/tag/batch | 管理员 |
| 标签列表 | POST | /api/tag/list | 公开 |
| 分页查询 | POST | /api/tag/paging | 公开 |
| 热门标签 | GET | /api/tag/hot?count=10 | 公开 |

### 4.2 接口详情

#### POST /api/tag - 创建标签

```json
{
  "name": "标签名称 (必填)",
  "color": "#FF0000 (可选, 标签颜色)"
}
```

#### GET /api/tag/hot?count=10 - 热门标签

**响应 data**:
```json
[
  {
    "id": "guid",
    "name": "CSharp",
    "color": "#2196F3",
    "createTime": "2026-05-20T10:00:00",
    "articleCount": 0
  }
]
```

---

## 5. 评论系统

**路由前缀**: `/api/comment`

### 5.1 接口列表

| 功能 | 方法 | 路径 | 权限 |
|------|------|------|------|
| 发表评论 | POST | /api/comment | 公开 |
| 获取评论 | GET | /api/comment/{id} | 公开 |
| 删除评论 | DELETE | /api/comment/{id} | 管理员 |
| 批量删除 | DELETE | /api/comment/batch | 管理员 |
| 评论列表 | POST | /api/comment/list | 公开 |
| 分页查询 | POST | /api/comment/paging | 管理员 |
| 审核评论 | PATCH | /api/comment/audit/{id} | 管理员 |
| 回复评论 | POST | /api/comment/reply | 公开 |

### 5.2 接口详情

#### POST /api/comment - 发表评论

> 权限: 公开

```json
{
  "articleId": "guid or null (文章ID, null用于留言)",
  "content": "评论内容 (必填)",
  "email": "email@example.com (必填)",
  "name": "昵称 (必填)",
  "url": "https:// (可选, 个人网站)",
  "avatar": "头像URL (可选)",
  "parentId": "00000000-0000-0000-0000-000000000000 (顶级评论用空Guid)",
  "ip": "客户端IP (可选)",
  "userAgent": "浏览器UA (可选)"
}
```

#### POST /api/comment/reply - 回复评论

> 权限: 公开

参数同发表评论，但 `parentId` 必须填入被回复评论的 ID。

#### PATCH /api/comment/audit/{id} - 审核评论

> 权限: 管理员

```json
{
  "id": "guid",
  "status": "approved"
}
```

| status值 | 说明 |
|----------|------|
| pending | 待审核(默认) |
| approved | 已通过(前台显示) |
| rejected | 已拒绝 |

#### POST /api/comment/list - 评论列表(树形)

> 权限: 公开

**请求体**:
```json
{
  "articleId": "guid (可选, 筛选文章评论)",
  "status": "approved (可选, 筛选审核状态)"
}
```

**响应 data (树形结构)**:
```json
[
  {
    "id": "guid",
    "articleId": "guid",
    "content": "评论内容",
    "email": "email@example.com",
    "name": "评论者",
    "url": "个人网站",
    "avatar": "头像URL",
    "parentId": "00000000-0000-0000-0000-000000000000",
    "status": "approved",
    "createTime": "2026-05-20T10:00:00",
    "replies": [
      {
        "id": "guid",
        "articleId": "guid",
        "content": "回复内容",
        "name": "回复者",
        "parentId": "parent-comment-guid",
        "status": "approved",
        "createTime": "2026-05-20T10:00:00",
        "replies": null
      }
    ]
  }
]
```

---

## 6. 留言墙

**路由前缀**: `/api/wall`
**留言分类路由**: `/api/wall-cate`

### 6.1 留言墙接口

| 功能 | 方法 | 路径 | 权限 |
|------|------|------|------|
| 发表留言 | POST | /api/wall | 公开 |
| 获取留言 | GET | /api/wall/{id} | 公开 |
| 删除留言 | DELETE | /api/wall/{id} | 管理员 |
| 批量删除 | DELETE | /api/wall/batch | 管理员 |
| 留言列表 | POST | /api/wall/list | 公开 |
| 分页查询 | POST | /api/wall/paging | 管理员 |
| 留言分类 | GET | /api/wall/cates | 公开 |

### 6.2 留言分类管理接口

| 功能 | 方法 | 路径 | 权限 |
|------|------|------|------|
| 创建分类 | POST | /api/wall-cate | 管理员 |
| 修改分类 | PATCH | /api/wall-cate/{id} | 管理员 |
| 删除分类 | DELETE | /api/wall-cate/{id} | 管理员 |

### 6.3 接口详情

#### POST /api/wall - 发表留言

```json
{
  "content": "留言内容 (必填)",
  "email": "email@example.com (必填)",
  "name": "昵称 (必填)",
  "url": "个人网站 (可选)",
  "avatar": "头像URL (可选)",
  "cateId": 1,
  "ip": "客户端IP (可选)"
}
```

#### POST /api/wall-cate - 创建留言分类

```json
{
  "name": "分类名称 (必填)",
  "icon": "图标 (可选)",
  "color": "#FF0000 (可选)"
}
```

---

## 7. 相册管理

**路由前缀**: `/api/album`

### 7.1 接口列表

| 功能 | 方法 | 路径 | 权限 |
|------|------|------|------|
| 创建相册分类 | POST | /api/album/cate | 管理员 |
| 获取分类 | GET | /api/album/cate/{id} | 公开 |
| 修改分类 | PATCH | /api/album/cate/{id} | 管理员 |
| 删除分类 | DELETE | /api/album/cate/{id} | 管理员 |
| 分类列表 | GET | /api/album/cates | 公开 |
| 上传图片 | POST | /api/album/image | 管理员 |
| 获取图片 | GET | /api/album/image/{id} | 公开 |
| 删除图片 | DELETE | /api/album/image/{id} | 管理员 |
| 图片列表 | POST | /api/album/images | 公开 |

### 7.2 接口详情

#### POST /api/album/cate - 创建相册分类

```json
{
  "name": "相册名称 (必填)",
  "description": "描述 (可选)",
  "cover": "封面图URL (可选)"
}
```

#### POST /api/album/image - 上传图片

```json
{
  "cateId": 1,
  "url": "图片URL (必填)",
  "thumbnail": "缩略图URL (可选)",
  "description": "图片描述 (可选)",
  "size": 102400,
  "width": 1920,
  "height": 1080
}
```

#### POST /api/album/images - 获取分类下图片

```json
{
  "cateId": 1
}
```

**响应 data**:
```json
[
  {
    "id": "guid",
    "cateId": 1,
    "url": "图片URL",
    "thumbnail": "缩略图URL",
    "description": "描述",
    "size": 102400,
    "width": 1920,
    "height": 1080,
    "createTime": "2026-05-20T10:00:00"
  }
]
```

---

## 8. 友情链接

**路由前缀**: `/api/link`
**友链类型路由**: `/api/link-type`

### 8.1 友链接口

| 功能 | 方法 | 路径 | 权限 |
|------|------|------|------|
| 申请友链 | POST | /api/link | 公开 |
| 获取友链 | GET | /api/link/{id} | 公开 |
| 修改友链 | PATCH | /api/link/{id} | 管理员 |
| 删除友链 | DELETE | /api/link/{id} | 管理员 |
| 批量删除 | DELETE | /api/link/batch | 管理员 |
| 友链列表 | POST | /api/link/list | 公开 |
| 分页查询 | POST | /api/link/paging | 管理员 |
| 友链类型 | GET | /api/link/types | 公开 |

### 8.2 友链类型管理接口

| 功能 | 方法 | 路径 | 权限 |
|------|------|------|------|
| 创建类型 | POST | /api/link-type | 管理员 |
| 修改类型 | PATCH | /api/link-type/{id} | 管理员 |
| 删除类型 | DELETE | /api/link-type/{id} | 管理员 |

### 8.3 接口详情

#### POST /api/link - 申请友链

```json
{
  "name": "网站名称 (必填)",
  "url": "https://example.com (必填)",
  "logo": "Logo URL (可选)",
  "description": "网站描述 (可选)",
  "typeId": 1,
  "email": "admin@example.com (可选)"
}
```

> 申请后状态默认为 `pending`，需管理员审核后在前台显示。

#### PATCH /api/link/{id} - 修改友链(含审核)

```json
{
  "id": "guid",
  "name": "网站名称",
  "url": "URL",
  "logo": "Logo",
  "description": "描述",
  "typeId": 1,
  "email": "email"
}
```

> 将 status 改为 "approved" 即可审核通过。

#### POST /api/link-type - 创建友链类型

```json
{
  "name": "类型名称 (必填)",
  "icon": "图标 (可选)"
}
```

---

## 9. 轮播图

**路由前缀**: `/api/swiper`

### 9.1 接口列表

| 功能 | 方法 | 路径 | 权限 |
|------|------|------|------|
| 创建轮播图 | POST | /api/swiper | 管理员 |
| 获取轮播图 | GET | /api/swiper/{id} | 公开 |
| 修改轮播图 | PATCH | /api/swiper/{id} | 管理员 |
| 删除轮播图 | DELETE | /api/swiper/{id} | 管理员 |
| 轮播图列表 | GET | /api/swipers | 公开 |

### 9.2 接口详情

#### POST /api/swiper - 创建轮播图

```json
{
  "image": "图片URL (必填)",
  "title": "标题 (可选)",
  "description": "描述 (可选)",
  "url": "跳转链接 (可选)",
  "sort": 0,
  "isActive": true
}
```

#### GET /api/swipers - 轮播图列表

> 权限: 公开，只返回 `isActive=true` 的轮播图，按 `sort` 升序排列

**响应 data**:
```json
[
  {
    "id": "guid",
    "image": "图片URL",
    "title": "标题",
    "description": "描述",
    "url": "跳转链接",
    "sort": 0,
    "isActive": true,
    "createTime": "2026-05-20T10:00:00"
  }
]
```

---

## 10. 足迹记录

**路由前缀**: `/api/footprint`

### 10.1 接口列表

| 功能 | 方法 | 路径 | 权限 |
|------|------|------|------|
| 创建足迹 | POST | /api/footprint | 管理员 |
| 获取足迹 | GET | /api/footprint/{id} | 公开 |
| 修改足迹 | PATCH | /api/footprint/{id} | 管理员 |
| 删除足迹 | DELETE | /api/footprint/{id} | 管理员 |
| 足迹列表 | GET | /api/footprints | 公开 |

### 10.2 接口详情

#### POST /api/footprint - 创建足迹

```json
{
  "time": "2026-05-20T10:00:00 (必填, 发生时间)",
  "location": "杭州西湖 (必填, 地点名称)",
  "content": "内容描述 (必填)",
  "images": "[\"url1\",\"url2\"] (图片JSON数组字符串)",
  "longitude": 120.1234567,
  "latitude": 30.1234567
}
```

#### GET /api/footprints - 足迹时间轴

> 按 time 倒序排列

**响应 data**:
```json
[
  {
    "id": "guid",
    "time": "2026-05-20T10:00:00",
    "location": "杭州西湖",
    "content": "内容描述",
    "images": "[\"url1\",\"url2\"]",
    "longitude": 120.1234567,
    "latitude": 30.1234567,
    "createTime": "2026-05-20T10:00:00"
  }
]
```

---

## 11. 备忘录

**路由前缀**: `/api/record`

### 11.1 接口列表

| 功能 | 方法 | 路径 | 权限 |
|------|------|------|------|
| 创建备忘 | POST | /api/record | 管理员 |
| 获取备忘 | GET | /api/record/{id} | 公开 |
| 修改备忘 | PATCH | /api/record/{id} | 管理员 |
| 删除备忘 | DELETE | /api/record/{id} | 管理员 |
| 备忘列表 | GET | /api/records | 公开 |

### 11.2 接口详情

#### POST /api/record - 创建备忘

```json
{
  "content": "文本内容 (必填)",
  "images": "[\"url1\"] (图片JSON数组字符串, 默认\"[]\")"
}
```

#### GET /api/records - 备忘列表

> 按 createTime 倒序

**响应 data**:
```json
[
  {
    "id": "guid",
    "content": "文本内容",
    "images": "[\"url1\",\"url2\"]",
    "createTime": "2026-05-20T10:00:00"
  }
]
```

---

## 12. RSS订阅

**路由前缀**: `/api/rss`

### 12.1 接口列表

| 功能 | 方法 | 路径 | 权限 |
|------|------|------|------|
| 添加RSS源 | POST | /api/rss | 管理员 |
| 获取RSS | GET | /api/rss/{id} | 公开 |
| 修改RSS | PATCH | /api/rss/{id} | 管理员 |
| 删除RSS | DELETE | /api/rss/{id} | 管理员 |
| RSS列表 | GET | /api/rsses | 公开 |
| 手动抓取 | POST | /api/rss/fetch/{id} | 管理员 |

### 12.2 接口详情

#### POST /api/rss - 添加RSS源

```json
{
  "name": "订阅源名称 (必填)",
  "url": "https://example.com/feed.xml (必填)",
  "logo": "Logo URL (可选)",
  "description": "描述 (可选)"
}
```

#### POST /api/rss/fetch/{id} - 手动抓取

> 标记该 RSS 源的最后抓取时间为当前时间。

---

## 13. 系统配置

### 13.1 配置接口

| 功能 | 方法 | 路径 | 权限 |
|------|------|------|------|
| 获取配置 | GET | /api/config/{name} | 公开 |
| 更新配置 | PATCH | /api/config/{name} | 管理员 |
| 配置列表 | GET | /api/configs | 管理员 |
| 页面配置 | GET | /api/page/config?name=home | 公开 |
| 更新JSON值 | PATCH | /api/config/json | 管理员 |

### 13.2 OSS存储配置

| 功能 | 方法 | 路径 | 权限 |
|------|------|------|------|
| 存储配置列表 | GET | /api/oss | 管理员 |
| 更新存储配置 | PATCH | /api/oss/{id} | 管理员 |

### 13.3 接口详情

#### GET /api/config/{name} - 获取指定配置

```
GET /api/config/site_name
```

**响应 data**:
```json
{
  "id": "guid",
  "name": "site_name",
  "value": "{\"title\":\"我的博客\"}",
  "notes": "网站名称配置",
  "createTime": "2026-05-20T10:00:00"
}
```

#### PATCH /api/config/{name} - 更新配置值

```json
{
  "name": "site_name (由路由提供)",
  "value": "{\"title\":\"新标题\"}"
}
```

#### PATCH /api/config/json - 更新/创建JSON配置

```json
{
  "name": "key_name",
  "value": "{\"key\":\"value\"}"
}
```

> 如果 name 不存在则自动创建。

#### GET /api/page/config?name=home - 获取页面配置

**响应 data**:
```json
{
  "id": "guid",
  "name": "home",
  "config": "{\"layout\":\"...\"}",
  "createTime": "2026-05-20T10:00:00"
}
```

#### PATCH /api/oss/{id} - 更新OSS存储配置

```json
{
  "id": "guid",
  "platform": "aliyun",
  "accessKey": "your-access-key",
  "secretKey": "your-secret-key",
  "bucket": "bucket-name",
  "domain": "https://cdn.example.com",
  "basePath": "uploads/",
  "isDefault": true
}
```

---

## 14. 文件管理

**路由前缀**: `/api/file`

### 14.1 接口列表

| 功能 | 方法 | 路径 | 权限 |
|------|------|------|------|
| 上传文件 | POST | /api/file/upload | 管理员 |
| 删除文件 | DELETE | /api/file/{id} | 管理员 |
| 文件列表 | POST | /api/file/list | 管理员 |

### 14.2 接口详情

#### POST /api/file/upload - 上传文件

> Content-Type: multipart/form-data

| 字段 | 类型 | 说明 |
|------|------|------|
| file | File | 上传的文件 |

**响应**:
```json
{
  "message": "文件上传功能需要配置存储服务"
}
```

> 注意: 当前上传接口为占位实现，需配置存储服务(阿里云OSS/腾讯云COS/七牛云/本地存储)后方可使用。

#### POST /api/file/list - 文件列表(分页)

```json
{
  "pageIndex": 1,
  "pageSize": 10
}
```

**响应 data**:
```json
[
  {
    "id": "guid",
    "fileName": "image.png",
    "filePath": "uploads/2026/05/image.png",
    "fileUrl": "https://cdn.example.com/uploads/2026/05/image.png",
    "fileSize": 102400,
    "fileType": "image/png",
    "platform": "aliyun",
    "createTime": "2026-05-20T10:00:00"
  }
]
```

---

## 附录 A: 数据模型速查

### Article (文章)
```typescript
interface Article {
  id: string;           // Guid
  title: string;
  content: string;       // Markdown/HTML
  description?: string;  // 摘要
  cover?: string;        // 封面图URL
  config: string;        // JSON字符串 {isEncrypt,password,status,isTop,isDel}
  viewCount: number;
  authorId: string;
  authorName: string;
  createTime: string;    // ISO 8601
  cateNames?: string[];
  tagNames?: string[];
  prev?: { id: string; title: string };
  next?: { id: string; title: string };
}
```

### Comment (评论)
```typescript
interface Comment {
  id: string;
  articleId?: string;
  content: string;
  email: string;
  name: string;
  url?: string;
  avatar?: string;
  parentId: string;       // 顶级评论为 "00000000-0000-0000-0000-000000000000"
  status: string;          // pending | approved | rejected
  createTime: string;
  replies?: Comment[];
}
```

### Category (分类)
```typescript
interface Category {
  id: string;
  name: string;
  icon?: string;
  parentId?: string;       // null 为顶级分类
  createTime: string;
  children?: Category[];
  articleCount: number;
}
```

### Tag (标签)
```typescript
interface Tag {
  id: string;
  name: string;
  color?: string;
  createTime: string;
  articleCount: number;
}
```

---

## 附录 B: 前端开发注意事项

### B.1 认证流程

1. 调用登录接口获取 JWT Token
2. 将 Token 存入 localStorage
3. 所有需要认证的请求在 Header 中携带 `Authorization: Bearer <token>`
4. 可根据 Token 校验接口 `/api/user/check` 判断 Token 是否有效

### B.2 分页处理

所有分页接口使用统一的请求体格式，响应体包含分页元数据：
- `pageIndex` / `pageSize` 控制分页
- `totalCount` / `totalPages` 用于计算总页数
- `hasPreviousPage` / `hasNextPage` 控制翻页按钮状态

### B.3 文章状态说明

| status | 说明 | 前台可见 |
|--------|------|----------|
| publish | 已发布 | 是 |
| hide | 隐藏 | 否(管理员可见) |
| draft | 草稿 | 否(管理员可见) |

### B.4 加密文章处理

1. 请求文章详情时若返回 `code: 612` 提示输入密码
2. 密码错误返回 `code: 613`
3. 带上正确密码参数 `?password=xxx` 重新请求
4. 管理员无需密码可直接查看

### B.5 ID 格式

所有主键使用 GUID 格式: `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`

### B.6 时间格式

所有时间字段使用 ISO 8601 格式: `2026-05-20T10:00:00`

---

**文档版本**: 1.0
**创建日期**: 2026-05-21
**适用对象**: 前端开发、AI 智能体
**对应后端版本**: RiverLi.Blog.Services.Blog v1.0
