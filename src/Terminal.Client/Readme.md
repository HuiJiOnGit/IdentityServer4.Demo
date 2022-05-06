# 客户端凭证模式

使用场景

- 一般用于后台,没有用户参与

使用方式

- `GetDiscoveryDocumentAsync` 从发现文档中获取各种接口 比如 `token` 和 `userinfo`接口
- `RequestClientCredentialsTokenAsync` 从`token`接口中获取`AccessToken`,需要参数是客户端id和客户端secret和Scope(范围)
- `GetAsync` 再用`AccessToken`请求资源服务器获取想要的数据