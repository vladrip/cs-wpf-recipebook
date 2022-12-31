using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace KursovaWPF.RecipeViews
{
    /// <summary>
    /// Interaction logic for RecipeCreator.xaml
    /// </summary>
    public partial class RecipeCreator : Window
    {
        public RecipeCreator()
        {
            InitializeComponent();

            foreach (string s in RecipeHelper.MEAL_TYPES)
                mealType.Items.Add(s);
            foreach (string s in RecipeHelper.CUISINES)
                cuisine.Items.Add(s);
        }

        private void Open_Image(object sender, RoutedEventArgs e)
        {
            OpenFileDialog chooseImage = new OpenFileDialog();
            chooseImage.Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*.webp|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" + "PNG (*.png)|*.png|" + "WEBP (*.webp)|*.webp";
            chooseImage.InitialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

            if (chooseImage.ShowDialog() == true)
                recipeImg.Source = new BitmapImage(new Uri(chooseImage.FileName));
        }

        private void ClearBoxes(UIElementCollection UIElCollection)
        {
            name.Text = null;
            components.Text = null;
            steps.Text = null;
            cuisine.SelectedIndex = 0; cuisine.Text = null;
            mealType.SelectedIndex = 0; mealType.Text = null;
            recipeImg.Source = null;
        }

        private void Btn_Clear(object sender, RoutedEventArgs e)
        {
            ClearBoxes(mainGrid.Children);
        }

        private bool IsAllFilled()
        {
            if (string.IsNullOrEmpty(name.Text) || string.IsNullOrEmpty(components.Text) || string.IsNullOrEmpty(steps.Text))
                errMsg.Text = "Потрібно заповнити всі текстові поля";
            else if (RecipeHelper.currentRecipes.Find(x => x.Name.ToLower() == name.Text.ToLower()) != null)
                errMsg.Text = "Рецепт з таким іменем вже існує";
            else if (string.IsNullOrEmpty(mealType.Text) || string.IsNullOrEmpty(cuisine.Text))
                errMsg.Text = "Виберіть тип страви та кухні";
            else if (recipeImg.Source == null)
                errMsg.Text = "Виберіть зображення";
            else return true;

            errMsg.Background = Brushes.Red;
            return false;
        }

        private void Btn_Add(object sender, RoutedEventArgs e)
        {
            if (!IsAllFilled())
                return;

            string imgPath = (recipeImg.Source as BitmapImage).UriSource.LocalPath;
            string localImgPath = Path.Combine(
                Path.GetFullPath(RecipeHelper.IMG_DIR), name.Text + ".jpg"
                );
            File.Copy(imgPath, localImgPath, true);

            RecipeInfo moreInfo = new RecipeInfo(mealType.Text, cuisine.Text);
            Recipe recipe = new Recipe(name.Text, components.Text, moreInfo, steps.Text, localImgPath);
            RecipeHelper.currentRecipes.Add(recipe);
            RecipeHelper.Serialize(RecipeHelper.currentRecipes);

            errMsg.Background = Brushes.Green;
            errMsg.Text = "Рецепт успішно додано";
            ClearBoxes(mainGrid.Children);
        }
    }
}
