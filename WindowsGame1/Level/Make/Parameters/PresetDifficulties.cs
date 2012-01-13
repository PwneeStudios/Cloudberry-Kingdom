using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class PresetUpgrades
    {
        public static Upgrades GetUpgrade1()
        {
            Upgrades u = new Upgrades();

            switch (Tools.Rnd.Next(0, 14))
            {
                case 0:
                    u[Upgrade.Jump] = 1;
                    u[Upgrade.Ceiling] = 1;
                    u[Upgrade.General] = 1;
                    u[Upgrade.Speed] = 1;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 1;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 1:
                    u[Upgrade.Jump] = 1;
                    u[Upgrade.Ceiling] = 1;
                    u[Upgrade.General] = 1;
                    u[Upgrade.Speed] = 1;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 1;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 2:
                    u[Upgrade.Jump] = 1;
                    u[Upgrade.Ceiling] = 1;
                    u[Upgrade.General] = 1;
                    u[Upgrade.Speed] = 1;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 1;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 3:
                    u[Upgrade.Jump] = 1;
                    u[Upgrade.Ceiling] = 1;
                    u[Upgrade.General] = 1;
                    u[Upgrade.Speed] = 1;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 1;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 4:
                    u[Upgrade.Jump] = 1;
                    u[Upgrade.Ceiling] = 1;
                    u[Upgrade.General] = 1;
                    u[Upgrade.Speed] = 1;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 1;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 5:
                    u[Upgrade.Jump] = 1;
                    u[Upgrade.Ceiling] = 1;
                    u[Upgrade.General] = 1;
                    u[Upgrade.Speed] = 1;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 1;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 6:
                    u[Upgrade.Jump] = 1;
                    u[Upgrade.Ceiling] = 1;
                    u[Upgrade.General] = 1;
                    u[Upgrade.Speed] = 1;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 1;
                    break;

                case 7:
                    u[Upgrade.Jump] = 1;
                    u[Upgrade.Ceiling] = 1;
                    u[Upgrade.General] = 1;
                    u[Upgrade.Speed] = 1;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 1;
                    u[Upgrade.FireSpinner] = 1;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 8:
                    u[Upgrade.Jump] = 1;
                    u[Upgrade.Ceiling] = 1;
                    u[Upgrade.General] = 1;
                    u[Upgrade.Speed] = 1;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 1;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 1;
                    u[Upgrade.Spike] = 0;
                    break;

                case 9:
                    u[Upgrade.Jump] = 1;
                    u[Upgrade.Ceiling] = 1;
                    u[Upgrade.General] = 1;
                    u[Upgrade.Speed] = 1;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 1;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 1;
                    u[Upgrade.Spike] = 0;
                    break;

                case 10:
                    u[Upgrade.Jump] = 1;
                    u[Upgrade.Ceiling] = 1;
                    u[Upgrade.General] = 1;
                    u[Upgrade.Speed] = 1;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 1;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 1;
                    break;

                case 11:
                    u[Upgrade.Jump] = 1;
                    u[Upgrade.Ceiling] = 1;
                    u[Upgrade.General] = 1;
                    u[Upgrade.Speed] = 1;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 1;
                    u[Upgrade.Laser] = 1;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 12:
                    u[Upgrade.Jump] = 1;
                    u[Upgrade.Ceiling] = 1;
                    u[Upgrade.General] = 1;
                    u[Upgrade.Speed] = 1;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 1;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 1;
                    break;

                case 13:
                    u[Upgrade.Jump] = 1;
                    u[Upgrade.Ceiling] = 1;
                    u[Upgrade.General] = 1;
                    u[Upgrade.Speed] = 1;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 1;
                    u[Upgrade.FlyBlob] = 1;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;
            }

            return u;
        }

        public static Upgrades GetUpgrade2()
        {
            Upgrades u = new Upgrades();

            switch (Tools.Rnd.Next(1, 17))
            {
                case 1:
                    u[Upgrade.Jump] = 0;
                    u[Upgrade.Ceiling] = 0;
                    u[Upgrade.General] = 0;
                    u[Upgrade.Speed] = 0;
                    u[Upgrade.Elevator] = 1;
                    u[Upgrade.FallingBlock] = 1;
                    u[Upgrade.Fireball] = 1;
                    u[Upgrade.FireSpinner] = 1;
                    u[Upgrade.SpikeyGuy] = 1;
                    u[Upgrade.FlyBlob] = 1;
                    u[Upgrade.GhostBlock] = 1;
                    u[Upgrade.Laser] = 1;
                    u[Upgrade.MovingBlock] = 1;
                    u[Upgrade.Spike] = 1;
                    break;

                case 2:
                    u[Upgrade.Jump] = 3;
                    u[Upgrade.Ceiling] = 3;
                    u[Upgrade.General] = 3;
                    u[Upgrade.Speed] = 3;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 3;
                    u[Upgrade.FireSpinner] = 3;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 3:
                    u[Upgrade.Jump] = 3;
                    u[Upgrade.Ceiling] = 3;
                    u[Upgrade.General] = 3;
                    u[Upgrade.Speed] = 3;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 3;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 3;
                    u[Upgrade.Spike] = 0;
                    break;

                case 4:
                    u[Upgrade.Jump] = 3;
                    u[Upgrade.Ceiling] = 3;
                    u[Upgrade.General] = 3;
                    u[Upgrade.Speed] = 3;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 3;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 3;
                    u[Upgrade.Spike] = 0;
                    break;

                case 5:
                    u[Upgrade.Jump] = 3;
                    u[Upgrade.Ceiling] = 3;
                    u[Upgrade.General] = 3;
                    u[Upgrade.Speed] = 3;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 3;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 3;
                    break;

                case 6:
                    u[Upgrade.Jump] = 3;
                    u[Upgrade.Ceiling] = 3;
                    u[Upgrade.General] = 3;
                    u[Upgrade.Speed] = 3;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 3;
                    u[Upgrade.Laser] = 3;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 7:
                    u[Upgrade.Jump] = 3;
                    u[Upgrade.Ceiling] = 3;
                    u[Upgrade.General] = 3;
                    u[Upgrade.Speed] = 3;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 3;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 3;
                    break;

                case 8:
                    u[Upgrade.Jump] = 3;
                    u[Upgrade.Ceiling] = 3;
                    u[Upgrade.General] = 3;
                    u[Upgrade.Speed] = 3;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 3;
                    u[Upgrade.FlyBlob] = 3;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 9:
                    u[Upgrade.Jump] = 3;
                    u[Upgrade.Ceiling] = 3;
                    u[Upgrade.General] = 3;
                    u[Upgrade.Speed] = 3;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 3;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 3;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 3;
                    u[Upgrade.Spike] = 0;
                    break;

                case 10:
                    u[Upgrade.Jump] = 3;
                    u[Upgrade.Ceiling] = 3;
                    u[Upgrade.General] = 3;
                    u[Upgrade.Speed] = 3;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 3;
                    u[Upgrade.GhostBlock] = 3;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 3;
                    u[Upgrade.Spike] = 0;
                    break;

                case 11:
                    u[Upgrade.Jump] = 3;
                    u[Upgrade.Ceiling] = 3;
                    u[Upgrade.General] = 3;
                    u[Upgrade.Speed] = 3;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 3;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 3;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 3;
                    u[Upgrade.Spike] = 0;
                    break;

                case 12:
                    u[Upgrade.Jump] = 3;
                    u[Upgrade.Ceiling] = 3;
                    u[Upgrade.General] = 3;
                    u[Upgrade.Speed] = 3;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 3;
                    u[Upgrade.FireSpinner] = 3;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 3;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 13:
                    u[Upgrade.Jump] = 3;
                    u[Upgrade.Ceiling] = 3;
                    u[Upgrade.General] = 3;
                    u[Upgrade.Speed] = 3;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 3;
                    u[Upgrade.FireSpinner] = 3;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 3;
                    u[Upgrade.Spike] = 0;
                    break;

                case 14:
                    u[Upgrade.Jump] = 3;
                    u[Upgrade.Ceiling] = 3;
                    u[Upgrade.General] = 3;
                    u[Upgrade.Speed] = 3;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 3;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 3;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 3;
                    u[Upgrade.Spike] = 0;
                    break;

                case 15:
                    u[Upgrade.Jump] = 3;
                    u[Upgrade.Ceiling] = 3;
                    u[Upgrade.General] = 3;
                    u[Upgrade.Speed] = 3;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 3;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 3;
                    u[Upgrade.Spike] = 3;
                    break;

                case 16:
                    u[Upgrade.Jump] = 3;
                    u[Upgrade.Ceiling] = 3;
                    u[Upgrade.General] = 3;
                    u[Upgrade.Speed] = 3;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 3;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 3;
                    u[Upgrade.Laser] = 3;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;
            }

            return u;
        }

        public static Upgrades GetUpgrade3()
        {
            Upgrades u = new Upgrades();

            switch (Tools.Rnd.Next(0, 25))
            {
                case 0:
                    u[Upgrade.Jump] = 2;
                    u[Upgrade.Ceiling] = 2;
                    u[Upgrade.General] = 2;
                    u[Upgrade.Speed] = 2;
                    u[Upgrade.Elevator] = 3;
                    u[Upgrade.FallingBlock] = 3;
                    u[Upgrade.Fireball] = 3;
                    u[Upgrade.FireSpinner] = 3;
                    u[Upgrade.SpikeyGuy] = 3;
                    u[Upgrade.FlyBlob] = 3;
                    u[Upgrade.GhostBlock] = 3;
                    u[Upgrade.Laser] = 3;
                    u[Upgrade.MovingBlock] = 3;
                    u[Upgrade.Spike] = 3;
                    break;

                case 1:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 5;
                    u[Upgrade.Speed] = 5;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 5;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 5;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 5;
                    u[Upgrade.Spike] = 0;
                    break;

                case 2:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 5;
                    u[Upgrade.Speed] = 5;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 5;
                    u[Upgrade.GhostBlock] = 5;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 5;
                    u[Upgrade.Spike] = 0;
                    break;

                case 3:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 5;
                    u[Upgrade.Speed] = 5;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 5;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 5;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 5;
                    u[Upgrade.Spike] = 0;
                    break;

                case 4:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 5;
                    u[Upgrade.Speed] = 5;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 5;
                    u[Upgrade.FireSpinner] = 5;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 5;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 5:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 5;
                    u[Upgrade.Speed] = 5;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 5;
                    u[Upgrade.FireSpinner] = 5;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 5;
                    u[Upgrade.Spike] = 0;
                    break;

                case 6:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 5;
                    u[Upgrade.Speed] = 5;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 5;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 5;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 5;
                    u[Upgrade.Spike] = 0;
                    break;

                case 7:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 5;
                    u[Upgrade.Speed] = 5;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 5;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 5;
                    u[Upgrade.Spike] = 5;
                    break;

                case 8:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 5;
                    u[Upgrade.Speed] = 5;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 5;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 5;
                    u[Upgrade.Laser] = 5;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 9:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 5;
                    u[Upgrade.Speed] = 5;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 5;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 5;
                    u[Upgrade.FlyBlob] = 5;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 5;
                    u[Upgrade.Spike] = 0;
                    break;

                case 10:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 5;
                    u[Upgrade.Speed] = 5;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 5;
                    u[Upgrade.GhostBlock] = 5;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 5;
                    u[Upgrade.Spike] = 5;
                    break;

                case 11:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 5;
                    u[Upgrade.Speed] = 5;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 5;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 5;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 5;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 5;
                    u[Upgrade.Spike] = 0;
                    break;

                case 12:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 5;
                    u[Upgrade.Speed] = 5;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 5;
                    u[Upgrade.FireSpinner] = 5;
                    u[Upgrade.SpikeyGuy] = 5;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 5;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 13:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 5;
                    u[Upgrade.Speed] = 5;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 5;
                    u[Upgrade.Fireball] = 5;
                    u[Upgrade.FireSpinner] = 5;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 5;
                    u[Upgrade.Spike] = 0;
                    break;

                case 14:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 5;
                    u[Upgrade.Speed] = 5;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 5;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 5;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 5;
                    u[Upgrade.MovingBlock] = 5;
                    u[Upgrade.Spike] = 0;
                    break;

                case 15:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 5;
                    u[Upgrade.Speed] = 5;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 5;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 5;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 5;
                    u[Upgrade.Spike] = 5;
                    break;

                case 16:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 5;
                    u[Upgrade.Speed] = 5;
                    u[Upgrade.Elevator] = 5;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 5;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 5;
                    u[Upgrade.Laser] = 5;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 17:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 7;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 7;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 18:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 7;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 7;
                    u[Upgrade.Spike] = 0;
                    break;

                case 19:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 3;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 7;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 7;
                    u[Upgrade.Spike] = 0;
                    break;

                case 20:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 7;
                    u[Upgrade.FireSpinner] = 7;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 21:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 7;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 7;
                    u[Upgrade.Spike] = 0;
                    break;

                case 22:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 7;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 7;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 23:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 7;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 7;
                    break;

                case 24:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 7;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 7;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;


            }

            return u;
        }

        public static Upgrades GetUpgrade4()
        {
            Upgrades u = new Upgrades();

            switch (Tools.Rnd.Next(0, 25))
            {
                case 0:
                    u[Upgrade.Jump] = 4;
                    u[Upgrade.Ceiling] = 4;
                    u[Upgrade.General] = 4;
                    u[Upgrade.Speed] = 4;
                    u[Upgrade.Elevator] = 5;
                    u[Upgrade.FallingBlock] = 5;
                    u[Upgrade.Fireball] = 5;
                    u[Upgrade.FireSpinner] = 5;
                    u[Upgrade.SpikeyGuy] = 5;
                    u[Upgrade.FlyBlob] = 5;
                    u[Upgrade.GhostBlock] = 5;
                    u[Upgrade.Laser] = 5;
                    u[Upgrade.MovingBlock] = 5;
                    u[Upgrade.Spike] = 5;
                    break;

                case 1:
                    u[Upgrade.Jump] = 7;
                    u[Upgrade.Ceiling] = 7;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 7;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 7;
                    u[Upgrade.FlyBlob] = 7;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 7;
                    u[Upgrade.Spike] = 0;
                    break;

                case 2:
                    u[Upgrade.Jump] = 7;
                    u[Upgrade.Ceiling] = 7;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 7;
                    u[Upgrade.GhostBlock] = 7;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 7;
                    u[Upgrade.Spike] = 7;
                    break;

                case 3:
                    u[Upgrade.Jump] = 7;
                    u[Upgrade.Ceiling] = 7;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 7;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 7;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 7;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 7;
                    u[Upgrade.Spike] = 0;
                    break;

                case 4:
                    u[Upgrade.Jump] = 7;
                    u[Upgrade.Ceiling] = 7;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 7;
                    u[Upgrade.FireSpinner] = 7;
                    u[Upgrade.SpikeyGuy] = 7;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 7;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 5:
                    u[Upgrade.Jump] = 7;
                    u[Upgrade.Ceiling] = 7;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 7;
                    u[Upgrade.Fireball] = 7;
                    u[Upgrade.FireSpinner] = 7;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 7;
                    u[Upgrade.Spike] = 0;
                    break;

                case 6:
                    u[Upgrade.Jump] = 7;
                    u[Upgrade.Ceiling] = 7;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 7;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 7;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 7;
                    u[Upgrade.MovingBlock] = 7;
                    u[Upgrade.Spike] = 0;
                    break;

                case 7:
                    u[Upgrade.Jump] = 7;
                    u[Upgrade.Ceiling] = 7;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 7;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 7;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 7;
                    u[Upgrade.Spike] = 7;
                    break;

                case 8:
                    u[Upgrade.Jump] = 7;
                    u[Upgrade.Ceiling] = 7;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 7;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 7;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 7;
                    u[Upgrade.Laser] = 7;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 9:
                    u[Upgrade.Jump] = 7;
                    u[Upgrade.Ceiling] = 7;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 7;
                    u[Upgrade.FallingBlock] = 7;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 7;
                    u[Upgrade.FlyBlob] = 7;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 7;
                    u[Upgrade.Spike] = 0;
                    break;

                case 10:
                    u[Upgrade.Jump] = 7;
                    u[Upgrade.Ceiling] = 7;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 7;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 7;
                    u[Upgrade.GhostBlock] = 7;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 7;
                    u[Upgrade.Spike] = 7;
                    break;

                case 11:
                    u[Upgrade.Jump] = 7;
                    u[Upgrade.Ceiling] = 7;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 7;
                    u[Upgrade.Fireball] = 7;
                    u[Upgrade.FireSpinner] = 7;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 7;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 7;
                    u[Upgrade.Spike] = 0;
                    break;

                case 12:
                    u[Upgrade.Jump] = 7;
                    u[Upgrade.Ceiling] = 7;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 7;
                    u[Upgrade.FireSpinner] = 7;
                    u[Upgrade.SpikeyGuy] = 7;
                    u[Upgrade.FlyBlob] = 7;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 7;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 13:
                    u[Upgrade.Jump] = 7;
                    u[Upgrade.Ceiling] = 7;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 7;
                    u[Upgrade.Fireball] = 7;
                    u[Upgrade.FireSpinner] = 7;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 7;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 7;
                    u[Upgrade.Spike] = 0;
                    break;

                case 14:
                    u[Upgrade.Jump] = 7;
                    u[Upgrade.Ceiling] = 7;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 7;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 7;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 7;
                    u[Upgrade.MovingBlock] = 7;
                    u[Upgrade.Spike] = 7;
                    break;

                case 15:
                    u[Upgrade.Jump] = 7;
                    u[Upgrade.Ceiling] = 7;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 7;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 7;
                    u[Upgrade.FlyBlob] = 7;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 7;
                    u[Upgrade.Spike] = 7;
                    break;

                case 16:
                    u[Upgrade.Jump] = 7;
                    u[Upgrade.Ceiling] = 7;
                    u[Upgrade.General] = 7;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 7;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 7;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 7;
                    u[Upgrade.Laser] = 7;
                    u[Upgrade.MovingBlock] = 7;
                    u[Upgrade.Spike] = 0;
                    break;

                case 17:
                    u[Upgrade.Jump] = 3;
                    u[Upgrade.Ceiling] = 3;
                    u[Upgrade.General] = 8;
                    u[Upgrade.Speed] = 8;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 9;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 18:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 8;
                    u[Upgrade.Speed] = 8;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 9;
                    u[Upgrade.GhostBlock] = 9;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 3;
                    u[Upgrade.Spike] = 0;
                    break;

                case 19:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 4;
                    u[Upgrade.Speed] = 4;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 9;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 20:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 5;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 9;
                    u[Upgrade.FireSpinner] = 9;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 9;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 21:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 3;
                    u[Upgrade.Speed] = 3;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 9;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 0;
                    break;

                case 22:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 5;
                    u[Upgrade.Speed] = 5;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 9;
                    break;

                case 23:
                    u[Upgrade.Jump] = 7;
                    u[Upgrade.Ceiling] = 3;
                    u[Upgrade.General] = 3;
                    u[Upgrade.Speed] = 7;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 0;
                    break;

                case 24:
                    u[Upgrade.Jump] = 5;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 5;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 9;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 9;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;
            }

            return u;
        }

        public static Upgrades GetUpgrade5()
        {
            Upgrades u = new Upgrades();

            switch (Tools.Rnd.Next(0, 30))
            {
                case 0:
                    u[Upgrade.Ceiling] = 6;
                    u[Upgrade.Elevator] = 7;
                    u[Upgrade.FallingBlock] = 7;
                    u[Upgrade.Fireball] = 7;
                    u[Upgrade.FireSpinner] = 7;
                    u[Upgrade.SpikeyGuy] = 7;
                    u[Upgrade.FlyBlob] = 7;
                    u[Upgrade.General] = 6;
                    u[Upgrade.GhostBlock] = 7;
                    u[Upgrade.Jump] = 6;
                    u[Upgrade.Laser] = 7;
                    u[Upgrade.MovingBlock] = 7;
                    u[Upgrade.Speed] = 6;
                    u[Upgrade.Spike] = 7;
                    break;

                case 1: u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 9;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 9;
                    u[Upgrade.FlyBlob] = 9;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 9;
                    break;

                case 2:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 9;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 9;
                    u[Upgrade.GhostBlock] = 9;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 9;
                    break;

                case 3:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 9;
                    u[Upgrade.FireSpinner] = 9;
                    u[Upgrade.SpikeyGuy] = 9;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 9;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 0;
                    break;

                case 4:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 9;
                    u[Upgrade.FireSpinner] = 9;
                    u[Upgrade.SpikeyGuy] = 9;
                    u[Upgrade.FlyBlob] = 9;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 9;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 5:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 9;
                    u[Upgrade.FireSpinner] = 9;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 9;
                    u[Upgrade.Laser] = 9;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 0;
                    break;

                case 6:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 9;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 9;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 9;
                    break;

                case 7:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 9;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 9;
                    u[Upgrade.FlyBlob] = 9;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 9;
                    break;

                case 8:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 9;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 9;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 9;
                    u[Upgrade.Laser] = 9;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 9;
                    break;

                case 9: u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 9;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 9;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 9;
                    u[Upgrade.FlyBlob] = 9;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 9;
                    break;

                case 10:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 9;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 9;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 9;
                    u[Upgrade.GhostBlock] = 9;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 9;
                    break;

                case 11:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 9;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 9;
                    u[Upgrade.FireSpinner] = 9;
                    u[Upgrade.SpikeyGuy] = 9;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 9;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 0;
                    break;

                case 12:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 9;
                    u[Upgrade.FireSpinner] = 9;
                    u[Upgrade.SpikeyGuy] = 9;
                    u[Upgrade.FlyBlob] = 9;
                    u[Upgrade.GhostBlock] = 9;
                    u[Upgrade.Laser] = 9;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 13:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 9;
                    u[Upgrade.FireSpinner] = 9;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 9;
                    u[Upgrade.GhostBlock] = 9;
                    u[Upgrade.Laser] = 9;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 0;
                    break;

                case 14:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 9;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 9;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 9;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 9;
                    break;

                case 15:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 9;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 9;
                    u[Upgrade.FlyBlob] = 9;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 9;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 9;
                    break;

                case 16:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 9;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 9;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 9;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 9;
                    u[Upgrade.Laser] = 9;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 9;
                    break;

                case 17:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 9;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 0;
                    break;

                case 18:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 9;
                    u[Upgrade.GhostBlock] = 9;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 0;
                    break;

                case 19:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 9;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 0;
                    break;

                case 20:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 9;
                    u[Upgrade.FireSpinner] = 9;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 9;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 21:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 9;
                    u[Upgrade.FireSpinner] = 9;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 0;
                    break;

                case 22:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 9;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 0;
                    break;

                case 23:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 9;
                    break;

                case 24:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 9;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 9;
                    u[Upgrade.Laser] = 9;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 25:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 9;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Spike] = 0;
                    break;

                case 26:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 9;
                    break;

                case 27:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 0;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 9;
                    u[Upgrade.Laser] = 9;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;

                case 28:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 9;
                    u[Upgrade.FlyBlob] = 0;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 9;
                    break;

                case 29:
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Elevator] = 0;
                    u[Upgrade.FallingBlock] = 0;
                    u[Upgrade.Fireball] = 0;
                    u[Upgrade.FireSpinner] = 0;
                    u[Upgrade.SpikeyGuy] = 9;
                    u[Upgrade.FlyBlob] = 9;
                    u[Upgrade.GhostBlock] = 0;
                    u[Upgrade.Laser] = 0;
                    u[Upgrade.MovingBlock] = 0;
                    u[Upgrade.Spike] = 0;
                    break;
            }

            return u;
        }

        public static Upgrades GetUpgrade6()
        {
            Upgrades u = new Upgrades();

            switch (Tools.Rnd.Next(0, 0))
            {
                case 0:
                    u[Upgrade.Ceiling] = 9;
                    u[Upgrade.Elevator] = 9;
                    u[Upgrade.FallingBlock] = 9;
                    u[Upgrade.Fireball] = 9;
                    u[Upgrade.FireSpinner] = 9;
                    u[Upgrade.SpikeyGuy] = 9;
                    u[Upgrade.FlyBlob] = 9;
                    u[Upgrade.General] = 9;
                    u[Upgrade.GhostBlock] = 9;
                    u[Upgrade.Jump] = 9;
                    u[Upgrade.Laser] = 9;
                    u[Upgrade.MovingBlock] = 9;
                    u[Upgrade.Speed] = 9;
                    u[Upgrade.Spike] = 9;
                    break;
            }

            return u;
        }

        public static Upgrades GetUpgrade(int Difficulty)
        {
            switch (Difficulty)
            {
                default:
                    return GetUpgrade1();
                case 2:
                    return GetUpgrade2();
                case 3:
                    return GetUpgrade3();
                case 4:
                    return GetUpgrade4();
                case 5:
                    return GetUpgrade5();
                case 6:
                    return GetUpgrade6();
            }
        }
    }
}