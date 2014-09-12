using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.intime.fashion.common.config
{
    public class EnvironmentConfiguration:CommonConfigurationBase
    {
        protected override string SectionName
        {
            get { return "env"; }
        }
        public string Name { get { return GetItem("name"); } }
        public int[] Tags4ImmediatePublic { get {
            var tags = GetItem("tags_for_immediate_public");
            if (string.IsNullOrWhiteSpace(tags))
                return new int[0];
            return tags.Split(',').Select(t => int.Parse(t)) as int[];
        } }
        public bool IsProduction { get { return string.Compare(Name,"Production",true)==0; } }
  
    }
}
