using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace KursovaWPF.RecipeViews
{
    public partial class RecipeViewer : Window
    {
        SortedSet<Recipe> filteredRecipes = new SortedSet<Recipe>(RecipeHelper.currentRecipes);
        HashSet<string> mealTypeFilters = new HashSet<string>(RecipeHelper.MEAL_TYPES.Length);
        HashSet<string> cuisineFilters = new HashSet<string>(RecipeHelper.CUISINES.Length);
        DockPanel previousSelectedRecipe = null;

        public RecipeViewer()
        {
            InitializeComponent();

            foreach (string s in RecipeHelper.MEAL_TYPES)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Content = s;
                checkBox.Margin = new Thickness(5);
                checkBox.Click += new RoutedEventHandler(Filter_MealType);
                mealTypes.Children.Add(checkBox);
                mealTypeFilters.Add(s);
            }

            foreach (string s in RecipeHelper.CUISINES)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Content = s;
                checkBox.Margin = new Thickness(5);
                checkBox.Click += new RoutedEventHandler(Filter_Cuisine);
                cuisines.Children.Add(checkBox);
                cuisineFilters.Add(s);
            }

            allMealTypesCheck.IsChecked = true;
            allCuisinesCheck.IsChecked = true;
            Filter_MealType(FindName(allMealTypesCheck.Name), null);
            Filter_Cuisine(FindName(allCuisinesCheck.Name), null);
        }

        private void FilterRecipes()
        {
            string find = search.Text;
            filteredRecipes.RemoveWhere(x => !mealTypeFilters.Contains(x.MoreInfo.mealType) 
                || !cuisineFilters.Contains(x.MoreInfo.cuisine) || !ContainsText(x, find));

            foreach (Recipe r in RecipeHelper.currentRecipes)
                if (!filteredRecipes.Contains(r)
                        && mealTypeFilters.Contains(r.MoreInfo.mealType)
                        && cuisineFilters.Contains(r.MoreInfo.cuisine)
                        && ContainsText(r, find))

                    filteredRecipes.Add(r);
        }

        /*
        <Border BorderBrush="Black" BorderThickness="1.5">
            <DockPanel MouseUp="SidebarItem_Click">
                <Image Width="100" Stretch="Fill" Source="test"></Image>
                <TextBlock TextWrapping="Wrap">test</TextBlock>
            </DockPanel>
        </Border>
        */
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

        private void selectAllInPanel(Panel panel, bool toSelect)
        {
            foreach (UIElement el in panel.Children)
                if (el is CheckBox)
                    (el as CheckBox).IsChecked = toSelect;

            if (toSelect)
            {
                if (panel.Name == "mealTypes")
                    mealTypeFilters = new HashSet<string>(RecipeHelper.MEAL_TYPES);
                else if (panel.Name == "cuisines")
                    cuisineFilters = new HashSet<string>(RecipeHelper.CUISINES);
            }
            else
            {
                if (panel.Name == "mealTypes")
                    mealTypeFilters.Clear();
                else if (panel.Name == "cuisines")
                    cuisineFilters.Clear();
            }
        }

        private void Filter_MealType(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            bool isChecked = checkBox.IsChecked == true ? true : false;
            if (checkBox.Name == "allMealTypesCheck")
                selectAllInPanel(checkBox.Parent as Panel, isChecked);
            else if (!isChecked)
            {
                mealTypeFilters.Remove(checkBox.Content as string);
                allMealTypesCheck.IsChecked = false;
            }
            else mealTypeFilters.Add(checkBox.Content as string);

            UpdateSidebar();
        }

        private void Filter_Cuisine(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            bool isChecked = checkBox.IsChecked == true ? true : false;
            if (checkBox.Name == "allCuisinesCheck")
                selectAllInPanel(checkBox.Parent as Panel, isChecked);
            else if (!isChecked)
            {
                cuisineFilters.Remove(checkBox.Content as string);
                allCuisinesCheck.IsChecked = false;
            }
            else cuisineFilters.Add(checkBox.Content as string);

            UpdateSidebar();
        }

        private void formRecipeView(Recipe r)
        {
            name.Text = r.Name;
            mealType.Text = r.MoreInfo.mealType;
            cuisine.Text = r.MoreInfo.cuisine;

            bigImg.Source = new BitmapImage(new System.Uri(r.ImagePath, System.UriKind.Absolute));

            components.Text = r.Components;
            steps.Text = r.Steps;
        }

        private void SidebarItem_Click(object sender, MouseButtonEventArgs e)
        {
            DockPanel selectedRecipe = sender as DockPanel;
            (selectedRecipe.Parent as Border).BorderBrush = Brushes.Red;
            if (previousSelectedRecipe != null)
                (previousSelectedRecipe.Parent as Border).BorderBrush = Brushes.Gray;
            previousSelectedRecipe = selectedRecipe;

            string name = (selectedRecipe.Children[1] as TextBlock).Text;
            Recipe r = RecipeHelper.currentRecipes.Find(x => x.Name == name);
            formRecipeView(r);
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
    }
}
