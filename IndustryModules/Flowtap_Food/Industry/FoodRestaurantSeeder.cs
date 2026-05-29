using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Food.DbContext;
using Flowtap_Food.Domain.Entities;
using Flowtap_Food.Domain.Enums;

namespace Flowtap_Food.Industry;

public static class FoodRestaurantSeeder
{
    public static async Task SeedAsync(IApplicationDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedProductCategories(context, companyId, out var rawMatCat,
            out var startersCat, out var soupsCat,
            out var vegDishesCat, out var nonVegDishesCat,
            out var riceNoodlesCat, out var hotBevCat, out var coldBevCat);

        if (context is IFoodDbContext foodDb)
        {
            SeedFoodTables(foodDb, companyId);
            SeedRecipes(foodDb, companyId, context);
        }

        await context.SaveChangesAsync(ct);
    }

    private static void SeedProductCategories(
        IApplicationDbContext context,
        Guid companyId,
        out ProductCategory rawMatCat,
        out ProductCategory startersCat,
        out ProductCategory soupsCat,
        out ProductCategory vegDishesCat,
        out ProductCategory nonVegDishesCat,
        out ProductCategory riceNoodlesCat,
        out ProductCategory hotBevCat,
        out ProductCategory coldBevCat)
    {
        // ── Top-level: Food ──────────────────────────────────────────────────────
        var food = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Food",
            SortOrder = 1,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(food);

        startersCat    = new ProductCategory { CompanyId = companyId, Name = "Starters & Appetizers", ParentCategoryId = food.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        soupsCat       = new ProductCategory { CompanyId = companyId, Name = "Soups & Salads",         ParentCategoryId = food.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };

        var mainCourse = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Main Course",
            ParentCategoryId = food.Id,
            SortOrder = 3,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(mainCourse);

        vegDishesCat    = new ProductCategory { CompanyId = companyId, Name = "Veg Dishes",     ParentCategoryId = mainCourse.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        nonVegDishesCat = new ProductCategory { CompanyId = companyId, Name = "Non-Veg Dishes", ParentCategoryId = mainCourse.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var seafoodCat  = new ProductCategory { CompanyId = companyId, Name = "Seafood",         ParentCategoryId = mainCourse.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(vegDishesCat, nonVegDishesCat, seafoodCat);

        riceNoodlesCat = new ProductCategory { CompanyId = companyId, Name = "Rice, Bread & Noodles", ParentCategoryId = food.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var bbqCat     = new ProductCategory { CompanyId = companyId, Name = "Grills & BBQ",           ParentCategoryId = food.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };
        var dessertsCat = new ProductCategory { CompanyId = companyId, Name = "Desserts & Sweets",    ParentCategoryId = food.Id, SortOrder = 6, IsDirectProductExist = true, IsActive = true };
        var snacksCat  = new ProductCategory { CompanyId = companyId, Name = "Snacks & Fast Food",    ParentCategoryId = food.Id, SortOrder = 7, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(startersCat, soupsCat, riceNoodlesCat, bbqCat, dessertsCat, snacksCat);

        // ── Top-level: Beverages ─────────────────────────────────────────────────
        var beverages = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Beverages",
            SortOrder = 2,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(beverages);

        hotBevCat     = new ProductCategory { CompanyId = companyId, Name = "Hot Beverages",   ParentCategoryId = beverages.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        coldBevCat    = new ProductCategory { CompanyId = companyId, Name = "Cold Beverages",  ParentCategoryId = beverages.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var juicesCat = new ProductCategory { CompanyId = companyId, Name = "Juices & Shakes", ParentCategoryId = beverages.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var mocktailsCat = new ProductCategory { CompanyId = companyId, Name = "Mocktails",    ParentCategoryId = beverages.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(hotBevCat, coldBevCat, juicesCat, mocktailsCat);

        // ── Top-level: Add-ons & Extras ──────────────────────────────────────────
        var addonsCat = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Add-ons & Extras",
            SortOrder = 3,
            IsDirectProductExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(addonsCat);

        // ── Top-level: Raw Materials ─────────────────────────────────────────────
        rawMatCat = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Raw Materials",
            SortOrder = 4,
            IsDirectProductExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(rawMatCat);

        // ── Raw Materials ────────────────────────────────────────────────────────
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = rawMatCat.Id, Name = "Chicken Breast",  Kind = ProductKind.RawMaterial, SKU = "RST-RM-CHB", DefaultCostPrice = 80m,  DefaultSalePrice = 80m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMatCat.Id, Name = "Butter",          Kind = ProductKind.RawMaterial, SKU = "RST-RM-BTR", DefaultCostPrice = 45m,  DefaultSalePrice = 45m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMatCat.Id, Name = "Fresh Cream",     Kind = ProductKind.RawMaterial, SKU = "RST-RM-CRM", DefaultCostPrice = 30m,  DefaultSalePrice = 30m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMatCat.Id, Name = "Tomatoes",        Kind = ProductKind.RawMaterial, SKU = "RST-RM-TOM", DefaultCostPrice = 20m,  DefaultSalePrice = 20m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMatCat.Id, Name = "Onions",          Kind = ProductKind.RawMaterial, SKU = "RST-RM-ONI", DefaultCostPrice = 15m,  DefaultSalePrice = 15m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMatCat.Id, Name = "Garlic",          Kind = ProductKind.RawMaterial, SKU = "RST-RM-GRL", DefaultCostPrice = 10m,  DefaultSalePrice = 10m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMatCat.Id, Name = "Ginger",          Kind = ProductKind.RawMaterial, SKU = "RST-RM-GNG", DefaultCostPrice = 12m,  DefaultSalePrice = 12m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMatCat.Id, Name = "Basmati Rice",    Kind = ProductKind.RawMaterial, SKU = "RST-RM-BSR", DefaultCostPrice = 60m,  DefaultSalePrice = 60m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMatCat.Id, Name = "Flour",           Kind = ProductKind.RawMaterial, SKU = "RST-RM-FLR", DefaultCostPrice = 25m,  DefaultSalePrice = 25m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMatCat.Id, Name = "Salt",            Kind = ProductKind.RawMaterial, SKU = "RST-RM-SLT", DefaultCostPrice = 5m,   DefaultSalePrice = 5m,   IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMatCat.Id, Name = "Black Pepper",    Kind = ProductKind.RawMaterial, SKU = "RST-RM-BPP", DefaultCostPrice = 8m,   DefaultSalePrice = 8m,   IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMatCat.Id, Name = "Garam Masala",    Kind = ProductKind.RawMaterial, SKU = "RST-RM-GMS", DefaultCostPrice = 18m,  DefaultSalePrice = 18m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMatCat.Id, Name = "Cumin",           Kind = ProductKind.RawMaterial, SKU = "RST-RM-CMN", DefaultCostPrice = 12m,  DefaultSalePrice = 12m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMatCat.Id, Name = "Turmeric",        Kind = ProductKind.RawMaterial, SKU = "RST-RM-TRM", DefaultCostPrice = 10m,  DefaultSalePrice = 10m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMatCat.Id, Name = "Black Lentils",   Kind = ProductKind.RawMaterial, SKU = "RST-RM-BLT", DefaultCostPrice = 35m,  DefaultSalePrice = 35m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMatCat.Id, Name = "Paneer",          Kind = ProductKind.RawMaterial, SKU = "RST-RM-PNR", DefaultCostPrice = 90m,  DefaultSalePrice = 90m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMatCat.Id, Name = "Vegetable Oil",   Kind = ProductKind.RawMaterial, SKU = "RST-RM-VGO", DefaultCostPrice = 22m,  DefaultSalePrice = 22m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMatCat.Id, Name = "Milk",            Kind = ProductKind.RawMaterial, SKU = "RST-RM-MLK", DefaultCostPrice = 24m,  DefaultSalePrice = 24m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft }
        );

        // ── Final Products (Menu Items) ──────────────────────────────────────────
        context.Products.AddRange(
            // Starters
            new Product { CompanyId = companyId, CategoryId = startersCat.Id,    Name = "Garlic Bread",          Kind = ProductKind.FinalProduct, SKU = "RST-ST-GBR", DefaultCostPrice = 40m,  DefaultSalePrice = 99m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = startersCat.Id,    Name = "Paneer Tikka",          Kind = ProductKind.FinalProduct, SKU = "RST-ST-PTK", DefaultCostPrice = 65m,  DefaultSalePrice = 149m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = startersCat.Id,    Name = "Chicken 65",            Kind = ProductKind.FinalProduct, SKU = "RST-ST-C65", DefaultCostPrice = 80m,  DefaultSalePrice = 179m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            // Soups
            new Product { CompanyId = companyId, CategoryId = soupsCat.Id,       Name = "Tomato Soup",           Kind = ProductKind.FinalProduct, SKU = "RST-SP-TMS", DefaultCostPrice = 30m,  DefaultSalePrice = 89m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = soupsCat.Id,       Name = "Mushroom Soup",         Kind = ProductKind.FinalProduct, SKU = "RST-SP-MSH", DefaultCostPrice = 35m,  DefaultSalePrice = 99m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            // Veg Dishes
            new Product { CompanyId = companyId, CategoryId = vegDishesCat.Id,   Name = "Paneer Butter Masala",  Kind = ProductKind.FinalProduct, SKU = "RST-VG-PBM", DefaultCostPrice = 85m,  DefaultSalePrice = 189m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = vegDishesCat.Id,   Name = "Dal Makhani",           Kind = ProductKind.FinalProduct, SKU = "RST-VG-DLM", DefaultCostPrice = 55m,  DefaultSalePrice = 159m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = vegDishesCat.Id,   Name = "Veg Biryani",           Kind = ProductKind.FinalProduct, SKU = "RST-VG-VBR", DefaultCostPrice = 75m,  DefaultSalePrice = 179m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            // Non-Veg Dishes
            new Product { CompanyId = companyId, CategoryId = nonVegDishesCat.Id, Name = "Butter Chicken",       Kind = ProductKind.FinalProduct, SKU = "RST-NV-BCH", DefaultCostPrice = 100m, DefaultSalePrice = 229m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = nonVegDishesCat.Id, Name = "Chicken Biryani",      Kind = ProductKind.FinalProduct, SKU = "RST-NV-CBR", DefaultCostPrice = 110m, DefaultSalePrice = 249m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = nonVegDishesCat.Id, Name = "Mutton Curry",         Kind = ProductKind.FinalProduct, SKU = "RST-NV-MTC", DefaultCostPrice = 130m, DefaultSalePrice = 279m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            // Rice, Bread & Noodles
            new Product { CompanyId = companyId, CategoryId = riceNoodlesCat.Id, Name = "Steamed Rice",          Kind = ProductKind.FinalProduct, SKU = "RST-RB-STR", DefaultCostPrice = 20m,  DefaultSalePrice = 59m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = riceNoodlesCat.Id, Name = "Butter Naan",           Kind = ProductKind.FinalProduct, SKU = "RST-RB-BNN", DefaultCostPrice = 15m,  DefaultSalePrice = 39m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = riceNoodlesCat.Id, Name = "Tandoori Roti",         Kind = ProductKind.FinalProduct, SKU = "RST-RB-TRT", DefaultCostPrice = 10m,  DefaultSalePrice = 29m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            // Hot Beverages
            new Product { CompanyId = companyId, CategoryId = hotBevCat.Id,      Name = "Masala Chai",           Kind = ProductKind.FinalProduct, SKU = "RST-HB-MCH", DefaultCostPrice = 12m,  DefaultSalePrice = 49m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = hotBevCat.Id,      Name = "Filter Coffee",         Kind = ProductKind.FinalProduct, SKU = "RST-HB-FCF", DefaultCostPrice = 15m,  DefaultSalePrice = 59m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            // Cold Beverages
            new Product { CompanyId = companyId, CategoryId = coldBevCat.Id,     Name = "Mango Lassi",           Kind = ProductKind.FinalProduct, SKU = "RST-CB-MLS", DefaultCostPrice = 25m,  DefaultSalePrice = 89m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = coldBevCat.Id,     Name = "Fresh Lime Soda",       Kind = ProductKind.FinalProduct, SKU = "RST-CB-FLS", DefaultCostPrice = 15m,  DefaultSalePrice = 69m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published }
        );
    }

    private static void SeedFoodTables(IFoodDbContext foodDb, Guid companyId)
    {
        // 6 Indoor (capacity 4), 2 Outdoor (capacity 2), 2 Private (capacity 6)
        for (int i = 1; i <= 6; i++)
        {
            foodDb.FoodTables.Add(new FoodTable
            {
                CompanyId = companyId,
                Name = $"Table {i}",
                Capacity = 4,
                Section = "Indoor",
                Status = FoodTableStatus.Available
            });
        }
        foodDb.FoodTables.Add(new FoodTable { CompanyId = companyId, Name = "Garden Table 1", Capacity = 2, Section = "Outdoor", Status = FoodTableStatus.Available });
        foodDb.FoodTables.Add(new FoodTable { CompanyId = companyId, Name = "Garden Table 2", Capacity = 2, Section = "Outdoor", Status = FoodTableStatus.Available });
        foodDb.FoodTables.Add(new FoodTable { CompanyId = companyId, Name = "Private Room A", Capacity = 6, Section = "Private", Status = FoodTableStatus.Available });
        foodDb.FoodTables.Add(new FoodTable { CompanyId = companyId, Name = "Private Room B", Capacity = 6, Section = "Private", Status = FoodTableStatus.Available });
    }

    private static void SeedRecipes(IFoodDbContext foodDb, Guid companyId, IApplicationDbContext context)
    {
        // Butter Chicken recipe
        var butterChickenRecipe = new Recipe
        {
            CompanyId = companyId,
            ProductId = Guid.NewGuid(), // placeholder — resolved at runtime by product lookup
            Name = "Butter Chicken",
            YieldQuantity = 1,
            Instructions = "Marinate chicken in yogurt and spices. Cook in tandoor or oven. Prepare makhani gravy with butter, cream, tomatoes, and spices. Combine and simmer for 15 minutes.",
            Ingredients =
            [
                new RecipeIngredient { RawMaterialProductId = Guid.NewGuid(), RawMaterialName = "Chicken Breast",  Quantity = 200m, Unit = "grams" },
                new RecipeIngredient { RawMaterialProductId = Guid.NewGuid(), RawMaterialName = "Butter",          Quantity = 30m,  Unit = "grams" },
                new RecipeIngredient { RawMaterialProductId = Guid.NewGuid(), RawMaterialName = "Fresh Cream",     Quantity = 50m,  Unit = "ml"    },
                new RecipeIngredient { RawMaterialProductId = Guid.NewGuid(), RawMaterialName = "Tomatoes",        Quantity = 100m, Unit = "grams" },
                new RecipeIngredient { RawMaterialProductId = Guid.NewGuid(), RawMaterialName = "Garam Masala",    Quantity = 5m,   Unit = "grams" },
            ]
        };

        // Dal Makhani recipe
        var dalMakhaniRecipe = new Recipe
        {
            CompanyId = companyId,
            ProductId = Guid.NewGuid(),
            Name = "Dal Makhani",
            YieldQuantity = 1,
            Instructions = "Soak black lentils overnight. Pressure cook with onions, tomatoes, ginger-garlic paste. Finish with butter and cream. Simmer on low heat for 30 minutes.",
            Ingredients =
            [
                new RecipeIngredient { RawMaterialProductId = Guid.NewGuid(), RawMaterialName = "Black Lentils",  Quantity = 100m, Unit = "grams" },
                new RecipeIngredient { RawMaterialProductId = Guid.NewGuid(), RawMaterialName = "Butter",         Quantity = 25m,  Unit = "grams" },
                new RecipeIngredient { RawMaterialProductId = Guid.NewGuid(), RawMaterialName = "Fresh Cream",    Quantity = 30m,  Unit = "ml"    },
                new RecipeIngredient { RawMaterialProductId = Guid.NewGuid(), RawMaterialName = "Tomatoes",       Quantity = 80m,  Unit = "grams" },
                new RecipeIngredient { RawMaterialProductId = Guid.NewGuid(), RawMaterialName = "Ginger",         Quantity = 10m,  Unit = "grams" },
                new RecipeIngredient { RawMaterialProductId = Guid.NewGuid(), RawMaterialName = "Garlic",         Quantity = 10m,  Unit = "grams" },
            ]
        };

        // Veg Biryani recipe
        var vegBiryaniRecipe = new Recipe
        {
            CompanyId = companyId,
            ProductId = Guid.NewGuid(),
            Name = "Veg Biryani",
            YieldQuantity = 1,
            Instructions = "Soak basmati rice for 30 minutes. Fry onions golden, add whole spices, ginger-garlic paste, vegetables. Layer with parboiled rice and cook on dum for 20 minutes.",
            Ingredients =
            [
                new RecipeIngredient { RawMaterialProductId = Guid.NewGuid(), RawMaterialName = "Basmati Rice",  Quantity = 150m, Unit = "grams" },
                new RecipeIngredient { RawMaterialProductId = Guid.NewGuid(), RawMaterialName = "Onions",        Quantity = 80m,  Unit = "grams" },
                new RecipeIngredient { RawMaterialProductId = Guid.NewGuid(), RawMaterialName = "Vegetable Oil", Quantity = 20m,  Unit = "ml"    },
                new RecipeIngredient { RawMaterialProductId = Guid.NewGuid(), RawMaterialName = "Garam Masala",  Quantity = 4m,   Unit = "grams" },
                new RecipeIngredient { RawMaterialProductId = Guid.NewGuid(), RawMaterialName = "Turmeric",      Quantity = 2m,   Unit = "grams" },
                new RecipeIngredient { RawMaterialProductId = Guid.NewGuid(), RawMaterialName = "Cumin",         Quantity = 3m,   Unit = "grams" },
            ]
        };

        foodDb.Recipes.AddRange(butterChickenRecipe, dalMakhaniRecipe, vegBiryaniRecipe);
    }
}
