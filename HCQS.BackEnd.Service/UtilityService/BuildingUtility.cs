namespace HCQS.BackEnd.Service.UtilityService
{
    public class BuildingUtility
    {
        public const int CEMENTDENSITY = 1440;
        public const int SANDDENSITY = 1600;
        public const int STONEDENSITY = 2650;

        public class BuildingInputModel
        {
            public double WallLength { get; set; }
            public double WallHeight { get; set; }
            public double CementRatio { get; set; }
            public double SandRatio { get; set; }
            public double StoneRatio { get; set; }
        }

        public class BuildingMaterialModel
        {
            public double WallLength { get; set; }
            public double WallHeight { get; set; }
            public double CementVolume { get; set; }
            public double SandVolume { get; set; }
            public double StoneVolume { get; set; }
        }

        public static int CalculateBrickCount(double wallLength, double wallHeight)
        {
            double wallArea = wallLength * wallHeight;

            double brickSize = 0.2 * 0.1;
            int brickCount = (int)(wallArea / brickSize);
            return brickCount;
        }

        public static BuildingMaterialModel CalculateMaterials(BuildingInputModel input)
        {
            double wallArea = input.WallLength * input.WallHeight;
            double cementWeight = wallArea * input.CementRatio * CEMENTDENSITY;
            double sandWeight = wallArea * input.SandRatio * SANDDENSITY;
            double stoneWeight = wallArea * input.StoneRatio * STONEDENSITY;

            double cementVolume = cementWeight / CEMENTDENSITY;
            double sandVolume = sandWeight / SANDDENSITY;
            double stoneVolume = stoneWeight / STONEDENSITY;

            BuildingMaterialModel result = new BuildingMaterialModel
            {
                CementVolume = cementVolume,
                SandVolume = sandVolume,
                StoneVolume = stoneVolume
            };

            return result;
        }
    }
}