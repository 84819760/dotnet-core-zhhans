# 下载
> 2.0.0 以下版本需要手动更新！    

[EXE文件及数据库](http://www.wyj55.cn/DotNetCoreZhHans.html)   


# API选择

提供商  | 单次最大请求 | QPS | 每月免费额度 | 免费额度超出后果
---  |  --      | --    | --        | --
百度 |  6000    | 1     |   无限      | 无
腾讯 |  2000    | 5     |   500w    | 停止服务
阿里 |  5000    | 50    |   100w    | 计费
华为 |  2000    | 未知  |   100w    | 计费

> 建议选择百度
[百度翻译注册网址](https://fanyi-api.baidu.com/)
---

# 关于翻译中的”异常”
异常并不代表整个文件不可用，而是某些行没有通过严格的检查，这些错误要么来自翻译引擎，要么是软件代码原因，以超大文件netstandard.xml为例，大约会出现10个异常。出现异常的内容不会写入数据库，等以后能解决的时候开启“重新生成”即可。

---

# 常见问题
1.	扫描中必须有对应的同名dll文件，否则被忽略。
1.  如果XML文件中包含阿拉伯文字，会被忽略。
1.  如果更新.NET自带的XML文档，需要开启管理员权限，否则出现错误” is denied”。

---

# 建议扫描路径
```
C:\Program Files\dotnet\packs\
C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\ReferenceAssemblies\Microsoft\Framework
%UserProFile%\.nuget\packages\

客户端需要升级到2.0.0版本，否则会出现意外退出现象。
```

---

# .NET官方汉化包
在官方汉化包的基础上，加个了个小程序自动拷贝。   
[下载](http://www.wyj55.cn/download/DotNetCorezhHans/Dotnet-Intellisense1.0.0.0.7z)

---

# 免责声明
本软件为免费提供，源码根据开源协议提供，本人不承担使用软件或源码导致的任何后果。