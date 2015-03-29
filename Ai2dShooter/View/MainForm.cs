using System.Windows.Forms;
using Ai2dShooter.Map;

namespace Ai2dShooter.View
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            Maze.CreateNew(10, 10);
            Maze.Instance.SaveMap("maze with path.png");
        }
    }
}
