﻿using CanonicalGrpcProtos;
using System.Text;
using YamlDotNet.Serialization;

namespace NeoWorlds.EmissaryContainerLib.Builder
{
    public partial class LoadEntityProjects
    {
        public class EntityProject
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string ProjectId { get; set; }
            public string Binary { get; set; }

            public string BuildTime { get; set; }

            public static EntityProject LoadProject(string projYaml)
            {
                var deserializer = new DeserializerBuilder().Build();

                var project = deserializer.Deserialize<EntityProject>(File.OpenText(projYaml));
                project.ProjectId = Guid.Parse(project.ProjectId).ToString();

                return project;
            }

            public EmissaryEntityProjects GetProto()
            {
                var ret = new EmissaryEntityProjects
                {
                    Name = Name,
                    Description = Description,
                    ProjectId = ProjectId,
                    Binary = Binary,
                    BuildTime = BuildTime,
                };

                return ret;
            }

            public (Stream,int) DumpProject() {
                BuildTime = DateTimeOffset.Now.ToString("R");

                var serializer = new SerializerBuilder().Build();

                var body = serializer.Serialize(this);
                var payload = Encoding.UTF8.GetBytes(body);
                return (new MemoryStream(payload), payload.Length);
            }
        }
    }
}
