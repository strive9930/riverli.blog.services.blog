# RiverLi Blog API 接口文档

> 最后更新: 2025-04-15
> 面向: 前端开发团队 (React / Vue / 小程序)

---

## 0. 通用约定

### 0.1 基础信息

| 项目 | 值 |
|---|---|
| 后端直连 | `http://localhost:5002` |
| 网关入口 | `http://localhost:5000/api/v1/blog` |
| 前端代理 | `/api/blog/` → `http://localhost:5000/api/v1/blog/` |
| 认证方式 | JWT Bearer Token |
| 请求格式 | `application/json` (除文件上传用 `multipart/form-data`) |
| 字符编码 | UTF-8 |

### 0.2 前端代理配置

前端需将 `/api/blog/` 代理到网关。以 Vite 为例：

```ts
// vite.config.ts
proxy: {
  '/api/blog': {
    target: 'http://localhost:5000',
    changeOrigin: true,
    rewrite: (path) => path.replace(/^\/api\/blog/, '/api/v1/blog')
  }
}
```

> 即前端请求 `http://localhost:3000/api/blog/article/page` → 网关 `http://localhost:5000/api/v1/blog/article/page` → 后端 `api/blog/article/page`

### 0.3 认证

所有接口 **均需登录**。在请求头中携带：

```
Authorization: Bearer <jwt_token>
```

> 获取 Token 请对接 Identity 服务，本服务只做校验。

### 0.4 统一响应格式

#### 成功 — 单条数据

```json
{
  "success": true,
  "code": 200,
  "message": "操作成功",
  "data": { ... }
}
```

#### 成功 — 分页数据

```json
{
  "success": true,
  "code": 200,
  "message": "操作成功",
  "data": {
    "pageIndex": 1,
    "pageSize": 10,
    "totalCount": 87,
    "totalPages": 9,
    "hasPreviousPage": false,
    "hasNextPage": true,
    "data": [ ... ]
  }
}
```

#### 失败

```json
{
  "success": false,
  "code": 400,
  "message": "错误描述"
}
```

### 0.5 枚举值

| 枚举 | 值 | 含义 |
|---|---|---|
| `ArticleStatus` | `Draft` | 草稿 |
| | `Published` | 已发布 |
| `CommentStatus` | `Pending` | 待审核 |
| | `Approved` | 已通过 |
| | `Rejected` | 已拒绝 |

---

## 1. 文章模块 `/api/blog/article`

### 1.1 分页查询文章列表

```
GET /api/blog/article/page?pageIndex=1&pageSize=10&keyword=&categoryId=&tagId=&status=&sortBy=
```

| 参数 | 类型 | 必填 | 默认 | 说明 |
|---|---|---|---|---|
| `pageIndex` | int | 否 | 1 | 页码 |
| `pageSize` | int | 否 | 10 | 每页条数 |
| `keyword` | string | 否 | — | 搜索关键词 (匹配标题+摘要) |
| `categoryId` | Guid | 否 | — | 按分类过滤 |
| `tagId` | Guid | 否 | — | 按标签过滤 |
| `status` | string | 否 | — | 按状态过滤 (`Draft` / `Published`) |
| `sortBy` | string | 否 | — | 排序: `viewcount` / `created` |

**响应 `data.data[]` (ArticleDto):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "文章标题",
  "summary": "摘要",
  "coverUrl": "https://cdn.example.com/cover.jpg",
  "authorName": "作者昵称",
  "status": "Published",
  "categoryName": null,
  "tags": ["tag-guid-1", "tag-guid-2"],
  "viewCount": 128,
  "commentCount": 5,
  "createdTime": "2025-04-15T10:30:00Z"
}
```

---

### 1.2 获取文章详情

```
GET /api/blog/article/{id}
```

**响应 `data` (ArticleDetailDto):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "文章标题",
  "content": "# Markdown 正文内容...",
  "summary": "摘要",
  "coverUrl": "https://cdn.example.com/cover.jpg",
  "authorId": "user-guid",
  "authorName": "作者昵称",
  "status": "Published",
  "categoryId": "category-guid",
  "categoryName": null,
  "tags": [
    { "id": "tag-guid", "name": "C#", "slug": "csharp", "articleCount": 12 }
  ],
  "viewCount": 128,
  "commentCount": 5,
  "createdTime": "2025-04-15T10:30:00Z",
  "modifiedTime": "2025-04-15T11:00:00Z"
}
```

