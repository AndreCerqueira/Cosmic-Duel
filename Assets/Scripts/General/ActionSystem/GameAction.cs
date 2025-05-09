using System.Collections.Generic;

namespace Project.Runtime.Scripts.General.ActionSystem
{
    public abstract class GameAction
    {
        public List<GameAction> PreReactions { get; private set; } = new();
        public List<GameAction> PerformReactions { get; private set; } = new();
        public List<GameAction> PostReactions { get; private set; } = new();
    }
}
