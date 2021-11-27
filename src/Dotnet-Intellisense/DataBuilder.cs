using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotnet_Intellisense
{
    internal class DataBuilder
    {       
        public static void InitJson(string dataFile)
        {
            var url = @"https://download.visualstudio.microsoft.com/download/pr";
            var datas = new DataModel[]
            {
                //3.0
                new ()
                {
                     Name ="3.0",
                     Url=@$"{url}/96ccd0e5-e67b-4796-b0f3-711c24faf820/dfb5c62d13dcf6274950cd76c327cc61/dotnet-intellisense-3.0-zh-hans.zip",
                     DataDetails=CreateDataDetails("3.0","netcoreapp"),
                },
                //3.1
                new ()
                {
                     Name ="3.1",
                     Url=@$"{url}/be05ed35-5e8c-48d5-a15e-3f919f47d2d1/488cec211e6926ff1de7834e4153538e/dotnet-intellisense-3.1-zh-hans.zip",
                      DataDetails=CreateDataDetails("3.1","netcoreapp"),
                },
                //5.0
                new ()
                {
                     Name ="5.0",
                     Url=@$"{url}/42239244-de7e-4018-b6b4-f7e4c15ddc82/2856eacff59637019ff16b017504f8e8/dotnet-intellisense-5.0-zh-hans.zip",
                      DataDetails=CreateDataDetails("5.0","net"),
                },
            };
            WriteFile(datas, dataFile);
        }

        private static void WriteFile(DataModel[] datas, string dataFile)
        {
           var json= JsonBuilder.GetJson(datas);
            File.WriteAllText(dataFile, json);
        }

        private static DataDetailsModel[] CreateDataDetails(string name, string netName,string netStandard="2.1") => 
            new DataDetailsModel[]
        {
            CreateDataDetail(name,"Microsoft.NETCore.App.Ref",netName),
            CreateDataDetail(name,"Microsoft.WindowsDesktop.App.Ref",netName),
            new()
            {
                Source=GetNETStandardSource(name),
                Target=$@"C:\Program Files\dotnet\packs\NETStandard.Library.Ref\{netStandard}.0\ref\netstandard{netStandard}\zh-hans",
            },
         };

        private static DataDetailsModel CreateDataDetail(string name, string target, string netName) => new()
        {
            Source = GetSource(name, target),
            Target = GetTarget(name, target, netName)
        };

        private static string GetSource(string name, string target) =>
            $@"dotnet-intellisense-{name}-zh-hans\{target}\zh-hans";

        private static string GetNETStandardSource(string name) =>
           $@"dotnet-intellisense-{name}-zh-hans\NETStandard.Library.Ref\zh-hans";

        private static string GetTarget(string name, string target,string netName) =>
          $@"C:\Program Files\dotnet\packs\{target}\{name}.0\ref\{netName}{name}\zh-hans";

      
    }
}
