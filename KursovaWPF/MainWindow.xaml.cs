using System.IO;
using System.ComponentModel;
using System.Windows;

namespace KursovaWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() 
        {
            if (!File.Exists(RecipeHelper.PATH))
            {
                FileStream fs = File.Create(RecipeHelper.PATH);
                fs.Close();
            }
            else RecipeHelper.currentRecipes = RecipeHelper.Deserialize();

            InitializeComponent();
        }

        private void Recepts_View(object sender, RoutedEventArgs e)
        {
            RecipeViews.RecipeViewer recipeViewer = new RecipeViews.RecipeViewer();
            recipeViewer.Show();
        }

        private void Recepts_Create(object sender, RoutedEventArgs e)
        {
            RecipeViews.RecipeCreator recipeCreator = new RecipeViews.RecipeCreator();
            recipeCreator.Show();
        }

        private void Recepts_Edit(object sender, RoutedEventArgs e)
        {
            RecipeViews.RecipeEditor recipeEditor = new RecipeViews.RecipeEditor();
            recipeEditor.Show();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            RecipeHelper.Serialize(RecipeHelper.currentRecipes);
            base.OnClosing(e);
        }
    }
}
