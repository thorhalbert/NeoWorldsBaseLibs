using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace NeoWorlds.EmissaryContainerLib.Builder
{
    public partial class LoadEntityProjects
    {
        public ProjectDefaults EntityDefaults { get; private set; }
        public EntityServers EntityServer { get; set; }
        public EntityProjectListing[] EntityProjects { get; private set; }

        public LoadEntityProjects(string projYaml)
        {
            var deserializer = new DeserializerBuilder()
                .Build();

            var EntityDefaults = deserializer.Deserialize<ProjectDefaults>(File.OpenText(projYaml));
            EntityProjects = EntityDefaults.EntityProjects.ToArray();

            //foreach (var v in EntityProjects)
            //{
            //    var g = v.ProjectId;
            //    v.ProjectId = Guid.Parse(g).ToString();
            //}
        }

        public class EntityServers
        {
            public string Host { get; set; }
        }
        public class ProjectDefaults
        {
            public string DefaultPlatform { get; set; }
            public List<EntityProjectListing> EntityProjects { get; set; }
        }
        public class EntityProjectListing
          {
            public string Name { get; set; }
            public string Directory { get; set; }
        }
    }
}
