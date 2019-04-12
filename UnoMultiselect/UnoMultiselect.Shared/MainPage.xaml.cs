using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UnoMultiselect
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Items = new List<Entity>
                    {
                        new Entity
                        {
                            Code = "1",
                            Name = "first"
                        },
                        new Entity
                        {
                            Code = "2",
                            Name = "second"
                        },
                        new Entity
                        {
                            Code = "3",
                            Name = "third"
                        },
                    };

            DataContext = this;
        }

        public List<Entity> Items { get; set; }
    }

    public class Entity
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
