using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;
using System;

namespace KursovaWPF.RecipeViews
{
    public partial class RecipeEditor : Window
    {
        SortedSet<Recipe> filteredRecipes = new SortedSet<Recipe>(RecipeHelper.currentRecipes);
        DockPanel previousSelectedRecipe = null;
        Recipe currentRecipe = null;

        public RecipeEditor()
        {
            InitializeComponent();
            UpdateSidebar();

            foreach (string s in RecipeHelper.MEAL_TYPES)
                mealType.Items.Add(s);
            foreach (string s in RecipeHelper.CUISINES)
                cuisine.Items.Add(s);
        }

        private void FilterRecipes()
        {
            string find = search.Text;
            filteredRecipes.RemoveWhere(x => !ContainsText(x, find));

            foreach (Recipe r in RecipeHelper.currentRecipes)
                if (!filteredRecipes.Contains(r) && ContainsText(r, find))
                    filteredRecipes.Add(r);
        }

        private void addSidebarRecipeView(Recipe r)
        {
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(1.5);

            DockPanel dockPanel = new DockPanel();
            dockPanel.MouseUp += new MouseButtonEventHandler(SidebarItem_Click);

            Image image = new Image();
            image.Width = 100;
            image.Stretch = Stretch.Fill;
            image.Source = new BitmapImage(new System.Uri(r.ImagePath, System.UriKind.Absolute));

            TextBlock textBlock = new TextBlock();
            textBlock.Margin = new Thickness(3);
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.Text = r.Name;

            border.Child = dockPanel;
            dockPanel.Children.Add(image);
            dockPanel.Children.Add(textBlock);

            sideViewer.Children.Add(border);
        }

        private void UpdateSidebar()
        {
            FilterRecipes();
            sideViewer.Children.Clear();
            foreach (Recipe r in filteredRecipes)
                addSidebarRecipeView(r);
        }

        private bool ContainsText(Recipe r, string find)
        {
            string allText = r.Name + r.MoreInfo.mealType + r.MoreInfo.cuisine + r.Components + r.Steps;
            return string.IsNullOrEmpty(find) || allText.ToLower().Contains(find.ToLower());
        }

        private void search_Start(object sender, RoutedEventArgs e)
        {
            UpdateSidebar();
        }

        private void fillEditForm(Recipe r)
        {
            name.Text = r.Name;
            mealType.Text = r.MoreInfo.mealType;
            cuisine.Text = r.MoreInfo.cuisine;
            components.Text = r.Components;
            steps.Text = r.Steps;
            if (string.IsNullOrEmpty(r.ImagePath))
                recipeImg.Source = null;
            else recipeImg.Source = new BitmapImage(new Uri(r.ImagePath, UriKind.Absolute));

            errMsg.Background = editGrid.Background;
            errMsg.Text = null;
        }

        private void SidebarItem_Click(object sender, RoutedEventArgs e)
        {
            DockPanel selectedRecipe = sender as DockPanel;
            (selectedRecipe.Parent as Border).BorderBrush = Brushes.Red;
            if (previousSelectedRecipe != null)
                (previousSelectedRecipe.Parent as Border).BorderBrush = Brushes.Gray;
            previousSelectedRecipe = selectedRecipe;

            string name = (selectedRecipe.Children[1] as TextBlock).Text;
            Recipe r = RecipeHelper.currentRecipes.Find(x => x.Name == name);
            currentRecipe = r;
            fillEditForm(r);
        }

        private void Open_Image(object sender, RoutedEventArgs e)
        {
            OpenFileDialog chooseImage = new OpenFileDialog();
            chooseImage.Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*.webp|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" + "PNG (*.png)|*.png|" + "WEBP (*.webp)|*.webp";
            chooseImage.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (chooseImage.ShowDialog() == true)
                recipeImg.Source = new BitmapImage(new Uri(chooseImage.FileName));
        }

        private bool IsAllFilled()
        {
            if (string.IsNullOrEmpty(name.Text) || string.IsNullOrEmpty(components.Text) || string.IsNullOrEmpty(steps.Text))
                errMsg.Text = "Потрібно заповнити всі текстові поля";
            else if (string.IsNullOrEmpty(mealType.Text) || string.IsNullOrEmpty(cuisine.Text))
                errMsg.Text = "Виберіть тип страви та кухні";
            else if (recipeImg.Source == null)
                errMsg.Text = "Виберіть зображення";
            else return true;

            errMsg.Background = Brushes.Red;
            return false;
        }

        private void Btn_Restore(object sender, RoutedEventArgs e)
        {
            fillEditForm(currentRecipe);
        }

        private void Btn_AcceptChanges(object sender, RoutedEventArgs e)
        {
            if (!IsAllFilled())
                return;

            currentRecipe.Name = name.Text;
            currentRecipe.MoreInfo = new RecipeInfo(mealType.Text, cuisine.Text);
            currentRecipe.Steps = steps.Text;
            currentRecipe.Components = components.Text;

            string imgPath = (recipeImg.Source as BitmapImage).UriSource.LocalPath;
            string localImgPath = Path.Combine(
                Path.GetFullPath(RecipeHelper.IMG_DIR), name.Text + ".jpg"
                );
            if (!File.Exists(localImgPath))
                File.Copy(imgPath, localImgPath);
            currentRecipe.ImagePath = localImgPath;

            errMsg.Background = Brushes.Green;
            errMsg.Text = "Зміни успішно прийняті";
        }
        
        private void Btn_Delete(object sender, RoutedEventArgs e)
        {
            RecipeHelper.currentRecipes.Remove(currentRecipe);
            filteredRecipes.Remove(currentRecipe);
            currentRecipe = new Recipe("", "", new RecipeInfo("", ""), "", null);
            fillEditForm(currentRecipe);
            UpdateSidebar();

            errMsg.Background = Brushes.Green;
            errMsg.Text = "Рецепт успішно видалено";
        }
    }
}
