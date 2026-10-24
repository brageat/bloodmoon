using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;

namespace Bloodmoon
{
    // The character is a physics ragdoll, so flying = no gravity on every body part
    // + setting their velocity from the player's input each physics step.
    internal sealed class FlyModule : Module
    {
        public override string Name => "Fly";

        private const float SprintBoost = 2f;
        private const float LandingGrace = 2f; // seconds of no fall damage after turning fly off

        private ConfigEntry<bool> enabled;
        private ConfigEntry<float> speed;
        private ConfigEntry<bool> noclip;
        private bool collisionOff;

        private readonly List<Rigidbody> gravityDisabled = new List<Rigidbody>();
        private bool flying;
        private float targetY;            // the height we should be at; drift is pulled back to it
        private const float HoldStrength = 10f; // how hard drift gets corrected (per second)
        private float graceUntil;

        public override void Init(ConfigFile config, List<MenuItem> items)
        {
            // Not saved as on: you always spawn walking
            enabled = config.Bind("Movement", "Fly", false, "Fly mode (always starts off)");
            enabled.Value = false;
            speed = config.Bind("Movement", "FlySpeed", 12f,
                new ConfigDescription("Flying speed", new AcceptableValueRange<float>(2f, 40f)));

            items.Add(new ToggleItem
            {
                Category = Category.Movement,
                Name = "Fly",
                Description = "WASD to move where you look, Space to go up, Ctrl to go down, Shift to go faster. Let go of everything to hover.",
                Entry = enabled,
            });
            items.Add(new SliderItem
            {
                Category = Category.Movement,
                Name = "Fly speed",
                Description = "How fast you fly. Hold Shift while flying for double speed.",
                Entry = speed,
                Min = 2f, Max = 40f, Step = 2f,
                Format = "0", Suffix = " m/s",
            });

            noclip = config.Bind("Movement", "Noclip", false, "Fly through walls and terrain while flying");
            items.Add(new ToggleItem
            {
                Category = Category.Movement,
                Name = "Noclip (while flying)",
                Description = "While flying, you pass through walls, rocks and the ground. Collision comes back when you stop flying or turn this off.",
                Entry = noclip,
            });
        }

        public override void FixedTick()
        {
            var character = Character.localCharacter;
            if (character == null)
            {
                gravityDisabled.Clear();
                flying = false;
                collisionOff = false;
                return;
            }

            if (enabled.Value)
            {
                if (!flying) targetY = character.Center.y; // start holding where we took off
                Fly(character);
                flying = true;
            }
            else if (flying)
            {
                Land();
                flying = false;
                graceUntil = Time.time + LandingGrace;
            }

            // Noclip only while actually flying; collision always comes back afterwards
            bool wantOff = flying && noclip.Value;
            if (wantOff != collisionOff)
            {
                character.refs.ragdoll.ToggleCollision(!wantOff);
                collisionOff = wantOff;
            }

            // While flying (and briefly after) the game must not think we're falling
            if (flying || Time.time < graceUntil)
            {
                character.data.sinceGrounded = 0f;
                character.data.fallSeconds = 0f;
            }
        }

        private void Fly(Character character)
        {
            var input = character.input;
            var data = character.data;

            Vector2 move = input.movementInput;
            float vertical = (input.jumpIsPressed ? 1f : 0f) - (input.crouchIsPressed ? 1f : 0f);

            Vector3 dir = data.lookDirection * move.y + data.lookDirection_Right * move.x + Vector3.up * vertical;
            if (dir.sqrMagnitude > 1f) dir.Normalize();

            float s = speed.Value * (input.sprintIsPressed ? SprintBoost : 1f);
            Vector3 velocity = dir * s;

            // PEAK keeps pushing the body upright (up), and with gravity off nothing cancels that,
            // so we drift up. Track the height we *meant* to reach and steer back to it.
            float y = character.Center.y;
            targetY += velocity.y * Time.fixedDeltaTime;
            targetY = Mathf.Clamp(targetY, y - 2f, y + 2f); // if a ceiling or floor stops us, don't build up a jump
            float drift = targetY - y;
            velocity.y += Mathf.Clamp(drift * HoldStrength, -s, s);

            foreach (var part in character.refs.ragdoll.partList)
            {
                var rig = part != null ? part.Rig : null;
                if (rig == null) continue;

                if (rig.useGravity)
                {
                    rig.useGravity = false;
                    gravityDisabled.Add(rig);
                }
                rig.linearVelocity = velocity; // zero input = hover in place
            }
        }

        private void Land()
        {
            foreach (var rig in gravityDisabled)
                if (rig != null) rig.useGravity = true;
            gravityDisabled.Clear();
        }
    }
}
