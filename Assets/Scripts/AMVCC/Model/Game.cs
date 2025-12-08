using AMVCC.Model.PlayerPkg;

namespace AMVCC.Model
{
    public class Game 
    {
        public Player Player;
        private string _classRecognized;
        public bool Reco { get; set; }
        
        public int CurrentLevel;
        public int NumberOfLevels;
        
        
        public Game()
        {
            Player = new Player(new Fighter());
        }
        public string ClassRecognized
        {
            get { return _classRecognized; }
            set
            {
                Reco = true;
                _classRecognized = value; 
            }
        }
    
    }
}