---

### 1.3 发布新文章

```
POST /api/blog/article
```

**请求体:**

```json
{
  "title": "string (必填)",
  "content": "string (必填, Markdown)",
  "summary": "string (必填)",
  "coverUrl": "string? (可为 null)",
  "categoryId": "guid (必填, 挂载的分类ID)",
  "tagIds": ["guid", "guid"]  // 可为 null 或空数组
}
```

**响应 `data`:** `Guid` (新文章 ID)

---

### 1.4 修改文章

```
PUT /api/blog/article/{id}
```

> ⚠️ 仅文章作者本人可修改

**请求体:**

```json
{
  "id": "guid (必填, 必须与路由一致)",
  "title": "string (必填)",
  "content": "string (必填)",
  "summary": "string (必填)",
  "coverUrl": "string?",
  "categoryId": "guid (必填)",
  "tagIds": ["guid"]  // 传空数组则清空所有标签
}
```

**响应 `data`:** 无 (仅 `success` + `message`)

---

### 1.5 删除文章 (软删除)

```
DELETE /api/blog/article/{id}
```

> ⚠️ 仅文章作者本人可删除

**响应 `data`:** 无

---

### 1.6 切换文章状态

```
PUT /api/blog/article/{id}/status
```

> ⚠️ 仅文章作者本人可操作

**请求体:**

```json
{
  "id": "guid (必填, 必须与路由一致)",
  "status": "Draft | Published"
}
```

**典型场景:**

| 操作 | 请求 status | 效果 |
|---|---|---|
| 发布草稿 | `Published` | 文章上架，对外可见 |
| 下架文章 | `Draft` | 退回草稿箱 |

**响应 `data`:** 无

---

## 2. 分类模块 `/api/blog/category`

### 2.1 获取分类树

```
GET /api/blog/category/tree
```

**响应 `data` (CategoryDto[]):**

```json
[
  {
    "id": "guid",
    "name": "技术博客",
    "slug": "tech",
    "description": "技术文章与编程心得",
    "sortOrder": 1,
    "articleCount": 42,
    "children": [
      {
        "id": "guid",
        "name": ".NET",
        "slug": "dotnet",
        "description": null,
        "sortOrder": 0,
        "articleCount": 15,
        "children": []
      }
    ]
  }
]
```

> 前端直接渲染即可，`children` 是递归嵌套的。

---

### 2.2 获取分类扁平选项 (下拉框)

```
GET /api/blog/category/options
```

**响应 `data` (CategoryOptionDto[]):**

```json
[
  { "id": "guid", "name": "未分类",   "parentId": null,  "sortOrder": 0, "depth": 0, "articleCount": 5 },
  { "id": "guid", "name": "技术博客", "parentId": null,  "sortOrder": 1, "depth": 0, "articleCount": 12 },
  { "id": "guid", "name": ".NET",     "parentId": "xxx", "sortOrder": 0, "depth": 1, "articleCount": 3 }
]
```

| 字段 | 说明 |
|---|---|
| `depth` | 树深度，0=顶级，前端可用 `depth × 1.5em` 做缩进 |
| `parentId` | 父级 ID，null=顶级 |
| `articleCount` | 该分类下文章数 |

### 2.3 新增分类

```
POST /api/blog/category
```

**请求体:**

```json
{
  "name": "string (必填, 最大50字符)",
  "slug": "string (必填, URL友好, 最大100字符)",
  "description": "string?",
  "parentId": "guid?",  // null = 顶级分类
  "sortOrder": 0        // 默认0, 越小越靠前
}
```

**响应 `data`:** `Guid` (新分类 ID)

---

### 2.4 修改分类

```
PUT /api/blog/category/{id}
```

**请求体:**

```json
{
  "id": "guid (必填, 必须与路由一致)",
  "name": "string (必填)",
  "slug": "string (必填)",
  "description": "string?",
  "parentId": "guid?",
  "sortOrder": 0
}
```

**响应 `data`:** 无

---

### 2.5 删除分类 (软删除)

