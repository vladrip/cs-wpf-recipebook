using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace KursovaWPF
{
    public struct RecipeInfo
    {
        public string mealType;
        public string cuisine;

        public RecipeInfo(string mealType, string cuisine)
        {
            this.mealType = mealType;
            this.cuisine = cuisine;
        }
    }

    class RecipeHelper
    {
        public static readonly string PATH = @"../../Resources/recipes.json";
        public static readonly string IMG_DIR = @"../../Resources/Images";

        internal static List<Recipe> currentRecipes = new List<Recipe>();

        public static readonly string[] MEAL_TYPES =
        {
            "Перше", "Друге", "Салат", "Випічка", "Напій", "Десерт"
        };
        public static readonly string[] CUISINES =
        {
            "Домашня", "Азіатська", "Європейська", "Американська", "Китайська", "Японська", "Французька",
            "Італійська", "Українська", "Російська", "Турецька", "Іспанська", "Індійська", "Мексиканська", 
            "Африканська"
        };

        public static void Serialize(List<Recipe> recipes)
        {
            string json = JsonConvert.SerializeObject(currentRecipes, Formatting.Indented);
            File.WriteAllText(PATH, json);
        }

        public static List<Recipe> Deserialize()
        {
            string json = File.ReadAllText(PATH);
            return JsonConvert.DeserializeObject<List<Recipe>>(json);
        }
    }

    public class Recipe : System.IComparable<Recipe>
    {
        public string Name { get; set; }
        public RecipeInfo MoreInfo { get; set; }
        public string Components { get; set; }
        public string Steps { get; set; }
        public string ImagePath { get; set; }

        public Recipe() {}
        public Recipe(string name, string components, RecipeInfo moreInfo, string steps, string imagePath)
        {
            Name = name;
            Components = components;
            MoreInfo = moreInfo;
            Steps = steps;
            ImagePath = imagePath;
        }

        public override bool Equals(object obj)
        {
            return obj is Recipe recipe &&
                   Name == recipe.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        public int CompareTo(Recipe other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
