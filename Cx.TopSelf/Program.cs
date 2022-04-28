// See https://aka.ms/new-console-template for more information


using Cx.Data;
using Topshelf;

var config = ConfigExtensions.GetConfig("config/service.json");

string ASSEMBLY = config.GetSection("assembly").Value;
string CLASSNAME = config.GetSection("class").Value;
string Description = config.GetSection("description").Value;
string DisplayName = config.GetSection("displayname").Value;
string ServiceName = config.GetSection("servername").Value;
IServer _Server = (IServer)Activator.CreateInstance(ASSEMBLY, CLASSNAME)!.Unwrap()!;
try
{
    var rc = HostFactory.Run(x =>
    {
        x.Service<IServer>(s =>                                   //2
        {
            s.ConstructUsing(name => _Server);                //3
            s.WhenStarted(tc => tc.Start());                         //4
            s.WhenStopped(tc => tc.Stop());                          //5
        });
        x.RunAsLocalSystem();                                       //6

        x.SetDescription(Description);                   //7
        x.SetDisplayName(DisplayName);                                  //8
        x.SetServiceName(ServiceName);                                  //9
        Cx.Data.Logger.Default.Info("服务【" + ServiceName + "】配置成功！");
    });                                                             //10
    Cx.Data.Logger.Default.Info("服务【" + ServiceName + "】启用成功！");
    var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());  //11
    Environment.ExitCode = exitCode;

}
catch (Exception e)
{
    Cx.Data.Logger.Default.Info("服务启用异常:" + e.Message);
}
