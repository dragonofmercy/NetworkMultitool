using System.Collections.Generic;

namespace NetworkMultitool.Tool
{
    /// <summary>Current keyboard shortcut of an action as text ("Ctrl+S"), read from the settings so the hints follow rebinds.</summary>
    public static class KeyNames
    {
        public static string Of(ToolAction action) => Of(action.ToString());

        /// <summary>Takes a <see cref="ToolActions"/> name; empty when unbound.</summary>
        public static string Of(string action)
        {
            var proxy = Mod.Settings?.GetAction(action);
            if (proxy == null)
                return "";

            foreach (var binding in proxy.bindings)
            {
                if (!binding.isKeyboard || !binding.isSet)
                    continue;
                var keys = new List<string>();
                foreach (var modifier in binding.modifiers)
                    keys.Add(Name(modifier.m_Path));
                keys.Add(Name(binding.path));
                return string.Join("+", keys);
            }

            return "";
        }

        // "<Keyboard>/ctrl" gives "Ctrl", "<Keyboard>/upArrow" gives "Up".
        private static string Name(string path)
        {
            var name = path.Substring(path.LastIndexOf('/') + 1).Replace("Arrow", "");
            return name.Length == 0 ? name : char.ToUpperInvariant(name[0]) + name.Substring(1);
        }
    }
}
