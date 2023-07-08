using Giny.Core;
using Giny.DatabasePatcher.Experience;
using Giny.DatabasePatcher.Idols;
using Giny.DatabasePatcher.Items;
using Giny.DatabasePatcher.Maps;
using Giny.DatabasePatcher.Monsters;
using Giny.DatabasePatcher.Skills;
using Giny.DatabasePatcher.Spells;
using Giny.World.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giny.DatabasePatcher
{
    [Module("Database patcher")]
    public class Module : IModule
    {
        public void CreateHooks()
        {

        }

        public void Initialize()
        {
            Experiences.Patch();
            ItemAppearances.Patch();
            IdolsSpell.Patch();
            LivingObjects.Patch();
            MapPlacements.Patch();
            SkillBones.Patch();
            InteractiveElements.Patch();
            MonsterKamas.Patch();
            MonsterSpawns.Patch();
            SpellCategories.Patch();
            Teleporters.Patch();
        }


    }
}
