using System.Collections.Generic;
using BepInEx.Configuration;

namespace Bloodmoon
{
    // A mod inside Bloodmoon. To add one: make a class like SpeedModule, then list it in Plugin.Modules.
    internal abstract class Module
    {
        public abstract string Name { get; }

        // Bind config entries and add this module's rows to the menu
        public abstract void Init(ConfigFile config, List<MenuItem> items);

        // Runs every frame, whether or not the menu is open
        public virtual void Tick() { }
    }
}
