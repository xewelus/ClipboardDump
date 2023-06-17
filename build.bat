rmdir "ClipboardDump\bin\Release" /s /q
del "ClipboardDump\bin\Release\ClipboardDump.zip"

dotnet build ClipboardDump.sln -c Release -v m

rmdir "ClipboardDump\bin\Release\net7.0-windows\runtimes" /s /q

del "ClipboardDump\bin\Release\net7.0-windows\System.DirectoryServices.AccountManagement.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.Runtime.Caching.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.Management.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.Data.OleDb.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.DirectoryServices.Protocols.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.Data.Odbc.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.Speech.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.IO.Ports.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.ComponentModel.Composition.Registration.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.ComponentModel.Composition.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.ServiceProcess.ServiceController.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.Reflection.Context.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.ServiceModel.Syndication.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.ServiceModel.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.ServiceModel.Http.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.ServiceModel.Primitives.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.ServiceModel.Security.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.ServiceModel.Duplex.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.Web.Services.Description.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.ServiceModel.NetTcp.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.Private.ServiceModel.dll"
del "ClipboardDump\bin\Release\net7.0-windows\Microsoft.Extensions.ObjectPool.dll"
del "ClipboardDump\bin\Release\net7.0-windows\System.Data.SqlClient.dll"
del "ClipboardDump\bin\Release\net7.0-windows\Microsoft.Bcl.AsyncInterfaces.dll"
del "ClipboardDump\bin\Release\net7.0-windows\ClipboardDump.deps.json"

cd "ClipboardDump\bin\Release\net7.0-windows"
tar -a -c -f ..\ClipboardDump.zip -C . *

start ..

cmd