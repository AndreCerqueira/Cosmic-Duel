using Match;

namespace Project.Runtime.Scripts.Game.Matches
{
    public class Match
    {
        // public readonly IMatchPlayer[] Players;
        
        public MatchPlayer SelfPlayer;
        public EnemyView Enemy;
        
        public Match(MatchPlayer player, EnemyView enemy)// , MatchEnemy enemy)
        {
            // Players = new IMatchPlayer[] { player, enemy };
            SelfPlayer = player;
            Enemy = enemy;
        }
    }
}
