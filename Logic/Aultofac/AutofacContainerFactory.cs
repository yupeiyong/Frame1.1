using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Autofac;
using Autofac.Builder;
using Autofac.Integration.Mvc;
using DotNet.Utilities.Log;


namespace Peiyong.Logic.Aultofac
{

    public class AutofacContainerFactory
    {

        private static readonly IContainer Container;


        static AutofacContainerFactory()
        {
            var types = GetRegisteringTypes(new[] {"namespacePrefixes"});

            var builder = new ContainerBuilder();
            builder.RegisterTypes(types).AsSelf().InstancePerDependency().PropertiesAutowired(PropertyWiringOptions.PreserveSetValues);
            Container = builder.Build(ContainerBuildOptions.None);
            //依赖注入
            DependencyResolver.SetResolver(new AutofacDependencyResolver(AutofacContainerFactory.GetContainer()));
        }


        public static IContainer GetContainer()
        {
            return Container;
        }


        public static Type[] GetRegisteringTypes(IReadOnlyCollection<string> namespacePrefixes, Type[] exceptingTypes = null)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var list = new List<Type>();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes()
                        .Where(t => t.IsClass && !t.IsNested && !t.IsNotPublic && !t.IsAbstract && (exceptingTypes == null || !exceptingTypes.Any(exceptType => exceptType.IsAssignableFrom(t))));
                    foreach (var type1 in types)
                    {
                        var type = type1;
                        var fullName = type.FullName;
                        if (namespacePrefixes == null || namespacePrefixes.Count == 0)
                        {
                            list.Add(type);
                        }
                        else if (namespacePrefixes.Any(prefix => !string.IsNullOrWhiteSpace(fullName) && fullName.StartsWith(prefix)))
                        {
                            list.Add(type);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(assembly.FullName + ex.Message);
                }
            }
            return list.ToArray();
        }

    }

}