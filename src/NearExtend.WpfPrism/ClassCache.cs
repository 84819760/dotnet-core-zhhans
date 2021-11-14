using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace NearExtend.WpfPrism
{
    public static class ClassCache
    {
        public static readonly Dictionary<Type, Type> ViewModelMap = GetMaps();

        private static Dictionary<Type, Type> GetMaps()
        {
            var types = GetExecutingTypes();
            var viewModeldic = types.Where(IsViewModel).ToDictionary(x => x.Name);
            return types.Where(IsView)
                   .Select(x => GetMap(viewModeldic, x))
                   .Where(x => x.isok)
                   .ToDictionary(x => x.viewType, x => x.viewModelType);
        }

        private static Type[] GetExecutingTypes() => Assembly
            .GetExecutingAssembly().GetTypes();

        private static (Type viewType, Type viewModelType, bool isok) GetMap(Dictionary<string, Type> dic
            , Type viewType)
        {
            var vmName = $"{viewType.Name}ViewModel";
            var isOk = dic.TryGetValue(vmName, out var viewModelType);
            return (viewType, viewModelType, isOk);
        }

        private static bool IsView(Type type) => Test(type, "DotNetCorezhHans.Views");

        private static bool IsViewModel(Type type) => Test(type, "DotNetCorezhHans.ViewModels");

        private static bool Test(Type type, string @namespace) => type.Namespace == @namespace;
    }
}
