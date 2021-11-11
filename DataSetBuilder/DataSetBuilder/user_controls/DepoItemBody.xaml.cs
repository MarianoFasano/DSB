using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataSetBuilder.user_controls
{
    /// <summary>
    /// Logica di interazione per DepoItemBody.xaml
    /// </summary>
    public partial class DepoItemBody : UserControl
    {
        public DepoItemBody()
        {
            InitializeComponent();
        }

        public DocumentViewer getDocViewer()
        {
            return DocViewer;
        }
        public StackPanel getDataList()
        {
            return DataList;
        }
        public Button getPlayButton()
        {
            return PlayImage;
        }
        public Button getPauseButton()
        {
            return PauseImage;
        }
        public Button getPrevButton()
        {
            return PrevImage;
        }
        public Button getNextButton()
        {
            return NextImage;
        }
        public ComboBox getSpeed()
        {
            return ImageSpeed;
        }
        public Image getImage()
        {
            return DepoImage;
        }
    }
}
