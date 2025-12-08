
namespace AMVCC
{
    public class Application : AppElement
    {
        public AppModel Model;

        public AppView View => Assert<AppView>(_view);
        private AppView _view;

        /// <summary>
        /// Verbose level.
        /// </summary>
        public int Verbose;

        public void Awake()
        {
            Model = new AppModel();
        }
    }
}
