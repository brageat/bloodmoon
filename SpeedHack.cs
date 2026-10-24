namespace Bloodmoon
{
    internal static class SpeedHack
    {
        private static bool applied;

        public static void Tick(float multiplier)
        {
            var character = Character.localCharacter;
            if (character == null) return;

            // Leave the game alone at 1x so its own modifiers still work normally
            bool active = !UnityEngine.Mathf.Approximately(multiplier, 1f);
            if (!active && !applied) return;

            character.refs.movement.movementModifier = active ? multiplier : 1f;
            applied = active;
        }
    }
}
