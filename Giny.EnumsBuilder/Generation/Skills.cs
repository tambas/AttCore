﻿using Giny.IO.D2I;
using Giny.IO.D2O;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giny.EnumsBuilder.Generation
{
    public class Skills : CustomEnum
    {
        public override string ClassName => "SkillTypeEnum";

        protected override string GenerateEnumContent(List<D2OReader> readers, D2IFile d2i)
        {
            var skills = readers.FirstOrDefault(x => x.Classes.Any(w => w.Value.Name == "Skill")).EnumerateObjects().Cast<Giny.IO.D2OClasses.Skill>();

            StringBuilder sb = new StringBuilder();

            foreach (var skill in skills)
            {
                string name = d2i.GetText((int)skill.NameId);
                sb.AppendLine(ApplyRules(name) + skill.id + "=" + skill.id + ",");
            }

            return sb.ToString();
        }
    }
}
