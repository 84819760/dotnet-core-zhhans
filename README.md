# 使用

先到各平台注册账号,并且获得相应的`SecretId`和`SecretKey`,注意妥善保管避免造成经济损失！填写配置文件后运行即可！

[百度 通用翻译](https://api.fanyi.baidu.com/)  
[腾讯 机器翻译](https://cloud.tencent.com/document/api/551/15611)  
[阿里 机器翻译](https://help.aliyun.com/document_detail/158244.html?spm=a2c4g.11186623.2.2.2ea22b579d7wQv)  


# 配置文件
> 配置文件名称 `DotNetCore-zhHans.Config.ini`
```ini
[Configuration]
; 每次翻译的字符数最大值(建议值:2000)
MaximumCharacter = 2000
; 是否保留原文(建议值:True)
IsKeepOriginal = True
; 要扫描的路径
Directorys = "C:\Program Files\dotnet\packs\", "%UserProFile%\.nuget\packages\"

; ============================百度云翻译API设置
[baidu]
; 百度为 'APP ID'
SecretId = "必填项"
; 百度为 '密钥'
SecretKey = "必填项"
; 间隔时间ms(例如百度免费版每秒只能请求一次)
IntervalTime = 1000
; 线程数量(百度免费版只能为1)
ThreadCount = 1
; ; 是否取消处理包含占位符的内容，某些情况下会丢失内容,除了百度都建议值:True。
; ; 如果检测到问题会记录到log文件夹。
CancelPlaceholder = False

;;不用的API记得用（分号 ;）注释掉，否则运行时会报错！

; ; ============================阿里云机器翻译API设置
; [aliyun]
; ; 阿里为 'AccessKey Id'
; SecretId = "必填项"
; SecretKey = "必填项"
; IntervalTime = 1000
; ThreadCount = 1
; ; 是否取消处理包含占位符的内容，某些情况下会丢失内容,除了百度都建议值:True。
; ; 如果检测到问题会记录到log文件夹。
; CancelPlaceholder = True
; ; 阿里区域设置默认为cn-beijing，不用记得注释掉！
; ; Region = "可选项"

; ; ============================腾讯云机器翻译API设置
; [tencent]
; SecretId = "必填项"
; SecretKey = "必填项"
; IntervalTime = 1000
; ThreadCount = 1
; ; 是否取消处理包含占位符的内容，某些情况下会丢失内容,除了百度都建议值:True。
; ; 如果检测到问题会记录到log文件夹。
; CancelPlaceholder = True
; ; 腾讯区域设置默认为ap-beijing，不用记得注释掉！
; ; Region = "可选项"
```



