using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace NeoWorlds.EmissaryContainerLib.Builder
{
    internal class LoadEntityProjects
    {
        public LoadEntityProjects(string projYaml)
        {
            var deserializer = new DeserializerBuilder()
                .Build();

            var entityProjects = deserializer.Deserialize<object>(projYaml);
        }
    }
}
