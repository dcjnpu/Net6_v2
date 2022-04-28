自用工具类库

.*Crypt:加密类
Base：无关紧要，一些偶尔用下的方法
DataUtils：数据处理类
ExpressionFunc：Func<T,bool>的linq查询拼接方法。用于SQL查询拼接条件
RegexHelper：正则处理类
EnumDescriptionExtension：枚举DescriptionAttribute的值读取，用了反射+字典缓存
EnumTextExtension：自定义枚举说明 ，用了反射+字典缓存
IExcelHelper：几个EXCEL操作类的接口类，类似还有IServer，ApiBackModel。都是给其他类库用的
Logger：基于NLOG的操作封装，计划重新编写
ConfigExtensions：json配置文件读取方法，有AppSettings，另可以读取自定义的json文件
ConfigSetting：AppSettings，的用法说明，没什么用
CxHttpContext：不通过IOC直接获取当前注入的服务
MemoryCacheService : ICacheService：本地缓存
FileHelper：FileHelperCore：文件操作
FileSave：上传文件保存
FileUpload：文件上传
ImageCompress：图片压缩
XmlSetting：Xml配置文件读取，新版不再使用，请用ConfigExtensions