```
DELETE /api/blog/category/{id}
```

**响应 `data`:** 无

---

## 3. 标签模块 `/api/blog/tag`

### 3.1 标签分页列表

```
GET /api/blog/tag/page?pageIndex=1&pageSize=20&keyword=
```

| 参数 | 类型 | 必填 | 默认 | 说明 |
|---|---|---|---|---|
| `pageIndex` | int | 否 | 1 | 页码 |
| `pageSize` | int | 否 | 20 | 每页条数 |
| `keyword` | string | 否 | — | 按名称搜索 |

**响应 `data.data[]` (TagDto):**

```json
{
  "id": "guid",
  "name": "C#",
  "slug": "csharp",
  "articleCount": 25
}
```

> 💡 前端可用 `articleCount` 渲染标签热度/大小。

---

### 3.2 新增标签

```
POST /api/blog/tag
```

**请求体:**

```json
{
  "name": "string (必填, 最大50字符)",
  "slug": "string (必填, 最大100字符)"
}
```

**响应 `data`:** `Guid` (新标签 ID)

---

### 3.3 修改标签

```
PUT /api/blog/tag/{id}
```

**请求体:**

```json
{
  "id": "guid (必填, 必须与路由一致)",
  "name": "string (必填)",
  "slug": "string (必填)"
}
```

**响应 `data`:** 无

---

### 3.4 删除标签 (软删除)

```
DELETE /api/blog/tag/{id}
```

**响应 `data`:** 无

---

## 4. 评论模块 `/api/blog/comment`

### 4.1 评论分页列表 (后台审核)

```
GET /api/blog/comment/page?pageIndex=1&pageSize=10&status=Pending&articleId=
```

| 参数 | 类型 | 必填 | 默认 | 说明 |
|---|---|---|---|---|
| `pageIndex` | int | 否 | 1 | 页码 |
| `pageSize` | int | 否 | 10 | 每页条数 |
| `status` | string | 否 | — | `Pending` / `Approved` / `Rejected` |
| `articleId` | Guid | 否 | — | 按文章过滤 |

**响应 `data.data[]` (CommentDto):**

```json
{
  "id": "guid",
  "articleId": "guid",
  "articleTitle": "所属文章标题",
  "reviewerName": "评论者昵称",
  "content": "评论内容",
  "status": "Pending",
  "createdTime": "2025-04-15T12:00:00Z"
}
```

---

### 4.2 提交评论 (前台)

```
POST /api/blog/comment
```

**请求体:**

```json
{
  "articleId": "guid (必填)",
  "content": "string (必填, 最大1000字符)"
}
```

> 评论者身份由 JWT 自动解析，无需手动传入。

**响应 `data`:** `Guid` (新评论 ID)

> ⚠️ 新评论默认状态为 `Pending`，需管理员审核通过后才对外展示。

---

### 4.3 审核评论

```
PUT /api/blog/comment/{id}/audit
```

**请求体:**

```json
{
  "id": "guid (必填, 必须与路由一致)",
  "status": "Approved | Rejected"
}
```

**响应 `data`:** 无

---

### 4.4 彻底删除评论

```
DELETE /api/blog/comment/{id}
```

> ⚠️ 物理删除，不可恢复。

**响应 `data`:** 无

---

## 5. 文件上传 `/api/blog/media`

> ⚠️ 文件上传使用 `multipart/form-data` 格式，不要设置 `Content-Type: application/json`

### 5.1 上传图片 (文章封面 / 正文插图)

```
POST /api/blog/media/upload-image
Content-Type: multipart/form-data
```

| 参数 | 类型 | 必填 | 说明 |
|---|---|---|---|
| `file` | File | 是 | 图片文件 (form-data 字段名 `file`) |

**限制:**

| 项目 | 值 |
|---|---|
| 最大体积 | 5 MB |
| 允许格式 | `.jpg` `.jpeg` `.png` `.gif` `.webp` `.svg` |

**前端示例 (fetch):**

```ts
const formData = new FormData();
formData.append('file', fileInput.files[0]);

const res = await fetch('/api/blog/media/upload-image', {
  method: 'POST',
  headers: { 'Authorization': `Bearer ${token}` },
  body: formData,  // 不要设置 Content-Type，浏览器自动带 boundary
});
const json = await res.json();
// json.data = "/uploads/images/2025/04/a1b2c3d4.jpg"
```

**前端示例 (axios):**

```ts
const formData = new FormData();
formData.append('file', file);

const { data } = await axios.post('/api/blog/media/upload-image', formData, {
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'multipart/form-data',
  },
});
// data.data = "/uploads/images/2025/04/a1b2c3d4.jpg"
```

**响应:**

```json
// 成功 — data 为图片 URL 字符串
{ "success": true, "code": 200, "message": "操作成功", "data": "/uploads/images/2025/04/a1b2c3d4.png" }

// 失败
{ "success": false, "code": 400, "message": "不支持的图片格式: .bmp，仅支持 .jpg, .jpeg, .png, .gif, .webp, .svg" }
```

> 上传成功后的 `data` 字段即为图片 URL，可直接赋值给文章的 `coverUrl` 字段，或插入 Markdown `![alt](url)` 中。

### 5.2 媒体库分页列表

```
GET /api/blog/media/page?pageIndex=1&pageSize=12&keyword=&contentType=image/
```

| 参数 | 类型 | 必填 | 默认 | 说明 |
|---|---|---|---|---|
| `pageIndex` | int | 否 | 1 | 页码 |
| `pageSize` | int | 否 | 12 | 每页条数 |
| `keyword` | string | 否 | — | 按文件名搜索 |
| `contentType` | string | 否 | — | 按 MIME 过滤，如 `image/` 只查图片 |

**响应 `data.data[]` (MediaDto):**

```json
{
  "id": "guid",
  "fileName": "photo.jpg",
  "url": "/uploads/images/2025/04/a1b2c3d4.jpg",
  "contentType": "image/jpeg",
  "fileSize": 204800,
  "uploadedBy": "user-guid",
  "createTime": "2025-04-15T14:00:00Z"
}
```

### 5.3 删除媒体文件

```
DELETE /api/blog/media/{id}
```

> ⚠️ 物理删除，同时删除磁盘文件和数据库记录。

**响应 `data`:** 无

---

## 6. 前端联动建议

### 6.1 文章发布流程

```
1. GET  /api/blog/category/options 或 /api/blog/category/tree  → 渲染分类选择器
2. GET  /api/blog/tag/page?pageSize=100                        → 渲染标签多选
3. POST /api/blog/media/upload-image                           → 上传封面图 (拿到 url)
4. POST /api/blog/article                                      → 提交文章 (传入 categoryId + tagIds + coverUrl)
```

### 6.2 文章列表页

```
GET /api/blog/article/page?pageIndex=1&pageSize=10&status=Published
```

配合筛选:
- 分类过滤: `&categoryId=xxx`
- 搜索: `&keyword=xxx`
- 排序: `&sortBy=viewcount`  (热门) 或 `&sortBy=created` (最新)

### 6.3 后台评论审核

```
GET    /api/blog/comment/page?status=Pending&pageSize=20  → 待审核列表
PUT    /api/blog/comment/{id}/audit { status: "Approved" } → 通过
PUT    /api/blog/comment/{id}/audit { status: "Rejected" } → 拒绝
DELETE /api/blog/comment/{id}                             → 删除
```

### 6.4 标签管理

```
GET    /api/blog/tag/page?pageSize=100  → 获取全部标签
POST   /api/blog/tag                    → 新建标签
PUT    /api/blog/tag/{id}               → 编辑标签名
DELETE /api/blog/tag/{id}               → 删除标签
```

### 6.5 Markdown 编辑器图片插入

```
1. 用户拖拽/粘贴/选择图片
2. POST /api/blog/media/upload-image    → 拿到返回的 url
3. 插入 Markdown 语法: ![alt](url)
4. 或直接赋值给封面字段 coverUrl
```

---

## 7. 异常处理建议

| HTTP 状态码 | 含义 | 前端处理 |
|---|---|---|
| 200 | 成功 | 解析 `data` 字段 |
| 400 | 业务异常 | 展示 `message` |
| 401 | 未认证 | 跳转登录页 |
| 403 | 无权限 | 提示"无操作权限" |

> 所有错误统一从 `message` 字段取文案展示即可。
